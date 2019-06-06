using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi;
using System.Configuration;

namespace QueryCommandAzDv.Commands
{
    public class AzureDevOPSCommand
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _personalUser;
        readonly string _project;

        /// <summary>
        /// Constructor. Manually set values to match your organization. 
        /// </summary>
        public AzureDevOPSCommand()
        {
            _uri = ConfigurationManager.AppSettings["URL_AzureDevOPS"];
            _personalAccessToken = ConfigurationManager.AppSettings["Token_AzureDevOPS"];
            _personalUser = ConfigurationManager.AppSettings["User_AzureDevOPS"];
            _project = ConfigurationManager.AppSettings["Project_AzureDevOPS"];
        }

        /// <summary>
        /// Create a bug using the .NET client library
        /// </summary>
        /// <returns>Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem</returns>    
        public WorkItem CreateBugUsingClientLib()
        {
            Uri uri = new Uri(_uri);
            VssBasicCredential credentials = new VssBasicCredential(_personalUser, _personalAccessToken);
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            // TITRE
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Test Bug by API"
                }
            );
            // REPROSTEPS
            patchDocument.Add(
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                Value = "Description du ticket"
            }
        );
            // PRIORITE
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Priority",
                    Value = "1"
                }
            );
            // SEVERITE
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Severity",
                    Value = "2 - High"
                }
            );
            // ORIGINE
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Custom.Origine",
                    Value = "Client"
                }
            );
            // ITERATIONPATH
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = "IT\\Run\\Actifs"
                }
            );

            // Instance de connection
            using (var connection = new VssConnection(uri, credentials))
            {
                var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

                try
                {
                    // Création du workItem
                    WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, _project, "Bug").Result;

                    return result;
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine("Error creating bug: {0}", ex.InnerException.Message);
                    return null;
                }
            }
        }
    }
}
