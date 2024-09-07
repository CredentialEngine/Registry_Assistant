using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input.profiles.QData
{
    /// <summary>
    /// DataSet Profile
    /// Particular characteristics or properties of a data set and its records.
    /// Required:
    /// - CTID
    /// - About
    /// qdata:DataSetProfile
    /// <see href="https://credreg.net/qdata/terms/DataSetProfile"/>
    /// </summary>
    public class DataSetProfile
    {
        #region Required
        /// <summary>
        /// CTID - Required 
        /// </summary>
        public string CTID { get; set; }

        /// <summary>
        /// Name of dataset profile
        /// Required
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  LanguageMap for Name
        /// </summary>
        public LanguageMap Name_Map { get; set; } = new LanguageMap();

        /// <summary>
        /// Description
        /// REQUIRED and must be a minimum of 15 characters.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// LanguageMap for Description
        /// </summary>
        public LanguageMap Description_Map { get; set; } = new LanguageMap();


        /// <summary>
        /// Credentialing organization or a third party providing the data.
        /// Required when publishing the DataSetProfile directly. Otherwise derived from owner of related resource.
        /// 
        /// </summary>
        public OrganizationReference DataProvider { get; set; } = new OrganizationReference();

        /// <summary>
        /// Subject matter of the resource.
        /// Means to point to a credential or other resource where data is published by a third party. 
        /// Allowed: 
        /// - any credential
        /// - assessment profile
        /// - any learning opportunity
        /// - scheduled offering
        /// - 23-10-23 - any organization
        /// REQUIRED when dataSetProfile published separately.
        /// CTID/URI
        /// schema:about
        /// </summary>
        public List<EntityReference> About { get; set; } = new List<EntityReference>();

        #endregion

        #region Recommended

        /// <summary>
        /// Data Set Time Period
        /// Short- and long-term post-award reporting intervals including start and end dates.
        /// NOTE: This property is replaced by DataSetTemporalCoverage
        /// NOTE: after official implementation, only one time period will be allowed
        ///     Also any data attributes may be ignored, although attempts may be made to convert them to the new structure.
        /// </summary>
        [Obsolete]
        public List<DataSetTimeFrame> DataSetTimePeriod { get; set; }


        /// <summary>
        /// Location or geographic area for a data set. 
        /// </summary>
        public List<Place> DataSetSpatialCoverage { get; set; }

        /// <summary>
        /// Data Suppression Policy
        /// Description of a data suppression policy for earnings and employment data when cell size is below a certain threshold to ensure an individual's privacy and security.
        /// </summary>
        public string DataSuppressionPolicy { get; set; }
        public LanguageMap DataSuppressionPolicy_Map { get; set; }

        #endregion

        /// <summary>
        /// Entity describing the process by which a credential, assessment, organization, or aspects of it, are administered.
        /// <see href="https://credreg.net/ctdl/terms/administrationProcess"/>
        /// </summary>
        public List<ProcessProfile> AdministrationProcess { get; set; }

        /// <summary>
        /// List of Alternate Names for this resource
        /// </summary>
        public List<string> AlternateName { get; set; }
        /// <summary>
        /// LanguageMap for AlternateName
        /// </summary>
        public LanguageMapList AlternateName_Map { get; set; }

        /// <summary>
        /// Effective date of this resource's content.
        /// </summary>
        public string DateEffective { get; set; }

        /// <summary>
        /// Distribution File
        /// Downloadable form of this dataset, at a specific location, in a specific format.
        /// NOTE: 
        ///     If any information is required to distinguish between different distribution files, use hasDataSetDistribution with the Data Set Distribution.
        /// URL
        /// </summary>
        public List<string> DistributionFile { get; set; }


        #region Instruction program and helpers
        /// <summary>
        /// Instructional Program Type
        /// Type of instructional program; select from an existing enumeration of such types.
        /// </summary>
        [Obsolete]
        public List<FrameworkItem> InstructionalProgramType { get; set; } = new List<FrameworkItem>();
        [Obsolete]
        public List<string> AlternativeInstructionalProgramType { get; set; } = new List<string>();

        /// <summary>
        /// Helper property - provided a list of CIP codes and API will validate and format 
        /// List of valid Classification of Instructional Program codes. See:
        /// https://nces.ed.gov/ipeds/cipcode/search.aspx?y=55
        /// </summary>
        [Obsolete]
        public List<string> CIP_Codes { get; set; } = new List<string>();
        #endregion

        /// <summary>
        /// Alphanumeric token that identifies this resource and information about the token's originating context or scheme.
        /// <see href="https://credreg.net/ctdl/terms/identifier">Identifier</see>
        /// ceterms:identifier
        /// </summary>
        public List<IdentifierValue> Identifier { get; set; }

        /// <summary>
        /// Jurisdiction Profile
        /// Geo-political information about applicable geographic areas and their exceptions.
        /// <see href="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
        /// </summary>
        [Obsolete]
        public List<JurisdictionProfile> Jurisdiction { get; set; }

        /// <summary>
        /// The publication status of the of this resource.
        /// </summary>
        public string PublicationStatusType { get; set; }

        /// <summary>
        /// A legal document giving official permission to do something with this resource.
        /// xsd:anyURI
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Information about rights held in and over this resource.
        /// </summary>
        public string Rights { get; set; }
        public LanguageMap Rights_Map { get; set; }

        /// <summary>
        /// Authoritative source of an entity's information.
        /// URL 
        /// </summary>
        public string Source { get; set; }

    }
}
