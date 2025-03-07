using DFC.Swagger.Standard.Annotations;
using NCS.DSS.AzureSearchUtility.ReferenceData;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NCS.DSS.AzureSearchUtility.Models
{
    public class CustomerSearch
    {
        // Customer

        [Display(Description = "Unique identifier of a customer")]
        [Example(Description = "b8592ff8-af97-49ad-9fb2-e5c3c717fd85")]
        [JsonPropertyName("id")]
        public Guid? CustomerId { get; set; }

        [Display(Description = "Date and time the customer was first recognised by the National Careers Service")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateOfRegistration { get; set; }

        [Display(Description = "Customers given title.")]
        [Example(Description = "1")]
        public Title? Title { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z ]+((['\,\.\- ][a-zA-Z ])?[a-zA-Z ]*)*$")]
        [Display(Description = "Customers first or given name")]
        [Example(Description = "Boris")]
        [StringLength(100)]
        public string GivenName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z ]+((['\,\.\- ][a-zA-Z ])?[a-zA-Z ]*)*$")]
        [Display(Description = "Customers family or surname")]
        [Example(Description = "Johnson")]
        [StringLength(100)]
        public string FamilyName { get; set; }

        [Display(Description = "Customers date of birth")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateofBirth { get; set; }

        [Display(Description = "Customers gender.")]
        [Example(Description = "3")]
        public Gender? Gender { get; set; }

        [Display(Description = "Customers unique learner number as issued by the learning record service")]
        [Example(Description = "3000000000")]
        [StringLength(10)]
        public string UniqueLearnerNumber { get; set; }

        [Display(Description = "An indicator to show whether an individual wishes to participate in User Research or not")]
        [Example(Description = "true/false")]
        public bool? OptInUserResearch { get; set; }

        [Display(Description = "An indicator to show whether an individual wishes to participate in Market Research or not")]
        [Example(Description = "true/false")]
        public bool? OptInMarketResearch { get; set; }

        [Display(Description = "Date the customer terminated their account")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? DateOfTermination { get; set; }

        [Display(Description = "Reason for why the customer terminated their account.")]
        [Example(Description = "3")]
        public ReasonForTermination? ReasonForTermination { get; set; }

        [Display(Description = "Introduced By.")]
        [Example(Description = "12345")]
        public IntroducedBy? IntroducedBy { get; set; }

        [Display(Description = "Additional information on how the customer was introduced to the National Careers Service")]
        [Example(Description = "Customer was introduced to NCS by party X on date Y")]
        [StringLength(100)]
        public string IntroducedByAdditionalInfo { get; set; }

        [Display(Description = "Identifier supplied by the touchpoint to indicate their subcontractor")]
        [Example(Description = "01234567899876543210")]
        public string SubcontractorId { get; set; }

        [Display(Description = "Date and time of the last modification to the record")]
        [Example(Description = "2018-06-21T14:45:00")]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        [Example(Description = "0000000001")]
        public string LastModifiedTouchpointId { get; set; }

        // Address
        [Display(Description = "Customer home address line 1")]
        [Example(Description = "Adddress Line 1")]
        public string Address1 { get; set; }
        [Display(Description = "Customers postcode within England.")]
        [Example(Description = "AA11AA")]
        public string PostCode { get; set; }

        // Contact Details
        [StringLength(20)]
        [Display(Description = "Customer UK mobile phone number")]
        [Example(Description = "UK mobile phone number with optional +44 national code, also allows optional brackets and spaces at appropriate positions e.g.   07222 555555 , (07222) 555555 , +44 7222 555 555 or 07222 55555, (07222) 55555, +44 7222 555 55")]
        public string MobileNumber { get; set; }

        [Display(Description = "Customer UK home phone number")]
        [Example(Description = "UK phone number. Allows 3, 4 or 5 digit regional prefix, with 8/7, 7/6 or 6/5 digit phone number respectively, plus optional 3 or 4 digit extension number prefixed with a # symbol. " +
                                "Also allows optional brackets surrounding the regional prefix and optional spaces between appropriate groups of numbers    e.g.   " +
                                "01222 555 555   or   (010) 55555555 #2222   or   0122 555 5555#222")]
        public string HomeNumber { get; set; }

        [Display(Description = "Customer alternative phone number")]
        [Example(Description = "Alternative UK phone number. Allows 3, 4 or 5 digit regional prefix, with 8/7, 7/6 or 6/5 digit phone number respectively, plus optional 3 or 4 digit extension number prefixed with a # symbol. " +
                               "Also allows optional brackets surrounding the regional prefix and optional spaces between appropriate groups of numbers    e.g.   " +
                               "01222 555 555   or   (010) 55555555 #2222   or   0122 555 5555#222")]
        public string AlternativeNumber { get; set; }

        [Display(Description = "Customer email address")]
        [Example(Description = "user@organisation.com")]
        public string EmailAddress { get; set; }
    }
}