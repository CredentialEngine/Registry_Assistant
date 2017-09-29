﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Json
{
	public class ConditionManifest : JsonLDDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:ConditionManifest";

		public ConditionManifest()
		{
			SubjectWebpage = new List<IdProperty>();
			Type = "ceterms:ConditionManifest";
            EntryConditions = new List<ConditionProfile>();
            Requires = new List<ConditionProfile>();
			Recommends = new List<ConditionProfile>();
			Corequisite = new List<ConditionProfile>();
			//ConditionManifestOf = new List<OrganizationBase>();
		}

		/// <summary>
		/// entity type
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string Ctid { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public List<IdProperty> SubjectWebpage { get; set; } //URL


		[JsonProperty( PropertyName = "ceterms:conditionManifestOf" )]
		public List<OrganizationBase> ConditionManifestOf { get; set; }

		[JsonProperty( PropertyName = "ceterms:requires" )]
        public List<ConditionProfile> Requires { get; set; }

		[JsonProperty( PropertyName = "ceterms:recommends" )]
		public List<ConditionProfile> Recommends { get; set; }

        [JsonProperty( PropertyName = "ceterms:entryCondition" )]
        public List<ConditionProfile> EntryConditions { get; set; }

		[JsonProperty( PropertyName = "ceterms:corequisite" )]
		public List<ConditionProfile> Corequisite { get; set; }
	}
}