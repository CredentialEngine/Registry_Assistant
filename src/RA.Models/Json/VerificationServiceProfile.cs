﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Json
{
    /// <summary>
    /// Common input class for all condition profiles
    /// </summary>
    public class VerificationServiceProfile
    {
        public VerificationServiceProfile()
        {
            EstimatedCost = new List<CostProfile>();
            Jurisdiction = new List<JurisdictionProfile>();
            //Region = new List<GeoCoordinates>();
            OfferedBy = new List<OrganizationBase>();
            VerifiedClaimType = new List<CredentialAlignmentObject>();
            VerificationDirectory = new List<Json.IdProperty>();
            VerificationService = new List<IdProperty>();
            TargetCredential = new List<EntityBase>();
            Type = "ceterms:VerificationServiceProfile";
        }

        [JsonProperty( "@type" )]
        public string Type { get; set; }

        [JsonProperty( PropertyName = "ceterms:description" )]
        public string Description { get; set; }

        [JsonProperty( PropertyName = "ceterms:dateEffective" )]
        public string DateEffective { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public List<IdProperty> SubjectWebpage { get; set; }

		[JsonProperty( PropertyName = "ceterms:estimatedCost" )]
        public List<CostProfile> EstimatedCost { get; set; }

        [JsonProperty( PropertyName = "ceterms:holderMustAuthorize" )]
        public bool? HolderMustAuthorize { get; set; }

        [JsonProperty( PropertyName = "ceterms:targetCredential" )]
        public List<EntityBase> TargetCredential { get; set; }

        [JsonProperty( PropertyName = "ceterms:verificationDirectory" )]
        public List<IdProperty> VerificationDirectory { get; set; }

        [JsonProperty( PropertyName = "ceterms:verificationMethodDescription" )]
        public string VerificationMethodDescription { get; set; }

        [JsonProperty( PropertyName = "ceterms:verificationService" )]
        public List<IdProperty> VerificationService { get; set; }

        [JsonProperty( PropertyName = "ceterms:verifiedClaimType" )]
        public List<CredentialAlignmentObject> VerifiedClaimType { get; set; }

        [JsonProperty( PropertyName = "ceterms:offeredBy" )]
        public List<OrganizationBase> OfferedBy { get; set; }

        [JsonProperty( PropertyName = "ceterms:jurisdiction" )]
        public List<JurisdictionProfile> Jurisdiction { get; set; }

        //[JsonProperty( PropertyName = "ceterms:region" )]
        //public List<GeoCoordinates> Region { get; set; }
    }
}
