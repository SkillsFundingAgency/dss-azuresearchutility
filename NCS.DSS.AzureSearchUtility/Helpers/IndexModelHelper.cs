﻿using System.Linq;
using Microsoft.Azure.Search.Models;

namespace NCS.DSS.AzureSearchUtility.Helpers
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
                    new Field("DateOfRegistration", DataType.DateTimeOffset){ IsFilterable = true, IsSortable = true},
                    new Field("Title", DataType.Int64),
                    new Field("GivenName", DataType.String) { IsSearchable = true, IsFilterable = true, IsSortable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("FamilyName", DataType.String) { IsSearchable = true, IsFilterable = true, IsSortable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("UniqueLearnerNumber", DataType.String) {IsSearchable = true },
                    new Field("DateofBirth", DataType.DateTimeOffset) { IsFilterable = true, IsSortable = true},
                    new Field("Gender", DataType.Int64) {IsFilterable = true},
                    new Field("OptInUserResearch", DataType.Boolean) {IsFilterable = true},
                    new Field("OptInMarketResearch", DataType.Boolean) {IsFilterable = true},
                    new Field("DateOfTermination", DataType.DateTimeOffset){IsFilterable = true},
                    new Field("ReasonForTermination", DataType.Int64) {IsFilterable = true},
                    new Field("IntroducedBy", DataType.Int64) {IsFilterable = true, IsSortable = true},
                    new Field("IntroducedByAdditionalInfo", DataType.String),
                    new Field("LastModifiedDate", DataType.DateTimeOffset),
                    new Field("LastModifiedTouchpointId", DataType.String) {IsFilterable = true},
                    new Field("Address1", DataType.String) { IsSearchable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("PostCode", DataType.String) { IsSearchable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("MobileNumber", DataType.String) { IsSearchable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("HomeNumber", DataType.String) { IsSearchable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("AlternativeNumber", DataType.String) {IsSearchable = true, Analyzer = AnalyzerName.EnLucene},
                    new Field("EmailAddress", DataType.String) { IsSearchable = true, Analyzer = AnalyzerName.EnLucene}
                }
            };

            return indexModel;
        }

        public static Index AddSynonymMapsToFields(Index index)
        {
            index.Fields.First(f => f.Name == "GivenName").SynonymMaps = new[] { "givenname-synonymmap" };
            return index;
        }
    }
}
