using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using NCS.DSS.AzureSearchUtility.Helpers;
using NCS.DSS.AzureSearchUtility.Models;

namespace NCS.DSS.AzureSearchUtility.CreateIndexer
{
    public static class CreateAddressIndexer
    {
        public static async Task<HttpResponseMessage> RunCreateAddressIndexer(string searchAdminKey, SearchConfig searchConfig, Index customerSearchIndex)
        {
            Console.WriteLine("{0}", "Retrieving Search Service for Address Indexer\n");
            var azureSearchService = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceName, searchAdminKey);

            if (azureSearchService == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("{0}", "Creating Address Data Source object...\n");
            var dataSource = DataSourceHelper.CreateDataSource(searchConfig.AddressSearchConfig.SearchDataSourceQuery,
                searchConfig.AddressSearchConfig.CollectionId,
                searchConfig.SearchIndexName,
                searchConfig.AddressSearchConfig.SearchDataSourceName,
                searchConfig.AddressSearchConfig.ConnectionString);

            try
            {
                Console.WriteLine("{0}", "Attempting to Create/Update Address Data Source...\n");
                await azureSearchService.DataSources.CreateOrUpdateWithHttpMessagesAsync(dataSource);
            }
            catch (CloudException e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }

            Indexer indexer;

            try
            {
                indexer = IndexerHelper.CreateIndexer(azureSearchService,
                    customerSearchIndex,
                    searchConfig.AddressSearchConfig.SearchIndexerName,
                    searchConfig.AddressSearchConfig.SearchDataSourceName,
                    new List<FieldMapping> { new FieldMapping("CustomerId", "CustomerId") });
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}{1}", "Unable to Create Address Indexer...\n", e);
                throw;
            }

            if (indexer == null)
            {
                Console.WriteLine("{0}", "Unable to find Indexer...\n");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            Console.WriteLine("{0}", "Run Address Indexer...\n");
            try
            {
                azureSearchService.Indexers.Run(indexer.Name);
            }
            catch (CloudException e)
            {
                Console.WriteLine("{0} {1}", "Unable to get data for Address Indexer...\n", e);
                throw;
            }

            var running = true;
            Console.WriteLine("{0}", "Synchronization running...\n");
            while (running)
            {
                IndexerExecutionInfo status = null;

                try
                {
                    status = azureSearchService.Indexers.GetStatus(indexer.Name);
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
                            Console.WriteLine("{0}Status: {1}, Item Count: {2}", "Synchronization running...\n", lastResult.Status, lastResult.ItemCount);
                            Thread.Sleep(1000);
                            break;
                        case IndexerExecutionStatus.Success:
                            running = false;
                            Console.WriteLine("Synchronized {0} rows...\n", lastResult.ItemCount.ToString());
                            break;
                        default:
                            running = false;
                            Console.WriteLine("Synchronization failed: {0}\n", lastResult.ErrorMessage);
                            break;
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.Created);

        }
    }
}
