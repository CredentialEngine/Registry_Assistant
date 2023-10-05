using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Duration Profile
	/// Either enter an ExactDuration or a range using Minimum duration, and maximum duration
	/// </summary>
	public class DurationProfile 
	{
		public DurationProfile()
		{
			MinimumDuration = new DurationItem();
			MaximumDuration = new DurationItem();
			ExactDuration = new DurationItem();
		}

		/// <summary>
		/// Description of this duration profile - optional
		/// </summary>
		public string Description { get; set; }
        public LanguageMap Description_Map { get; set; } = new LanguageMap();
        public DurationItem MinimumDuration { get; set; }
		public DurationItem MaximumDuration { get; set; }
		public DurationItem ExactDuration { get; set; }

	}
	//

	/// <summary>
	/// Enter either the Duration_ISO8601 value, OR the necessary combination of years, months, weeks, etc
	/// </summary>
	public class DurationItem
	{
		/// <summary>
		/// A duration in the registry is stored using the ISO8601 durations format. 
		/// P is the duration designator (for period) placed at the start of the duration representation. P is always required, even if only time related designators are included. 
		///		Y is the year designator that follows the value for the number of years.
		///		M is the month designator that follows the value for the number of months.
		///		W is the week designator that follows the value for the number of weeks.
		///		D is the day designator that follows the value for the number of days
		/// T is the time designator that precedes the time components of the representation.
		///		H is the hour designator that follows the value for the number of hours.
		///		M is the minute designator that follows the value for the number of minutes.
		///		S is the second designator that follows the value for the number of seconds.
		///	Examples:
		///	P2Y		- two years
		///	P10M	- 10 months
		///	PT10H	- 10 hours
		/// <seealso href="https://en.wikipedia.org/wiki/ISO_8601#Durations">ISO_8601 Durations</seealso>
		/// </summary>
		public string Duration_ISO8601 { get; set; }
		//TODO - technically a decimal can be used. So P2.5Y instead of P2Y6M. Or more precise: P4.38Y.
		public decimal? Years { get; set; }
		public decimal? Months { get; set; }
		public decimal? Weeks { get; set; }
		public decimal? Days { get; set; }
		public decimal? Hours { get; set; }
		public decimal? Minutes { get; set; }

	}
}
