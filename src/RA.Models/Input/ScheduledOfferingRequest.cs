using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// ScheduledOffering Request
	/// TBD - handling a list?
	/// </summary>
    public class ScheduledOfferingRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public ScheduledOfferingRequest()
		{
			ScheduledOffering = new ScheduledOffering();
		}
		/// <summary>
		/// ScheduledOffering Input Class
		/// </summary>
		public ScheduledOffering ScheduledOffering { get; set; }
	}
}
