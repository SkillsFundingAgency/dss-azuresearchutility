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

            if (string.IsNullOrEmpty(config.SearchServiceName))
                throw new ArgumentNullException("SearchServiceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchIndexName))
                throw new ArgumentNullException("CustomerSearchIndexName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchIndexerName))
                throw new ArgumentNullException("CustomerSearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchDataSourceName))
                throw new ArgumentNullException("CustomerSearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerSearchDataSourceQuery))
                throw new ArgumentNullException("CustomerSearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerCollectionId))
                throw new ArgumentNullException("CustomerCollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.CustomerConnectionString))
                throw new ArgumentNullException("CustomerConnectionString is missing from /SearchConfigFile file");


            if (string.IsNullOrEmpty(config.AddressSearchIndexerName))
                throw new ArgumentNullException("AddressSearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressSearchDataSourceName))
                throw new ArgumentNullException("AddressSearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressSearchDataSourceQuery))
                throw new ArgumentNullException("AddressSearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressCollectionId))
                throw new ArgumentNullException("AddressCollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.AddressConnectionString))
                throw new ArgumentNullException("AddressConnectionString is missing from /SearchConfigFile file");


            if (string.IsNullOrEmpty(config.ContactDetailsSearchIndexerName))
                throw new ArgumentNullException("ContactDetailsSearchIndexerName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsSearchDataSourceName))
                throw new ArgumentNullException("ContactDetailsSearchDataSourceName is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsSearchDataSourceQuery))
                throw new ArgumentNullException("ContactDetailsSearchDataSourceQuery is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsCollectionId))
                throw new ArgumentNullException("ContactDetailsCollectionId is missing from /SearchConfigFile file");

            if (string.IsNullOrEmpty(config.ContactDetailsConnectionString))
                throw new ArgumentNullException("ContactDetailsConnectionString is missing from /SearchConfigFile file");


            return config;
        }
    }
}
