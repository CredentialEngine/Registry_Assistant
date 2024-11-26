using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
    public class MetricRequest : BaseRequest
    {
        /// <summary>
        /// constructor
        /// </summary>
        public MetricRequest()
        {
            Metric = new Metric();
        }
        /// <summary>
        /// Metric Input Class
        /// </summary>
        public Metric Metric { get; set; }

    }
}
