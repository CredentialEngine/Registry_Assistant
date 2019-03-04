namespace RA.Models.Input
{
	public class BaseRequest 
    {
        /// <summary>
        /// DefaultLanguage is used with Language maps where there is more than one entry for InLanguage, and the user doesn't want to have the first language in the list be the language used with language maps. 
        /// </summary>
        public string DefaultLanguage { get; set; }
        /// <summary>
        /// Identifier for Organization which Owns the data being published
        /// 2017-12-13 - this will be the CTID for the owning org.
        /// </summary>
        public string PublishForOrganizationIdentifier { get; set; }

		/// <summary>
		/// Envelope Identifier
		/// Optional property, used where the publishing entity wishes to store the identifier.
		/// Contains registry envelope identifier for a document in the registy. It should be empty for a new document. 
		/// </summary>
		public string RegistryEnvelopeId { get; set; }

        /// <summary>
        /// Sept. 30, 2018
        /// The full registry document is now accessed via the /graph/ path. All @id (or equivalent) references should be using /graph/. If this value is false, the API will look for and convert all /resource/ urls to /graph/ urls
        /// NOTE: using default of true as norm in case property not populated.
        /// </summary>
        public bool NotConvertingFromResourceLinkToGraphLink { get; set; }
    }

}
