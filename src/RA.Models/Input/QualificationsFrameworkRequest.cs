namespace RA.Models.Input
{
    /// <summary>
    /// Request class for publishing an QualificationsFramework
    /// </summary>
    public class QualificationsFrameworkRequest : BaseRequest
    {
        /// <summary>
        /// Profession, trade, or career field that may involve training and/or a formal qualification.
        /// </summary>
        public QualificationsFramework QualificationsFramework { get; set; } = new QualificationsFramework();

    }
}
