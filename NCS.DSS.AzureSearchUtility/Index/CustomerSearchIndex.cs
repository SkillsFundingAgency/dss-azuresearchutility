using NCS.DSS.AzureSearchUtility.Indexer;
using NCS.DSS.AzureSearchUtility.Helpers;
using NCS.DSS.AzureSearchUtility.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NCS.DSS.AzureSearchUtility.Index
{
    public static class CustomerSearchIndex
    {
        public static async Task CreateIndex(string searchAdminKey, SearchConfig searchConfig, string synonymPath)
        {

            Console.WriteLine("Retrieving Search Service\n");
            var azureSearchService = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceEndpoint, searchAdminKey);

            if (azureSearchService == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("Deleting Customer Search Index...\n");
            SearchHelper.DeleteIndexIfExists(searchConfig.SearchServiceEndpoint, searchAdminKey, searchConfig.SearchIndexName);

            Console.WriteLine("Creating Customer Search Index Model...\n");
            var indexModelForCustomer = IndexModelHelper.CreateIndexModelForCustomer(searchConfig.SearchIndexName);

            Console.WriteLine("Attempting to Create Customer Synonym Map...\n");
            IndexModelHelper.AddSynonymMapsToFields(indexModelForCustomer);

            Console.WriteLine("Add Customer Synonym Map to service client...\n");
            SearchHelper.UploadSynonymsForGivenName(synonymPath);

            Console.WriteLine("Creating Index for Customer Search...\n");
            SearchHelper.CreateIndex(indexModelForCustomer);

            try
            {

                Console.WriteLine("Deleting Customer Indexer...\n");
                if (await azureSearchService.GetIndexerAsync(searchConfig.CustomerSearchConfig.SearchIndexerName) != null)
                {
                    await azureSearchService.DeleteIndexerAsync(searchConfig.CustomerSearchConfig.SearchIndexerName);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Deleting Customer Indexer...\n Error: {e}");
                throw;
            }


            try
            {

                Console.WriteLine("Attempting to Create Customer Indexer...\n");
                var response = await CustomerIndexer.RunCreateCustomerIndexer(searchAdminKey, searchConfig, indexModelForCustomer);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully Created Customer Indexer...\n");
                }
                else
                {
                    Console.WriteLine("Error Creating Customer Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Creating Customer Indexer...\n Error: {e}");
                throw;
            }

            try
            {
                Console.WriteLine("Deleting Address Indexer...\n");
                if (await azureSearchService.GetIndexerAsync(searchConfig.AddressSearchConfig.SearchIndexerName) != null)
                {
                    await azureSearchService.DeleteIndexerAsync(searchConfig.AddressSearchConfig.SearchIndexerName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Deleting Address Indexer...\n Error: {e}");
                throw;
            }

            try
            {

                Console.WriteLine("Attempting to Create Address Indexer...\n");
                var response = await AddressIndexer.RunCreateAddressIndexer(searchAdminKey, searchConfig, indexModelForCustomer);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully Created Address Indexer...\n");
                }
                else
                {
                    Console.WriteLine("Error Creating Address Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Creating Address Indexer...\n Error: {e}");
                throw;
            }

            try
            {

                Console.WriteLine("Deleting Contact Search Details Indexer...\n");
                if (await azureSearchService.GetIndexerAsync(searchConfig.ContactDetailsSearchConfig.SearchIndexerName) != null)
                {
                    await azureSearchService.DeleteIndexerAsync(searchConfig.ContactDetailsSearchConfig.SearchIndexerName);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Deleting Contact Details Indexer...\n Error: {e}");
                throw;
            }

            try
            {

                Console.WriteLine("Attempting to Create Contact Details Indexer...\n");
                var response = await ContactDetailsIndexer.RunCreateContactDetailsIndexer(searchAdminKey, searchConfig, indexModelForCustomer);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully Created Contact Details Indexer...\n");
                }
                else
                {
                    Console.WriteLine("Error Creating Contact Details Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Creating Contact Details Indexer...\n Error: {e}");
                throw;
            }

            Console.WriteLine($"Completed...  Status: {HttpStatusCode.Created}");
        }

        public static async Task UpdateIndex(string searchAdminKey, SearchConfig searchConfig)
        {

            Console.WriteLine("Retrieving Search Service\n");
            var indexerClient = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceEndpoint, searchAdminKey);

            if (indexerClient == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            var indexClient = SearchHelper.GetIndexClient(searchConfig.SearchServiceEndpoint, searchAdminKey);



            Console.WriteLine("Getting Customer Search Index...\n");

            var index = await indexClient.GetIndexAsync(searchConfig.SearchIndexName);
            var indexer = await indexerClient.GetIndexerAsync(searchConfig.CustomerSearchConfig.SearchIndexerName);

            
            try
            {

                Console.WriteLine("Attempting to Update Customer Indexer...\n");
                var response = await CustomerIndexer.RunUpdateCustomerIndexer(searchAdminKey, searchConfig, index);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully Updated Customer Indexer...\n");
                }
                else
                {
                    Console.WriteLine("Error Updating Customer Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Creating Customer Indexer...\n Error: {e}");
                throw;
            }

            try
            {
                Console.WriteLine("Deleting Address Indexer...\n");
                if (await indexerClient.GetIndexerAsync(searchConfig.AddressSearchConfig.SearchIndexerName) != null)
                {
                    await indexerClient.DeleteIndexerAsync(searchConfig.AddressSearchConfig.SearchIndexerName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Deleting Address Indexer...\n Error: {e}");
                throw;
            }

            try
            {

                Console.WriteLine("Attempting to Create Address Indexer...\n");
                var response = await AddressIndexer.RunCreateAddressIndexer(searchAdminKey, searchConfig, index);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully Created Address Indexer...\n");
                }
                else
                {
                    Console.WriteLine("Error Creating Address Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Creating Address Indexer...\n Error: {e}");
                throw;
            }

            try
            {

                Console.WriteLine("Deleting Contact Search Details Indexer...\n");
                if (await indexerClient.GetIndexerAsync(searchConfig.ContactDetailsSearchConfig.SearchIndexerName) != null)
                {
                    await indexerClient.DeleteIndexerAsync(searchConfig.ContactDetailsSearchConfig.SearchIndexerName);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Deleting Contact Details Indexer...\n Error: {e}");
                throw;
            }

            try
            {

                Console.WriteLine("Attempting to Create Contact Details Indexer...\n");
                var response = await ContactDetailsIndexer.RunCreateContactDetailsIndexer(searchAdminKey, searchConfig, index);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully Created Contact Details Indexer...\n");
                }
                else
                {
                    Console.WriteLine("Error Creating Contact Details Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Creating Contact Details Indexer...\n Error: {e}");
                throw;
            }

            Console.WriteLine($"Completed...  Status: {HttpStatusCode.Created}");
        }

    }
}