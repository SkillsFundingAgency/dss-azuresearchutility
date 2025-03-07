using Azure.Search.Documents.Indexes.Models;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class DataSourceHelper
    {
        public static SearchIndexerDataSourceConnection CreateDataSource(string sqlQuery, string collectionName, string indexName, string dataSourceName, string dataSourceConnectionString)
        {
            var dataSource = new SearchIndexerDataSourceConnection(
                name: dataSourceName,
                type: SearchIndexerDataSourceType.CosmosDb,
                connectionString: dataSourceConnectionString,
                container: CreateDataSourceContainer(sqlQuery, collectionName))
            {
                DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy("_ts")
            };

            return dataSource;
        }

        private static SearchIndexerDataContainer CreateDataSourceContainer(string sqlQuery, string collectionName)
        {
            var container = new SearchIndexerDataContainer(collectionName)
            {
                Query = sqlQuery
            };
            return container;
        }
    }
}