using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Concept Scheme Publishing Request
	/// </summary>
	public class ProgressionModelRequest : ConceptSchemeRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public ProgressionModelRequest()
		{
			ProgressionModel = new ConceptScheme();
		}

		/// <summary>
		/// ConceptScheme
		/// Required
		/// </summary>
		public ConceptScheme ProgressionModel { get; set; } = new ConceptScheme();

		/// <summary>
		/// Concepts for ConceptScheme
		/// Required
		/// </summary>
		public List<Concept> ProgressionLevels { get; set; } = new List<Concept>();
	
	}

	///// <summary>
	///// Progression Model
	///// Currently identical to a ConceptScheme
	///// </summary>
	//public class ProgressionModel : ConceptScheme
	//{
	//	public ProgressionModel()
	//	{
	//		Type = "ProgressionModel";
	//		ConceptTerm = null;
	//		HasTopConcept = null;
	//	}

	//}
	///// <summary>
	///// ProgressionLevel currently the same as the Concept class
	///// </summary>
	//public class ProgressionLevel: Concept
	//{
	//}
}
