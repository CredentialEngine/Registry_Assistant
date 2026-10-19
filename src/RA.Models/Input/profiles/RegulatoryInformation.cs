// <copyright file="RegulatoryInformation.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;
using RA.Models.JsonV2;

namespace RA.Models.Input
{
	/// <summary>
	/// Regulatory Information
	/// Statements provided about a resource in order to comply with legal requirements.
	/// Required properties:
	/// - CTID
	/// - Name
	/// - Description
	/// - OwnedBy
	/// - PublicationStatusType
	/// - SubjectWebpage
	/// </summary>
	public class RegulatoryInformation : BaseResourceDocument
	{

		public RegulatoryInformation()
		{
			Type = "ceterms:RegulatoryInformation";
			Jurisdiction = null;
			OwnedBy = new List<OrganizationReference>();

			SubjectWebpage = null;
		}

		#region Required properties
		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }

		public string CTID { get; set; }

		public string Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public LanguageMap NameLangMap { get; set; }

		public string Description { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public LanguageMap DescriptionLangMap { get; set; }

		/// <summary>
		/// Organization or person with an enforceable claim or legal title to the resource.
		/// REQUIRED
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; }

		/// <summary>
		/// The publication status of the resource
		/// REQUIRED
		/// </summary>
		public string PublicationStatusType { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		public List<string> AlternateName { get; set; }

		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateNameLangMap { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// Range: skos:Concept
		/// - there is no existing CTDL concept scheme for Classification.
		/// </summary>
		public List<string> Classification { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// Approximate date after which this resource is available or applicable.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// One of ApproximateDateEffective, or DateEffective is required
		/// </summary>
		public string ApproximateDateEffective { get; set; }

		/// <summary>
		/// Approximate date after which this resource is no longer offered or available.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		public string ApproximateExpirationDate { get; set; }

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// One of ApproximateDateEffective, or DateEffective is required
		/// </summary>
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		public List<StructuredStatement> HasStructuredStatement { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		public List<IdentifierValue> Identifier { get; set; }

		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		public List<string> Keyword { get; set; }

		/// <summary>
		/// Language map list for Keyword
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:keyword" )]
		public LanguageMapList KeywordLangMap { get; set; } = null;

		/// <summary>
		/// Webpage or online document that describes the criteria, standards, and/or requirements used with a process.
		/// </summary>
		public string ProcessStandards { get; set; }

		/// <summary>
		/// Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
		public string ProcessStandardsDescription { get; set; }

		/// <summary>
		/// Language map - Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescriptionLangMap { get; set; }

		/// <summary>
		/// List of Organizations that recognize this credential
		/// </summary>
		public List<OrganizationReference> RecognizedBy { get; set; }

		/// <summary>
		/// List of Organizations that regulate this credential
		/// </summary>
		public List<OrganizationReference> RegulatedBy { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// Range: skos:Concept
		/// - there is no existing CTDL concept scheme for regulation types.
		/// </summary>
		public List<string> RegulationType { get; set; }

		/// <summary>
		/// Resource that this regulatory information is about.
		/// RECOMMENDED
		/// Range: Credential, Assessment, and more
		/// </summary>
		public List<string> RegulatoryInformationFor { get; set; }


		public List<string> SameAs { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// Recommended
		/// </summary>
		public string SubjectWebpage { get; set; }

		#region Versions
		public string LatestVersion { get; set; }

		public string PreviousVersion { get; set; }

		public string NextVersion { get; set; }

		public List<string> SupersededBy { get; set; }

		public List<string> Supersedes { get; set; }

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		public string VersionCode { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		public List<IdentifierValue> VersionIdentifier { get; set; }
		#endregion

		#region process profiles
		public List<ProcessProfile> AdministrationProcess { get; set; }

		public List<ProcessProfile> ComplaintProcess { get; set; }

		public List<ProcessProfile> DevelopmentProcess { get; set; }

		public List<ProcessProfile> MaintenanceProcess { get; set; }

		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion

	}
}
