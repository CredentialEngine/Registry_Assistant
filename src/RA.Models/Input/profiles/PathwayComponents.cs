using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	/// <summary>
	/// Resource that identifies a direct, indirect, formative, and summative evaluation or estimation of the nature, ability, or quality of a resource, performance, or outcome of an action.
	/// </summary>
	public class AssessmentComponent : PathwayComponent
	{
	}

	/// <summary>
	/// Resource that identifies a resource not otherwise covered by the enumerated PathwayComponent subclasses.
	/// </summary>
	public class BasicComponent : PathwayComponent
	{

	}
	/// <summary>
	/// Resource that identifies an activity, program, or informal learning experience such as a civic or service activity that supplements and complements the curriculum.
	/// </summary>
	public class CocurricularComponent : BasicComponent
	{
	}
	/// <summary>
	/// Resource that identifies an activity, program, or informal learning experience that may be offered or provided by a school, college, or other organization that is not connected to a curriculum.
	/// </summary>
	public class ExtracurricularComponent : BasicComponent
	{
	}

	/// <summary>
	/// Resource that identifies a structured sequence of one or more learning activities that aims to develop a prescribed set of knowledge, skill, or ability of learners.
	/// </summary>
	public class CourseComponent : PathwayComponent
	{
		///// <summary>
		///// ProgramTerm
		///// Categorization of a term sequence based on the normative time between entry into a program of study and its completion such as "1st quarter", "2nd quarter"..."5th quarter".
		///// </summary>
		////public string ProgramTerm { get; set; }
		///// <summary>
		///// Alternately can provide a language map
		///// </summary>
		////public LanguageMap ProgramTerm_Map { get; set; } = new LanguageMap();


	}
	public class CredentialComponent : PathwayComponent
	{
		///// <summary>
		///// Type of credential such as badge, certification, bachelor degree.
		///// The credential type as defined in CTDL. Use EntityReference to provide one of: CTID, Registry URI, or 'blank node'
		///// Used by: 
		///// ceterms:CredentialComponent 
		///// </summary>
		////public EntityReference CredentialType { get; set; } = new EntityReference();

	}
}
