using System.Collections.Generic;

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
