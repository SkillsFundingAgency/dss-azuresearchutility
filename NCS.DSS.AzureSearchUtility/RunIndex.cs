using System;
using System.IO;
using NCS.DSS.AzureSearchUtility.CreateIndex;
using NCS.DSS.AzureSearchUtility.Models;
using Newtonsoft.Json;

namespace NCS.DSS.AzureSearchUtility
{
    public class RunIndex
    {
        /// <param name="args"> Command line arguments: /SearchAdminKey:blah /SearchConfigFile:pathtoblah</param>
        public static void Main(string[] args)
        {
            var searchAdminKey = string.Empty;
            var searchConfigFile = string.Empty;

            if (args.Length == 0)
                throw (new NotSupportedException("Missing arguments"));

            foreach (var arg in args)
            {
                if(arg.StartsWith("/SearchAdminKey:"))
                {
                    searchAdminKey = arg.Split(':')[1];
                }
                else if (arg.StartsWith("/SearchConfigFile:"))
                {
                    searchConfigFile = arg.Split(':')[1];
                }
                else
                {
                    throw (new NotSupportedException(string.Format("Argument: {0} is invalid", arg)));
                }
            }

            if (string.IsNullOrEmpty(searchAdminKey))
                throw new ArgumentNullException("Check /SearchAdminKey: has a valid value");

            if (string.IsNullOrEmpty(searchConfigFile))
                throw new ArgumentNullException("Check /SearchConfigFile: is a valid path");

            var searchConfig = RunIndex.GetAppConfig(searchConfigFile);

            new CreateCustomerSearchIndex().CreateIndex(searchAdminKey, searchConfig).GetAwaiter().GetResult();

        }

        private static SearchConfig GetAppConfig(string filePath)
        {
            SearchConfig config = new SearchConfig();
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    var json = sr.ReadToEnd();
                    config = JsonConvert.DeserializeObject<SearchConfig>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            if (string.IsNullOrWhiteSpace(config.SearchServiceName))
                throw new ArgumentNullException("SearchServiceName is missing from /SearchConfigFile file");

            if (string.IsNullOrWhiteSpace(config.SearchIndexName))
                throw new ArgumentNullException("SearchIndexName is missing from /SearchConfigFile file");

            if (string.IsNullOrWhiteSpace(config.CosmosDbConnectionString))
                throw new ArgumentNullException("CosmosDBConnectionString is missing from /SearchConfigFile file");


            if (string.IsNullOrWhiteSpace(config.CustomerSearchConfig.SearchIndexerName))
                throw new ArgumentNullException("CustomerSearchConfig.SearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrWhiteSpace(config.CustomerSearchConfig.SearchDataSourceName))
                throw new ArgumentNullException("CustomerSearchConfig.SearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrWhiteSpace(config.CustomerSearchConfig.SearchDataSourceQuery))
                throw new ArgumentNullException("CustomerSearchConfig.SearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchConfig.CollectionId))
                throw new ArgumentNullException("CustomerSearchConfig.CollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchConfig.DatabaseId))
                throw new ArgumentNullException("CustomerSearchConfig.DatabaseId is missing from /SearchConfigFile file");
            

            if (string.IsNullOrEmpty(config.AddressSearchConfig.SearchIndexerName))
                throw new ArgumentNullException("AddressSearchConfig.SearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressSearchConfig.SearchDataSourceName))
                throw new ArgumentNullException("AddressSearchConfig.SearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressSearchConfig.SearchDataSourceQuery))
                throw new ArgumentNullException("AddressSearchConfig.SearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressSearchConfig.CollectionId))
                throw new ArgumentNullException("AddressSearchConfig.CollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressSearchConfig.DatabaseId))
                throw new ArgumentNullException("AddressSearchConfig.DatabaseId is missing from /SearchConfigFile file");


            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.SearchIndexerName))
                throw new ArgumentNullException("ContactDetailsSearchConfig.SearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.SearchDataSourceName))
                throw new ArgumentNullException("ContactDetailsSearchConfig.SearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.SearchDataSourceQuery))
                throw new ArgumentNullException("ContactDetailsSearchConfig.SearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.CollectionId))
                throw new ArgumentNullException("ContactDetailsSearchConfig.CollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.DatabaseId))
                throw new ArgumentNullException("ContactDetailsSearchConfig.DatabaseId is missing from /SearchConfigFile file");

            PopulateConnectionStrings(config);

            return config;
        }

        private static void PopulateConnectionStrings(SearchConfig config)
        {
            var cosmosDbConnectionString = config.CosmosDbConnectionString;
            var addressDatabaseId = config.AddressSearchConfig.DatabaseId;
            var customerDatabaseId = config.CustomerSearchConfig.DatabaseId;
            var contactDetailsDatabaseId = config.ContactDetailsSearchConfig.DatabaseId;

            config.AddressSearchConfig.ConnectionString = string.Format("{0}Database={1};", cosmosDbConnectionString, addressDatabaseId);
            config.CustomerSearchConfig.ConnectionString = string.Format("{0}Database={1};", cosmosDbConnectionString, customerDatabaseId);
            config.ContactDetailsSearchConfig.ConnectionString = string.Format("{0}Database={1};", cosmosDbConnectionString, contactDetailsDatabaseId);
        }
    }
}
