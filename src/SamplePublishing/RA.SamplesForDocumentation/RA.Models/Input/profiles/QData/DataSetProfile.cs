﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{
	/// <summary>
	/// DataSet Profile
	/// Particular characteristics or properties of a data set and its records.
	/// qdata:DataSetProfile
	/// <see cref="https://credreg.net/qdata/terms/DataSetProfile"/>
	/// </summary>
	public class DataSetProfile
	{
		/// <summary>
		/// Entity describing the process by which a credential, assessment, organization, or aspects of it, are administered.
		/// <see cref="https://credreg.net/ctdl/terms/administrationProcess#administrationProcess"/>
		/// </summary>
		public List<ProcessProfile> AdministrationProcess { get; set; } = new List<ProcessProfile>();

		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		#region Instruction program and helpers
		/// <summary>
		/// Instructional Program Type
		/// Type of instructional program; select from an existing enumeration of such types.
		/// </summary>
		public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();
		public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

		/// <summary>
		/// Helper property - provided a list of CIP codes and API will validate and format 
		/// List of valid Classification of Instructional Program codes. See:
		/// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
		/// </summary>
		public List<string> CIP_Codes { get; set; } = new List<string>();
		#endregion

		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();
		public string Name { get; set; }
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Authoritative source of an entity's information.
		/// URL
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Credentialing organization or a third party providing the data.
		/// </summary>
		public OrganizationReference DataProvider { get; set; } = new OrganizationReference();

		/// <summary>
		/// Data Set Time Period
		/// Short- and long-term post-award reporting intervals including start and end dates.
		/// </summary>
		public DataSetTimeFrame DataSetTimePeriod { get; set; } = new DataSetTimeFrame();

		/// <summary>
		/// Data Suppression Policy
		/// Description of a data suppression policy for earnings and employment data when cell size is below a certain threshold to ensure an individual's privacy and security.
		/// </summary>
		public string DataSuppressionPolicy { get; set; }
		public LanguageMap DataSuppressionPolicy_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Distribution File
		/// Downloadable form of this dataset, at a specific location, in a specific format.
		/// URL
		/// </summary>
		public string DistributionFile { get; set; }

		/// <summary>
		/// Relevant Data Set For
		/// Data set for the entity being referenced.
		/// ??Inverse property so not relevent for API input.
		/// </summary>
		//public EarningsProfile RelevantDataSetFor { get; set; } = new EarningsProfile();

		/// <summary>
		/// Identification of data point(s) in the data set that describe personal subject attribute(s) used to uniquely identify a subject for the purpose of matching records and an indication of level of confidence in the accuracy of the match.
		/// </summary>
		public string SubjectIdentification { get; set; }
		public LanguageMap SubjectIdentification_Map { get; set; } = new LanguageMap();

	}
}