using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.AzureSearchUtilities.Models
{
    public class SearchConfig
    {
        public string SearchServiceName { get; set; }
        public string CustomerSearchIndexName { get; set; }
        public string CustomerSearchIndexerName { get; set; }
        public string CustomerSearchDataSourceName { get; set; }
        public string CustomerSearchDataSourceQuery { get; set; }
        public string CustomerCollectionId { get; set; }
        public string CustomerDatabaseId { get; set; }
    }
}
