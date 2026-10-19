using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class ConditionManifestRequest : BaseRequest
	{
		public ConditionManifestRequest()
		{
			ConditionManifest = new ConditionManifest();
		}

		/// <summary>
		/// Condition Manifest for an organization
		/// </summary>
		public ConditionManifest ConditionManifest { get; set; }

		// public List<BlankNode> BlankNodes = new List<BlankNode>();
	}

	public class ConditionManifest
	{
		public ConditionManifest()
		{
			EntryCondition = new List<ConditionProfile>();
			Requires = new List<ConditionProfile>();
			Renewal = new List<ConditionProfile>();
			Recommends = new List<ConditionProfile>();
			Corequisite = new List<ConditionProfile>();
		}

		/// <summary>
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Name for this ConditionManifest
		/// OPTIONAL, but if provided, must be a minimum of 5 characters.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Required
		/// </summary>
		public string SubjectWebpage { get; set; } // URL

		/// <summary>
		/// Organization that owns this ConditionManifest
		/// Required
		/// </summary>
		public OrganizationReference ConditionManifestOf { get; set; } = new OrganizationReference();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateNameLangMap { get; set; } = new LanguageMapList();

		/// <summary>
		/// Condition Excludes
		/// Resource that does not count toward fulfilling a constraint, prerequisite, entry condition, or requirement.
		/// Range: anyURI
		/// Realistically could be any resource, in the registry or not
		/// </summary>
		public List<string> ConditionExcludes { get; set; }

		/// <summary>
		/// Required condition profiles
		/// </summary>
		public List<ConditionProfile> Requires { get; set; }

		/// <summary>
		/// Entity describing the constraints, prerequisites, entry conditions, or requirements necessary to maintenance and renewal of a resource.
		/// </summary>
		public List<ConditionProfile> Renewal { get; set; }

		/// <summary>
		/// Recommended condition profiles Recommends
		/// </summary>
		public List<ConditionProfile> Recommends { get; set; }

		/// <summary>
		/// Entry Conditions condition profiles
		/// </summary>
		public List<ConditionProfile> EntryCondition { get; set; }

		/// <summary>
		/// Corequisite Conditions
		/// ceterms:corequisite
		/// The resource being referenced must be pursued concurrently with the resource being described.
		/// </summary>
		/// See: <a href="http://purl.org/ctdl/terms/corequisite">corequisite</a>
		/// <remarks>Includes dual (double) degrees that cannot be earned independently of each other.</remarks>
		public List<ConditionProfile> Corequisite { get; set; }

		/// <summary>
		///  Resource that must be completed prior to, or pursued at the same time as, this resource.
		/// </summary>
		public List<ConditionProfile> CoPrerequisite { get; set; } = new List<ConditionProfile>();

		#region -- Process Profiles --

		/// <summary>
		/// Description of a process by which a resource was created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		///  Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion
	}
}
