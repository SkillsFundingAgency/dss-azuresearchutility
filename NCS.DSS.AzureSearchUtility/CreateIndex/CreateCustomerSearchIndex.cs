using System;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.AzureSearchUtility.CreateIndexer;
using NCS.DSS.AzureSearchUtility.Helpers;
using NCS.DSS.AzureSearchUtility.Models;

namespace NCS.DSS.AzureSearchUtility.CreateIndex
{
    public class CreateCustomerSearchIndex
    {
        public async Task CreateIndex(string searchAdminKey, SearchConfig searchConfig)
        {

            Console.WriteLine("{0}", "Retrieving Search Service\n");
            var azureSearchService = SearchHelper.GetSearchServiceClient(searchConfig.SearchServiceName, searchAdminKey);

            if (azureSearchService == null)
            {
                throw new WebException("Unable to find Search Service");
            }

            Console.WriteLine("{0}", "Deleting Customer Search Index...\n");
            SearchHelper.DeleteIndexIfExists(searchConfig.SearchIndexName);

            Console.WriteLine("{0}", "Creating Customer Search Index Model...\n");
            var indexModelForCustomer = IndexModelHelper.CreateIndexModelForCustomer(searchConfig.SearchIndexName);

            if (indexModelForCustomer == null)
            {
                Console.WriteLine("Unable to create Index Model");
                throw new NullReferenceException("indexModelForCustomer");
            }

            Console.WriteLine("{0}", "Attempting to Create Customer Synonym Map...\n");
            IndexModelHelper.AddSynonymMapsToFields(indexModelForCustomer);

            Console.WriteLine("{0}", "Add Customer Synonym Map to service client...\n");
            SearchHelper.UploadSynonymsForGivenName();

            Console.WriteLine("{0}", "Creating Index for Customer Search...\n");
            SearchHelper.CreateIndex(indexModelForCustomer);

            try
            {

                Console.WriteLine("{0}", "Attempting to Create Customer Indexer...\n");
                var response = await CreateCustomerIndexer.RunCreateCustomerIndexer(searchAdminKey, searchConfig, indexModelForCustomer);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("{0}", "Successfully Created Customer Indexer...\n");
                }
                else
                {
                    Console.WriteLine("{0}", "Error Creating Customer Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Error: {1}", "Error Creating Customer Indexer...\n", e);
                throw;
            }

            try
            {

                Console.WriteLine("{0}", "Attempting to Create Address Indexer...\n");
                var response = await CreateAddressIndexer.RunCreateAddressIndexer(searchAdminKey, searchConfig, indexModelForCustomer);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("{0}", "Successfully Created Address Indexer...\n");
                }
                else
                {
                    Console.WriteLine("{0}", "Error Creating Address Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Error: {1}", "Error Creating Address Indexer...\n", e);
                throw;
            }

            try
            {

                Console.WriteLine("{0}", "Attempting to Create Contact Details Indexer...\n");
                var response = await CreateContactDetailsIndexer.RunCreateContactDetailsIndexer(searchAdminKey, searchConfig, indexModelForCustomer);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("{0}", "Successfully Created Contact Details Indexer...\n");
                }
                else
                {
                    Console.WriteLine("{0}", "Error Creating Contact Details Indexer...\n");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Error: {1}", "Error Creating Contact Details Indexer...\n", e);
                throw;
            }

            Console.WriteLine("{0} Status: {1}", "Completed... ", HttpStatusCode.Created);
        }
    }
}