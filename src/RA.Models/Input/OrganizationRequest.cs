using System;
using System.Collections.Generic;

namespace RA.Models.Input
{
	/// <summary>
	/// Class used with an Organization format or publish request
	/// </summary>
	public class OrganizationRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
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
	public class Organization : BaseRequestClass
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Organization()
		{
			Type = "CredentialOrganization";
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

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }
		/// <summary>
		/// Language map list for Keyword
		/// </summary>
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
		/// Department of the organization.
		/// </summary>
		public List<OrganizationReference> Department { get; set; }
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
		/// <summary>
		/// Dun and Bradstreet DUNS number for identifying an organization or business person.
		/// </summary>
		public string Duns { get; set; }
		/// <summary>
		/// Federal Employer Identification Number (FEIN) identifying organizations, persons, states, government agencies, corporations, and companies.
		/// </summary>
		public string Fein { get; set; }

		/// <summary>
		/// Unique six digit identifier assigned to all U.S. institutions that have submitted data to the Integrated Postsecondary Education Data System (IPEDS).
		/// </summary>
		public string IpedsId { get; set; }
		/// <summary>
		/// OPE ID number (U.S. Office of Postsecondary Education Identification), sometimes referred to as the Federal School Code.
		/// </summary>
		public string OpeId { get; set; }
		/// <summary>
		/// A 20-digit, alpha-numeric code, based on the ISO 17442 standard, for identifying legal entities participating in financial transactions.
		/// </summary>
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
		/// Reference to condition manifests
		/// A condition manifest can never be a third party reference, so a url is expected
		/// </summary>
		public List<string> HasConditionManifest { get; set; }
		/// <summary>
		/// Reference to cost manifests
		/// A cost manifest can never be a third party reference, so a url is expected
		/// </summary>
		public List<string> HasCostManifest { get; set; }

		/// <summary>
		/// Type of industry; select from an existing enumeration of such types such as the SIC, NAICS, and ISIC classifications.
		/// </summary>
		public List<FrameworkItem> IndustryType { get; set; }
		/// <summary>
		/// AlternativeIndustryType
		/// Industries that are not found in a formal framework can be still added using AlternativeIndustryType. 
		/// Any industries added using this property will be added to or appended to the IndustryType output.
		/// </summary>
		public List<string> AlternativeIndustryType { get; set; } = new List<string>();
		/// <summary>
		/// Language map list for AlternativeIndustryType
		/// </summary>
		public LanguageMapList AlternativeIndustryType_Map { get; set; } = new LanguageMapList();

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"></see>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; }

		/// <summary>
		/// Type of official status of this resource. Select a valid concept from the LifeCycleStatus concept scheme.
		/// Provide the string value. API will format correctly. The name space of lifecycle doesn't have to be included
		/// Required
		/// lifecycle:Developing, lifecycle:Active", lifecycle:Suspended, lifecycle:Ceased
		/// <see href="https://credreg.net/ctdl/terms/LifeCycleStatus">ceterms:LifeCycleStatus</see>
		/// </summary>
		public string LifeCycleStatusType { get; set; } = "lifeCycle:Active";


		//
		/// <summary>
		/// Webpage or online document that defines or explains the mission and goals of the organization.
		/// URI
		/// </summary>
		public string MissionAndGoalsStatement { get; set; }
		/// <summary>
		/// Textual statement of the mission and goals of the organization.
		/// </summary>
		public string MissionAndGoalsStatementDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap MissionAndGoalsStatementDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// North American Industry Classification System (NAICS) code of an organization or business person.
		/// A list of NAICS codes
		/// </summary>
		public List<string> Naics { get; set; }

		/// <summary>
		/// Resource over which the organization or person claims legal title.
		/// Could be any of Credential, Assessment, LearningOpportunity, Pathway,etc.
		/// It is not necessary to include owns/offers when publishing an organization. It is considered a best practice to use the OwnedBy, OfferedBy properties of credential, etc to establish the relationships.
		/// </summary>
		public List<EntityReference> Owns { get; set; }
		/// <summary>
		/// Resource offered or conferred by the organization or person.
		/// See Owns for best pratice recommendation
		/// </summary>
		public List<EntityReference> Offers { get; set; }


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


		#region Quality Assurance Performed
		// Organization performs QA on these entities

		/// <summary>
		/// Credential, assessment, organization, or learning opportunity for which this organization provides official authorization or approval based on prescribed standards or criteria.
		/// </summary>
		public List<EntityReference> Accredits { get; set; }
		/// <summary>
		/// Credential, assessment, learning opportunity, or organization for which this organization pronounces favorable judgment.
		/// </summary>
		public List<EntityReference> Approves { get; set; }
		/// <summary>
		/// Resource that the agent recommends, endorses, indicates preference for, or otherwise provides a positive judgment.
		/// </summary>
		public List<EntityReference> Recognizes { get; set; }
		/// <summary>
		/// Credential, learning opportunity, assessment or organization that this quality assurance organization monitors, including enforcement of applicable legal requirements or standards.
		/// </summary>
		public List<EntityReference> Regulates { get; set; } = new List<EntityReference>();
		/// <summary>
		/// Credential type that has its validity extended by the organization or person.
		/// </summary>
		public List<EntityReference> Renews { get; set; }
		/// <summary>
		/// Credential type that can be invalidated or retracted by the awarding agent.
		/// </summary>
		public List<EntityReference> Revokes { get; set; }

		#endregion


		#region -- Process Profiles --
		/// <summary>
		/// Entity describing the process by which a credential, assessment, organization, or aspects of it, are administered.
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; }
		/// <summary>
		/// Entity describing the process by which a credential, or aspects of it, were created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }
		/// <summary>
		/// Entity describing the process by which the credential is maintained including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }
		/// <summary>
		/// Formal process for objecting to decisions of the organization regarding credentials, assessments or processes.
		/// </summary>
		public List<ProcessProfile> AppealProcess { get; set; }
		/// <summary>
		/// Process for handling complaints about a credential, or aspects of it including related learning opportunities and assessments.
		/// </summary>
		public List<ProcessProfile> ComplaintProcess { get; set; }
		/// <summary>
		/// Entity that describes the process by which the credential, or aspects of it, are reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }
		/// <summary>
		/// Entity describing the process by which the credential is revoked.
		/// </summary>
		public List<ProcessProfile> RevocationProcess { get; set; }
		#endregion

		/// <summary>
		/// Organization in a subordinate or lower position than a parent organization.
		/// </summary>
		public List<OrganizationReference> SubOrganization { get; set; }

		/// <summary>
		///  Resource that replaces this resource.
		///  full URL OR CTID (recommended)
		/// </summary>
		public string SupersededBy { get; set; }
		/// <summary>
		/// Resource that this resource replaces.
		/// full URL OR CTID (recommended)
		/// </summary>
		public string Supersedes { get; set; }

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

		/// <summary>
		/// Entity describing the means by which someone can verify whether a credential has been attained.
		/// </summary>
		public List<VerificationServiceProfile> VerificationServiceProfile { get; set; } = new List<VerificationServiceProfile>();
		//VerificationServiceProfiles was originally added uncorrectly as plural. The latter is incorrrect and is being maintained for legacy references. VerificationServiceProfile should be used, and is checked first
		[Obsolete]
		public List<VerificationServiceProfile> VerificationServiceProfiles { get; set; } = new List<VerificationServiceProfile>();

		//pending
		public List<CredentialingAction> AccreditAction { get; set; } = new List<CredentialingAction>();

	}
}
