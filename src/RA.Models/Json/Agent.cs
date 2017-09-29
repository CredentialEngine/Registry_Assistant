﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using RA.Models.Json;
using System.ComponentModel;

namespace RA.Models.Json
{
    public class Agent : JsonLDDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:Agent";

		public Agent()
        {
            Industries = new List<CredentialAlignmentObject>();
            Keyword = new List<string>();
            SubjectWebpage = new List<IdProperty>();
           // MissionAndGoalsStatement = new List<IdProperty>();
            Image = new List<IdProperty>();
            AgentType = new List<CredentialAlignmentObject>();
            AgentSectorType = new List<CredentialAlignmentObject>();
            //AgentPurpose = new List<IdProperty>();
            ServiceType = new List<CredentialAlignmentObject>();
            AvailabilityListing = new List<IdProperty>();
			AlternativeIdentifier = new List<IdentifierValue>();
			Type = "ceterms:CredentialOrganization";

			SameAs = new List<IdProperty>();
			SocialMedia = new List<IdProperty>();
            Address = new List<Address>();
            ContactPoint = new List<Json.ContactPoint>();

			HasConditionManifest = new List<EntityBase>();
			HasCostManifest = new List<EntityBase>();

			AccreditedBy = null;
            ApprovedBy = null;
            RegulatedBy = null;
            RecognizedBy = null;

			Approves = null;
			Offers = null;
			Owns = null;
			Renews = null;
			Revokes = null;
			Recognizes = null;

			AccreditedIn = null;
            ApprovedIn = null;
            RecognizedIn = null;
            RegulatedIn = null;
            VerificationServiceProfiles = new List<VerificationServiceProfile>();
            Department = new List<OrganizationBase>();
            SubOrganization = new List<OrganizationBase>();
			Jurisdiction = new List<JurisdictionProfile>();
        }

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// </summary>

		[JsonProperty( "@type" )]
		public string Type { get; set; }

        [DefaultValue("")]
		[JsonProperty( "ceterms:name" )]
		public string Name { get; set; }
		[JsonProperty( "ceterms:description" )]
		public string Description { get; set; }


		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string Ctid { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public List<IdProperty> SubjectWebpage { get; set; } //URL

        [JsonProperty( PropertyName = "ceterms:accreditedIn" )]
        public List<JurisdictionProfile> AccreditedIn { get; set; }

        [JsonProperty( PropertyName = "ceterms:approvedIn" )]
        public List<JurisdictionProfile> ApprovedIn { get; set; }

        [JsonProperty( PropertyName = "ceterms:recognizedIn" )]
        public List<JurisdictionProfile> RecognizedIn { get; set; }

        [JsonProperty( PropertyName = "ceterms:regulatedIn" )]
        public List<JurisdictionProfile> RegulatedIn { get; set; }

        [JsonProperty( PropertyName = "ceterms:sameAs" )]
		public List<IdProperty> SameAs { get; set; }


		[JsonProperty( PropertyName = "ceterms:socialMedia" )]
		public List<IdProperty> SocialMedia { get; set; }

		[JsonProperty( PropertyName = "ceterms:image" )]
		public List<IdProperty> Image { get; set; } //Image URL

		[JsonProperty( PropertyName = "ceterms:foundingDate" )]
		public string FoundingDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:agentType" )]
		public List<CredentialAlignmentObject> AgentType { get; set; }


		[JsonProperty( PropertyName = "ceterms:agentSectorType" )]
		public List<CredentialAlignmentObject> AgentSectorType { get; set; }


		[JsonProperty( PropertyName = "ceterms:industryType" )]
        public List<CredentialAlignmentObject> Industries { get; set; }

        [JsonProperty( PropertyName = "ceterms:keyword" )]
        public List<string> Keyword { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternativeIdentifier" )]
		public List<IdentifierValue> AlternativeIdentifier { get; set; }

		[JsonProperty( PropertyName = "ceterms:missionAndGoalsStatement" )]
		public IdProperty MissionAndGoalsStatement { get; set; }
		//public List<IdProperty> MissionAndGoalsStatement { get; set; }

		[JsonProperty( PropertyName = "ceterms:missionAndGoalsStatementDescription" )]
		public string MissionAndGoalsStatementDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:agentPurpose" )]
		public IdProperty AgentPurpose { get; set; }

		[JsonProperty( PropertyName = "ceterms:agentPurposeDescription" )]
		public string AgentPurposeDescription { get; set; }

		[JsonProperty( PropertyName = "ceterms:duns" )]
		public string DUNS { get; set; }


		[JsonProperty( PropertyName = "ceterms:fein" )]
		public string FEIN { get; set; }


		[JsonProperty( PropertyName = "ceterms:ipedsID" )]
		public string IpedsID { get; set; }

		[JsonProperty( PropertyName = "ceterms:opeID" )]
		public string OPEID { get; set; }

		[JsonProperty( PropertyName = "ceterms:email" )]
		public List<string> Email { get; set; }


        [JsonProperty( PropertyName = "ceterms:availabilityListing" )]
        public List<IdProperty> AvailabilityListing { get; set; }

        [JsonProperty( PropertyName = "ceterms:serviceType" )]
        public List<CredentialAlignmentObject> ServiceType { get; set; }

        //public Jurisdiction Jurisdiction { get; set; }
        [JsonProperty( PropertyName = "ceterms:address" )]
        public  List<Address> Address { get; set; }

        [JsonProperty( PropertyName = "ceterms:targetContactPoint" )]
        public List<ContactPoint> ContactPoint { get; set; }

		[JsonProperty( PropertyName = "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		[JsonProperty( PropertyName = "ceterms:accreditedBy" )]
        public List<OrganizationBase> AccreditedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:approvedBy" )]
        public List<OrganizationBase> ApprovedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:recognizedBy" )]
        public List<OrganizationBase> RecognizedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:regulatedBy" )]
        public List<OrganizationBase> RegulatedBy { get; set; }


		[JsonProperty( PropertyName = "ceterms:approves" )]
		public List<EntityBase> Approves { get; set; }

		[JsonProperty( PropertyName = "ceterms:offers" )]
		public List<EntityBase> Offers { get; set; }

		[JsonProperty( PropertyName = "ceterms:owns" )]
		public List<EntityBase> Owns { get; set; }

		[JsonProperty( PropertyName = "ceterms:renews" )]
		public List<EntityBase> Renews { get; set; }

		[JsonProperty( PropertyName = "ceterms:revokes" )]
		public List<EntityBase> Revokes { get; set; }

		[JsonProperty( PropertyName = "ceterms:recognizes" )]
		public List<EntityBase> Recognizes { get; set; }

		[JsonProperty( PropertyName = "ceterms:hasConditionManifest" )]
		public List<EntityBase> HasConditionManifest { get; set; }

		[JsonProperty( PropertyName = "ceterms:hasCostManifest" )]
		public List<EntityBase> HasCostManifest { get; set; }

		[JsonProperty( PropertyName = "ceterms:hasVerificationService" )]
        public List<VerificationServiceProfile> VerificationServiceProfiles { get; set; }

        [JsonProperty( PropertyName = "ceterms:administrationProcess", NullValueHandling = NullValueHandling.Ignore )]
        public List<ProcessProfile> AdministrationProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:developmentProcess" )]
        public List<ProcessProfile> DevelopmentProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
        public List<ProcessProfile> MaintenanceProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:appealProcess" )]
        public List<ProcessProfile> AppealProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:complaintProcess" )]
        public List<ProcessProfile> ComplaintProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:reviewProcess" )]
        public List<ProcessProfile> ReviewProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:revocationProcess" )]
        public List<ProcessProfile> RevocationProcess { get; set; }

        [JsonProperty( PropertyName = "ceterms:department" )]
        public List<OrganizationBase> Department { get; set; }

        [JsonProperty( PropertyName = "ceterms:subOrganization" )]
        public List<OrganizationBase> SubOrganization { get; set; }
    }
}
