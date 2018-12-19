
namespace NCS.DSS.AzureSearchUtility.Models
{
    public class AddressSearchConfig : IConfigSettings
    {
        public string SearchIndexerName { get; set; }
        public string SearchDataSourceName { get; set; }
        public string SearchDataSourceQuery { get; set; }
        public string CollectionId { get; set; }
        public string DatabaseId { get; set; }
        public string ConnectionString { get; set; }
    }
}
