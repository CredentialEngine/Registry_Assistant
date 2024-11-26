using System.Collections.Generic;

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

		//public List<BlankNode> BlankNodes = new List<BlankNode>();
	}

	public class ConditionManifest
	{
		public ConditionManifest()
		{
            EntryConditions = new List<ConditionProfile>();
            Requires = new List<ConditionProfile>();
            RenewedConditions = new List<ConditionProfile>();
            RecommendedConditions = new List<ConditionProfile>();
			Corequisite = new List<ConditionProfile>();
		}
		/// <summary>
		/// Required
		/// </summary>
		public string CTID { get; set; }
		/// <summary>
		/// Nmae for this ConditionManifest
		/// </summary>
		public string Name { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }
        /// <summary>
        /// Alternately can provide a language map
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();
        public string SubjectWebpage { get; set; } //URL

        /// <summary>
        /// List of Alternate Names for this resource
        /// </summary>
        public List<string> AlternateName { get; set; } = new List<string>();
        /// <summary>
        /// LanguageMap for AlternateName
        /// </summary>
        public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();


        /// <summary>
        /// Organization that owns this ConditionManifest
        /// </summary>
        public OrganizationReference ConditionManifestOf { get; set; } = new OrganizationReference();


        /// <summary>
        /// Required condition profiles
        /// </summary>
        public List<ConditionProfile> Requires { get; set; }

        /// <summary>
        /// Renewed condition profiles
        /// </summary>
        public List<ConditionProfile> RenewedConditions { get; set; }

        /// <summary>
        /// Recommended condition profiles Recommends
        /// </summary>
        public List<ConditionProfile> RecommendedConditions { get; set; }

		/// <summary>
		/// Entry Conditions condition profiles
		/// </summary>
		public List<ConditionProfile> EntryConditions { get; set; }

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
