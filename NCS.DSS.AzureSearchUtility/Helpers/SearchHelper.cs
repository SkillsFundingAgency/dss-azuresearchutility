using System;
using System.IO;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class SearchHelper
    {
        private static string _searchServiceKey;
        private static SearchServiceClient _serviceClient;
        private static ISearchIndexClient _indexClient;

        public static SearchServiceClient GetSearchServiceClient(string searchServiceName, string searchServiceKey)
        {
            _searchServiceKey = searchServiceKey;

            if (_serviceClient != null)
                return _serviceClient;

            _serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(_searchServiceKey));

            if (_serviceClient == null)
                throw new ArgumentNullException("_serviceClient doesn't exist");

            return _serviceClient;
        }
        
        public static ISearchIndexClient GetIndexClient(string indexName)
        {
            if (_indexClient != null)
                return _indexClient;

            _indexClient = _serviceClient?.Indexes?.GetClient(indexName);

            if(_indexClient == null)
                throw new ArgumentNullException("_indexClient doesn't exist");

            return _indexClient;
        }

        public static void DeleteIndexIfExists(string indexName)
        {
            if (_serviceClient == null)
                throw new ArgumentNullException("_serviceClient doesn't exist");

            if (_serviceClient.Indexes.Exists(indexName))
            {
                _serviceClient.Indexes.Delete(indexName);
            }
        }

        public static void UploadSynonymsForGivenName(string synonymPath)
        {
            string synonymData;

            try
            {
                using (var sr = new StreamReader(synonymPath))
                {
                    synonymData = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var synonymMap = new SynonymMap()
            {
                Name = "givenname-synonymmap",
                ETag = "solr",
                Synonyms = synonymData
            };

            _serviceClient.SynonymMaps.CreateOrUpdate(synonymMap);
        }

        public static void CreateIndex(Microsoft.Azure.Search.Models.Index index)
        {
            _serviceClient?.Indexes?.Create(index);
        }
    }
}
