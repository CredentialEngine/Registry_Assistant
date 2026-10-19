namespace RA.Models.Input
{
	/// <summary>
	/// Request class for publishing an industry
	/// </summary>
	public class IndustryRequest : BaseRequest
	{
		public IndustryRequest()
		{
			Industry = new Industry();
		}

		/// <summary>
		/// Broad category of economic activities encompassing various sectors.
		/// </summary>
		public Industry Industry { get; set; }
	}
}
