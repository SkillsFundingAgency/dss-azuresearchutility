using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using NCS.DSS.AzureSearchUtilities.Helpers;
using NCS.DSS.AzureSearchUtilities.Models;

namespace NCS.DSS.AzureSearchUtilities.CreateIndex
{
    public class CreateCustomerSearchIndex
    {
        public async Task CreateIndex(string searchAdminKey, string cosmosConnectionString, SearchConfig searchConfig)
        {

            Console.WriteLine("{0}", "Retrieving Search Service\n");
            var azureSearchService = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceName, searchAdminKey);

            if (azureSearchService == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("{0}", "Deleting Index...\n");
            SearchHelper.DeleteIndexIfExists(searchConfig.CustomerSearchIndexName);

            Console.WriteLine("{0}", "Creating Index Model...\n");
            var indexModelForCustomer = IndexModelHelper.CreateIndexModelForCustomer(searchConfig.CustomerSearchIndexName);

            if (indexModelForCustomer == null)
            {
                Console.WriteLine("Unable to create Index Model");
                throw new Exception("Unable to create Index Model");
            }

            Console.WriteLine("{0}", "Creating Index...\n");
            SearchHelper.CreateIndex(indexModelForCustomer);

            Console.WriteLine("{0}", "Creating Data Source object...\n");
            var dataSource = DataSourceHelper.CreateDataSource(searchConfig.CustomerSearchDataSourceQuery, searchConfig.CustomerCollectionId,
                searchConfig.CustomerSearchIndexName, searchConfig.CustomerSearchDataSourceName, cosmosConnectionString);

            try
            {
                Console.WriteLine("{0}", "Attempting to Create/Update Data Source...\n");
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
                Console.WriteLine("{0}", "Attempting to Create Indexer...\n");
                indexer = IndexerHelper.CreateIndexer(azureSearchService, searchConfig.CustomerSearchIndexerName, indexModelForCustomer, searchConfig.CustomerSearchDataSourceName);

            }
            catch (CloudException e)
            {
                Console.WriteLine("{0}{1}", "Unable to Create Indexer...\n", e);
                throw;
            }

            Console.WriteLine("{0}", "Run Indexer...\n");
            try
            {
                azureSearchService.Indexers.Run(indexer.Name);
            }
            catch (CloudException e)
            {
                Console.WriteLine("{0} {1}", "Unable to get data for the Indexer...\n", e);
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
                        case IndexerExecutionStatus.InProgress:
                            Console.WriteLine("{0} Status: {1}, Item Count: {2}", "Synchronization running...\n", lastResult.Status, lastResult.ItemCount);
                            Thread.Sleep(1000);
                            break;
                        case IndexerExecutionStatus.Reset:
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

            Console.WriteLine("{0} Status: {1}", "Completed... ", HttpStatusCode.Created);
        }
    }
}