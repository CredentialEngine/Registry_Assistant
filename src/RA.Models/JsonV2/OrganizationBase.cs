﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	/// <summary>
	/// Base class to use for references to organizations that are not in the registry
	/// </summary>
	public class OrganizationBase 
	{
		public OrganizationBase()
		{
			//SubjectWebpage = new List<string>();
			SocialMedia = new List<string>();
		}
		

		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }


		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		//22-05-11 mp - noticed that name and description were not language maps.
		//				- while need to start using language maps, will likely need a notification. Or use an object?
		//				- actually this OK, as they get converted to language maps in blank nodes
		[JsonProperty( "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		[JsonProperty( PropertyName = "ceterms:socialMedia" )]
		public List<string> SocialMedia { get; set; }

		[JsonProperty( PropertyName = "ceterms:email" )]
		public List<string> Email { get; set; }


		//public Jurisdiction Jurisdiction { get; set; }
		[JsonProperty( PropertyName = "ceterms:address" )]
		public List<Place> Address { get; set; }


		[JsonProperty( PropertyName = "ceterms:availabilityListing" )]
		public List<string> AvailabilityListing { get; set; }


		public void NegateNonIdProperties()
		{
			Type = null;
			Address = null;
			AvailabilityListing = null;
			Name = null;
			Description = null;
			Email = null;
			SubjectWebpage = null;
			CTID = null;
			SocialMedia = null;
		}
	}


}
