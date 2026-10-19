// <copyright file="RegulatoryInformation.cs" company="Credential Engine">
//     Copyright (c) Credential Engine. All rights reserved.
// </copyright>
// <license>Apache License 2.0 - https://www.apache.org/licenses/LICENSE-2.0</license>

using System.Collections.Generic;
using Newtonsoft.Json;

namespace RA.Models.JsonV2
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

			SubjectWebpage = null;
		}

		#region Required properties
		/// <summary>
		/// Need a custom mapping to @type based on input value
		/// </summary>
		[JsonProperty( "@type" )]
		public string Type { get; set; }


		[JsonProperty( "@id" )]
		public string Id { get; set; }

		[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		[JsonProperty( "ceterms:name" )]
		public LanguageMap Name { get; set; }

		[JsonProperty( "ceterms:description" )]
		public LanguageMap Description { get; set; }

		/// <summary>
		/// Organization or person with an enforceable claim or legal title to the resource.
		/// </summary>
		[JsonProperty( "ceterms:ownedBy" )]
		public List<string> OwnedBy { get; set; }

		/// <summary>
		/// The publication status of the resource
		/// </summary>
		[JsonProperty( "ceasn:publicationStatusType" )]
		public string PublicationStatusType { get; set; }

		/// <summary>
		/// Webpage that describes this entity.
		/// </summary>
		[JsonProperty( "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

		#endregion

		/// <summary>
		/// Alias for the entity including acronyms, alpha-numeric notations, and other forms of name abbreviations in common use such as PhD, MA, and BA.
		/// </summary>
		[JsonProperty( PropertyName = "ceterms:alternateName" )]
		public LanguageMapList AlternateName { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:classification" )]
		public List<string> Classification { get; set; }

		/// <summary>
		/// Set of alpha-numeric symbols as defined by the body responsible for this resource that uniquely identifies this resource and supports its discovery and use.
		/// </summary>
		[JsonProperty( "ceterms:codedNotation" )]
		public string CodedNotation { get; set; }

		/// <summary>
		/// Approximate date after which this resource is available or applicable.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		[JsonProperty( "ceterms:approximateDateEffective" )]
		public string ApproximateDateEffective { get; set; }

		/// <summary>
		/// Approximate date after which this resource is no longer offered or available.
		/// The value should be in ISO 8601 date format; specifically YYYY or YYY-MM.
		/// </summary>
		[JsonProperty( "ceterms:approximateExpirationDate" )]
		public string ApproximateExpirationDate { get; set; }

		/// <summary>
		/// Date after which this resource is available or applicable.
		/// </summary>
		[ JsonProperty( "ceterms:dateEffective" )]
		public string DateEffective { get; set; }

		/// <summary>
		/// Date beyond which the resource is no longer offered or available.
		/// </summary>
		[JsonProperty( "ceterms:expirationDate" )]
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Indicates contextualized data that reproduces or links to text such as part of a document or information about some aspect of a resource.
		/// Do not use this property if a simple text property such as ceterms:description is adequate
		/// Range: ceterms:StructuredStatement
		/// </summary>
		[JsonProperty( "ceterms:hasStructuredStatement" )]
		public List<StructuredStatement> HasStructuredStatement { get; set; }

		/// <summary>
		/// Identifier
		/// Means of identifying a resource, typically consisting of an alphanumeric token and a context or scheme from which that token originates.
		/// List of IdentifierValue
		/// </summary>
		[JsonProperty( "ceterms:identifier" )]
		public List<IdentifierValue> Identifier { get; set; }

		[JsonProperty( "ceterms:jurisdiction" )]
		public List<JurisdictionProfile> Jurisdiction { get; set; }

		/// <summary>
		/// Keyword or key phrase describing relevant aspects of an entity.
		/// </summary>
		[JsonProperty( "ceterms:keyword" )]
		public LanguageMapList Keyword { get; set; }

		/// <summary>
		/// Webpage or online document that describes the criteria, standards, and/or requirements used with a process.
		/// </summary>
		[JsonProperty( "ceterms:processStandards" )]
		public string ProcessStandards { get; set; }

		/// <summary>
		/// Textual description of the criteria, standards, and/or requirements used with a process
		/// </summary>
		[JsonProperty( "ceterms:processStandardsDescription" )]
		public LanguageMap ProcessStandardsDescription { get; set; }

		/// <summary>
		/// List of Organizations that recognize this credential
		/// </summary>
		[JsonProperty( "ceterms:recognizedBy" )]
		public List<string> RecognizedBy { get; set; }

		/// <summary>
		/// List of Organizations that regulate this credential
		/// </summary>
		[JsonProperty( "ceterms:regulatedBy" )]
		public List<string> RegulatedBy { get; set; }

		/// <summary>
		/// Category or classification of this resource.
		/// List of URIs that point to a concept
		/// </summary>
		[JsonProperty( "ceterms:regulationType" )]
		public List<string> RegulationType { get; set; }

		/// <summary>
		///  Resource that this regulatory information is about.
		/// Range: Credential, Assessment, and more
		/// </summary>
		[JsonProperty( "ceterms:regulatoryInformationFor" )]
		public List<string> RegulatoryInformationFor { get; set; }

		[JsonProperty( "ceterms:sameAs" )]
		public List<string> SameAs { get; set; }

		#region Versions
		[JsonProperty( "ceterms:latestVersion" )]
		public string LatestVersion { get; set; }

		[JsonProperty( "ceterms:previousVersion" )]
		public string PreviousVersion { get; set; }

		[JsonProperty( "ceterms:nextVersion" )]
		public string NextVersion { get; set; }

		[JsonProperty( "ceterms:supersededBy" )]
		public List<string> SupersededBy { get; set; }

		[JsonProperty( "ceterms:supersedes" )]
		public List<string> Supersedes { get; set; }

		/// <summary>
		/// alphanumeric identifier of the version of the resource that is unique within the organizational context of its owner and which does not need the context of other information in order to be interpreted.
		/// </summary>
		[JsonProperty( "ceterms:versionCode" )]
		public string VersionCode { get; set; }

		/// <summary>
		/// VersionIdentifier
		/// Alphanumeric identifier of the version of the credential that is unique within the organizational context of its owner.
		/// The credential version captured here is any local identifier used by the credential owner to identify the version of the credential in the its local system.
		/// </summary>
		[JsonProperty( "ceterms:versionIdentifier" )]
		public List<IdentifierValue> VersionIdentifier { get; set; }
		#endregion

		#region process profiles
		[JsonProperty( "ceterms:administrationProcess" )]
		public List<ProcessProfile> AdministrationProcess { get; set; }

		[JsonProperty( "ceterms:complaintProcess" )]
		public List<ProcessProfile> ComplaintProcess { get; set; }

		[JsonProperty( "ceterms:developmentProcess" )]
		public List<ProcessProfile> DevelopmentProcess { get; set; }

		[JsonProperty( "ceterms:maintenanceProcess" )]
		public List<ProcessProfile> MaintenanceProcess { get; set; }

		[JsonProperty( "ceterms:reviewProcess" )]
		public List<ProcessProfile> ReviewProcess { get; set; }

		#endregion

	}
}
