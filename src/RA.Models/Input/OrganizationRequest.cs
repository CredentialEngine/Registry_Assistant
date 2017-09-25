using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Class used with an Organization format or publish request
    /// </summary>
    public class OrganizationRequest
    {
        public OrganizationRequest()
        {
            Organization = new Organization();
        }

        /// <summary>
        /// Organization Input Class
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// API key for the requesting partner - required for publishing
        /// May ultimately be passed in the header
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        /// Envelope Identifier
        /// Previously required to update an existing document
        /// </summary>
        public string RegistryEnvelopeId { get; set; }

    }

    public class Organization
    {
        public Organization()
        {

            AgentType = new List<string>();

            AgentSectorType = new List<string>();
            Industries = new List<FrameworkItem>();
            Keyword = new List<string>();
            Email = new List<string>();
            SocialMedia = new List<string>();
            SameAs = new List<string>();
            AvailabilityListing = new List<string>();
            ServiceType = new List<string>();
            Jurisdiction = new List<Input.Jurisdiction>();
            Address = new List<PostalAddress>();
            ContactPoint = new List<ContactPoint>();
            //
            AccreditedBy = new List<Input.OrganizationReference>();
            ApprovedBy = new List<Input.OrganizationReference>();
            RecognizedBy = new List<Input.OrganizationReference>();
            RegulatedBy = new List<Input.OrganizationReference>();
			//
            Approves = new List<EntityReference>();
            Offers = new List<EntityReference>();
            Owns = new List<EntityReference>();
            Renews = new List<EntityReference>();
            Revokes = new List<EntityReference>();
            Recognizes = new List<EntityReference>();
			//
            HasConditionManifests = new List<Input.EntityReference>();
            HasCostManifests = new List<EntityReference>();
            VerificationServiceProfiles = new List<Input.VerificationServiceProfile>();
			//
            AdministrationProcess = new List<ProcessProfile>();
            DevelopmentProcess = new List<ProcessProfile>();
            MaintenanceProcess = new List<ProcessProfile>();
            AppealProcess = new List<ProcessProfile>();
            ComplaintProcess = new List<ProcessProfile>();
            ReviewProcess = new List<ProcessProfile>();
            RevocationProcess = new List<ProcessProfile>();
			//
            Department = new List<OrganizationReference>();
            SubOrganization = new List<OrganizationReference>();
            JurisdictionAssertions = new List<JurisdictionAssertedInProfile>();
        }

        /// <summary>
        /// The type of organization is one of :
        /// - CredentialOrganization
        /// - QACredentialOrganization
        /// Required
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Name 
        /// Required
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Organization description 
        /// Required
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Credential Identifier
        /// format: 
        /// ce-UUID (guid)
        /// Required
        /// </summary>
        public string Ctid { get; set; }

        /// <summary>
        /// Organization subject web page
        /// Required
        /// </summary>
        public string SubjectWebpage { get; set; }

        /// <summary>
        /// The type of the described agent.
        /// Must provide valid organization types
        /// </summary>
        public List<string> AgentType { get; set; }

        /// <summary>
        /// Url for Organization image
        /// </summary>
        public string Image { get; set; }

        public string FoundingDate { get; set; }

        public string Duns { get; set; }
        public string Fein { get; set; }
        public string IpedsId { get; set; }
        public string OpeId { get; set; }
        public string AlternativeIdentifier { get; set; }
        public string MissionAndGoalsStatement { get; set; }
        public string MissionAndGoalsStatementDescription { get; set; }
        public string AgentPurpose { get; set; }
        public string AgentPurposeDescription { get; set; }


        /// <summary>
        /// Need process to provide valid types (agentSectorType)
        /// </summary>
        public List<string> AgentSectorType { get; set; }
        public List<FrameworkItem> Industries { get; set; }
        public List<string> Keyword { get; set; }

        public List<string> Email { get; set; }

        public List<string> SocialMedia { get; set; }

        //all phone numbers are entered in contact points

        public List<ContactPoint> ContactPoint { get; set; }
        /// <summary>
        /// A resource that unambiguously indicates the identity of the resource being described.
        /// Resources that may indicate identity include, but are not limited to, descriptions of entities in open databases such as DBpedia and Wikidata or social media accounts such as FaceBook and LinkedIn.
        /// </summary>
        public List<string> SameAs { get; set; }
        public List<string> AvailabilityListing { get; set; }
        public List<string> ServiceType { get; set; }
        public List<Jurisdiction> Jurisdiction { get; set; }
        public List<PostalAddress> Address { get; set; }


        public List<OrganizationReference> AccreditedBy { get; set; }
        public List<OrganizationReference> ApprovedBy { get; set; }
        public List<OrganizationReference> RecognizedBy { get; set; }
        public List<OrganizationReference> RegulatedBy { get; set; }
        public List<JurisdictionAssertedInProfile> JurisdictionAssertions { get; set; }

		/// <summary>
		/// Organization Approves these entities
		/// The entities could be any of credential, assessment, or learning opportunity
		/// </summary>
        public List<EntityReference> Approves { get; set; }
        public List<EntityReference> Offers { get; set; }
        public List<EntityReference> Owns { get; set; }

        public List<EntityReference> Renews { get; set; }
        public List<EntityReference> Revokes { get; set; }
        public List<EntityReference> Recognizes { get; set; }

        /// <summary>
        /// Reference to condition manifests
        /// A condition manifest must have already been published, in which case the URL is entered for the EntityReference.Id, or if the condition manifest exist at a url, provide:
        /// <list type="steps"><item>Name</item><item>Description</item><item>Subject webpage</item></list>
        /// </summary>
        public List<EntityReference> HasConditionManifests { get; set; }
        public List<EntityReference> HasCostManifests { get; set; }

        public List<VerificationServiceProfile> VerificationServiceProfiles { get; set; }
        public List<ProcessProfile> AdministrationProcess { get; set; }
        public List<ProcessProfile> DevelopmentProcess { get; set; }
        public List<ProcessProfile> MaintenanceProcess { get; set; }
        public List<ProcessProfile> AppealProcess { get; set; }
        public List<ProcessProfile> ComplaintProcess { get; set; }
        public List<ProcessProfile> ReviewProcess { get; set; }
        public List<ProcessProfile> RevocationProcess { get; set; }

        public List<OrganizationReference> Department { get; set; }
        public List<OrganizationReference> SubOrganization { get; set; }

    }
}
