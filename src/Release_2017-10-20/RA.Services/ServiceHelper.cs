using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.SessionState;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization;
using Utilities;

using Models;
using MI = RA.Models.Input;
using MJ = RA.Models.Json;
using RA.Models.Json;
using RA.Models.Input;
using System.Reflection;
using System.Collections;

namespace RA.Services
{
    public class ServiceHelper
    {

        private static string thisClassName = "ServiceHelper";
        static bool isUrlPresent = true;

        static string DEFAULT_GUID = "00000000-0000-0000-0000-000000000000";
        public static string idUrl = ServiceHelper.GetAppKeyValue( "credRegistryResourceUrl" );
        public string codeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
        //
        /// <summary>
        /// Session variable for message to display in the system console
        /// </summary>
        public const string SYSTEM_CONSOLE_MESSAGE = "SystemConsoleMessage";
        public static bool GeneratingCtidIfNotFound()
        {
            bool generatingCtidIfNotFound = ServiceHelper.GetAppKeyValue( "generateCtidIfNotFound", true );

            return generatingCtidIfNotFound;
        }

        #region Code validation
        /// <summary>
        /// Validate CTID
        /// TODO - should we generate if not found
        /// </summary>
        /// <param name="ctid"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static bool IsCtidValid( string ctid, ref List<string> messages )
        {
            bool isValid = true;

            if ( string.IsNullOrWhiteSpace( ctid ) )
            {

                messages.Add( "Error - A CTID property must be entered." );
                return false;
            }
			//just in case, handle old formats
            ctid = ctid.Replace( "urn:ctid:", "ce-" );
            if ( ctid.Length != 39 )
            {
                messages.Add( "Error - Invalid CTID format. The proper format is ce-UUID. ex. ce-84365AEA-57A5-4B5A-8C1C-EAE95D7A8C9B" );
                return false;
            }

            if ( !ctid.StartsWith( "ce-" ) )
            {
                //actually we could add this if missing - but maybe should NOT
                messages.Add( "Error - The CTID property must begin with ce-." );
                return false;
            }
            //now we have the proper length and format, the remainder must be a valid guid
            if ( !ServiceHelper.IsValidGuid( ctid.Substring( 3, 36 ) ) )
            {
                //actually we could add this if missing - but maybe should NOT
                messages.Add( "Error - Invalid CTID format. The proper format is ce-UUID. ex. ce-84365AEA-57A5-4B5A-8C1C-EAE95D7A8C9B" );
                return false;
            }

            return isValid;
        }
        #endregion

        public static string GenerateCtid()
        {
            string ctid = "ce-" + Guid.NewGuid().ToString();

            return ctid;
        }

        public static string AsSchemaDuration( MJ.DurationItem entity )
        {
            string duration = "P";

            if ( entity.Years > 0 )
            {
                duration += entity.Years.ToString() + "Y";
            }
            if ( entity.Months > 0 )
            {
                duration += entity.Months.ToString() + "M";
            }

            if ( entity.Weeks > 0 )
            {
                duration += entity.Weeks.ToString() + "W";
            }

            if ( entity.Days > 0 )
            {
                duration += entity.Days.ToString() + "D";
            }

            if ( entity.Hours > 0 || entity.Minutes > 0 )
                duration += "T";

            if ( entity.Hours > 0 )
            {
                duration += entity.Hours.ToString() + "H";
            }

            if ( entity.Minutes > 0 )
            {
                duration += entity.Minutes.ToString() + "M";
            }

            return duration;
        }
        #region Organization
        /// <summary>
        /// Format list of organization references to a target list 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="propertyName"></param>
        /// <param name="dataIsRequired"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static List<OrganizationBase> FormatOrganizationReferences( List<OrganizationReference> input,
               string propertyName,
               bool dataIsRequired,
               ref List<string> messages,
                bool isQAOrg = false )
        {
            List<OrganizationBase> output = new List<OrganizationBase>();
            EntityReferenceHelper helper = new EntityReferenceHelper();
            foreach ( var target in input )
            {
                if ( FormatOrganizationReference( target, propertyName, ref helper, dataIsRequired, ref messages, isQAOrg ) )
                {
                    if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
                    {
                        output.Add( helper.OrgBaseList[0] );
                    }
                }
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

        /// <summary>
        /// Format single OrganizationReference to List<OrganizationBase>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="propertyName"></param>
        /// <param name="dataIsRequired"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static List<OrganizationBase> FormatOrganizationReferenceToList( OrganizationReference input,
               string propertyName,
               bool dataIsRequired,
               ref List<string> messages,
                bool isQAOrg = false )
        {
            List<OrganizationBase> output = new List<OrganizationBase>();
            EntityReferenceHelper helper = new EntityReferenceHelper();
            if ( FormatOrganizationReference( input, propertyName, ref helper, dataIsRequired, ref messages, isQAOrg ) )
            {
                if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
                {
                    output.Add( helper.OrgBaseList[0] );
                }
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

        /// <summary>
        /// Handle a reference to an entity such as an organizaion. 
        /// The input should either have an @id value, or all of:
        /// - name
        /// - subject webpage
        /// - description
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool FormatOrganizationReference( OrganizationReference entity,
                string propertyName,
                ref EntityReferenceHelper helper,
                bool dataIsRequired,
                ref List<string> messages,
                bool isQAOrg = false )
        {
            bool hasData = false;
            bool isValid = true;
            helper = new EntityReferenceHelper();
            OrganizationBase org = new OrganizationBase();
            //org.Context = null;
            //just in case
            helper.OrgBaseList = new List<OrganizationBase>();

            if ( entity == null || entity.IsEmpty() )
            {
                if ( dataIsRequired )
                    messages.Add( string.Format( "Error - a valid organization reference is required for {0}", propertyName ) );

                return false;
            }

            string statusMessage = "";

			if ( !string.IsNullOrWhiteSpace( entity.Id ) )
			{
				//if valid url, then format as an id property and return
				//Note: should this be resolvable, or just a valid uri?
				if ( IsUrlValid( entity.Id, ref statusMessage, ref isUrlPresent ) )
				{
					//var item = new IdProperty() { Id = entity.Id };
					//helper.IdPropertyList.Add( item );

					org.NegateNonIdProperties();
					org.CtdlId = entity.Id;
					if ( isQAOrg )
						org.Type = AgentServices.QACredentialOrganization;
					else
						org.Type = AgentServices.CredentialOrganization;
					//or just agent
					org.Type = MJ.Agent.classType;

					helper.ReturnedDataType = 1;

					helper.OrgBaseList.Add( org );

					//consider whether a warning should be returned if additional data was included - that is, it will be ignored.
					return true;
				}
				else
				{
					messages.Add( string.Format( "Invalid Id property for {0} ({1}). When the Id property is provided for an organization, it must be a valid resolvable URL", propertyName, statusMessage ) );
					return false;
				}
			}
			else if ( !string.IsNullOrWhiteSpace( entity.CTID ) )
			{
				if ( !IsCtidValid( entity.CTID, ref messages ) )
					return false;
				org.NegateNonIdProperties();
				string url = string.Format(UtilityManager.GetAppKeyValue( "credRegistryResourceIdTemplate" ),entity.CTID) ;
				org.CtdlId = url;
				return true;
			}

			// ============================================================

			helper.ReturnedDataType = 2;
            if ( dataIsRequired && entity.HasNecessaryProperties() == false )
            {
                messages.Add( string.Format( "Invalid Organization reference for {0}. Either a resolvable URL must be provided in the Id property, or all of the following properties are expected: Type, Name, Description, Subject Webpage, and Social Media.", propertyName ) );
                return false;
            }

            //set id to null
            org.CtdlId = null;
            //a type must be provided
            //TODO - not clear if necessary??
            bool orgReferencesRequireOrgType = UtilityManager.GetAppKeyValue( "orgReferencesRequireOrgType", false );

            string orgType = "";
            if ( ValidateOrgType( entity.Type, ref orgType,
                ref messages ) )
            {
                org.Type = orgType;
                //or just agent
                org.Type = MJ.Agent.classType;
            }
            else
            {
                if ( orgReferencesRequireOrgType )
                    isValid = false;
            }

            //at this point, all data should be present
            //there is no reason to provide an org ref id not

            if ( !string.IsNullOrWhiteSpace( entity.Name ) )
            {
                org.Name = entity.Name;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.Description ) )
            {
                org.Description = entity.Description;
                hasData = true;
            }

            if ( string.IsNullOrWhiteSpace( entity.SubjectWebpage ) )
            {
                messages.Add( string.Format( "A Subject Webpage must be entered with organization reference for {0}.", propertyName ) );
            }
            else
           if ( !IsUrlValid( entity.SubjectWebpage, ref statusMessage, ref isUrlPresent ) )
                messages.Add( string.Format( "The Subject Webpage for {0} is invalid: {1}", propertyName, statusMessage ) );
            else
            {
                if ( isUrlPresent )
                {
                    IdProperty item = new IdProperty() { Id = entity.SubjectWebpage };
                    org.SubjectWebpage.Add( item );
                }
            }

            if ( entity.SocialMedia != null && entity.SocialMedia.Count() > 0 )
            {
                foreach ( var url in entity.SocialMedia )
                {
                    statusMessage = "";
                    if ( !IsUrlValid( url, ref statusMessage, ref isUrlPresent ) )
                        messages.Add( string.Format( "The SocialMedia URL ({0}) for {1} is invalid. ", url, propertyName ) + statusMessage );
                    else
                    {
                        if ( isUrlPresent )
                        {
                            IdProperty id = new IdProperty() { Id = url };
                            org.SocialMedia.Add( id );
                        }
                    }
                }

            }

            if ( !hasData )
            {
                if ( dataIsRequired )
                {
                    isValid = false;
                    messages.Add( "Invalid Organization reference. Either a resolvable URL must provided in the Id property, or the following properties are expected: Name, Description, Subject Webpage, and Social Media." );
                }
            }
            else
            {
                helper.OrgBaseList.Add( org );
            }

            return isValid;
        }


        public static bool ValidateOrgType( string inputType,
                ref string outputType,
                ref List<string> messages )
        {
            bool isValid = false;
            if ( string.IsNullOrWhiteSpace( inputType ) )
            {
                messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
            }
            else if ( "CredentialOrganization" == inputType || "ceterms:CredentialOrganization" == inputType )
            {
                outputType = "ceterms:CredentialOrganization";
                isValid = true;
            }
            else if ( "QACredentialOrganization" == inputType || "ceterms:QACredentialOrganization" == inputType )
            {
                outputType = "ceterms:QACredentialOrganization";
                isValid = true;
            }
            else
            {
                messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
            }

            return isValid;
        }

        #endregion

        #region Entity References 
        public static List<EntityBase> FormatEntityReferences( List<EntityReference> input,
               string classSchema,
               bool dataIsRequired, //may not be necessary
               ref List<string> messages )
        {
            List<EntityBase> output = new List<EntityBase>();
            EntityReferenceHelper helper = new EntityReferenceHelper();
            foreach ( var target in input )
            {
                if ( FormatEntityReference( target, classSchema, ref helper, true, ref messages ) )
                {
                    if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
                    {
                        output.Add( helper.EntityBaseList[0] );
                    }
                }
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

		/// <summary>
		/// Format an entity reference
		/// If classSchema is present, it will be assumed to be valid. Otherwise entity.Type will be validated
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="classSchema"></param>
		/// <param name="helper"></param>
		/// <param name="dataIsRequired"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static bool FormatEntityReference( EntityReference entity,
                string classSchema,
                ref EntityReferenceHelper helper,
                bool dataIsRequired, //may not be necessary
                ref List<string> messages )
        {
            bool hasData = false;
            bool isValid = true;
            helper = new EntityReferenceHelper();
            EntityBase entityBase = new EntityBase();
            //just in case
            helper.EntityBaseList = new List<EntityBase>();

            if ( entity == null || entity.IsEmpty() )
            {
                //ignore
                return false;
            }

            string statusMessage = "";
            if ( !string.IsNullOrWhiteSpace( entity.Id ) )
            {
                //if valid url, then format as an id property and return
                //Note: should this be resolvable, or just a valid uri?
                if ( IsUrlValid( entity.Id, ref statusMessage, ref isUrlPresent ) )
                {
                    entityBase.NegateNonIdProperties();
                    entityBase.CtdlId = entity.Id;
					//the type is not to be included with @id
					entityBase.Type = null;	// classSchema;

                    helper.ReturnedDataType = 1;

                    helper.EntityBaseList.Add( entityBase );

                    //consider whether a warning should be returned if additional data was included - that is, it will be ignored.
                    return true;
                }
                else
                {
                    messages.Add( string.Format( "Invalid Id property for {0}. When the Id property is provided for an entity, it must be a valid resolvable URL", classSchema ) );
                    return false;
                }
            }
			else if ( !string.IsNullOrWhiteSpace( entity.CTID ) )
			{
				if ( !IsCtidValid( entity.CTID, ref messages ) )
					return false;
				entityBase.NegateNonIdProperties();
				string url = string.Format( UtilityManager.GetAppKeyValue( "credRegistryResourceIdTemplate" ), entity.CTID );
				entityBase.CtdlId = url;
				return true;
			}

			// ============================================================
			helper.ReturnedDataType = 2;
            if ( dataIsRequired && entity.HasNecessaryProperties() == false )
            {
                messages.Add( string.Format( "Invalid Entity reference for {0}. Either a resolvable URL must be provided in the Id property, or all of the following properties are expected: Type, Name, Description, and Subject Webpage.", string.IsNullOrWhiteSpace(classSchema) ? entity.Type : classSchema ) );
                return false;
            }

			//if classSchema empty, the entity.Type must be a valid type
			string validSchema = "";
			if (string.IsNullOrWhiteSpace( classSchema ) )
			{
				//entity.Type
				if ( ValidationServices.IsSchemaNameValid( entity.Type, ref validSchema ))
					entityBase.Type = validSchema;
				else
				{
					messages.Add( string.Format( "Invalid Entity Type of {0} for SubjectWebpage: {1}. ", entity.Type, entity.SubjectWebpage ) );
					return false;
				}
			} else
				entityBase.Type = classSchema;
			//set id to null
			entityBase.CtdlId = null;
			
            //at this point, all data should be present
            //there is no reason to provide an entityBase ref id not

            if ( !string.IsNullOrWhiteSpace( entity.Name ) )
            {
                entityBase.Name = entity.Name;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.Description ) )
            {
                entityBase.Description = entity.Description;
                hasData = true;
            }

            if ( string.IsNullOrWhiteSpace( entity.SubjectWebpage ) )
            {
                messages.Add( string.Format( "A Subject Webpage must be entered with organization reference for {0}.", classSchema ) );
            }
            else
           if ( !IsUrlValid( entity.SubjectWebpage, ref statusMessage, ref isUrlPresent ) )
                messages.Add( string.Format( "The Subject Webpage for {0} is invalid: {1}", classSchema, statusMessage ) );
            else
            {
                if ( isUrlPresent )
                {
                    IdProperty item = new IdProperty() { Id = entity.SubjectWebpage };
                    entityBase.SubjectWebpage.Add( item );
                }
            }

            if ( !hasData )
            {
                if ( dataIsRequired )
                {
                    isValid = false;
                    messages.Add( "Invalid Entity reference. Either a resolvable URL must provided in the Id property, or the following properties are expected: Name, Description, and Subject Webpage." );
                }
            }
            else
            {
                helper.EntityBaseList.Add( entityBase );
            }

            return isValid;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="outputType"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static bool ValidateEntityRefType( string inputType,
            ref string outputType,
            ref List<string> messages )
        {
            bool isValid = false;
            if ( string.IsNullOrWhiteSpace( inputType ) )
            {
                messages.Add( "Error - An entity reference type must be entered. For example: ConditionManifest, CostManifest, Digital Badge, DigitalBadge, Doctoral Degree, DoctoralDegree." );
            }


            else
            {
                messages.Add( "Error - An valid CTDL class type must be entered." );
            }

            return isValid;
        }

        #endregion

        #region costs 
        public static List<MJ.CostProfile> FormatCosts( List<MI.CostProfile> costs, ref List<string> messages )
        {
            if ( costs == null || costs.Count == 0 )
                return null;

            List<MJ.CostProfile> list = new List<MJ.CostProfile>();
            var cp = new MJ.CostProfile();

			int cntr = 0;
            foreach ( MI.CostProfile item in costs )
            {
				cntr++;
                cp = new MJ.CostProfile();
                cp.Description = item.Description;

                cp.CostDetails = AssignValidUrlAsPropertyId( item.CostDetails, "Cost Details", ref messages );

                //NEED validation
                cp.Currency = item.Currency;

				if (!string.IsNullOrWhiteSpace( item.Name ) )
					cp.Name = item.Name;
				else
					messages.Add( string.Format( "A Name is required for a Cost Profile(#{0}). ", cntr ) );
				cp.EndDate = item.EndDate;
                cp.StartDate = item.StartDate;

				cp.Condition = item.Condition;
				cp.Jurisdiction = MapJurisdictions( item.Jurisdiction, ref messages );
				//cp.Region = MapRegions( item.Region, ref messages );

				// ============== cost items =========================
				//if using this route, can only have one direct cost!
				//if ( item.DirectCostTypeList != null && item.DirectCostTypeList.Count > 0 )
				//	foreach ( var type in item.DirectCostTypeList )
				//		cp.DirectCostType.Add( FormatCredentialAlignment( "costType", type, ref messages ) );
				/*
				if ( !string.IsNullOrWhiteSpace( item.DirectCostType ) )
				{
					cp.DirectCostType.Add( FormatCredentialAlignment( "costType", item.DirectCostType, ref messages ) );
				}
				else
				{
					messages.Add( string.Format( "A Direct Cost Type is required for each Cost Profile line item ({0}). " )  );
					cp.DirectCostType = null;
				}

                if ( item.ResidencyType != null && item.ResidencyType.Count > 0 )
                    foreach ( var type in item.ResidencyType )
                        cp.ResidencyType.Add( FormatCredentialAlignment( "residency", type, ref messages ) );
                else
                    cp.ResidencyType = null;

                if ( item.AudienceType != null && item.AudienceType.Count > 0 )
                    foreach ( var type in item.AudienceType )
                        cp.AudienceType.Add( FormatCredentialAlignment( "audience", type, ref messages ) );
                else
                    cp.AudienceType = null;
                cp.PaymentPattern = item.PaymentPattern;
                cp.Price = item.Price;
				*/

				//cost items
				//We are publishing 'flattened' cost profiles. So for each cost profile item, add a whole new cost profile
				int icntr = 0;
                foreach ( MI.CostProfileItem cpi in item.CostItems )
                {
					icntr++;
					//initialize cpi related properties - should not be necessary
					//direct cost type is required
					if ( !string.IsNullOrWhiteSpace( cpi.DirectCostType ) )
					{
						cp.DirectCostType.Add( FormatCredentialAlignment( "costType", cpi.DirectCostType, ref messages ) );
					}
					else
					{
						messages.Add( string.Format( "A Direct Cost Type is required for each Cost Profile line item. Cost Profile Name: {0}, item # ({1}). ", cp.Name, icntr ) );
						cp.DirectCostType = null;
					}

					if ( cpi.ResidencyType != null && cpi.ResidencyType.Count > 0 )
						foreach ( var type in cpi.ResidencyType )
							cp.ResidencyType.Add( FormatCredentialAlignment( "residency", type, ref messages ) );
					else
						cp.ResidencyType = null;

					if ( cpi.AudienceType != null && cpi.AudienceType.Count > 0 )
						foreach ( var type in cpi.AudienceType )
							cp.AudienceType.Add( FormatCredentialAlignment( "audience", type, ref messages ) );
					else
						cp.AudienceType = null;
					cp.PaymentPattern = cpi.PaymentPattern;
					cp.Price = cpi.Price;

					//add flattened cost profile
					list.Add( cp );
				}

				//if no items, add the cost profile
				if ( item.CostItems == null || item.CostItems.Count == 0)
					list.Add( cp );

			}

            return list;
        }
        #endregion

        #region ConditionManifest
        public static List<MJ.ConditionManifest> FormatConditionManifest( List<MI.ConditionManifest> list, ref List<string> messages )
        {
            var result = new List<MJ.ConditionManifest>();
            foreach ( var input in list )
            {
                var cm = new MJ.ConditionManifest();
                cm.Ctid = input.Ctid;
                cm.Name = input.Name;
                cm.Description = input.Description;

                cm.Recommends = FormatConditionProfile( input.RecommendedConditions, ref messages );
                cm.Requires = FormatConditionProfile( input.RequiredConditions, ref messages );
                cm.EntryConditions = FormatConditionProfile( input.EntryConditions, ref messages );
                cm.Corequisite = FormatConditionProfile( input.CorequisiteConditions, ref messages );
                result.Add( cm );
            }
            return result;
        }
        #endregion

        #region ConditionProfile
        public static List<MJ.ConditionProfile> FormatConditionProfile( List<MI.ConditionProfile> profiles, ref List<string> messages )
        {
            if ( profiles == null || profiles.Count == 0 )
                return null;
            string status = "";
			EntityReferenceHelper helper = new EntityReferenceHelper();

			var list = new List<MJ.ConditionProfile>();

            foreach ( var input in profiles )
            {
                var cp = new MJ.ConditionProfile();
                cp.Name = input.Name;
                cp.Description = input.Description;
                foreach ( string subjectWebpage in input.SubjectWebpage )
                {
                    if ( !IsUrlValid( subjectWebpage, ref status, ref isUrlPresent ) )
                        messages.Add( string.Format( "The Condtion Profile Subject Webpage is invalid ({0}). ", subjectWebpage ) + status );
                    else
                    {
                        if ( isUrlPresent )
                        {
                            var atId = new IdProperty() { Id = subjectWebpage };
                            cp.SubjectWebpage.Add( atId );
                        }
                    }
                }

                cp.AudienceLevelType = FormatCredentialAlignmentStrings( input.AudienceLevelType );

                cp.AudienceType = FormatCredentialAlignmentStrings( input.AudienceType );

                cp.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );

                cp.Condition = input.Condition;
                cp.SubmissionOf = input.SubmissionOf;

                //cp.AssertedBy = FormatOrganizationReference( input.AssertedBy, "Asserted By", true, ref messages );
				if ( FormatOrganizationReference( input.AssertedBy, "Asserted By", ref helper, true, ref messages ) )
				{
					if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
					{
						cp.AssertedBy = helper.OrgBaseList[ 0 ] ;
					}
				}

				cp.Experience = input.Experience;
                cp.MinimumAge = input.MinimumAge;
                cp.YearsOfExperience = input.YearsOfExperience;
                cp.Weight = input.Weight;

                if ( ValidateCreditUnitOrHoursProperties( input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref messages ) )
                {
                    cp.CreditUnitTypeDescription = input.CreditUnitTypeDescription;
                    //credential alignment object
                    if ( !string.IsNullOrWhiteSpace( input.CreditUnitType ) )
                    {
                        cp.CreditUnitType.Add( FormatCredentialAlignment( "creditUnit", input.CreditUnitType, ref messages ) );
                    }
                    else
                        cp.CreditUnitType = null;

                    cp.CreditUnitValue = input.CreditUnitValue;
                    cp.CreditHourType = input.CreditHourType;
                    cp.CreditHourValue = input.CreditHourValue;
                }

                cp.AlternativeCondition = FormatConditionProfile( input.AlternativeCondition, ref messages );
                cp.EstimatedCosts = FormatCosts( input.EstimatedCosts, ref messages );

                //jurisdictions
                JurisdictionProfile newJp = new JurisdictionProfile();
                foreach ( var jp in input.Jurisdiction )
                {
                    newJp = MapToJurisdiction( jp, ref messages );
                    if ( newJp != null )
                        cp.Jurisdiction.Add( newJp );
                }
                foreach ( var jp in input.ResidentOf )
                {
                    newJp = MapToJurisdiction( jp, ref messages );
                    if ( newJp != null )
                        cp.ResidentOf.Add( newJp );
                }

                //targets
                cp.TargetCredential = FormatEntityReferences( input.TargetCredential, MJ.Credential.classType, false, ref messages );
                cp.TargetAssessment = FormatEntityReferences( input.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
                cp.TargetLearningOpportunity = FormatEntityReferences( input.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

                cp.TargetCompetency = FormatCompetencies( input.RequiresCompetency, "Requires", ref messages );


                list.Add( cp );
            }

            return list;
        }
        #endregion

        #region Connections
        public static List<MJ.ConditionProfile> FormatConnections( List<MI.Connections> requires, ref List<string> messages )
        {
            if ( requires == null || requires.Count == 0 ) return null;

            var list = new List<MJ.ConditionProfile>();
			EntityReferenceHelper helper = new EntityReferenceHelper();
			foreach ( var item in requires )
            {
                var cp = new Models.Json.ConditionProfile();
                //if ( string.IsNullOrWhiteSpace( item.CreditUnitTypeDescription ) )
                //    messages.Add( "CreditUnitTypeDescription is null in Connections" );
                //else
                cp.CreditUnitTypeDescription = item.CreditUnitTypeDescription;

                //cp.AssertedBy = FormatOrganizationReferenceToList( item.AssertedBy, "Asserted By", true, ref messages );
				if ( FormatOrganizationReference( item.AssertedBy, "Asserted By", ref helper, true, ref messages ) )
				{
					if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
					{
						cp.AssertedBy = helper.OrgBaseList[ 0 ];
					}
				}

				cp.CreditUnitType.Add( FormatCredentialAlignment( item.CreditUnitType ) );
                // cp.Name = item.Name;
                cp.Description = item.Description;
                cp.CreditUnitValue = item.CreditUnitValue;
                cp.CreditHourType = item.CreditHourType;
                cp.CreditHourValue = item.CreditHourValue;
                cp.Weight = item.Weight;

                //targets
                //must have at least one target
                cp.TargetCredential = FormatEntityReferences( item.TargetCredential, MJ.Credential.classType, false, ref messages );
                cp.TargetAssessment = FormatEntityReferences( item.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
                cp.TargetLearningOpportunity = FormatEntityReferences( item.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

                list.Add( cp );
            }

            return list;
        }
        #endregion

        #region ProcessProfile
        public static List<MJ.ProcessProfile> FormatProcessProfile( List<MI.ProcessProfile> profiles, ref List<string> messages )
        {
            if ( profiles == null || profiles.Count == 0 )
                return null;
            string status = "";
            EntityReferenceHelper helper = new EntityReferenceHelper();
            var output = new List<MJ.ProcessProfile>();
            foreach ( var input in profiles )
            {
                var cp = new MJ.ProcessProfile
                {
                    DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages ),
                    Description = input.Description,

                    ProcessFrequency = input.ProcessFrequency,
                    ProcessMethodDescription = input.ProcessMethodDescription,
                    ProcessStandardsDescription = input.ProcessStandardsDescription,
                    ScoringMethodDescription = input.ScoringMethodDescription,
                    ScoringMethodExampleDescription = input.ScoringMethodExampleDescription,
                    VerificationMethodDescription = input.VerificationMethodDescription
                };

                cp.SubjectWebpage = AssignValidUrlAsPropertyIdList( input.SubjectWebpage, "Subject Webpage", ref messages, false );

                foreach ( var type in input.ExternalInputType )
                    cp.ExternalInputType.Add( FormatCredentialAlignment( type ) );

                //short replacement method
                cp.ProcessMethod = AssignValidUrlAsPropertyId( input.ProcessMethod, "Process Method", ref messages );

                cp.ProcessStandards = AssignValidUrlAsPropertyId( input.ProcessStandards, "Process Standards", ref messages );

                cp.ScoringMethodExample = AssignValidUrlAsPropertyId( input.ScoringMethodExample, "Scoring Method Example", ref messages );

                cp.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );
				//cp.Region = MapRegions( input.Region, ref messages );
				cp.ProcessingAgent = FormatOrganizationReferenceToList( input.ProcessingAgent, "Processing Agent", true, ref messages );

				//targets
				cp.TargetCredential = FormatEntityReferences( input.TargetCredential, MJ.Credential.classType, false, ref messages );
                cp.TargetAssessment = FormatEntityReferences( input.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
                cp.TargetLearningOpportunity = FormatEntityReferences( input.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

                output.Add( cp );
            }

            return output;
        }
        #endregion

        #region estimatedDuration
        public static List<MJ.DurationProfile> FormatDuration( List<MI.DurationProfile> input, ref List<string> messages )
        {
            if ( input == null || input.Count == 0 )
                return null;

            List<MJ.DurationProfile> list = new List<MJ.DurationProfile>();
            var cp = new MJ.DurationProfile();

            foreach ( MI.DurationProfile item in input )
            {
                cp = new MJ.DurationProfile
                {
                    Description = item.Description,

                    ExactDuration = FormatDurationItem( item.ExactDuration ),
                    MaximumDuration = FormatDurationItem( item.MaximumDuration ),
                    MinimumDuration = FormatDurationItem( item.MinimumDuration )
                };
                // only allow an exact duration or a range, not both 
                if ( cp.IsRange && cp.ExactDuration.HasValue )
                    messages.Add( "Duration Profile error - provide either an exact duration or a minimum and maximum range, but not both." );
                else if ( cp.IsRange || cp.ExactDuration.HasValue )
                    list.Add( cp );
            }
            if ( list.Count > 0 )
                return list;
            else
                return null;
        }

        private static MJ.DurationItem FormatDurationItem( MI.DurationItem duration )
        {
            if ( duration == null ) return null;
            var output = new MJ.DurationItem
            {
                Days = duration.Days,
                Hours = duration.Hours,
                Minutes = duration.Minutes,
                Months = duration.Months,
                Weeks = duration.Weeks,
                Years = duration.Years
            };

            return output;
        }
        #endregion

        #region === Jurisdictions/addresses ===
        public static List<JurisdictionProfile> MapJurisdictions( List<Models.Input.Jurisdiction> list, ref List<string> messages )
        {
            List<JurisdictionProfile> output = new List<JurisdictionProfile>();
            if ( list == null || list.Count == 0 )
                return null;
            JurisdictionProfile jp = new JurisdictionProfile();
            foreach ( var j in list )
            {
                jp = MapToJurisdiction( j, ref messages );
                if ( jp != null )
                    output.Add( jp );
            }

            return output;
        }
        public static JurisdictionProfile MapJurisdictionAssertions( MI.JurisdictionAssertedInProfile profile,
            ref EntityReferenceHelper helper,
            ref List<string> messages )
        {

            JurisdictionProfile jp = new JurisdictionProfile();

            jp = MapToJurisdiction( profile.Jurisdiction, ref messages );
            if ( jp == null )
                return null;
			//additional check for asserted by org, and list of assertion types
			//jp.AssertedBy = FormatOrganizationReferenceToList( profile.AssertedBy, "Asserted By", true, ref messages );

			if ( FormatOrganizationReference( profile.AssertedBy, "Asserted By", ref helper, true, ref messages ) )
			{
				if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
				{
					jp.AssertedBy = helper.OrgBaseList[ 0 ];
				}
			}

			//??are we assuming an error here?
			if ( jp.AssertedBy == null )
                return null;

            return jp;
        }

        public static MJ.JurisdictionProfile MapToJurisdiction( Models.Input.Jurisdiction jp, ref List<string> messages )
        {
            var jpOut = new Models.Json.JurisdictionProfile();
            //make sure there is data here!!!
            jpOut.Description = jp.Description;
            //NEED to handle at least the main jurisdiction
            if ( jp.MainJurisdiction != null &&
                !string.IsNullOrEmpty( jp.MainJurisdiction.Name ) )
            {
                jpOut.GlobalJurisdiction = null;
                var gc = new MJ.GeoCoordinates();
                if ( FormatGeoCoordinates( jp.MainJurisdiction, ref gc, ref messages ) )
                {
                    //jpOut.MainJurisdiction = ( gc );
                    jpOut.MainJurisdiction = new List<Models.Json.GeoCoordinates>();
                    jpOut.MainJurisdiction.Add( gc );
                }
            }
            else if ( jp.GlobalJurisdiction != null )
            {
                jpOut.MainJurisdiction = null;
                jpOut.GlobalJurisdiction = jp.GlobalJurisdiction;
            }
            else
            {
                //must have a description
                if ( string.IsNullOrWhiteSpace( jp.Description ) )
                {
                    messages.Add( "A jurisdiction profile must contain either:<ul><li>A global jurisdiction of true</li><li>A main jurisdiction with a valid GeoCoordinate</li><li></li></ul>" );
                    return null;
                }
            }

            //and exceptions
            if ( jp.JurisdictionException == null || jp.JurisdictionException.Count == 0 )
                jpOut.JurisdictionException = null;
            else
            {
                MJ.GeoCoordinates gc = new MJ.GeoCoordinates();
                foreach ( var item in jp.JurisdictionException )
                {
                    gc = new MJ.GeoCoordinates();
                    if ( FormatGeoCoordinates( item, ref gc, ref messages ) )
                        jpOut.JurisdictionException.Add( gc );

                }
            }
            return jpOut;
        }

        public static List<JurisdictionProfile> JurisdictionProfileAdd( JurisdictionProfile input, List<JurisdictionProfile> output )
        {
            if ( input == null )
                return output;
            if ( output == null )
                output = new List<JurisdictionProfile>();
            output.Add( input );
            return output;
        }

        public static List<MJ.GeoCoordinates> MapRegions( List<Models.Input.GeoCoordinates> list, ref List<string> messages )
        {
            List<MJ.GeoCoordinates> output = new List<MJ.GeoCoordinates>();
            if ( list == null || list.Count == 0 )
                return null;

            return output;
        }
        public static bool FormatGeoCoordinates( MI.GeoCoordinates input, ref MJ.GeoCoordinates entity, ref List<string> messages )
        {
            bool isValid = true;
            entity = new MJ.GeoCoordinates();
            string statusMessage = "";

            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                //although the name will usually equal the country or region
                messages.Add( "Error - a name must be provided with a jurisidiction GeoCoordinate. The name is typically the country or region within a country, but could also be a continent." );
                isValid = false;
            }
            else
                entity.Name = input.Name;

            //we don't know for sure if the url is a useful geonames like one.
            if ( !string.IsNullOrWhiteSpace( input.Url ) )
            {
                if ( IsUrlValid( input.Url, ref statusMessage, ref isUrlPresent ) )
                {
                    entity.GeoURI = new IdProperty() { Id = input.Url };
                    //should any additional data be also provided?
                    //entity.Country = input.Country;
                    entity.Name = input.Region;
                    entity.Latitude = input.Latitude;
                    entity.Longitude = input.Longitude;
					if (input.Address != null)
					{
						entity.Address.Name = input.Address.Name;
						//entity.Address.StreetAddress	<== actually published
						entity.Address = FormatAddress( input.Address );

						//entity.Address.Address1 = input.Address.Address1;
						////entity.Address.Address2 = input.Address.Address2;
						//entity.Address.AddressRegion = input.Address.AddressRegion;
						//entity.Address.City = input.Address.City;
						//entity.Address.PostalCode = input.Address.PostalCode;
						//entity.Address.PostOfficeBoxNumber = input.Address.PostOfficeBoxNumber;
						//entity.Address.Country = input.Address.Country;
					}

                }
                else
                {
                    //must have some geo url
                    messages.Add( "Error - a valid geo coded URL, such as from geonames.com, must be provided." );
                    isValid = false;
                }

            }
            else
            {
                //where no url, can we accept just text?
                //==> won't be validated!
                //entity.Country = input.Country;
                //entity.Region = input.Region;

                //entity.Latitude = input.Latitude;
                //entity.Longitude = input.Longitude;

                messages.Add( "Error - a valid geo coded URL, such as from geonames.com, must be provided." );
                isValid = false;

                //will need separate method for geocoordinates with addresses
                //entity.Region = input.Region;
                //entity.Country = input.Country;
            }

            if ( !isValid )
                entity = null;

            return isValid;
        }

        public static List<MJ.AvailableAt> FormatAvailableAt( List<MI.PostalAddress> addresses, ref List<string> messages )
        {

            List<MJ.AvailableAt> list = new List<MJ.AvailableAt>();
            MJ.AvailableAt output = new MJ.AvailableAt();
            MJ.Address jaddress = new MJ.Address();
            MJ.ContactPoint cp = new MJ.ContactPoint();

            foreach ( var address in addresses )
            {
                output = new MJ.AvailableAt();
                output.Name = address.Name;

				jaddress = FormatAddress( address );
				//jaddress = new MJ.Address()
    //            {
    //                Name = address.Name,
    //                Address1 = address.Address1,
    //                Address2 = address.Address2,
    //                City = address.City,
    //                Country = address.Country,
    //                AddressRegion = address.AddressRegion,
    //                PostalCode = address.PostalCode
    //            };

                if ( address.ContactPoint != null && address.ContactPoint.Count > 0 )
                {
                    foreach ( var item in address.ContactPoint )
                    {
                        cp = new MJ.ContactPoint()
                        {
                            Name = item.Name,
                            ContactType = item.ContactType
                        };
                        cp.ContactOption = item.ContactOption;
                        cp.PhoneNumbers = item.PhoneNumbers;
                        cp.Emails = item.Emails;
                        cp.SocialMediaPages = HandleUrlFields( item.SocialMediaPages, "Social Media", ref messages );

                        jaddress.ContactPoint.Add( cp );
                    }
                }
                output.Address.Add( jaddress );
                list.Add( output );
            }

            return list;
        }

		public static Models.Json.Address FormatAddress( PostalAddress address )
		{
			Models.Json.Address output = new Models.Json.Address
			{
				Name = address.Name,
				StreetAddress = ( address.Address1 ?? "" ),
				City = address.City,
				Country = address.Country,
				AddressRegion = address.AddressRegion,
				PostalCode = address.PostalCode
			};

			if ( !string.IsNullOrWhiteSpace( address.Address2 ) )
				output.StreetAddress += ", " + address.Address2;

			return output;
		}
		#endregion

		public static bool ValidateCreditUnitOrHoursProperties( decimal creditHourValue, string creditHourType, string creditUnitType, decimal creditUnitValue, string creditUnitTypeDescription, ref List<string> messages )
        {
            //can only have credit hours properties, or credit unit properties, not both
            bool hasCreditHourData = false;
            bool hasCreditUnitData = false;
            if ( creditHourValue > 0 || ( creditHourType ?? "" ).Length > 0 )
                hasCreditHourData = true;
            if ( ( creditUnitType ?? "" ).Length > 0
                || ( creditUnitTypeDescription ?? "" ).Length > 0
                || creditUnitValue > 0 )
                hasCreditUnitData = true;

            if ( hasCreditHourData && hasCreditUnitData )
            {
                messages.Add( "Error: Data can be entered for Credit Hour related properties or Credit Unit related properties, but not for both." );
                return false;
            }

            return true;
        }

        #region === CredentialAlignmentObject ===
        public static List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentStrings( List<string> terms )
        {
            List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
            if ( terms == null || terms.Count == 0 )
                return null;
            foreach ( string item in terms )
            {
                list.Add( FormatCredentialAlignment( item ) );
            }

            return list;
        }
        public static MJ.CredentialAlignmentObject FormatCredentialAlignment( string name )
        {
            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
            ca.TargetNodeName = name;
            return ca;
        }

        public static List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentVocabs( List<string> terms, string name, ref List<string> messages )
        {
            List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
            if ( terms == null || terms.Count == 0 )
                return null;
            foreach ( string item in terms )
            {
                list.Add( FormatCredentialAlignment( "audLevel", item, ref messages ) );
            }

            return list;
        }

        /// <summary>
        /// Format Vocabulary properties/Concepts as credential alignment objects.
        /// </summary>
        /// <param name="vocabulary"></param>
        /// <param name="name"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static MJ.CredentialAlignmentObject FormatCredentialAlignment( string vocabulary, string name, ref List<string> messages )
        {
            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
            CodeItem code = new CodeItem();
            if ( vocabulary.IndexOf( ":" ) == -1 )
                vocabulary += ":";
            //do code look up
            if ( name.IndexOf( vocabulary ) == -1 )
                name = vocabulary + name.Trim();
            if ( ValidationServices.IsCodeValid( vocabulary, name, ref code ) )
            {
                IdProperty item = new IdProperty() { Id = name };
                ca.TargetNode = item;
                ca.TargetNodeName = code.Name;
                ca.TargetNodeDescription = code.Description;
            }
            else
            {
                messages.Add( string.Format( "Warning - The {0} type of {1} is invalid.", vocabulary, name ) );
            }

            return ca;
        }

        public static MJ.CredentialAlignmentObject AgentServiceFormatCredentialAlignment( string vocabulary, string name, ref List<string> messages )
        {
            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
            CodeItem code = new CodeItem();
            if ( vocabulary.IndexOf( ":" ) == -1 )
                vocabulary += ":";
            //do code look up
            if ( name.IndexOf( vocabulary ) == -1 )
                name = vocabulary + name.Trim();
            if ( ValidationServices.IsAgentServiceCodeValid( vocabulary, name, ref code ) )
            {
                IdProperty item = new IdProperty() { Id = name };
                ca.TargetNode = item;
                ca.TargetNodeName = code.Name;
                ca.TargetNodeDescription = code.Description;
            }
            else
            {
                messages.Add( string.Format( "Warning - The {0} type of {1} is invalid.", vocabulary, name ) );
            }

            return ca;
        }
        public static MJ.CredentialAlignmentObject FormatCredentialAlignment( FrameworkItem entity, bool includingCodedNotation )
        {
            bool hasData = false;
            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
            if ( !string.IsNullOrWhiteSpace( entity.Name ) )
            {
                ca.TargetNodeName = entity.Name;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.Description ) )
            {
                ca.TargetNodeDescription = entity.Description;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.FrameworkName ) )
            {
                ca.FrameworkName = entity.FrameworkName;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.CodedNotation ) )
            {
                if ( includingCodedNotation )
                {
                    ca.CodedNotation.Add( entity.CodedNotation );
                    hasData = true;
                }

            }
            if ( !hasData )
                ca = null;
            return ca;
        }
        public static List<MJ.CredentialAlignmentObject> FormatCompetencies( List<MI.CredentialAlignmentObject> entities, string alignmentType, ref List<string> messages )
        {
            List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
            MJ.CredentialAlignmentObject cao = new MJ.CredentialAlignmentObject();

            if ( entities == null || entities.Count == 0 )
                return null;
            foreach ( var item in entities )
            {
                cao = FormatCompetency( item, alignmentType, ref messages );
                if ( cao != null )
                    list.Add( cao );
            }

            return list;
        }//

        public static MJ.CredentialAlignmentObject FormatCompetency( MI.CredentialAlignmentObject entity, string alignmentType, ref List<string> messages )
        {
            bool hasData = false;
            if ( entity == null )
                return null;

            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
            //if present, the framework must be a valid url
            if ( !string.IsNullOrWhiteSpace( entity.Framework ) )
            {
                ca.Framework = AssignValidUrlAsPropertyId( entity.Framework, "Competency Framework", ref messages, false );
                if ( ca.Framework != null )
                    hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.FrameworkName ) )
            {
                ca.FrameworkName = entity.FrameworkName;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.CodedNotation ) )
            {
                ca.CodedNotation.Add( entity.CodedNotation );
                //hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.TargetNode ) )
            {
                ca.TargetNode = AssignValidUrlAsPropertyId( entity.TargetNode, "Competency", ref messages, false );
                if ( ca.TargetNode != null )
                    hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.TargetNodeName ) )
            {
                ca.TargetNodeName = entity.TargetNodeName;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.TargetNodeDescription ) )
            {
                ca.TargetNodeDescription = entity.TargetNodeDescription;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( alignmentType ) )
            {
                ca.AlignmentType = alignmentType;
                //alignmentType is not enough for valid data
                //hasData = true;
            }
            if ( !hasData )
                ca = null;
            return ca;
        }
        #endregion

        #region ID property helpers
        /// <summary>
        /// Validate a URL, and if valid assign to an IdProperty
        /// </summary>
        /// <param name="url">Url to validate</param>
        /// <param name="propertyName">Literal for property name - for messages</param>
        /// <param name="messages"></param>
        /// <param name="isRequired">If true, produce a message if url is missing</param>
        /// <returns>null or an IdProperty</returns>
        public static IdProperty AssignValidUrlAsPropertyId( string url, string propertyName, ref List<string> messages, bool isRequired = false )
        {
            string statusMessage = "";
            IdProperty idProp = null;
            if ( string.IsNullOrWhiteSpace( url ) )
            {
                if ( isRequired )
                    messages.Add( string.Format( "The {0} URL is a required property.", propertyName ) );
                return null;
            }

            if ( !IsUrlValid( url, ref statusMessage, ref isUrlPresent ) )
            {
                if ( isUrlPresent )
                {
                    messages.Add( string.Format( "The {0} URL is invalid. {1}", propertyName, statusMessage ) );
                }
                return null;
            }

            idProp = new IdProperty() { Id = url };

            return idProp;
        } //
        public static List<IdProperty> AssignValidUrlAsPropertyIdList( string url, string propertyName, ref List<string> messages, bool isRequired = false )
        {
            string status = "";
            List<IdProperty> urlId = new List<IdProperty>();
            if ( string.IsNullOrWhiteSpace( url ) )
            {
                if ( isRequired )
                    messages.Add( string.Format( "The {0} URL is a required property.", propertyName ) );
                return null;
            }

            if ( !IsUrlValid( url, ref status, ref isUrlPresent ) )
                messages.Add( string.Format( "The URL for {0} is invalid. ", propertyName ) + status );
            else
            {
                if ( isUrlPresent )
                {
                    IdProperty item = new IdProperty() { Id = url };
                    urlId.Add( item );
                }
            }

            return urlId;
        }

        public static List<IdProperty> AssignValidUrlAsPropertyIdList( List<string> list, string title, ref List<string> messages )
        {
            string status = "";
            List<IdProperty> urlId = new List<IdProperty>();
            if ( list == null || list.Count == 0 )
                return null;
            int cntr = 0;
            foreach ( string url in list )
            {
                cntr++;
                if ( !string.IsNullOrWhiteSpace( url ) )
                {
                    if ( !IsUrlValid( url, ref status, ref isUrlPresent ) )
                        messages.Add( string.Format( "The URL #{0}: {1} for list: {2} is invalid. ", cntr, url, title ) + status );
                    else
                    {
                        if ( isUrlPresent )
                        {
                            urlId.Add( new IdProperty() { Id = url } );
                        }
                    }
                }
            }
            if ( cntr == 0 )
                return null;
            else
                return urlId;
        }
        public static List<IdProperty> HandleUrlFields( List<string> list, string title, ref List<string> messages )
        {
            string status = "";
            List<IdProperty> urlId = new List<IdProperty>();
            if ( list == null || list.Count == 0 )
                return null;
            int cntr = 0;
            foreach ( string url in list )
            {
                cntr++;
                if ( !string.IsNullOrWhiteSpace( url ) )
                {
                    if ( !IsUrlValid( url, ref status, ref isUrlPresent ) )
                        messages.Add( string.Format( "The URL #{0}: {1} for list: {2} is invalid. ", cntr, url, title ) + status );
                    else
                    {
                        if ( isUrlPresent )
                        {
                            urlId.Add( new IdProperty() { Id = url } );
                        }
                    }
                }
            }
            if ( cntr == 0 )
                return null;
            else
                return urlId;
        }
        #endregion
        public static List<IdentifierValue> AssignIdentifierValue( string value )
        {
            string status = "";
            if ( string.IsNullOrWhiteSpace( value ) )
                return null;

            List<IdentifierValue> list = new List<IdentifierValue>();
            list.Add( new IdentifierValue()
            {
                IdentifierValueCode = value
            } );

            return list;
        }

        public static List<string> AssignStringToList( string value )
        {
            string status = "";
            if ( string.IsNullOrWhiteSpace( value ) )
                return null;

            List<string> list = new List<string>();
            list.Add( value );

            return list;
        }
        #region JSON helpers
        public static JsonSerializerSettings GetJsonSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new EmptyNullResolver()
            };

            return settings;
        }
        //Force properties to be serialized in alphanumeric order
        public class AlphaNumericContractResolver : DefaultContractResolver
        {
            protected override System.Collections.Generic.IList<JsonProperty> CreateProperties( System.Type type, MemberSerialization memberSerialization )
            {
                return base.CreateProperties( type, memberSerialization ).OrderBy( m => m.PropertyName ).ToList();
            }
        }

        public class EmptyNullResolver : AlphaNumericContractResolver
        {
            protected override JsonProperty CreateProperty( MemberInfo member, MemberSerialization memberSerialization )
            {
                var property = base.CreateProperty( member, memberSerialization );
                var isDefaultValueIgnored = ( ( property.DefaultValueHandling ?? DefaultValueHandling.Ignore ) & DefaultValueHandling.Ignore ) != 0;

                if ( isDefaultValueIgnored )
                    if ( !typeof( string ).IsAssignableFrom( property.PropertyType ) && typeof( IEnumerable ).IsAssignableFrom( property.PropertyType ) )
                    {
                        Predicate<object> newShouldSerialize = obj =>
                        {
                            var collection = property.ValueProvider.GetValue( obj ) as ICollection;
                            return collection == null || collection.Count != 0;
                        };
                        Predicate<object> oldShouldSerialize = property.ShouldSerialize;
                        property.ShouldSerialize = oldShouldSerialize != null ? o => oldShouldSerialize( oldShouldSerialize ) && newShouldSerialize( oldShouldSerialize ) : newShouldSerialize;
                    }
                    else if ( typeof( string ).IsAssignableFrom( property.PropertyType ) )
                    {
                        Predicate<object> newShouldSerialize = obj =>
                        {
                            var value = property.ValueProvider.GetValue( obj ) as string;
                            return !string.IsNullOrEmpty( value );
                        };

                        Predicate<object> oldShouldSerialize = property.ShouldSerialize;
                        property.ShouldSerialize = oldShouldSerialize != null ? o => oldShouldSerialize( oldShouldSerialize ) && newShouldSerialize( oldShouldSerialize ) : newShouldSerialize;
                    }
                return property;
            }
        }
        #endregion

        #region === Security related Methods ===
        public static bool ValidateApiKey( string apiKey, ref string statusMessage )
        {
            if ( UtilityManager.GetAppKeyValue( "requiringApiKey", true ) == false )
            {
                return true;
            }
            else
            {
                //validate APIkey
                if ( string.IsNullOrWhiteSpace( apiKey ) )
                {
                    statusMessage = "An API Key must be provided.";
                }
                else
                {
                    string encrypted = UtilityManager.Encrypt( apiKey );
                    if ( encrypted == UtilityManager.GetAppKeyValue( "siteApiKey" ) )
                        return true;
                    else
                    {
                        statusMessage = "The Api Key is not a known key.";
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Encrypt the text using MD5 crypto service
        /// This is used for one way encryption of a user password - it can't be decrypted
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt( string data )
        {
            byte[] byDataToHash = ( new UnicodeEncoding() ).GetBytes( data );
            byte[] bytHashValue = new MD5CryptoServiceProvider().ComputeHash( byDataToHash );
            return BitConverter.ToString( bytHashValue );
        }

        /// <summary>
        /// Encrypt the text using the provided password (key) and CBC CipherMode
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Encrypt_CBC( string text, string password )
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes( password );

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if ( len > keyBytes.Length )
                len = keyBytes.Length;

            System.Array.Copy( pwdBytes, keyBytes, len );

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes( text );

            byte[] cipherBytes = transform.TransformFinalBlock( plainText, 0, plainText.Length );

            return Convert.ToBase64String( cipherBytes );

        }

        /// <summary>
        /// Decrypt the text using the provided password (key) and CBC CipherMode
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Decrypt_CBC( string text, string password )
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String( text );

            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes( password );

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if ( len > keyBytes.Length )
                len = keyBytes.Length;

            System.Array.Copy( pwdBytes, keyBytes, len );

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock( encryptedData, 0, encryptedData.Length );

            return Encoding.UTF8.GetString( plainText );

        }

        #endregion


        #region Helpers and validaton

        public static bool IsUrlValid( string url, ref string statusMessage, ref bool urlPresent )
        {
            statusMessage = "";
            if ( string.IsNullOrWhiteSpace( url ) )
            {
                urlPresent = false;
                return true;
            }

            if ( !Uri.IsWellFormedUriString( url, UriKind.Absolute ) )
            {
                statusMessage = "The URL is not in a proper format (for example, must begin with http or https).";
                return false;
            }

            //may need to allow ftp, and others - not likely for this context?
            if ( url.ToLower().StartsWith( "http" ) == false )
            {
                statusMessage = "A URL must begin with http or https";
                return false;
            }

            var isOk = DoesRemoteFileExists( url, ref statusMessage );
            //optionally try other methods, or again with GET
            if ( !isOk && statusMessage == "999" )
                isOk = true;
            //	isOk = DoesRemoteFileExists( url, ref responseStatus, "GET" );
            return isOk;
        } //


        /// <summary>
        /// Checks the file exists or not.
        /// </summary>
        /// <param name="url">The URL of the remote file.</param>
        /// <returns>True : If the file exits, False if file not exists</returns>
        public static bool DoesRemoteFileExists( string url, ref string responseStatus )
        {
            //this is only a conveniece for testing, and is normally false
            if ( UtilityManager.GetAppKeyValue( "skippingLinkChecking", false ) )
                return true;

            bool treatingRemoteFileNotExistingAsError = UtilityManager.GetAppKeyValue( "treatingRemoteFileNotExistingAsError", true );
            //consider stripping off https?
            //or if not found and https, try http
            try
            {
                if ( SkippingValidation( url ) )
                    return true;

                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create( url ) as HttpWebRequest;
                //NOTE - do use the HEAD option, as many sites reject that type of request
                request.Method = "GET";
                //var agent = HttpContext.Current.Request.AcceptTypes;

                //request.Accept = "text/html;text/*;image/*";
                request.ContentType = "text/html;charset=\"utf-8\";image/*";
                //request.Headers.Set( const_AcceptLanguageHeaderName, const_AcceptLanguageHeader );
                request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_2) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1309.0 Safari/537.17";

                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    if ( url.ToLower().StartsWith( "https:" ) )
                    {
                        url = url.ToLower().Replace( "https:", "http:" );
                        LoggingHelper.DoTrace( 5, string.Format( "_____________Failed for https, trying again using http: {0}", url ) );

                        return DoesRemoteFileExists( url, ref responseStatus );
                    }
                    else
                    {
                        LoggingHelper.DoTrace( 5, string.Format( "Url validation failed for: {0}, using method: GET, with status of: {1}", url, response.StatusCode ) );
                    }
                }
                responseStatus = response.StatusCode.ToString();

                return ( response.StatusCode == HttpStatusCode.OK );
                //apparantly sites like Linked In have can be a  problem
                //http://stackoverflow.com/questions/27231113/999-error-code-on-head-request-to-linkedin
                //may add code to skip linked In?, or allow on fail - which the same.
                //or some update, refer to the latter link

                //
            }
            catch ( WebException wex )
            {
                responseStatus = wex.Message;
                //
                if ( wex.Message.IndexOf( "(404)" ) > 1 )
                    return false;
                else if ( wex.Message.IndexOf( "Too many automatic redirections were attempted" ) > -1 )
                    return false;
                else if ( wex.Message.IndexOf( "(999" ) > 1 )
                    return true;
                else if ( wex.Message.IndexOf( "(400) Bad Request" ) > 1 )
                    return true;
                else if ( wex.Message.IndexOf( "(401) Unauthorized" ) > 1 )
                    return true;
                else if ( wex.Message.IndexOf( "(406) Not Acceptable" ) > 1 )
                    return true;
                else if ( wex.Message.IndexOf( "(500) Internal Server Error" ) > 1 )
                    return true;
                else if ( wex.Message.IndexOf( "Could not create SSL/TLS secure channel" ) > 1 )
                {
                    //https://www.naahq.org/education-careers/credentials/certification-for-apartment-maintenance-technicians 
                    return true;

                }
                else if ( wex.Message.IndexOf( "Could not establish trust relationship for the SSL/TLS secure channel" ) > -1 )
                {
                    return true;
                }
                else if ( wex.Message.IndexOf( "The underlying connection was closed: An unexpected error occurred on a send" ) > -1 )
                {
                    return true;
                }
                else if ( wex.Message.IndexOf( "Detail=CR must be followed by LF" ) > 1 )
                {
                    return true;
                }

                //var pageContent = new StreamReader( wex.Response.GetResponseStream() )
                //		 .ReadToEnd();
                if ( !treatingRemoteFileNotExistingAsError )
                {
                    LoggingHelper.LogError( string.Format( "ServiceHelper.DoesRemoteFileExists url: {0}. Exception Message:{1}; URL: {2}", url, wex.Message, GetWebUrl() ), true, "SKIPPING - Exception on URL Checking" );

                    return true;
                }

                LoggingHelper.LogError( string.Format( "ServiceHelper.DoesRemoteFileExists url: {0}. Exception Message:{1}", url, wex.Message ), true, "Exception on URL Checking" );
                responseStatus = wex.Message;
                return false;
            }
            catch ( Exception ex )
            {

                if ( ex.Message.IndexOf( "(999" ) > -1 )
                {
                    //linked in scenario
                    responseStatus = "999";
                }
                else if ( ex.Message.IndexOf( "Could not create SSL/TLS secure channel" ) > 1 )
                {
                    //https://www.naahq.org/education-careers/credentials/certification-for-apartment-maintenance-technicians 
                    return true;

                }
                else if ( ex.Message.IndexOf( "(500) Internal Server Error" ) > 1 )
                {
                    return true;
                }
                else if ( ex.Message.IndexOf( "(401) Unauthorized" ) > 1 )
                {
                    return true;
                }
                else if ( ex.Message.IndexOf( "Could not establish trust relationship for the SSL/TLS secure channel" ) > 1 )
                {
                    return true;
                }
                else if ( ex.Message.IndexOf( "Detail=CR must be followed by LF" ) > 1 )
                {
                    return true;
                }

                if ( !treatingRemoteFileNotExistingAsError )
                {
                    LoggingHelper.LogError( string.Format( "ServiceHelper.DoesRemoteFileExists url: {0}. Exception Message:{1}", url, ex.Message ), true, "SKIPPING - Exception on URL Checking" );

                    return true;
                }

                LoggingHelper.LogError( string.Format( "ServiceHelper.DoesRemoteFileExists url: {0}. Exception Message:{1}", url, ex.Message ), true, "Exception on URL Checking" );
                //Any exception will returns false.
                responseStatus = ex.Message;
                return false;
            }
        }
        private static bool SkippingValidation( string url )
        {
            Uri myUri = new Uri( url );
            string host = myUri.Host;

            string exceptions = UtilityManager.GetAppKeyValue( "urlExceptions" );
            //quick method to avoid loop
            if ( exceptions.IndexOf( host ) > -1 )
                return true;


            //string[] domains = exceptions.Split( ';' );
            //foreach ( string item in domains )
            //{
            //	if ( url.ToLower().IndexOf( item.Trim() ) > 5 )
            //		return true;
            //}

            return false;
        }
        /// <summary>
        /// Get the current url for reporting purposes
        /// </summary>
        /// <returns></returns>
        private static string GetWebUrl()
        {
            string queryString = "n/a";

            if ( HttpContext.Current != null && HttpContext.Current.Request != null )
                queryString = HttpContext.Current.Request.RawUrl.ToString();

            return queryString;
        }


        public static int StringToInt( string value, int defaultValue )
        {
            int returnValue = defaultValue;
            if ( Int32.TryParse( value, out returnValue ) == true )
                return returnValue;
            else
                return defaultValue;
        }


        public static bool StringToDate( string value, ref DateTime returnValue )
        {
            if ( System.DateTime.TryParse( value, out returnValue ) == true )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IsInteger - test if passed string is an integer
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        public static bool IsInteger( string stringToTest )
        {
            int newVal;
            bool result = false;
            try
            {
                newVal = Int32.Parse( stringToTest );

                // If we get here, then number is an integer
                result = true;
            }
            catch
            {

                result = false;
            }
            return result;

        }


        /// <summary>
        /// IsDate - test if passed string is a valid date
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        public static bool IsDate( string stringToTest, bool doingReasonableCheck = true )
        {

            DateTime newDate;
            bool result = false;
            try
            {
                newDate = System.DateTime.Parse( stringToTest );
                result = true;
                //check if reasonable - may what a lower date, for older organizations
                if ( doingReasonableCheck && newDate < new DateTime( 1800, 1, 1 ) )
                    result = false;
            }
            catch
            {
                result = false;
            }
            return result;

        } //end

        public static string MapDate( string date, string dateName, ref List<string> messages, bool doingReasonableCheck = true )
        {
            if ( string.IsNullOrWhiteSpace( date ) )
                return null;

            DateTime newDate = new DateTime();

            if ( DateTime.TryParse( date, out newDate ) )
            {
                if ( doingReasonableCheck && newDate < new DateTime( 1800, 1, 1 ) )
                    messages.Add( string.Format( "Error - {0} is out of range (prior to 1800-01-01 ", dateName ) );
            }
            else
            {
                messages.Add( string.Format( "Error - {0} is invalid ", dateName ) );
                return null;
            }
            return newDate.ToString( "yyyy-MM-dd" );

        } //end

        public static bool IsValidGuid( Guid field )
        {
            if ( ( field == null || field == Guid.Empty ) )
                return false;
            else
                return true;
        }
        public static bool IsValidGuid( string field )
        {
            Guid guidOutput;
            if ( ( field == null || field.ToString() == DEFAULT_GUID ) )
                return false;
            else if ( !Guid.TryParse( field, out guidOutput ) )
                return false;
            else
                return true;
        }
        /// <summary>
        /// Check if the passed dataset is indicated as one containing an error message (from a web service)
        /// </summary>
        /// <param name="wsDataset">DataSet for a web service method</param>
        /// <returns>True if dataset contains an error message, otherwise false</returns>
        public static bool HasErrorMessage( DataSet wsDataset )
        {

            if ( wsDataset.DataSetName == "ErrorMessage" )
                return true;
            else
                return false;

        } //


        /// <summary>
        /// Convert a comma-separated list (as a string) to a list of integers
        /// </summary>
        /// <param name="csl">A comma-separated list of integers</param>
        /// <returns>A List of integers. Returns an empty list on error.</returns>
        public static List<int> CommaSeparatedListToIntegerList( string csl )
        {
            try
            {
                return CommaSeparatedListToStringList( csl ).Select( int.Parse ).ToList();
            }
            catch
            {
                return new List<int>();
            }

        }

        /// <summary>
        /// Convert a comma-separated list (as a string) to a list of strings
        /// </summary>
        /// <param name="csl">A comma-separated list of strings</param>
        /// <returns>A List of strings. Returns an empty list on error.</returns>
        public static List<string> CommaSeparatedListToStringList( string csl )
        {
            try
            {
                return csl.Trim().Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries ).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        #endregion


        #region === Application Keys Methods ===

        /// <summary>
        /// Gets the value of an application key from web.config. Returns blanks if not found
        /// </summary>
        /// <remarks>This clientProperty is explicitly thread safe.</remarks>
        public static string GetAppKeyValue( string keyName )
        {

            return GetAppKeyValue( keyName, "" );
        } //

        /// <summary>
        /// Gets the value of an application key from web.config. Returns the default value if not found
        /// </summary>
        /// <remarks>This clientProperty is explicitly thread safe.</remarks>
        public static string GetAppKeyValue( string keyName, string defaultValue )
        {
            string appValue = "";

            try
            {
                appValue = System.Configuration.ConfigurationManager.AppSettings[keyName];
                if ( appValue == null )
                    appValue = defaultValue;
            }
            catch
            {
                appValue = defaultValue;
                LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
        public static int GetAppKeyValue( string keyName, int defaultValue )
        {
            int appValue = -1;

            try
            {
                appValue = Int32.Parse( System.Configuration.ConfigurationManager.AppSettings[keyName] );

                // If we get here, then number is an integer, otherwise we will use the default
            }
            catch
            {
                appValue = defaultValue;
                LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
        public static bool GetAppKeyValue( string keyName, bool defaultValue )
        {
            bool appValue = false;

            try
            {
                appValue = bool.Parse( System.Configuration.ConfigurationManager.AppSettings[keyName] );

                // If we get here, then number is an integer, otherwise we will use the default
            }
            catch
            {
                appValue = defaultValue;
                LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
        #endregion

        #region Error Logging ================================================
        /// <summary>
        /// Format an exception and message, and then log it
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message">Additional message regarding the exception</param>
        public static void LogError( Exception ex, string message, string subject = "Registry Assistant Application Exception encountered" )
        {
            bool notifyAdmin = false;
            LogError( ex, message, notifyAdmin, subject );
        }

        /// <summary>
        /// Format an exception and message, and then log it
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message">Additional message regarding the exception</param>
        /// <param name="notifyAdmin">If true, an email will be sent to admin</param>
        public static void LogError( Exception ex, string message, bool notifyAdmin, string subject = "Registry Assistant Application Exception encountered" )
        {

            string sessionId = "unknown";
            string remoteIP = "unknown";
            string path = "unknown";
            //string queryString = "unknown";
            string url = "unknown";
            string parmsString = "";

            try
            {
                if ( UtilityManager.GetAppKeyValue( "notifyOnException", "no" ).ToLower() == "yes" )
                    notifyAdmin = true;

                string serverName = GetAppKeyValue( "serverName", "unknown" );

            }
            catch
            {
                //eat any additional exception
            }

            try
            {
                string errMsg = message +
                    "\r\nType: " + ex.GetType().ToString() + ";" +
                    "\r\nSession Id - " + sessionId + "____IP - " + remoteIP +
                    "\r\nException: " + ex.Message.ToString() + ";" +
                    "\r\nStack Trace: " + ex.StackTrace.ToString() +
                    "\r\nServer\\Template: " + path +
                    "\r\nUrl: " + url;

                if ( parmsString.Length > 0 )
                    errMsg += "\r\nParameters: " + parmsString;

                LogError( errMsg, notifyAdmin );
            }
            catch
            {
                //eat any additional exception
            }

        } //


        /// <summary>
        /// Write the message to the log file.
        /// </summary>
        /// <remarks>
        /// The message will be appended to the log file only if the flag "logErrors" (AppSetting) equals yes.
        /// The log file is configured in the web.config, appSetting: "error.log.path"
        /// </remarks>
        /// <param name="message">Message to be logged.</param>
        public static void LogError( string message, string subject = "Registry Assistant Application Exception encountered" )
        {

            if ( GetAppKeyValue( "notifyOnException", "no" ).ToLower() == "yes" )
            {
                LogError( message, true, subject );
            }
            else
            {
                LogError( message, false, subject );
            }

        } //
          /// <summary>
          /// Write the message to the log file.
          /// </summary>
          /// <remarks>
          /// The message will be appended to the log file only if the flag "logErrors" (AppSetting) equals yes.
          /// The log file is configured in the web.config, appSetting: "error.log.path"
          /// </remarks>
          /// <param name="message">Message to be logged.</param>
          /// <param name="notifyAdmin"></param>
        public static void LogError( string message, bool notifyAdmin, string subject = "Registry Assistant Application Exception encountered" )
        {
            if ( GetAppKeyValue( "logErrors" ).ToString().Equals( "yes" ) )
            {
                try
                {
                    string datePrefix = System.DateTime.Today.ToString( "u" ).Substring( 0, 10 );
                    string logFile = GetAppKeyValue( "path.error.log", "C:\\VOS_LOGS.txt" );
                    string outputFile = logFile.Replace( "[date]", datePrefix );

                    StreamWriter file = File.AppendText( outputFile );
                    file.WriteLine( DateTime.Now + ": " + message );
                    file.WriteLine( "---------------------------------------------------------------------" );
                    file.Close();

                    if ( notifyAdmin )
                    {
                        if ( GetAppKeyValue( "notifyOnException", "no" ).ToLower() == "yes" )
                        {
                            EmailManager.NotifyAdmin( subject, message );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    //eat any additional exception
                    DoTrace( 5, thisClassName + ".LogError(string message, bool notifyAdmin). Exception: " + ex.Message );
                }
            }
        } //

        /// <summary>
        /// Sends an email message to the system administrator
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="message">Email message</param>
        /// <returns>True id message was sent successfully, otherwise false</returns>
        public static bool NotifyAdmin( string subject, string message )
        {
            string emailTo = UtilityManager.GetAppKeyValue( "systemAdminEmail", "mparsons@siuccwd.com" );
            //work on implementing some specific routing based on error type


            return EmailManager.NotifyAdmin( emailTo, subject, message );
        }


        /// <summary>
        /// Handle trace requests - typically during development, but may be turned on to track code flow in production.
        /// </summary>
        /// <param name="message">Trace message</param>
        /// <remarks>This is a helper method that defaults to a trace level of 10</remarks>
        public static void DoTrace( string message )
        {
            //default level to 8
            //should get app key value
            int appTraceLevel = UtilityManager.GetAppKeyValue( "appTraceLevel", 8 );
            if ( appTraceLevel < 8 )
                appTraceLevel = 8;
            DoTrace( appTraceLevel, message, true );
        }

        /// <summary>
        /// Handle trace requests - typically during development, but may be turned on to track code flow in production.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void DoTrace( int level, string message )
        {
            DoTrace( level, message, true );
        }

        /// <summary>
        /// Handle trace requests - typically during development, but may be turned on to track code flow in production.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="showingDatetime">If true, precede message with current date-time, otherwise just the message> The latter is useful for data dumps</param>
        public static void DoTrace( int level, string message, bool showingDatetime )
        {
            //TODO: Future provide finer control at the control level
            string msg = "";
            int appTraceLevel = 0;
            //bool useBriefFormat = true;

            try
            {
                appTraceLevel = GetAppKeyValue( "appTraceLevel", 1 );

                //Allow if the requested level is <= the application thresh hold
                if ( level <= appTraceLevel )
                {
                    if ( showingDatetime )
                        msg = "\n " + System.DateTime.Now.ToString() + " - " + message;
                    else
                        msg = "\n " + message;


                    string datePrefix = System.DateTime.Today.ToString( "u" ).Substring( 0, 10 );
                    string logFile = GetAppKeyValue( "path.trace.log", "C:\\VOS_LOGS.txt" );
                    string outputFile = logFile.Replace( "[date]", datePrefix );

                    StreamWriter file = File.AppendText( outputFile );

                    file.WriteLine( msg );
                    file.Close();

                }
            }
            catch
            {
                //ignore errors
            }

        }

        public static void DoBotTrace( int level, string message )
        {
            string msg = "";
            int appTraceLevel = 0;

            try
            {
                appTraceLevel = GetAppKeyValue( "botTraceLevel", 5 );

                //Allow if the requested level is <= the application thresh hold
                if ( level <= appTraceLevel )
                {
                    msg = "\n " + System.DateTime.Now.ToString() + " - " + message;

                    string datePrefix = System.DateTime.Today.ToString( "u" ).Substring( 0, 10 );
                    string logFile = GetAppKeyValue( "path.botTrace.log", "" );
                    if ( logFile.Length > 5 )
                    {
                        string outputFile = logFile.Replace( "[date]", datePrefix );

                        StreamWriter file = File.AppendText( outputFile );

                        file.WriteLine( msg );
                        file.Close();
                    }

                }
            }
            catch
            {
                //ignore errors
            }

        }
        #endregion

        #region Common Utility Methods
        public static string HandleApostrophes( string strValue )
        {

            if ( strValue.IndexOf( "'" ) > -1 )
            {
                strValue = strValue.Replace( "'", "''" );
            }
            if ( strValue.IndexOf( "''''" ) > -1 )
            {
                strValue = strValue.Replace( "''''", "''" );
            }

            return strValue;
        }
        public static String CleanText( String text )
        {
            if ( String.IsNullOrEmpty( text.Trim() ) )
                return String.Empty;

            String output = String.Empty;
            try
            {
                String rxPattern = "<(?>\"[^\"]*\"|'[^']*'|[^'\">])*>";
                Regex rx = new Regex( rxPattern );
                output = rx.Replace( text, String.Empty );
                if ( output.ToLower().IndexOf( "<script" ) > -1
                    || output.ToLower().IndexOf( "javascript" ) > -1 )
                {
                    output = "";
                }
            }
            catch ( Exception ex )
            {

            }

            return output;
        }

        #endregion
    }

}
