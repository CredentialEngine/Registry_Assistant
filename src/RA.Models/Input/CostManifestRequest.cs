using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	public class CostManifestRequest : BaseRequest
	{
		public CostManifestRequest()
		{
			CostManifest = new CostManifest();
		}

		public CostManifest CostManifest { get; set; }
	}

	public class CostManifest
	{
		public CostManifest()
		{
			EstimatedCost = new List<CostProfile>();
		}

		/// <summary>
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Description
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Organization that owns this CostManifest
		/// Required
		/// </summary>
		public OrganizationReference CostManifestOf { get; set; } = new OrganizationReference();

		/// <summary>
		/// URL for cost details
		/// Required
		/// </summary>
		public string CostDetails { get; set; }

		/// <summary>
		/// Cost Manifest Name
		/// Recommended
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateNameLangMap { get; set; } = new LanguageMapList();

		public string StartDate { get; set; }

		/// <summary>
		/// Approximate date the validity or usefulness of the information in this resource begins.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		public string ApproximateStartDate { get; set; }

		public string EndDate { get; set; }

		/// <summary>
		/// Approximate date some event or activity ends.
		/// Comment: The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// Values may be just a year (e.g. 2026) or a year and a month (e.g. (2026-05).
		/// If the full date, i.e. year, month and day, are known use ceterms:closingDate.
		/// </summary>
		public string ApproximateEndDate { get; set; }

		/// <summary>
		/// List of cost profiles
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; }

		#region -- Process Profiles --

		/// <summary>
		/// Description of a process by which a resource was created.
		/// </summary>
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		/// <summary>
		///  Description of a process by which a resource is maintained, including review and updating.
		/// </summary>
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		/// <summary>
		/// Description of a process by which a resource is reviewed.
		/// </summary>
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion
	}
}
