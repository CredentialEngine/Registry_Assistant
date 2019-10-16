using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class PathwayRequest : BaseRequest
	{
		public PathwayRequest()
		{
			Pathway = new Pathway();
		}

		public Pathway Pathway { get; set; }

		/// <summary>
		/// Pathway Components
		/// </summary>
		public List<PathwayComponent> PathwayComponents { get; set; } = new List<PathwayComponent>();

		public List<ComponentCondition> ComponentConditions { get; set; } = new List<ComponentCondition>();
	}
	/// <summary>
	/// Resource composed of a structured set of PathwayComponents defining points along a route to fulfillment of a goal or objective.
	/// </summary>
	public class Pathway
	{
		public Pathway(){}

		#region *** Required Properties ***
		public string Ctid { get; set; }


		//List of language codes. ex: en, es
		//TBD
		public List<string> InLanguage { get; set; } = new List<string>();

		/// <summary>
		/// Pathway Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Pathway Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// The webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// This property identifies a child pathway(s) or pathwayComponent(s) in the downward path.
		/// Provide the CTID or the full URI for the target environment. 
		/// It would be a burden to have a user provide a blank node Id.
		/// However, we could recommend that a CTID is provided, and just convert.
		/// </summary>
		public List<string> HasChild { get; set; } = new List<string>();

		/// <summary>
		/// Goal or destination node of the pathway. 
		/// Provide the CTID or the full URI for the target environment. 
		/// URI for a ceterms:PathwayComponent
		/// Multipicity: Many
		/// </summary>
		public List<string> HasDestinationComponent { get; set; } = new List<string>();

		#region at least one of

		/// <summary>
		/// Organization that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
		//OR
		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; }
		#endregion


		#endregion

	}
	public class PathwayComponent
	{
		/// <summary>
		/// Type of PathwayComponent. 
		/// Valid values (with or without ceterms:) :
		/// ceterms:AssessmentComponent	
		/// ceterms:BasicComponent	
		/// ceterms:CocurricularComponent	
		/// ceterms:CompetencyComponent	
		/// ceterms:CourseComponent 	
		/// ceterms:CredentialComponent 	
		/// ceterms:ExtracurricularComponent 	
		/// ceterms:JobComponent 	
		/// ceterms:WorkExperienceComponent
		/// </summary>
		public string PathwayComponentType { get; set; }

		#region Common Properties
		public string CTID { get; set; }

		public string CodedNotation { get; set; }

		/// <summary>
		/// PathwayComponent Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Resource(s) that describes what must be done to complete a PathwayComponent, or part thereof, as determined by the issuer of the Pathway.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:ComponentCondition
		/// </summary>
		public List<string> HasCondition { get; set; } = new List<string>();

		/// <summary>
		/// This property identifies a child pathway(s) or pathwayComponent(s) in the downward path.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:PathwayComponent
		/// </summary>
		public List<string> HasChild { get; set; } = new List<string>();

		/// <summary>
		/// The referenced resource is higher in some arbitrary hierarchy than this resource.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:PathwayComponent
		/// </summary>
		public List<string> IsChildOf { get; set; } = new List<string>();


		/// <summary>
		/// Pathway for which this resource is the goal or destination.
		/// Like IsTopChildOf
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:Pathway
		/// </summary>
		public List<string> IsDestinationComponentOf { get; set; } = new List<string>();

		/// <summary>
		/// PathwayComponent Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Points associated with this resource, or points possible.
		/// </summary>
		public decimal Points { get; set; }

		/// <summary>
		/// Resource that logically comes after this resource.
		/// This property indicates a simple or suggested ordering of resources; if a required ordering is intended, use ceterms:prerequisite instead.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:ComponentCondition
		/// </summary>
		public List<string> Preceeds { get; set; } = new List<string>();

		/// <summary>
		/// Resource(s) that is required as a prior condition to this resource.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:ComponentCondition
		/// </summary>
		public List<string> Prerequisite { get; set; } = new List<string>();


		/// <summary>
		/// URL to structured data representing the resource.
		/// The preferred data serialization is JSON-LD or some other serialization of RDF.
		/// URL
		/// </summary>
		public string SourceData { get; set; }

		/// <summary>
		/// The webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }
		#endregion

		#region BasicComponent,	CocurricularComponent, ExtracurricularComponent 
		/// <summary>
		/// Component Category
		/// Identifies the type of PathwayComponent subclass not explicitly covered in the current array of PathwayComponent subclasses.
		/// Used by: 
		/// ceterms:BasicComponent,	ceterms:CocurricularComponent, ceterms:ExtracurricularComponent 
		/// </summary>
		public string ComponentCategory { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap ComponentCategory_Map { get; set; } = new LanguageMap();
		#endregion


		#region CourseComponent
		/// <summary>
		/// ProgramTerm
		/// Categorization of a term sequence based on the normative time between entry into a program of study and its completion such as "1st quarter", "2nd quarter"..."5th quarter".
		/// </summary>
		public string ProgramTerm { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap ProgramTerm_Map { get; set; } = new LanguageMap();

		#endregion

		#region CredentialComponent
		/// <summary>
		/// Type of credential such as badge, certification, bachelor degree.
		/// The credential type as defined in CTDL. Use EntityReference to provide one of: CTID, Registry URI, or 'blank node'
		/// Used by: 
		/// ceterms:CredentialComponent 
		/// </summary>
		public EntityReference CredentialType { get; set; } = new EntityReference();

		#endregion
	}

	public class ComponentCondition
	{
		//public string CTID { get; set; }

		/// <summary>
		/// ComponentCondition Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// ComponentCondition Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Number of targetComponent resources that must be fulfilled in order to satisfy the ComponentCondition.
		/// Integer
		/// </summary>
		public int RequiredNumber { get; set; }

		/// <summary>
		/// Candidate PathwayComponent for the ComponentCondition as determined by applying the RuleSet.
		/// Provide the CTID or the full URI for the target environment. 
		/// ceterms:PathwayComponent
		/// </summary>
		public List<string> TargetComponent { get; set; } = new List<string>();
	}
}
