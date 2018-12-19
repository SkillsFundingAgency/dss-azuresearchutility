namespace NCS.DSS.AzureSearchUtility.Models
{
    public class SearchConfig
    {
        public string SearchServiceName { get; set; }
        public string SearchIndexName { get; set; }
        public string CosmosDbConnectionString { get; set; }
        public CustomerSearchConfig CustomerSearchConfig { get; set; }
        public AddressSearchConfig AddressSearchConfig { get; set; }
        public ContactDetailsSearchConfig ContactDetailsSearchConfig { get; set; }

    }
}
