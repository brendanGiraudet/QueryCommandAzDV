using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryCommandAzDv
{
    class Program
    {
        static void Main(string[] args)
        {
            // Exemple d'utilisation de création d'un bug
            var cmd = new Commands.AzureDevOPSCommand();
            var re = cmd.CreateBugUsingClientLib();
            // Exemple de récupération de la liste des Bugs
            var query = new Queries.AzureDevOPSQuery();
            var ret = query.GetBugWorkItems().GetAwaiter().GetResult();
        }
    }
}
