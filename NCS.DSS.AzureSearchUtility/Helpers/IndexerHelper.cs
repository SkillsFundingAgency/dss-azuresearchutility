using System;
using System.Collections.Generic;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class IndexerHelper
    {
        public static Indexer CreateIndexer(SearchServiceClient searchService, Index index, string indexerName, string dataSourceName, List<FieldMapping> fieldMappings)
        {
            if (searchService == null)
                return null;

            if (index == null)
                return null;

            var indexer = new Indexer(
                indexerName,
                dataSourceName,
                index.Name,
                fieldMappings: fieldMappings,
                schedule: new IndexingSchedule(TimeSpan.FromHours(2)));

            DeleteIndexer(searchService, indexer);

            CreateIndexerOnSearchService(searchService, indexer);

            return indexer;
        }

        private static void DeleteIndexer(SearchServiceClient searchService, Indexer indexer)
        {
            if (searchService == null)
                return;

            if (indexer == null)
                return;

            var exists = searchService.Indexers.ExistsAsync(indexer.Name).GetAwaiter().GetResult();
            if (exists)
            {
                searchService.Indexers.ResetAsync(indexer.Name).Wait();
            }
        }

        private static void CreateIndexerOnSearchService(SearchServiceClient searchService, Indexer indexer)
        {
            if (searchService == null)
                return;

            if (indexer == null)
                return;

            searchService.Indexers.CreateOrUpdateAsync(indexer).Wait();
        }
    }
}