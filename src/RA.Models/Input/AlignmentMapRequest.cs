namespace RA.Models.Input
{
    /// <summary>
    /// Request class for publishing an AlignmentMap
    /// </summary>
    public class AlignmentMapRequest : BaseRequest
    {
        /// <summary>
        /// Profession, trade, or career field that may involve training and/or a formal qualification.
        /// </summary>
        public AlignmentMap AlignmentMap { get; set; } = new AlignmentMap();

    }
}
