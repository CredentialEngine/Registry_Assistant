using System.Collections.Generic;

using MJ = RA.Models.JsonV2;
namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing an Occupation
	/// </summary>
	public class OccupationRequest : BaseRequest
	{
		/// <summary>
		/// Profession, trade, or career field that may involve training and/or a formal qualification.
		/// </summary>
		public Occupation Occupation { get; set; } = new Occupation();

		/// <summary>
		/// Occupation already formatted as JSON-LD
		/// ONLY USED WITH PUBLISH LIST
		/// </summary>
		public MJ.Occupation FormattedOccupation { get; set; } = new MJ.Occupation();
	}
	/// <summary>
	/// Request class for publishing a list of Occupations
	/// </summary>
	public class OccupationListRequest : BaseRequest
	{
		/// <summary>
		/// List of Occupations
		/// Using data type of object to allow handling plain requests or those with language maps
		/// </summary>
		public List<object> OccupationList { get; set; } = new List<object>();
		/// <summary>
		/// HasLanguageMaps
		/// If false, will format input using the plain Occupation classes otherwise the JSON-LD class
		/// </summary>
		public bool HasLanguageMaps { get; set; }
	}



}
