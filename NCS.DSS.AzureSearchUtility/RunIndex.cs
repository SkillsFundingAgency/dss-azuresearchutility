using NCS.DSS.AzureSearchUtility.CreateIndex;
using NCS.DSS.AzureSearchUtility.Helpers;
using NCS.DSS.AzureSearchUtility.Models;
using System;
using System.IO;
using System.Text.Json;

namespace NCS.DSS.AzureSearchUtility
{
    public static class RunIndex
    {
        /// <summary>
        /// Command-line arguments format:
        /// /SearchAdminKey:foo /SearchConfigFile:bar /EnvironmentName:AT /SynonymPath:baz
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var searchAdminKey = string.Empty;
            var searchConfigFile = string.Empty;
            var environmentName = string.Empty;
            var synonymPath = string.Empty;

            if (args.Length == 0)
            {
                throw (new NotSupportedException("Missing arguments"));
            }

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
                else if (arg.StartsWith("/EnvironmentName:"))
                {
                    environmentName = arg.Split(':')[1];
                }
                else if (arg.StartsWith("/SynonymPath:"))
                {
                    synonymPath = arg.Split(':')[1];
                }
                else
                {
                    throw (new NotSupportedException($"Argument: {arg} is invalid"));
                }
            }

            if (string.IsNullOrEmpty(searchAdminKey))
            {
                throw new ArgumentNullException(searchAdminKey);
            }

            if (string.IsNullOrEmpty(searchConfigFile))
            {
                throw new ArgumentNullException(searchConfigFile);
            }

            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException(environmentName);
            }

            if (string.IsNullOrEmpty(synonymPath))
            {
                throw new ArgumentNullException(synonymPath);
            }

            var searchConfig = GetAppConfig(searchConfigFile);

            CreateCustomerSearchIndex.CreateIndex(searchAdminKey, searchConfig, synonymPath).GetAwaiter().GetResult();

            Console.WriteLine("Generate Swagger File Name");
            var fileName = FileHelper.GenerateSwaggerFileName(environmentName);

            Console.WriteLine($"Generate File Path for Swagger Doc:{fileName}");
            var destPath = FileHelper.GenerateFilePath(fileName);

            Console.WriteLine("Generating Swagger Doc");
            var swaggerDoc = APIDefinition.GenerateAzureSearchSwaggerDoc.GenerateSwaggerDoc(searchConfig.SearchServiceName);

            Console.WriteLine($"Generate File for Swagger Doc: {destPath}");
            FileHelper.GenerateFileOnServer(destPath, swaggerDoc);
        }

        private static SearchConfig GetAppConfig(string filePath)
        {
            Console.WriteLine($"Attempting to read appconfig.json at path {filePath}");
            var config = new SearchConfig();
            try
            {
                using var sr = new StreamReader(filePath);
                var json = sr.ReadToEnd();
                config = JsonSerializer.Deserialize<SearchConfig>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            Console.WriteLine($"Successfully read appconfig.json at path {filePath}");

            if (string.IsNullOrWhiteSpace(config.SearchServiceName))
            {
                throw new ArgumentNullException(config.SearchServiceName);
            }

            if (string.IsNullOrWhiteSpace(config.SearchServiceEndpoint))
            {
                throw new ArgumentNullException(config.SearchServiceEndpoint);
            }

            if (string.IsNullOrWhiteSpace(config.SearchIndexName))
            {
                throw new ArgumentNullException(config.SearchIndexName);
            }

            if (string.IsNullOrWhiteSpace(config.CosmosDbConnectionString))
            {
                throw new ArgumentNullException(config.CosmosDbConnectionString);
            }

            if (string.IsNullOrWhiteSpace(config.CustomerSearchConfig.SearchIndexerName))
            {
                throw new ArgumentNullException(config.CustomerSearchConfig.SearchIndexerName);
            }

            if (string.IsNullOrWhiteSpace(config.CustomerSearchConfig.SearchDataSourceName))
            {
                throw new ArgumentNullException(config.CustomerSearchConfig.SearchDataSourceName);
            }

            if (string.IsNullOrWhiteSpace(config.CustomerSearchConfig.SearchDataSourceQuery))
            {
                throw new ArgumentNullException(config.CustomerSearchConfig.SearchDataSourceQuery);
            }

            if (string.IsNullOrEmpty(config.CustomerSearchConfig.CollectionId))
            {
                throw new ArgumentNullException(config.CustomerSearchConfig.CollectionId);
            }

            if (string.IsNullOrEmpty(config.CustomerSearchConfig.DatabaseId))
            {
                throw new ArgumentNullException(config.CustomerSearchConfig.DatabaseId);
            }


            if (string.IsNullOrEmpty(config.AddressSearchConfig.SearchIndexerName))
            {
                throw new ArgumentNullException(config.AddressSearchConfig.SearchIndexerName);
            }

            if (string.IsNullOrEmpty(config.AddressSearchConfig.SearchDataSourceName))
            {
                throw new ArgumentNullException(config.AddressSearchConfig.SearchIndexerName);
            }

            if (string.IsNullOrEmpty(config.AddressSearchConfig.SearchDataSourceQuery))
            {
                throw new ArgumentNullException(config.AddressSearchConfig.SearchDataSourceQuery);
            }

            if (string.IsNullOrEmpty(config.AddressSearchConfig.CollectionId))
            {
                throw new ArgumentNullException(config.AddressSearchConfig.CollectionId);
            }

            if (string.IsNullOrEmpty(config.AddressSearchConfig.DatabaseId))
            {
                throw new ArgumentNullException(config.AddressSearchConfig.DatabaseId);
            }


            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.SearchIndexerName))
            {
                throw new ArgumentNullException(config.ContactDetailsSearchConfig.SearchIndexerName);
            }

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.SearchDataSourceName))
            {
                throw new ArgumentNullException(config.ContactDetailsSearchConfig.SearchDataSourceName);
            }

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.SearchDataSourceQuery))
            {
                throw new ArgumentNullException(config.ContactDetailsSearchConfig.SearchDataSourceQuery);
            }

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.CollectionId))
            {
                throw new ArgumentNullException(config.ContactDetailsSearchConfig.CollectionId);
            }

            if (string.IsNullOrEmpty(config.ContactDetailsSearchConfig.DatabaseId))
            {
                throw new ArgumentNullException(config.ContactDetailsSearchConfig.DatabaseId);
            }

            PopulateConnectionStrings(config);

            Console.WriteLine($"Finished configuring settings for {nameof(AzureSearchUtility)}");
            return config;
        }

        private static void PopulateConnectionStrings(SearchConfig config)
        {
            var cosmosDbConnectionString = config.CosmosDbConnectionString;
            var addressDatabaseId = config.AddressSearchConfig.DatabaseId;
            var customerDatabaseId = config.CustomerSearchConfig.DatabaseId;
            var contactDetailsDatabaseId = config.ContactDetailsSearchConfig.DatabaseId;

            config.AddressSearchConfig.ConnectionString = $"{cosmosDbConnectionString}Database={addressDatabaseId};";
            config.CustomerSearchConfig.ConnectionString = $"{cosmosDbConnectionString}Database={customerDatabaseId};";
            config.ContactDetailsSearchConfig.ConnectionString = $"{cosmosDbConnectionString}Database={contactDetailsDatabaseId};";
        }
    }
}
