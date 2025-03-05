using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System;

namespace NCS.DSS.AzureSearchUtility.Models
{
    public class CustomerIndexFields
    {
        [SimpleField(IsKey = true)]
        public string CustomerId { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTimeOffset DateOfRegistration { get; set; }

        [SimpleField]
        public long Title { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string GivenName { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string FamilyName { get; set; }

        [SearchableField]
        public string UniqueLearnerNumber { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTimeOffset DateOfBirth { get; set; }

        [SimpleField(IsFilterable = true)]
        public long Gender { get; set; }

        [SimpleField(IsFilterable = true)]
        public bool OptInUserResearch { get; set; }

        [SimpleField(IsFilterable = true)]
        public bool OptInMarketResearch { get; set; }

        [SimpleField(IsFilterable = true)]
        public DateTimeOffset DateOfTermination { get; set; }

        [SimpleField(IsFilterable = true)]
        public long ReasonForTermination { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public long IntroducedBy { get; set; }

        [SimpleField]
        public string IntroducedByAdditionalInfo { get; set; }

        [SimpleField]
        public DateTimeOffset LastModifiedDate { get; set; }

        [SimpleField(IsFilterable = true)]
        public string LastModifiedTouchpointId { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Address1 { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string PostCode { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string MobileNumber { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string HomeNumber { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string AlternativeNumber { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string EmailAddress { get; set; }
    }
}
