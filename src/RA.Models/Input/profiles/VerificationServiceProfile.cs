using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Common input class for all verification profiles
    /// 2018-09-02 Where LanguageMap alternates are available, only enter one. The system will check the string version first. 
    /// </summary>
    public class VerificationServiceProfile
    {
        public VerificationServiceProfile()
        {
            EstimatedCost = new List<CostProfile>();
            Jurisdiction = new List<JurisdictionProfile>();
            OfferedBy = new List<Input.OrganizationReference>();
            TargetCredential = new List<EntityReference>();
            VerificationDirectory = new List<string>();
            VerificationService = new List<string>();
            VerifiedClaimType = new List<string>();
        }

        /// <summary>
        /// Globally unique Credential Transparency Identifier (CTID) 
        /// Required
        /// </summary>
        public string CTID { get; set; }

        /// <summary>
        /// Description of this profile
        /// Required
        /// </summary>
        public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Agent that offers the resource.
        /// Required
        /// </summary>
        public List<OrganizationReference> OfferedBy { get; set; }

		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }

        /// <summary>
        /// Estimated cost of this service
        /// </summary>
		public List<CostProfile> EstimatedCost { get; set; }

        /// <summary>
        /// Whether or not the credential holder must authorize the organization to provide the verification service.
        /// </summary>
        public bool? HolderMustAuthorize { get; set; }

        /// <summary>
        /// Webpage that describes this entity.
        /// </summary>
        public string SubjectWebpage { get; set; }

        /// <summary>
        /// Credential that is a focus or target of the condition, process or verification service.
        /// </summary>
        public List<EntityReference> TargetCredential { get; set; }

        /// <summary>
        /// Directory of credential holders and their current statuses.
        /// </summary>
        public List<string> VerificationDirectory { get; set; }

        /// <summary>
        /// Textual description of the methods used to evaluate an assessment, learning opportunity, process or verificaiton service for validity or reliability.
        /// </summary>
        public string VerificationMethodDescription { get; set; }
        public LanguageMap VerificationMethodDescription_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Direct access to the verification service.
        /// This property identifies machine-accessible services, such as API endpoints, that provide direct access to the verification service being described.
        /// xsd:anyURI
        /// </summary>
        public List<string> VerificationService { get; set; }

        /// <summary>
        /// Type of claim provided through a verification service; select from an existing enumeration of such types.
        /// Best practice for developing credential descriptions for the Credential Engine Registry is to use the ceterms:ClaimType vocabulary.
        /// CER Target Scheme:	ceterms:ClaimType
        /// </summary>
        public List<string> VerifiedClaimType { get; set; }

        /// <summary>
        /// Geographic or political region in which the credential is formally applicable or an organization has authority to act.
        /// </summary>
        public List<JurisdictionProfile> Jurisdiction { get; set; }
        /// <summary>
        /// List of Organizations that offer this entity in a specific Jurisdiction. 
        /// </summary>
        public List<JurisdictionAssertion> OfferedIn { get; set; } = new List<JurisdictionAssertion>();

    }
}



