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
        /// Future use
        /// Allow publishing of custom concepts etc. The property type would be required, as well as an Id. 
        /// The Id would be a Guid (TBD). The related property would have the same indentifier as say CreditLevelType
        /// Maybe a custom class
        /// Allowed (So far)
        /// </summary>
        public List<object> ReferenceObjects { get; set; } = new List<object>();
        public List<ReferenceObject> ReferenceObject { get; set; } = new List<ReferenceObject>();
	}
	public class ReferenceObject
    {
		/// <summary>
		/// The Id must be a valid Guid.
		/// Required.
		/// </summary>
		public string Id { get; set; }	
		/// <summary>
		/// The class type.
		/// Required.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The content for the blank node, example: credential, 
		/// </summary>
		public object Resource { get; set; }
    }
}
