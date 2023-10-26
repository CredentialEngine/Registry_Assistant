using System.Collections.Generic;

namespace RA.Models.Input
{
	public class BaseRequest 
    {
		/// <summary>
		/// DefaultLanguage is used with Language maps where there is more than one entry for InLanguage, and the user doesn't want to have the first language in the list be the language used with language maps. 
		/// </summary>
		public string DefaultLanguage { get; set; } = "en-US";
        /// <summary>
        /// Identifier for Organization which Owns the data being published
        /// This will be the CTID for the owning org, even if publisher is third party.
		/// REQUIRED
        /// </summary>
        public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// Leave blank for default
		/// Formerly known as Community
		/// </summary>
		public string Registry { get; set; } = "";

		/// <summary>
		/// List of objects that will be published as blank nodes. 
		/// Allow publishing of custom concepts etc. 
		/// 
		/// Required properies
		/// - Type	A valid CTDL type
		/// - Id	A blank node identifier: format of _:plus guid (ex: _:48069056-13c3-4973-9b93-a905875619c2)
		/// - any required properties for the target resource type, typically name and subject webpage (or equivalent URL property)
		/// 
		/// Used By Request types
		/// Job, Occupation, Task, WorkRole
		/// 
		/// 
		/// Allowed types - probably no limit
		/// </summary>
		public List<object> ReferenceObjects { get; set; } = new List<object>();
	}

}
