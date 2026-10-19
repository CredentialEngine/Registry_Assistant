using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
{
	public class CostManifest : BaseResourceDocument
	{
		[JsonIgnore]
		public static string classType = "ceterms:CostManifest";

		public CostManifest()
		{
			Type = "ceterms:CostManifest";
			CtdlId = string.Empty;
		}

		/// <summary>
		/// entity type
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		[JsonProperty( "@id" )]
		public string CtdlId { get; set; } = string.Empty;

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:costManifestOf" )]
		public List<string> CostManifestOf { get; set; }

		[JsonProperty( PropertyName = "ceterms:costDetails" )]
		public string CostDetails { get; set; }

		[JsonProperty( PropertyName = "ceterms:startDate" )]
		public string StartDate { get; set; }

		/// <summary>
		/// Approximate date the validity or usefulness of the information in this resource begins.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approximateStartDate" )]
		public string ApproximateStartDate { get; set; }

		[JsonProperty( "ceterms:endDate" )]
		public string EndDate { get; set; }

		/// <summary>
		/// Approximate date some event or activity ends.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:approximateEndDate" )]
		public string ApproximateEndDate { get; set; }

		[JsonProperty( PropertyName = "ceterms:estimatedCost" )]
		public List<CostProfile> EstimatedCost { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		#region process profiles

		[JsonProperty( PropertyName = "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( PropertyName = "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion
	}
}
