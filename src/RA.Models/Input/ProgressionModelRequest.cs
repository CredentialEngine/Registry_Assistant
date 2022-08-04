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
			ProgressionModel = new ProgressionModel();
		}

		/// <summary>
		/// ProgressionModel
		/// Required
		/// </summary>
		public ProgressionModel ProgressionModel { get; set; } = new ProgressionModel();

		/// <summary>
		/// ProgressionLevels for ProgressionModel
		/// Required
		/// </summary>
		public List<Concept> ProgressionLevel { get; set; } = new List<Concept>();
	
	}

    /// <summary>
    /// Progression Model
    /// Currently identical to a ConceptScheme
    /// </summary>
    public class ProgressionModel : ConceptScheme
    {
		/// <summary>
		/// constructor
		/// </summary>
        public ProgressionModel()
        {
            Type = "ProgressionModel";
            ConceptTerm = null;
        }

    }


 //   /// <summary>
 //   /// ProgressionLevel currently the same as the Concept class
 //   /// </summary>
 //   public class ProgressionLevel : Concept
	//{
	//	/// <summary>
	//	///	Concept scheme to which this concept belongs.
	//	/// </summary>
	//	[Obsolete( "Not supported in this class.", true )]
	//	private new string InScheme { get; set; }

	//	/// <summary>
	//	/// Progression Model to which this Progression Level belongs.
	//	/// </summary>
	//	public string InProgressionModel { get; set; }
	//}
}
