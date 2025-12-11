using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RA.Models.JsonV2.QData
{
	/// <summary>
	/// DataSet Time Frame
	/// Time frame including earnings and employment start and end dates of the data set.
	/// https://credreg.net/qdata/terms/DataSetTimeFrame
	/// </summary>
	public class DataSetTimeFrame
	{
		public DataSetTimeFrame()
		{
			TimeInterval = null;
		}

		/// <summary>
		/// The type of the entity
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; } = "qdata:DataSetTimeFrame";

		/// <summary>
		/// An identifier for use with blank nodes. 
		/// It will be ignored if included with a primary resource
		/// </summary>
		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Length of the interval between two events. 
		/// schema:Duration
		/// </summary>
		[JsonProperty( PropertyName = "qdata:timeInterval" )]
		public string TimeInterval { get; set; }

		[JsonProperty( PropertyName = "ceterms:startDate" )]
		public string StartDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:endDate" )]
		public string EndDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		[JsonProperty( PropertyName = "qdata:dataAttributes" )]
		public List<DataProfile> DataAttributes { get; set; }

		/// <summary>
		/// Requires at least one of: timeInterval, startDate, endDate
		/// </summary>
		public bool HasRequiredData()
		{
			return !( string.IsNullOrWhiteSpace( StartDate ) && string.IsNullOrWhiteSpace( EndDate ) && string.IsNullOrWhiteSpace( TimeInterval ) );
		}
	}
}
