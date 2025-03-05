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
    public static class CreateContactDetailsIndexer
    {
        public static async Task<HttpResponseMessage> RunCreateContactDetailsIndexer(string searchAdminKey, SearchConfig searchConfig, SearchIndex customerSearchIndex)
        {
            Console.WriteLine("Retrieving Search Service for Contact Details Indexer\n");

            var searchIndexerClient = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceName, searchAdminKey);

            if (searchIndexerClient == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("Deleting old Contact Data Source...\n");

            try
            {
                var dataSourceExists = await searchIndexerClient.GetDataSourceConnectionAsync(searchConfig.ContactDetailsSearchConfig.SearchDataSourceName) != null;

                if (dataSourceExists)
                {
                    await searchIndexerClient.DeleteDataSourceConnectionAsync(searchConfig.ContactDetailsSearchConfig.SearchDataSourceName);
                }
            }
            catch (RequestFailedException e) when (e.Status == (int)HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Data source '{searchConfig.ContactDetailsSearchConfig.SearchDataSourceName}' not found, skipping delete.");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error deleting data source: {e}");
                throw;
            }

            Console.WriteLine("Creating Contact Details Data Source object...\n");
            var dataSource = DataSourceHelper.CreateDataSource(
                searchConfig.ContactDetailsSearchConfig.SearchDataSourceQuery,
                searchConfig.ContactDetailsSearchConfig.CollectionId,
                searchConfig.SearchIndexName,
                searchConfig.ContactDetailsSearchConfig.SearchDataSourceName,
                searchConfig.ContactDetailsSearchConfig.ConnectionString);

            try
            {
                Console.WriteLine("Attempting to Create/Update Contact Details Data Source...\n");
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
                    searchConfig.ContactDetailsSearchConfig.SearchIndexerName,
                    searchConfig.ContactDetailsSearchConfig.SearchDataSourceName,
                    fieldMapping);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to Create Contact Details Indexer...\n" + e);
                throw;
            }

            if (indexer == null)
            {
                Console.WriteLine("Unable to find Contact Details Indexer...\n");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            Console.WriteLine("Run Contact Details Indexer...\n");
            try
            {
                await searchIndexerClient.RunIndexerAsync(indexer.Name);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Unable to run Contact Details Indexer: {e}");
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
