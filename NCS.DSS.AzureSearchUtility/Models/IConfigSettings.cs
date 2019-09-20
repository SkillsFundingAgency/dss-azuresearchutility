namespace NCS.DSS.AzureSearchUtility.Models
{
    public interface IConfigSettings
    {
        string SearchIndexerName { get; set; }
        string SearchDataSourceName { get; set; }
        string SearchDataSourceQuery { get; set; }
        string CollectionId { get; set; }
        string DatabaseId { get; set; }
        string ConnectionString { get; set; }
    }
}