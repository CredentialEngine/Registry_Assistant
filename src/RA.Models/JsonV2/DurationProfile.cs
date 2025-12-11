using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class DurationProfile
	{
		public DurationProfile()
		{
			MinimumDuration = null;
			MaximumDuration = null;
			ExactDuration = null;
			TimeRequired = null;
			Type = "ceterms:DurationProfile";
		}

		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:minimumDuration" )]
		public string MinimumDuration { get; set; }

		[JsonProperty( PropertyName = "ceterms:maximumDuration" )]
		public string MaximumDuration { get; set; }

		[JsonProperty( PropertyName = "ceterms:exactDuration" )]
		public string ExactDuration { get; set; }

		[JsonProperty( PropertyName = "ceterms:timeRequired" )]
		public string TimeRequired { get; set; }

		/// <summary>
		/// Check if there is any input data
		/// </summary>
		[JsonIgnore]
		public bool HasValue
		{
			get
			{
				return !string.IsNullOrWhiteSpace( ExactDuration )
					|| !string.IsNullOrWhiteSpace( MinimumDuration )
					|| !string.IsNullOrWhiteSpace( MaximumDuration )
					|| !string.IsNullOrWhiteSpace( TimeRequired )
					|| ( Description != null && Description.Count > 0 );
			}
		}
	}
}
