// <copyright file="MetricManager.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.Input
{
	/// <summary>
	/// FinancialAssistanceProfile is a class that describes financial assistance that is offered or available
	/// The Name is always required.
	/// At least one of the following must be provided:
	/// Description, Subject Webpage, or Financial Assistance Type
	/// </summary>
	public class FinancialAssistanceProfile
	{
		/// <summary>
		/// Name for this profile
		/// Required
		/// </summary>
		public string Name { get; set; }

		public LanguageMap NameLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// Description is not required.
		/// There is no minimum length, but should be reasonable.
		/// </summary>
		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; } = new LanguageMap();

		/// <summary>
		/// The financial assistance type is a list of one or more concepts from the ceterms:FinancialAssistance concept scheme.
		/// <see cref="https://credreg.net/ctdl/terms/financialAssistanceType"/>
		/// </summary>
		public List<string> FinancialAssistanceType { get; set; } = new List<string>();

		/// <summary>
		/// The Financial Assistance Value(s) available for this profile
		/// The QuantitativeValue includes the UnitText property. Financial Assistance Value, the UnitText if present, is expected to be a currency. It is not required if a description is available.
		/// Recommended
		/// </summary>
		public List<QuantitativeValue> FinancialAssistanceValue { get; set; } = new List<QuantitativeValue>();

		/// <summary>
		/// Optional
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();

		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateNameLangMap { get; set; } = new LanguageMapList();
	}
}
