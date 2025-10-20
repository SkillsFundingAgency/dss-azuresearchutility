using Azure;
using Azure.Search.Documents.Indexes.Models;
using NCS.DSS.AzureSearchUtility.Helpers;
using NCS.DSS.AzureSearchUtility.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.AzureSearchUtility.CreateIndexer
{
    public static class CreateAddressIndexer
    {
        public static async Task<HttpResponseMessage> RunCreateAddressIndexer(string searchAdminKey, SearchConfig searchConfig, SearchIndex customerSearchIndex)
        {
            Console.WriteLine("Retrieving Search Service for Address Indexer\n");
            
            var searchIndexerClient = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceEndpoint, searchAdminKey);

            if (searchIndexerClient == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("Deleting old Address Data Source...\n");

            try
            {
                var dataSourceExists = await searchIndexerClient.GetDataSourceConnectionAsync(searchConfig.AddressSearchConfig.SearchDataSourceName) != null;

                if (dataSourceExists)
                {
                    await searchIndexerClient.DeleteDataSourceConnectionAsync(searchConfig.AddressSearchConfig.SearchDataSourceName);
                }
            }
            catch (RequestFailedException e) when (e.Status == (int)HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Data source '{searchConfig.AddressSearchConfig.SearchDataSourceName}' does not exist, skipping delete.");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error deleting data source: {e}");
                throw;
            }

            Console.WriteLine("Creating Address Data Source object...\n");
            var dataSource = DataSourceHelper.CreateDataSource(
                searchConfig.AddressSearchConfig.SearchDataSourceQuery,
                searchConfig.AddressSearchConfig.CollectionId,
                searchConfig.SearchIndexName,
                searchConfig.AddressSearchConfig.SearchDataSourceName,
                searchConfig.AddressSearchConfig.ConnectionString);

            try
            {
                Console.WriteLine("Attempting to Create/Update Address Data Source...\n");
                await searchIndexerClient.CreateOrUpdateDataSourceConnectionAsync(dataSource);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error creating/updating data source: {e}");
                throw;
            }

            SearchIndexer indexer;
            FieldMapping fieldMapping = new("CustomerId")
            {
                TargetFieldName = "CustomerId"
            };

            try
            {
                indexer = await IndexerHelper.CreateIndexerAsync(
                    searchIndexerClient,
                    customerSearchIndex,
                    searchConfig.AddressSearchConfig.SearchIndexerName,
                    searchConfig.AddressSearchConfig.SearchDataSourceName,
                    fieldMapping);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to Create Address Indexer...\n" + e);
                throw;
            }

            if (indexer == null)
            {
                Console.WriteLine("Unable to find Address Indexer...\n");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            Console.WriteLine("Run Address Indexer...\n");
            try
            {
                await searchIndexerClient.RunIndexerAsync(indexer.Name);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("Unable to get data for Address Indexer...\n" + e);
                throw;
            }

            var running = true;
            Console.WriteLine("Synchronization running...\n");

            while (running)
            {
                SearchIndexerStatus status;

                try
                {
                    status = await searchIndexerClient.GetIndexerStatusAsync(indexer.Name);
                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine($"Error polling for indexer status: {ex.Message}");
                    throw;
                }

                var lastResult = status.LastResult;
                if (lastResult != null)
                {
                    switch (lastResult.Status)
                    {
                        case IndexerExecutionStatus.Reset:
                        case IndexerExecutionStatus.InProgress:
                            Console.WriteLine($"Synchronization running...\nStatus: {lastResult.Status}, Item Count: {lastResult.ItemCount}");
                            await Task.Delay(1000);
                            break;
                        case IndexerExecutionStatus.Success:
                            running = false;
                            Console.WriteLine($"Synchronized {lastResult.ItemCount} rows...\n");
                            break;
                        default:
                            running = false;
                            Console.WriteLine($"Synchronization failed: {lastResult.ErrorMessage}\n");
                            break;
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
