using System;
using System.Collections.Generic;

namespace RA.Models.Input
{
	/// <summary>
	/// Class used with an Organization format or publish request
	/// </summary>
	public class OrganizationRequest : BaseRequest
	{
		public OrganizationRequest()
		{
			Organization = new Organization();
		}

		/// <summary>
		/// Organization Input Class
		/// </summary>
		public Organization Organization { get; set; }

		//public List<BlankNode> BlankNodes = new List<BlankNode>();
	}

	/// <summary>
	/// Organization
	/// Required:
	/// - CTID
	/// - Type
	/// - Name
	/// - Description
	/// - SubjectWebpage
	/// - AgentType
	/// - AgentSectorType
	/// - At least one of email or address
	/// 
	/// </summary>
	public class Organization
	{
		public Organization()
		{

			AgentType = new List<string>();

			//AgentSectorType = new List<string>();
			IndustryType = new List<FrameworkItem>();
			Naics = new List<string>();
			Keyword = new List<string>();
			Email = new List<string>();
			SocialMedia = new List<string>();
			SameAs = new List<string>();
			AvailabilityListing = new List<string>();
			ServiceType = new List<string>();
			Jurisdiction = new List<Input.Jurisdiction>();
			Address = new List<Place>();
			AlternateName = new List<string>();
			//ContactPoint = new List<ContactPoint>();
			//
			AccreditedBy = new List<Input.OrganizationReference>();
			ApprovedBy = new List<Input.OrganizationReference>();
			RecognizedBy = new List<Input.OrganizationReference>();
			RegulatedBy = new List<Input.OrganizationReference>();
			//
			Accredits = new List<EntityReference>();
			Approves = new List<EntityReference>();
			Offers = new List<EntityReference>();
			Owns = new List<EntityReference>();
			Renews = new List<EntityReference>();
			Revokes = new List<EntityReference>();
			Recognizes = new List<EntityReference>();
			//
			//OwnsCredentials = new List<EntityReference>();
			//OwnsAssessments = new List<EntityReference>();
			//OwnsLearningOpportunities = new List<EntityReference>();
			//
			HasConditionManifest = new List<string>();
			HasCostManifest = new List<string>();
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

		}

		#region *** Required Properties ***
		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// - Organization (new 2021-05-31)
		/// Required
		/// </summary>
		public string Type { get; set; }
		//Helper to check if the current organization is a QA organization
		public bool IsQAOrganization
		{
			get 
			{ 
				if ( !string.IsNullOrWhiteSpace(Type) && Type.ToLower().IndexOf("qacredentialorganization") > -1 )
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Name 
		/// Required
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Credential Identifier
		/// format: 
		/// ce-UUID (guid)
		/// Required
		/// </summary>
		public string CTID { get; set; }
		//original API used the following property. Both are supported but of course only one should be provided. CTID will take precedence. 
		public string Ctid { get; set; }

		/// <summary>
		/// Organization subject web page
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// The type of the described agent.
		/// Must provide valid organization types.
		/// May provide with or without the orgType namespace
		/// Required
		/// Example: orgType:CertificationBody
		/// <see href="https://credreg.net/ctdl/terms/agentType"></see>
		/// </summary>
		public List<string> AgentType { get; set; }

		/// <summary>
		/// The types of sociological, economic, or political subdivision of society served by an agent. 
		/// Required
		/// Enter one of:
		/// <value>
		/// agentSector:PrivateForProfit 
		/// agentSector:PrivateNonProfit 
		/// agentSector:Public
		/// </value>
		/// </summary>
		public string AgentSectorType { get; set; }


		//also require contact information via at least one of 
		/// <summary>
		/// Email addresses for organization
		/// Require at least Email or Address
		/// </summary>
		public List<string> Email { get; set; }
		/// <summary>
		/// Addresses for organization
		/// Require at least Email or Address
		/// </summary>
		public List<Place> Address { get; set; }
		#endregion

		#region *** Recommended Benchmark Properties ***
		/// <summary>
		/// Organization's primary purpose as found on an "about" page of a website.
		/// </summary>
		public string AgentPurpose { get; set; }

		/// <summary>
		/// Short, key phrases describing the primary purpose of an organization as might be derived from the "about" page of it's website.
		/// </summary>
		public string AgentPurposeDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap AgentPurposeDescription_Map { get; set; } = new LanguageMap();

		//External Quality Assurance
		/// <summary>
		/// Quality assurance organization that provides official authorization to this  organization.
		/// </summary>
		public List<OrganizationReference> AccreditedBy { get; set; }
		/// <summary>
		/// Organization that pronounces favorable judgment for this organization.
		/// </summary>
		public List<OrganizationReference> ApprovedBy { get; set; }
		/// <summary>
		/// Agent that acknowledges the validity of the organization.
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }
		/// <summary>
		/// Quality assurance organization that enforces the legal requirements of this organization
		/// </summary>
		public List<OrganizationReference> RegulatedBy { get; set; }

		/// <summary>
		/// Larger organization exercising authority over this organization.
		/// </summary>
		public List<OrganizationReference> ParentOrganization { get; set; }

		#endregion
		#region *** Recommended Properties ***

		/// <summary>
		/// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
		/// <see href="https://purl.org/ctdl/terms/identifier"></see>
		/// ceterms:identifier
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; } = new List<IdentifierValue>();

		/// <summary>
		/// Url for Organization image
		/// </summary>
		public string Image { get; set; }

		public List<string> Keyword { get; set; }
		public LanguageMapList Keyword_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Social media access point for an agent or an agent's contact point.
		/// List of URLs
		/// </summary>
		public List<string> SocialMedia { get; set; }

		/// <summary>
		/// Type of service offered by the agent being described; select from an existing enumeration of such terms.
		/// List of concepts from ConceptScheme: ceterms:AgentServiceType
		/// Valid values:
		/// serviceType:AccreditService 
		/// serviceType:ApproveService 
		/// serviceType:OfferService 
		/// serviceType:RecognizeService 
		/// serviceType:RegulateService 
		/// serviceType:RenewService
		/// </summary>
		public List<string> ServiceType { get; set; }
		#endregion


		/// <summary>
		/// Alias for the organization including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use
		/// </summary>
		public List<string> AlternateName { get; set; }
		/// <summary>
		/// alternate name using a LanguageMapList
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Listing of online and/or physical locations
		/// List of URLs
		/// </summary>
		public List<string> AvailabilityListing { get; set; }


		/// <summary>
		/// Founding Date - the year, year-month, or year-month-day the organization was founded. 
		/// Maximum length of 20
		/// Examples:
		/// 2000
		/// Jan, 2000
		/// Jan. 1, 2000
		/// November 11, 2000 (len=17)
		/// </summary>
		public string FoundingDate { get; set; }

		#region Concrete codes/identifiers
		public string Duns { get; set; }
		public string Fein { get; set; }


		public string IpedsId { get; set; }
		public string OpeId { get; set; }
		public string LEICode { get; set; }
		/// <summary>
		/// ISIC Revision 4 Code
		/// The International Standard of Industrial Classification of All Economic Activities (ISIC), Revision 4 code for a particular organization, business person, or place.
		/// </summary>
		public string ISICV4 { get; set; }
		/// <summary>
		/// Identifier comprised of a 12 digit code issued by the National Center for Education Statistics (NCES) for educational institutions where the first 7 digits are the NCES District ID.
		/// </summary>
		public string NcesID { get; set; }

		#endregion

		/// <summary>
		/// Type of official status of the TransferProfile; select from an enumeration of such types.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// </summary>
		public string LifeCycleStatusType { get; set; }

		/// <summary>
		/// Webpage or online document that defines or explains the nature of transfer value handled by the organization.
		/// URI
		/// </summary>
		public string TransferValueStatement { get; set; }
		/// <summary>
		/// Description of the nature of transfer value handled by the organization.
		/// </summary>
		public string TransferValueStatementDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap TransferValueStatementDescription_Map { get; set; } = new LanguageMap();

		//
		public List<Jurisdiction> Jurisdiction { get; set; }

		public string MissionAndGoalsStatement { get; set; }
		public string MissionAndGoalsStatementDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap MissionAndGoalsStatementDescription_Map { get; set; } = new LanguageMap();

		public List<FrameworkItem> IndustryType { get; set; }
		//public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		//public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();
		public List<string> Naics { get; set; }


		//NOTE: ContactPoint can only be entered with Address
		//[Obsolete]
		//public List<ContactPoint> ContactPoint { get; set; } = new List<ContactPoint>();

		/// <summary>
		/// A resource that unambiguously indicates the identity of the resource being described.
		/// Resources that may indicate identity include, but are not limited to, descriptions of entities in open databases such as DBpedia and Wikidata or social media accounts such as FaceBook and LinkedIn.
		/// </summary>
		public List<string> SameAs { get; set; }



		#region Quality Assurance IN - Jurisdiction based Quality Assurance  (INs)
		//There are currently two separate approaches to publishing properties like assertedIn
		//- Publish all 'IN' properties using JurisdictionAssertions
		//- Publish using ehe separate specific properties like AccreditedIn, ApprovedIn, etc
		// 2010-01-06 The property JurisdictionAssertions may become obsolete soon. We recomend to NOT use this property.


		/// <summary>
		/// List of Organizations that accredit this organization in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> AccreditedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that approve this organization in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> ApprovedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that recognize this organization in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RecognizedIn { get; set; } = new List<JurisdictionAssertion>();

		/// <summary>
		/// List of Organizations that regulate this organization in a specific Jurisdiction. 
		/// </summary>
		public List<JurisdictionAssertion> RegulatedIn { get; set; } = new List<JurisdictionAssertion>();

		#endregion

		/// <summary>
		/// A credentialling organization must own or offer something
		/// </summary>
		public List<EntityReference> Owns { get; set; }
		public List<EntityReference> Offers { get; set; }

		#region Quality Assurance Performed
		/// <summary>
		/// Organization performs QA on these entities
		/// A QA organization must have QA on at least one document
		/// The entities could be any of organization, credential, assessment, or learning opportunity
		/// </summary>
		public List<EntityReference> Accredits { get; set; }
		public List<EntityReference> Approves { get; set; }
		public List<EntityReference> Recognizes { get; set; }
		public List<EntityReference> Regulates { get; set; } = new List<EntityReference>();
		public List<EntityReference> Renews { get; set; }
		public List<EntityReference> Revokes { get; set; }

		#endregion

		/// <summary>
		/// Reference to condition manifests
		/// A condition manifest can never be a third party reference, so a url is expected
		/// </summary>
		public List<string> HasConditionManifest { get; set; }
		/// <summary>
		/// Reference to cost manifests
		/// A cost manifest can never be a third party reference, so a url is expected
		/// </summary>
		public List<string> HasCostManifest { get; set; }


		public List<ProcessProfile> AdministrationProcess { get; set; }
		public List<ProcessProfile> DevelopmentProcess { get; set; }
		public List<ProcessProfile> MaintenanceProcess { get; set; }
		public List<ProcessProfile> AppealProcess { get; set; }
		public List<ProcessProfile> ComplaintProcess { get; set; }
		public List<ProcessProfile> ReviewProcess { get; set; }
		public List<ProcessProfile> RevocationProcess { get; set; }

		public List<VerificationServiceProfile> VerificationServiceProfiles { get; set; }

		public List<OrganizationReference> Department { get; set; }
		public List<OrganizationReference> SubOrganization { get; set; }


	}
}
