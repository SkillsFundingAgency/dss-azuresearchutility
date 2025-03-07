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
    public static class CreateCustomerIndexer
    {
        public static async Task<HttpResponseMessage> RunCreateCustomerIndexer(string searchAdminKey, SearchConfig searchConfig, SearchIndex customerSearchIndex)
        {
            Console.WriteLine("Retrieving Search Service for Customer Indexer\n");
            var searchIndexerClient = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceEndpoint, searchAdminKey);

            if (searchIndexerClient == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("Deleting old Customer Data Source ...\n");

            try
            {
                if (await searchIndexerClient.GetDataSourceConnectionAsync(searchConfig.CustomerSearchConfig.SearchDataSourceName) != null)
                {
                    await searchIndexerClient.DeleteDataSourceConnectionAsync(searchConfig.CustomerSearchConfig.SearchDataSourceName);
                }
            }
            catch (RequestFailedException e) when (e.Status == (int)HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Data source '{searchConfig.CustomerSearchConfig.SearchDataSourceName}' not found, skipping delete.");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error deleting data source: {e}");
                throw;
            }

            Console.WriteLine("Creating Customer Data Source object...\n");
            var dataSource = DataSourceHelper.CreateDataSource(
                searchConfig.CustomerSearchConfig.SearchDataSourceQuery,
                searchConfig.CustomerSearchConfig.CollectionId,
                searchConfig.SearchIndexName,
                searchConfig.CustomerSearchConfig.SearchDataSourceName,
                searchConfig.CustomerSearchConfig.ConnectionString);

            try
            {
                Console.WriteLine("Attempting to Create/Update Customer Data Source...\n");
                await searchIndexerClient.CreateOrUpdateDataSourceConnectionAsync(dataSource);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error creating/updating data source: {e}");
            }

            SearchIndexer indexer;
            FieldMapping fieldMapping = new("id")
            {
                TargetFieldName = "CustomerId"
            };

            try
            {
                Console.WriteLine("Attempting to Create Indexer...\n");
                indexer = await IndexerHelper.CreateIndexerAsync(
                    searchIndexerClient,
                    customerSearchIndex,
                    searchConfig.CustomerSearchConfig.SearchIndexerName,
                    searchConfig.CustomerSearchConfig.SearchDataSourceName,
                    fieldMapping);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to Create Customer Indexer...\n" + e);
                throw;
            }

            if (indexer == null)
            {
                Console.WriteLine("Unable to find Customer Indexer...\n");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            Console.WriteLine("Run Customer Indexer...\n");
            try
            {
                await searchIndexerClient.RunIndexerAsync(indexer.Name);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Unable to run Customer Indexer: {e}");
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
                catch (Exception ex)
                {
                    Console.WriteLine("Error polling for indexer status: {0}", ex.Message);
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
