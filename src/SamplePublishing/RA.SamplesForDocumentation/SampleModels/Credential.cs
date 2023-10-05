using System.Collections.Generic;

namespace RA.SamplesForDocumentation.SampleModels
{
	public class Credential : BaseModel
	{
        //provide valid concept from schema 
        public string CredentialType { get; set; }
		public string Type
		{
			get { return CredentialType; }
		}
		//provide credential status type, with a default of Active
        public string CredentialStatusType { get; set; } = "Active";
        public string InLanguage { get; set; }
        public string AvailabilityListing { get; set; }
		public string AvailableOnlineAt { get; set; }
		public string ImageUrl { get; set; }
		public string LatestVersionUrl { get; set; }
		public string PreviousVersion { get; set; }
		public List<string> Subject { get; set; }
		public List<string> Keyword { get; set; }
        public List<string> OccupationCodes { get; set; }
		public Organization OwningOrganization { get; set; }
    }
}
