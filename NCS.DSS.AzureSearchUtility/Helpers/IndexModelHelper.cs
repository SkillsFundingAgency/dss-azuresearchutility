using Microsoft.Azure.Search.Models;

namespace NCS.DSS.AzureSearchUtilities.Helpers
{
    public static class IndexModelHelper
    {
        public static Index CreateIndexModelForCustomer(string indexName)
        {
            if (string.IsNullOrWhiteSpace(indexName))
                return null;

            var indexModel = new Index
            {
                Name = indexName,
                Fields = new[]
                {
                    new Field("CustomerId", DataType.String) { IsKey = true},
                    new Field("DateOfRegistration", DataType.DateTimeOffset),
                    new Field("Title", DataType.Int64),
                    new Field("GivenName", DataType.String) { IsSearchable = true, IsFilterable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("FamilyName", DataType.String) { IsSearchable = true, IsFilterable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("UniqueLearnerNumber", DataType.String) {IsSearchable = true },
                    new Field("DateofBirth", DataType.DateTimeOffset) { IsFilterable = true},
                    new Field("Gender", DataType.Int64),
                    new Field("OptInUserResearch", DataType.Boolean),
                    new Field("OptInMarketResearch", DataType.Boolean),
                    new Field("DateOfTermination", DataType.DateTimeOffset),
                    new Field("ReasonForTermination", DataType.Int64),
                    new Field("IntroducedBy", DataType.Int64),
                    new Field("IntroducedByAdditionalInfo", DataType.String),
                    new Field("LastModifiedDate", DataType.DateTimeOffset),
                    new Field("LastModifiedTouchpointId", DataType.String),
                }
            };

            return indexModel;
        }
    }
}
