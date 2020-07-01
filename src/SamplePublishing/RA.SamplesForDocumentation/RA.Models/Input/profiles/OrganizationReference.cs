using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class BlankNode
	{

		/// <summary>
		/// An identifier for use with blank nodes, to minimize duplicates
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// the type of the entity must be provided. examples
		/// ceterms:AssessmentProfile
		/// ceterms:LearningOpportunityProfile
		/// ceterms:ConditionManifest
		/// ceterms:CostManifest
		/// or the many credential subclasses!!
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Name of the entity (required)
		/// </summary>
		public LanguageMap Name { get; set; } = new LanguageMap();

		/// <summary>
		/// Description of the entity (optional)
		/// </summary>
		public LanguageMap Description { get; set; } = new LanguageMap();

		/// <summary>
		/// Subject webpage of the entity
		/// </summary> (required)
		public string SubjectWebpage { get; set; }

		public List<string> SocialMedia { get; set; } = null;
	}

	/// <summary>
	/// Class for handling references to an organization
	/// Either the Id as an resolvable URL, a CTID (that will be use to format the Id as a URI) or provide all of the properities:
	/// - Type
	/// - Name
	/// - Description
	/// - Subject webpage
	/// - Social media
	/// </summary>
	public class OrganizationReference : EntityReference
	{
		public static string CredentialOrganization = "CredentialOrganization";
		public static string QACredentialOrganization = "QACredentialOrganization";
		/// <summary>
		/// The type of organization is one of :
		/// - CredentialOrganization
		/// - QACredentialOrganization
		/// Required
		/// </summary>
		public override string Type { get; set; }

		/// <summary>
		/// Social Media URL links
		/// For example, Facebook, LinkedIn
		/// </summary>
		public List<string> SocialMedia { get; set; } //URL

		public new bool HasNecessaryProperties()
		{
			//skip social media for now
			//	|| ( SocialMedia == null || SocialMedia.Count == 0 )
			//				|| string.IsNullOrWhiteSpace( Description )
			if ( string.IsNullOrWhiteSpace( Type )
				|| string.IsNullOrWhiteSpace( Name )
				|| string.IsNullOrWhiteSpace( SubjectWebpage )
				)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Purpose is to determine if class has data
		/// </summary>
		/// <returns></returns>
		public override bool  IsEmpty()
		{
			if ( string.IsNullOrWhiteSpace( Id )
				&& string.IsNullOrWhiteSpace( Name )
                && string.IsNullOrWhiteSpace( CTID )
                && string.IsNullOrWhiteSpace( Description )
				&& string.IsNullOrWhiteSpace( SubjectWebpage )
				&& ( SocialMedia == null || SocialMedia.Count == 0 )
				)
				return true;
			else
				return false;
		}
	}

}
