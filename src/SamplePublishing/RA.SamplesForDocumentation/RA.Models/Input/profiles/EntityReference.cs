using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	/// <summary>
	/// Class for handling references to an entity such as an Assessment, Organization, Learning opportunity, or credential that may or may not be in the Credential Registry.
	/// Either the Id as an resolvable URL, a CTID where the document exists in the Credential Registry, or provide specific properities for the entity.
	/// If neither a CTID or Id is provided, a blank node will be added the @graph.
	/// </summary>
	public class EntityReference
	{
		/// <summary>
		/// Id is a resovable URI
		/// If the entity exists in the registry, provide the URI. 
		/// If not sure of the exact URI, especially if just publishing the entity, then provide the CTID and the API will format the URI.
		/// Alterate URIs are under consideration. For example
		/// http://dbpedia.com/Stanford_University
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Optionally, a CTID can be entered instead of an Id. 
		/// A CTID is recommended for flexibility.
		/// Only enter Id or CTID, but not both.
		/// </summary>
		public string CTID { get; set; }

		//if there is no available Id/CTID, enter the following, where Type, Name, Description, and subjectwebpage would be required

		/// <summary>
		/// the type of the entity must be provided if the Id was not provided. examples
		/// ceterms:AssessmentProfile
		/// ceterms:LearningOpportunityProfile
		/// ceterms:ConditionManifest
		/// ceterms:CostManifest
		/// or the many credential subclasses!!
		/// </summary>
		public virtual string Type { get; set; }


		/// <summary>
		/// Name of the entity (required)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Subject webpage of the entity (required)
		/// This should be for the referenced entity. 
		/// For example, if the reference is for an organization, the subject webpage should be on the organization site.
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// Description of the entity (optional)
		/// This should be the general description of the entity. 
		/// For example, for an organization, the description should be about the organization specifically not, how the organization is related to, or interacts with the refering entity. 
		/// </summary>
		public string Description { get; set; }


		#region Assessment/Lopp related properties
		//2020-05-18 Additional properties have been added to the EntityReference. 
		public List<CredentialAlignmentObject> Assesses { get; set; } = new List<CredentialAlignmentObject>();

		/// <summary>
		/// Assessment Method Description 
		/// Description of the assessment methods for a resource.
		/// </summary>
		public string AssessmentMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap AssessmentMethodDescription_Map { get; set; } = new LanguageMap();
		//
		public List<Place> AvailableAt { get; set; }
		//
		public string CodedNotation { get; set; }
		//
		public QuantitativeValue CreditValue { get; set; } = new QuantitativeValue();
		//
		public List<DurationProfile> EstimatedDuration { get; set; } = new List<DurationProfile>();

		/// <summary>
		/// Learning Method Description 
		///  Description of the learning methods for a resource.		/// 
		/// </summary>
		public string LearningMethodDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap LearningMethodDescription_Map { get; set; } = new LanguageMap();
		//
		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; } = new List<OrganizationReference>();
		//
		/// <summary>
		/// Organization(s) that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
		//
		/// <summary>
		/// For Learning Opportunities only, list of competencies being taught
		/// </summary>
		public List<CredentialAlignmentObject> Teaches { get; set; } = new List<CredentialAlignmentObject>();
		#endregion


		/// <summary>
		/// Check if all properties for a reference request are present
		/// 17-08-27 We do need a type if only providing reference data
		/// </summary>
		/// <returns></returns>
		public bool HasNecessaryProperties()
		{
			//	|| string.IsNullOrWhiteSpace( Description )
			if ( string.IsNullOrWhiteSpace( Name )
				|| string.IsNullOrWhiteSpace( Type )
				|| string.IsNullOrWhiteSpace( SubjectWebpage )
				)
				return false;
			else
				return true;
		}
		public virtual bool IsEmpty()
		{
			if ( string.IsNullOrWhiteSpace( Id )
				&& string.IsNullOrWhiteSpace( CTID )
				&& string.IsNullOrWhiteSpace( Name )
				&& string.IsNullOrWhiteSpace( Description )
				&& string.IsNullOrWhiteSpace( SubjectWebpage )
				)
				return true;
			else
				return false;
		}
	}
}
