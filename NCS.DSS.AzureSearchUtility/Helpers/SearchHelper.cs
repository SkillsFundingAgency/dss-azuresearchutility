using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System;
using System.IO;
using System.Linq;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class SearchHelper
    {
        private static SearchIndexerClient _indexerClient;
        private static SearchIndexClient _indexClient;

        public static SearchIndexerClient GetSearchServiceClient(string searchServiceEndpoint, string searchServiceKey)
        {
            if (_indexerClient != null)
            {
                return _indexerClient;
            }

            _indexerClient = new SearchIndexerClient(new Uri(searchServiceEndpoint), new AzureKeyCredential(searchServiceKey));

            return _indexerClient;
        }

        public static void DeleteIndexIfExists(string indexName)
        {
            if (_indexClient == null)
            {
                throw new ArgumentNullException($"{_indexClient} is null");
            }

            if (_indexClient.GetIndexNames().Contains(indexName))
            {
                _indexClient.DeleteIndex(indexName);
            }
        }

        public static void UploadSynonymsForGivenName(string synonymPath)
        {
            string synonymData;

            if (!File.Exists(synonymPath))
            {
                throw new FileNotFoundException($"Synonym file not found: {synonymPath}");
            }

            try
            {
                using var sr = new StreamReader(synonymPath);
                synonymData = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (string.IsNullOrWhiteSpace(synonymData))
            {
                throw new InvalidOperationException("Synonym file is empty.");
            }

            var synonymMap = new SynonymMap("givenname-synonymmap", synonymData);

            _indexClient.CreateOrUpdateSynonymMap(synonymMap);
        }

        public static void CreateIndex(SearchIndex index)
        {
            _indexClient.CreateOrUpdateIndex(index);
        }
    }
}
