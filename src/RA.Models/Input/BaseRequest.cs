using System.Collections.Generic;

namespace RA.Models.Input
{
	public class BaseRequest 
    {
        /// <summary>
        /// DefaultLanguage is used with Language maps where there is more than one entry for InLanguage, and the user doesn't want to have the first language in the list be the language used with language maps. 
        /// REQUIRED but will default to "en-US"
        /// </summary>
        public string DefaultLanguage { get; set; } = "en-US";

        /// <summary>
        /// Identifier for Organization which Owns the data being published
        /// This will be the CTID for the owning org, even if publisher is third party.
		/// REQUIRED
        /// </summary>
        public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// Flag to indicate if the data being published is from a primary source (true) or a secondary source (false)
		/// There would be a risk where if not provided, defaults to false
		/// </summary>
		public bool IsPrimarySourceRecord { get; set; } = true;

        /// <summary>
        /// Leave blank for default (ce-registry)
        /// Formerly known as Community
        /// </summary>
        public string Registry { get; set; }

		/// <summary>
		/// Where an organization is publishing to a private registry, set this property to true to automatically sync the resource to the public registry 
		/// (that is without an additonal publishing step)
		/// </summary>
        public bool AutomaticallySyncPrivateRequestToPublicRegistry { get; set; } 

		/// <summary>
		/// List of objects that will be published as blank nodes. 
		/// The Id would be a Guid. The related property would have the same indentifier as say CreditLevelType
		/// Allow publishing of custom concepts etc. 
		/// 
		/// Required properies
		/// - Type	A valid CTDL type
		/// - Id	A guid or blank node format of _:plus guid (ex: _:48069056-13c3-4973-9b93-a905875619c2)
		/// - any required properties for the target resource type, typically name and subject webpage (or equivalent URL property)
		/// Used By Newer Request types for blank nodes. 
		/// Where used the property will be a list of strings, rather than say a list of EntityReferences or OrganizationReferences
		/// Allowed types - realistically no limit, however older requests types may not be "ReferenceObjects" aware!
		/// </summary>
		public List<object> ReferenceObjects { get; set; } = new List<object>();
	}

}
