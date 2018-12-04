using NCS.DSS.AzureSearchUtilities.CreateIndex;
using NCS.DSS.AzureSearchUtilities.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace NCS.DSS.AzureSearchUtilities
{
    public class RunIndex
    {
        /// <param name="args"> Command line arguments: /SearchAdminKey:blah /CosmosAccount:dss-at-shared-cdb /CosmosAccountKey:blah /SearchConfigFile:pathtoblah</param>
        public static void Main(string[] args)
        {
            var searchAdminKey = string.Empty;
            var cosmosAccount = string.Empty;
            var cosmosAccountKey = string.Empty;
            string cosmosConnectionString;
            string searchConfigFile = string.Empty;

            if (args.Length == 0)
                throw (new NotSupportedException("Missing arguments"));

            foreach (var arg in args)
            {
                if(arg.StartsWith("/SearchAdminKey:"))
                {
                    searchAdminKey = arg.Split(':')[1];
                }
                else if (arg.StartsWith("/CosmosAccount:"))
                {
                    cosmosAccount = arg.Split(':')[1];
                }
                else if (arg.StartsWith("/CosmosAccountKey:"))
                {
                    cosmosAccountKey = arg.Split(':')[1];
                }
                else if (arg.StartsWith("/SearchConfigFile:"))
                {
                    searchConfigFile = arg.Split(':')[1];
                }
                else
                {
                    throw (new NotSupportedException(String.Format("Argument: {0} is invalid", arg)));
                }
            }

            if (cosmosAccount.Length > 0 && cosmosAccountKey.Length > 0)
            {
                cosmosConnectionString = $"AccountEndpoint=https://{cosmosAccount}.documents.azure.com:443/;AccountKey={cosmosAccountKey};Database=customers;";
            }
            else
            {
                throw new ArgumentNullException("Connection string not created, check /CosmosAccount: and /CosmosAccountKey both have valid values");
            }

            if (String.IsNullOrEmpty(searchAdminKey))
                throw new ArgumentNullException("Check /SearchAdminKey: has a valid value");

            if (String.IsNullOrEmpty(searchConfigFile))
                throw new ArgumentNullException("Check /SearchConfigFile: is a valid path");

            SearchConfig searchConfig = RunIndex.GetAppConfig(searchConfigFile);

            new CreateCustomerSearchIndex().CreateIndex(searchAdminKey, cosmosConnectionString, searchConfig).GetAwaiter().GetResult();

        }

        private static SearchConfig GetAppConfig(string filePath)
        {
            SearchConfig config = new SearchConfig();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    String json = sr.ReadToEnd();
                    config = JsonConvert.DeserializeObject<SearchConfig>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            if (string.IsNullOrEmpty(config.CustomerCollectionId))
                throw new ArgumentNullException("CustomerCollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchDataSourceName))
                throw new ArgumentNullException("CustomerSearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchDataSourceQuery))
                throw new ArgumentNullException("CustomerSearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchIndexName))
                throw new ArgumentNullException("CustomerSearchIndexName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchIndexerName))
                throw new ArgumentNullException("CustomerSearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.SearchServiceName))
                throw new ArgumentNullException("SearchServiceName is missing from /SearchConfigFile file");

            return config;
        }
    }
}
