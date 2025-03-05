using System.Linq;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using NCS.DSS.AzureSearchUtility.Models;

namespace NCS.DSS.AzureSearchUtility.Helpers
{
    public static class IndexModelHelper
    {
        public static SearchIndex CreateIndexModelForCustomer(string indexName)
        {
            if (string.IsNullOrWhiteSpace(indexName))
                return null;

            var indexModel = new SearchIndex(indexName)
            {
                Fields = new FieldBuilder().Build(typeof(CustomerIndexFields))
            };

            return indexModel;
        }

        public static SearchIndex AddSynonymMapsToFields(SearchIndex index)
        {
            var givenNameField = index.Fields.FirstOrDefault(f => f.Name == "GivenName");
            if (givenNameField != null)
            {
                givenNameField.SynonymMapNames.Add("givenname-synonymmap");
            }
            return index;
        }
    }
}