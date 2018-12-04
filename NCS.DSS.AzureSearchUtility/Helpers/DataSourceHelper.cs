using Microsoft.Azure.Search.Models;

namespace NCS.DSS.AzureSearchUtilities.Helpers
{
    public static class DataSourceHelper
    {

        public static DataSource CreateDataSource(string sqlQuery, string collectionName, string indexName, string dataSourceName, string dataSourceConnectionString)
        {
            var dataSource = new DataSource
            {
                Name = dataSourceName,
                Container = CreateDataSourceContainer(sqlQuery, collectionName),
                Credentials = new DataSourceCredentials(dataSourceConnectionString),
                Type = DataSourceType.DocumentDb,
                DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy { HighWaterMarkColumnName = "_ts"}
            };
            return dataSource;
        }

        private static DataContainer CreateDataSourceContainer(string sqlQuery, string collectionName)
        {
            var container = new DataContainer { Query = sqlQuery, Name = collectionName };
            return container;
        }
    }
}
