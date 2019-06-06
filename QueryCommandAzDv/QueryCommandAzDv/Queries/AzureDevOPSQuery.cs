using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace QueryCommandAzDv.Queries
{
    public class AzureDevOPSQuery
    {
        string _uri;
        string _personalAccessToken;
        string _project;
        /// <summary>
        /// Constructeur; Initialisation de l'URI, Token et projet
        /// </summary>
        public AzureDevOPSQuery()
        {
            _uri = ConfigurationManager.AppSettings["URL_AzureDevOPS"];
            _personalAccessToken = ConfigurationManager.AppSettings["Token_AzureDevOPS"];
            _project = ConfigurationManager.AppSettings["Project_AzureDevOPS"];
        }
        /// <summary>
        /// Recupère une liste de bug
        /// </summary>
        /// <returns>List of Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<List<WorkItem>> GetBugWorkItems()
        {
            var query = "Select * " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + _project + "' " +
                        "And [System.State] <> 'Closed' " +
                        "Order By [State] Asc, [Changed Date] Desc";
            return await GetWorkItemByQuery(query);
        }

        /// <summary>
        /// Execute une requete WIQL pour retourner une liste de workItem en utilisant la librairie .NET client
        /// </summary>
        /// <returns>List of Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>
        public async Task<List<WorkItem>> GetWorkItemByQuery(string query)
        {
            Uri uri = new Uri(_uri);
            VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);

            // Création de la requete
            Wiql wiql = new Wiql()
            {
                Query = query
            };

            // instance http client
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                // execute la requete
                WorkItemQueryResult workItemQueryResult = await workItemTrackingHttpClient.QueryByWiqlAsync(wiql);

                //Au cas ou il y aurait un problème ou qu'il n'y ai pas de workitem                
                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    //listing des ID des WorkItem à charger
                    List<int> list = new List<int>();
                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        list.Add(item.Id);
                    }
                    int[] arr = list.ToArray();

                    //List des champs a récuperer dans le workItem
                    //string[] fields = new string[3];
                    //fields[0] = "System.Id";
                    //fields[1] = "System.Title";
                    //fields[2] = "System.State";
                    //var workItems = await workItemTrackingHttpClient.GetWorkItemsAsync(arr, fields, workItemQueryResult.AsOf);

                    //Récupération des workItems selon leurs ID  avec tous leur champs
                    var workItems = await workItemTrackingHttpClient.GetWorkItemsAsync(arr, asOf: workItemQueryResult.AsOf);

                    return workItems;
                }

                return null;
            }
        }
    }
}
