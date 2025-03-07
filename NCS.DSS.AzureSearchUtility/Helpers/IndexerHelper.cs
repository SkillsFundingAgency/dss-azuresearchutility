using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System;
using System.Threading.Tasks;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class IndexerHelper
    {
        public static async Task<SearchIndexer> CreateIndexerAsync(SearchIndexerClient searchIndexerClient, SearchIndex index, string indexerName, string dataSourceName, FieldMapping fieldMapping)
        {
            if (searchIndexerClient == null || index == null)
            {
                return null;
            }

            var indexer = new SearchIndexer(
                name: indexerName,
                dataSourceName: dataSourceName,
                targetIndexName: index.Name
            )
            {
                Schedule = new IndexingSchedule(TimeSpan.FromHours(2)),
                FieldMappings =
                {
                    fieldMapping
                }
            };

            await DeleteIndexerAsync(searchIndexerClient, indexer);
            await CreateIndexerOnSearchServiceAsync(searchIndexerClient, indexer);

            return indexer;
        }

        private static async Task DeleteIndexerAsync(SearchIndexerClient searchIndexerClient, SearchIndexer indexer)
        {
            if (searchIndexerClient == null || indexer == null)
            {
                return;
            }

            try
            {
                await searchIndexerClient.DeleteIndexerAsync(indexer.Name);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                Console.WriteLine($"Indexer '{indexer.Name}' not found, skipping delete.");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error deleting indexer: {e.Message}");
                throw;
            }
        }

        private static async Task CreateIndexerOnSearchServiceAsync(SearchIndexerClient searchIndexerClient, SearchIndexer indexer)
        {
            if (searchIndexerClient == null || indexer == null)
            {
                return;
            }

            try
            {
                await searchIndexerClient.CreateOrUpdateIndexerAsync(indexer);
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error creating/updating indexer: {e.Message}");
                throw;
            }
        }
    }
}
