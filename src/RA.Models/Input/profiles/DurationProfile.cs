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


		/// <summary>
		/// Overall span of time it will take to complete the activity, event, or resource.
		/// Usage Note: If a resource takes 50 hours over a span of 6 months to complete, use this to record 6 months as P6M.
		/// Comment: This is intended to indicate overall time span, regardless of what portion of that time is spent actively pursuing the completion of the activity, event, or resource.
		/// </summary>
		public DurationItem ExactDuration { get; set; }

		/// <summary>
		/// Minimum overall span of time it will take to complete the activity, event, or resource.
		/// Usage Note: If a resource takes 50 hours over a span of 3-9 months to complete, use this to record 3 months as P3M.
		/// Comment: This is intended to indicate overall time span, regardless of what portion of that time is spent actively pursuing the completion of the activity, event, or resource.
		/// </summary>
		public DurationItem MinimumDuration { get; set; }

		/// <summary>
		/// Maximum overall span of time it will take to complete the activity, event, or resource.
		/// Usage Note: If a resource takes 50 hours over a span of 3-9 months to complete, use this to record 9 months as P9M.
		/// This is intended to indicate overall time span, regardless of what portion of that time is spent actively pursuing the completion of the activity, event, or resource.
		/// </summary>
		public DurationItem MaximumDuration { get; set; }

		/// <summary>
		/// NEW 2024-04
		/// Total engaged or participating time it will take to complete the activity, event, or resource.
		/// - Recommended to be used with exactDuration to indicate effort within the duration
		/// Usage Note: If a resource takes 50 hours over a span of 6 months to complete, use this to record 50 hours as PT50H.
		/// Comment: This is intended to indicate only the sum of the individual amounts of time which are spent actively engaged in pursuing the completion of the activity, event, or resource, regardless of the overall time span required to complete it.
		/// TBD:
		///		Only hours or minutes can be used with time required
		/// </summary>
		public DurationTimeItem TimeRequired { get; set; }

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
		/// Time durations cannot be included if there is non-time durations present!
		///		H is the hour designator that follows the value for the number of hours.
		///		M is the minute designator that follows the value for the number of minutes.
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

        public string Print()
        {
            var parts = new List<string>();
            if ( Years > 0 ) { parts.Add( Years + " year" + ( Years == 1 ? "" : "s" ) ); }
            if ( Months > 0 ) { parts.Add( Months + " month" + ( Months == 1 ? "" : "s" ) ); }
            if ( Weeks > 0 ) { parts.Add( Weeks + " week" + ( Weeks == 1 ? "" : "s" ) ); }
            if ( Days > 0 ) { parts.Add( Days + " day" + ( Days == 1 ? "" : "s" ) ); }
            if ( Hours > 0 ) { parts.Add( Hours + " hour" + ( Hours == 1 ? "" : "s" ) ); }
            if ( Minutes > 0 ) { parts.Add( Minutes + " minute" + ( Minutes == 1 ? "" : "s" ) ); }

            if ( parts.Count > 0 )
                return string.Join( ", ", parts );
            else
                return string.Empty;
        }
    }

	/// <summary>
	/// A duration item that is for time (hours or minutes).
	/// Enter either the Duration_ISO8601 value, OR one of hours or minutes.
	/// </summary>
	public class DurationTimeItem
	{
		/// <summary>
		/// A duration in the registry is stored using the ISO8601 durations format. 
		/// P is the duration designator (for period) placed at the start of the duration representation. P is always required, even if only time related designators are included. 
		/// T is the time designator that precedes the time components of the representation.
		/// Time durations cannot be included if there is non-time durations present!
		///		H is the hour designator that follows the value for the number of hours.
		///		M is the minute designator that follows the value for the number of minutes.
		///	Examples:
		///	PT10H	- 10 hours
		///	PT90M	- 90 minutes
		/// <seealso href="https://en.wikipedia.org/wiki/ISO_8601#Durations">ISO_8601 Durations</seealso>
		/// </summary>
		public string Duration_ISO8601 { get; set; }


		/// <summary>
		/// Enter the time required in hours. 
		/// Only one of hours or minutes may be specified.
		/// </summary>
		public decimal? Hours { get; set; }

		/// Enter the time required in minutes. 
		/// Only one of hours or minutes may be specified.
		public decimal? Minutes { get; set; }

		public string Print()
		{
			var parts = new List<string>();
			if ( Hours > 0 ) { parts.Add( Hours + " hour" + ( Hours == 1 ? "" : "s" ) ); }
			if ( Minutes > 0 ) { parts.Add( Minutes + " minute" + ( Minutes == 1 ? "" : "s" ) ); }

			if ( parts.Count > 0 )
				return string.Join( ", ", parts );
			else
				return string.Empty;
		}
	}
}
