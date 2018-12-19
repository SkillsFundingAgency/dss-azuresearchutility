namespace NCS.DSS.AzureSearchUtility.Models
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
        public string CustomerConnectionString { get; set; }


        public string AddressSearchIndexerName { get; set; }
        public string AddressSearchDataSourceName { get; set; }
        public string AddressSearchDataSourceQuery { get; set; }
        public string AddressCollectionId { get; set; }
        public string AddressDatabaseId { get; set; }
        public string AddressConnectionString { get; set; }


        public string ContactDetailsSearchIndexerName { get; set; }
        public string ContactDetailsSearchDataSourceName { get; set; }
        public string ContactDetailsSearchDataSourceQuery { get; set; }
        public string ContactDetailsCollectionId { get; set; }
        public string ContactDetailsDatabaseId { get; set; }
        public string ContactDetailsConnectionString { get; set; }
    }
}
