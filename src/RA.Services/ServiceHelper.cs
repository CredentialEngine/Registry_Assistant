﻿using System;
using System.Collections;
using System.Collections.Generic;

using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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

using RA.Models;
using MI = RA.Models.Input;
using MJ = RA.Models.Json;
using MIPlace = RA.Models.Input.Place;
using MOPlace = RA.Models.Json.Place;
//using MIPlace = RA.Models.Input.PostalAddress;
//using MOPlace = RA.Models.Json.Address;

using RA.Models.Json;
using RA.Models.Input;
using LanguageMap = RA.Models.JsonV2.LanguageMap;
using LanguageMapList = RA.Models.JsonV2.LanguageMapList;
namespace RA.Services
{
	public class ServiceHelper
	{
        #region Properties
        private static string thisClassName = "ServiceHelper";

		static string DEFAULT_GUID = "00000000-0000-0000-0000-000000000000";
		public static string idBaseUrl = ServiceHelper.GetAppKeyValue( "credRegistryResourceUrl" );
        public string credRegistryGraphUrl = ServiceHelper.GetAppKeyValue( "credRegistryGraphUrl" );
        public static string codeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
        public static bool usingSingleDirectCost = UtilityManager.GetAppKeyValue( "usingSingleDirectCost", false );

        public static bool IsAPublishRequest = true;
        //this can only be true for a format request
        public static bool includingMinDataWithReferenceId = UtilityManager.GetAppKeyValue( "includeMinDataWithReferenceId", false );
		public static string CurrentEntityType = "";
		public static string CurrentEntityName = "";
		public static string CurrentCtid = "";
		public static string LastProfileType = "";

		//
		/// <summary>
		/// Session variable for message to display in the system console
		/// </summary>
		public const string SYSTEM_CONSOLE_MESSAGE = "SystemConsoleMessage";

        static bool requiringQAOrgForQAQRoles = ServiceHelper.GetAppKeyValue("requireQAOrgForQAQRoles", false);
        public static bool GeneratingCtidIfNotFound()
		{
			bool generatingCtidIfNotFound = ServiceHelper.GetAppKeyValue( "generateCtidIfNotFound", true );

			return generatingCtidIfNotFound;
		}
        #endregion
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
				messages.Add( "Error - The CTID property must begin with ce-" );
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

        public static string FormatCtid(string ctid, ref List<string> messages )
        {
            //todo determine if will generate where not found
            if ( string.IsNullOrWhiteSpace(ctid) && GeneratingCtidIfNotFound() )
                ctid = GenerateCtid();

            if ( IsCtidValid(ctid, ref messages) )
            {
                //can't do this yet, as the registry may treat an existing records as new!!!
                if ( UtilityManager.GetAppKeyValue("forceCtidToLowerCase", false) )
                    ctid = ctid.ToLower();

                CurrentCtid = ctid;
            }

            return ctid;
        }
        public static string GenerateCtid()
		{
			string ctid = "ce-" + Guid.NewGuid().ToString().ToLower();

			return ctid;
		}
        #region Entity References 
        public static List<EntityBase> FormatEntityReferences( List<EntityReference> input,
               string classSchema,
               bool dataIsRequired, //may not be necessary
               ref List<string> messages )
        {
            if ( input == null || input.Count == 0 )
                return null;

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
        /// <param name="classSchema">can be blank, where used with generic props like offers, approves</param>
        /// <param name="helper"></param>
        /// <param name="dataIsRequired">True if the property is required</param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static bool FormatEntityReference( EntityReference entity,
                string classSchema,
                ref EntityReferenceHelper helper,
                bool dataIsRequired,
                ref List<string> messages )
        {
            bool hasData = false;
            bool isValid = true;
            bool isUrlPresent = true;
            helper = new EntityReferenceHelper();
            EntityBase entityBase = new EntityBase();
            //just in case
            helper.EntityBaseList = new List<EntityBase>();
            //default
            helper.ReturnedDataType = 1;

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
                //17-10-10 - no may not be resolvable if not yet published. Added false parm for IsUrlValid, so no url check will be done
                if ( IsUrlValid( entity.Id, ref statusMessage, ref isUrlPresent, false ) )
                {
                    entityBase.NegateNonIdProperties();
                    entityBase.CtdlId = entity.Id;
                    //the type is not to be included with @id
                    if ( includingMinDataWithReferenceId && !IsAPublishRequest
                        && !string.IsNullOrWhiteSpace( entity.Name )
                        && !string.IsNullOrWhiteSpace( entity.SubjectWebpage )
                        )
                    {
                        //as means of minimizing impact of pending imports, include basic data
                        entityBase.Name = entity.Name;
                        entityBase.Description = entity.Description;
                        entityBase.SubjectWebpage = entity.SubjectWebpage;
                    }

                    helper.ReturnedDataType = 1;
                    helper.EntityBaseList.Add( entityBase );

                    //consider whether a warning should be returned if additional data was included - that is, it will be ignored.
                    //need to add separate warning messages first
                    return true;
                }
                else
                {
                    messages.Add( string.Format( "Invalid Id property for {0} ({1}). When the Id property is provided for an entity, it must have valid format for a URL", entity.Id, classSchema ) );
                    return false;
                }
            }
            else if ( !string.IsNullOrWhiteSpace( entity.CTID ) )
            {
                if ( !IsCtidValid( entity.CTID, ref messages ) )
                    return false;
                entityBase.NegateNonIdProperties();

                entityBase.CtdlId = idBaseUrl + entity.CTID;

                helper.EntityBaseList.Add( entityBase );
                return true;
            }

            // ============================================================
            helper.ReturnedDataType = 2;
            if ( dataIsRequired && entity.HasNecessaryProperties() == false )
            {
                messages.Add( string.Format( "Invalid Entity reference for {0}. Either a resolvable URL must be provided in the Id property, or all of the following properties are expected: Type: {0}, Name: {1}, and Subject Webpage {2}.", !string.IsNullOrWhiteSpace( entity.Type ) ? entity.Type : classSchema, entity.Name, entity.SubjectWebpage ) );
                return false;
            }

            //if classSchema empty, the entity.Type must be a valid type
            string validSchema = "";
            //start by validating entity type
            if ( ValidationServices.IsSchemaNameValid( entity.Type, ref validSchema ) )
                entityBase.Type = validSchema;
            else if ( ValidationServices.IsSchemaNameValid( classSchema, ref validSchema ) )
            {
                entityBase.Type = validSchema;
            }
            else
            {
                messages.Add( string.Format( "Invalid Entity Type of {0} for Name: {1}, SubjectWebpage: {2}. ", entity.Type, entity.Name, entity.SubjectWebpage ) );
                return false;
            }


            //set id to null
            entityBase.CtdlId = null;
            int msgcount = messages.Count();
            //at this point, all data should be present
            //there is no reason to provide an entityBase ref id not

            if ( string.IsNullOrWhiteSpace( entity.Name ) )
            {
                messages.Add( string.Format( "A Name must be entered with a reference for {0}.", entityBase.Type ) );
            }
            else
            {
                entityBase.Name = entity.Name;
                hasData = true;
            }
            if ( !string.IsNullOrWhiteSpace( entity.Description ) )
            {
                entityBase.Description = entity.Description;
            }

            if ( string.IsNullOrWhiteSpace( entity.SubjectWebpage ) )
            {
                messages.Add( string.Format( "A Subject Webpage must be entered with entity reference for {0}.", entityBase.Type ) );
            }
            else
            if ( !IsUrlValid( entity.SubjectWebpage, ref statusMessage, ref isUrlPresent ) )
            {
                messages.Add( string.Format( "The Subject Webpage for {0} is invalid: {1}", entityBase.Type, statusMessage ) );
            }
            else
            {
                if ( isUrlPresent )
                {
                    entityBase.SubjectWebpage = AssignValidUrlAsString( entity.SubjectWebpage, "Entity reference Subject Webpage", ref messages, true );
                }
            }

            if ( !hasData )
            {
                if ( dataIsRequired )
                {
                    isValid = false;
                    messages.Add( "Invalid Entity reference. Either a resolvable URL must provided in the Id property, or the following properties are expected: Name, and Subject Webpage." );
                }
            }
            else
            {
                helper.EntityBaseList.Add( entityBase );
            }

            if ( messages.Count > msgcount )
                isValid = false;

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

        #region Organization
        public static List<OrganizationBase> FormatOrganizationReferences( object orgReference,
                    string propertyName,
                    bool dataIsRequired,
                    ref List<string> messages,
                    bool isQAOrgReference = false )
        {

            OrganizationReference input = new OrganizationReference();
            List<OrganizationReference> list = new List<OrganizationReference>();
            if ( orgReference.GetType() == typeof( List<OrganizationReference> ) )
            {
                list = orgReference as List<OrganizationReference>;
                return FormatOrganizationReferences( list, propertyName, dataIsRequired, ref messages, isQAOrgReference );
            }
            else if ( orgReference.GetType() == typeof( OrganizationReference ) )
            {
                input = orgReference as OrganizationReference;
                return FormatOrganizationReferenceToList( input, propertyName, dataIsRequired, ref messages, isQAOrgReference );
            }
            else if ( orgReference.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
            {
                Newtonsoft.Json.Linq.JObject o = ( Newtonsoft.Json.Linq.JObject ) orgReference;
                foreach ( Newtonsoft.Json.Linq.JProperty property in o.Properties() )
                {
                    Console.WriteLine( property.Name + " - " + property.Value );
                    if ( property.Name == "Id" ) {
                        input.Id = property.Value.ToString();
                        continue;
                    }
                    if ( property.Name == "CTID" ) input.CTID = property.Value.ToString();
                    if ( property.Name == "Type" ) input.Type = property.Value.ToString();
                    if ( property.Name == "Name" ) input.Name = property.Value.ToString();
                    if ( property.Name == "SubjectWebpage" ) input.SubjectWebpage = property.Value.ToString();
                }
                
                return FormatOrganizationReferenceToList( input, propertyName, dataIsRequired, ref messages, isQAOrgReference );
            }
            else if ( orgReference.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
            {
                Newtonsoft.Json.Linq.JArray ar = new Newtonsoft.Json.Linq.JArray();
                ar = orgReference as Newtonsoft.Json.Linq.JArray;
                //list = ar.ToObject<List<string>>();
                List<OrganizationReference> items = ( ( Newtonsoft.Json.Linq.JArray ) ar ).Select( x => new OrganizationReference
                {
                    Id = ( string ) x[ "id" ]
                } ).ToList();
                //foreach (var item in ar)
                //{
                //    input = new OrganizationReference() { Id = item.First.ToString() };
                //}
                return FormatOrganizationReferences( items, propertyName, dataIsRequired, ref messages, isQAOrgReference );
            }
            else
            {
                //unexpected
                messages.Add( "Error unexpected type for Organization Reference for " + propertyName + " with a type of " + orgReference.GetType().ToString() );
                return null;
            }
        }


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
				bool isQAOrgReference = false )
		{
			if ( input == null || input.Count == 0 )
				return null;

			List<OrganizationBase> output = new List<OrganizationBase>();
			EntityReferenceHelper helper = new EntityReferenceHelper();
			foreach ( var target in input )
			{
				if ( FormatOrganizationReference( target, propertyName, ref helper, dataIsRequired, ref messages, isQAOrgReference ) )
				{
					if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
					{
						output.Add( helper.OrgBaseList[ 0 ] );
					}
				}
			}
            if ( output.Count == 0 )
                return null;
            else
            {
                return output;
            }
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
				bool isQAOrgReference = false )
		{
			List<OrganizationBase> output = new List<OrganizationBase>();
			EntityReferenceHelper helper = new EntityReferenceHelper();
			if ( FormatOrganizationReference( input, propertyName, ref helper, dataIsRequired, ref messages, isQAOrgReference ) )
			{
				if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
				{
					output.Add( helper.OrgBaseList[ 0 ] );
				}
			}
			if ( output.Count == 0 )
				return null;
            else
            {
                return output;
            }
        }

        /// <summary>
        /// Handle a reference to an entity such as an organizaion. 
        /// The input should either have an @id value, or all of:
        /// - name
        /// - subject webpage
        /// - description
        /// </summary>
        /// <param name="entity">OrganizationReference</param>
        /// <param name="propertyName">CTDL Property</param>
        /// <param name="helper"></param>
        /// <param name="dataIsRequired"></param>
        /// <param name="messages"></param>
        /// <param name="isQAOrgReference"></param>
        /// <returns></returns>
        public static bool FormatOrganizationReference( OrganizationReference entity,
				string propertyName,
				ref EntityReferenceHelper helper,
				bool dataIsRequired,
				ref List<string> messages,
				bool isQAOrgReference = false )
		{
			bool hasData = false;
			bool isValid = true;
			bool isUrlPresent = true;
			helper = new EntityReferenceHelper();
			OrganizationBase org = new OrganizationBase();
			//org.Context = null;
			//just in case
			helper.OrgBaseList = new List<OrganizationBase>();
			//default
			helper.ReturnedDataType = 1;

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
				//17-10-10 - no may not be resolvable if not yet published. Added false parm for IsUrlValid, so no url check will be done
				if ( IsUrlValid( entity.Id, ref statusMessage, ref isUrlPresent, false ) )
				{
					//var item = new IdProperty() { Id = entity.Id };
					//helper.IdPropertyList.Add( item );

					org.NegateNonIdProperties();
					org.CtdlId = entity.Id;
					//17-09-?? per Stuart don't include a type with an @id
					org.Type = null;
                    if ( includingMinDataWithReferenceId && !IsAPublishRequest
                        && !string.IsNullOrWhiteSpace( entity.Name)
                        && !string.IsNullOrWhiteSpace( entity.SubjectWebpage )
                        )
                    {
                        //as means of minimizing impact of pending imports, include basic data
                        org.Name = entity.Name;
                        org.Description = entity.Description;
                        org.SubjectWebpage = entity.SubjectWebpage;
                    }
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
				org.CtdlId = idBaseUrl + entity.CTID;
				helper.OrgBaseList.Add( org );
				return true;
			}

			// ============================================================

			helper.ReturnedDataType = 2;
			if ( dataIsRequired && entity.HasNecessaryProperties() == false )
			{
				messages.Add( string.Format( "Invalid Organization reference for {0}. Either a resolvable URL must be provided in the Id property, or all of the following properties are expected: Type: {0}, Name: {1}, and Subject Webpage {2}, and optionally Social Media.", propertyName, entity.Name, entity.SubjectWebpage ) );
				return false;
			}

			//set id to null
			org.CtdlId = null;
            //at this point, all data should be present
            //there is no reason to provide an org ref id not
            int msgcount = messages.Count();
            if ( string.IsNullOrWhiteSpace(entity.Name) )
            {
                messages.Add(string.Format("A Name must be entered with a reference for {0}.", org.Type));
            }
            else
            {
                org.Name = entity.Name;
                hasData = true;
            }
            //a type must be provided
            //TODO - not clear if necessary??
            bool orgReferencesRequireOrgType = UtilityManager.GetAppKeyValue( "orgReferencesRequireOrgType", false );

			string orgType = "";
            bool isQAOrgReferenceType = false;

            if ( ValidateOrgType( entity.Type, ref orgType, ref isQAOrgReferenceType, ref messages ) )
			{
				org.Type = orgType;
				//may need check for being QA if for a QA connection
                if ( isQAOrgReference && !isQAOrgReferenceType)
                {
                    if ( requiringQAOrgForQAQRoles )
                        messages.Add(string.Format("The referenced organization must have a type of ceterms:QACredentialOrganization when used in a QA connection such as Accredits, or Regulates. {0}.", org.Name));
                }
            }
			else
			{
				if ( orgReferencesRequireOrgType )
					isValid = false;
				else
					org.Type = MJ.Agent.classType;
			}


			if ( !string.IsNullOrWhiteSpace( entity.Description ) )
			{
				org.Description = entity.Description;
				//hasData = true;
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
					org.SubjectWebpage = AssignValidUrlAsString( entity.SubjectWebpage, "Organization reference Subject Webpage", ref messages, true );
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
							org.SocialMedia.Add( url );
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

			if ( messages.Count > msgcount )
				isValid = false;

			return isValid;
        }     
        
		public static bool ValidateOrgType( string inputType,
				ref string outputType,
                ref bool isQAOrgReferenceType,
				ref List<string> messages )
		{
			bool isValid = false;
            isQAOrgReferenceType = false;

            if ( string.IsNullOrWhiteSpace( inputType ) )
			{
				messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
			}
			else if ( "credentialorganization" == inputType.ToLower() || "ceterms:credentialorganization" == inputType.ToLower() )
			{
				outputType = "ceterms:CredentialOrganization";
				isValid = true;
			}
			else if ( "qacredentialorganization" == inputType.ToLower() || "ceterms:qacredentialorganization" == inputType.ToLower() )
			{
				outputType = "ceterms:QACredentialOrganization";
				isValid = true;
                isQAOrgReferenceType = true;

            }
			else
			{
				messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
			}

			return isValid;
		}

		#endregion



		#region costs 
		public static List<MJ.CostProfile> FormatCosts( List<MI.CostProfile> costs, ref List<string> messages )
		{
			if ( costs == null || costs.Count == 0 )
				return null;

			List<MJ.CostProfile> output = new List<MJ.CostProfile>();
			var cp = new MJ.CostProfile();
			var hold = new MJ.CostProfile();
			int costNbr = 0;
			foreach ( MI.CostProfile item in costs )
			{
				costNbr++;

				hold = new MJ.CostProfile();
				//NEED validation
				hold.Currency = item.Currency;

				hold.Name = item.Name;
				hold.Description = item.Description;
				hold.CostDetails = AssignValidUrlAsString( item.CostDetails, "Cost Details", ref messages, true );

				hold.EndDate = item.EndDate;
				hold.StartDate = item.StartDate;

				hold.Condition = item.Condition;

				hold.Jurisdiction = MapJurisdictions( item.Jurisdiction, ref messages );
				//cp.Region = MapRegions( item.Region, ref messages );

				//there is only one direct cost type. if price or other cost item data exists, there must be a direct cost - could perhaps handle in the CostProfile manager to simplify this steps

				//cost items - should be an array. If none, add the base information
				if (item.CostItems == null || item.CostItems.Count == 0 )
				{
					cp = new MJ.CostProfile();
					cp.CostDetails = hold.CostDetails;
					cp.Currency = hold.Currency;
					cp.Name = hold.Name;
					cp.Description = hold.Description;
					cp.StartDate = hold.StartDate;
					cp.EndDate = hold.EndDate;
					cp.Condition = hold.Condition;
					cp.Jurisdiction = hold.Jurisdiction;
                    cp.DirectCostType = null;
                    //cp.DirectCostTypes = null;
                    output.Add( cp );
				} else
				{
					foreach ( MI.CostProfileItem cpi in item.CostItems )
					{
						cp = new MJ.CostProfile();
						cp.CostDetails = hold.CostDetails;
						cp.Currency = hold.Currency;
						cp.Name = hold.Name;
						cp.Description = hold.Description;
						cp.StartDate = hold.StartDate;
						cp.EndDate = hold.EndDate;
						cp.Condition = hold.Condition;
						cp.Jurisdiction = hold.Jurisdiction;

                       // if (usingSingleDirectCost)
                       // {
                            cp.DirectCostType = FormatCredentialAlignment( "directCostType", cpi.DirectCostType, ref messages );
                            //cp.DirectCostTypes = null;
                            //cp.DirectCostTypes = FormatCredentialAlignmentVocabToList( "directCostType", cpi.DirectCostType, ref messages );
                       // }
                       // else
                        //{
                            //cp.DirectCostTypes = FormatCredentialAlignmentVocabToList( "directCostType", cpi.DirectCostType, ref messages );
                            //cp.DirectCostType = null;
                        //}
						
						cp.ResidencyType = FormatCredentialAlignmentVocabs( "residencyType", cpi.ResidencyType, ref messages );
						cp.AudienceType = FormatCredentialAlignmentVocabs( "audienceType", cpi.AudienceType, ref messages );

						cp.Price = cpi.Price;
						cp.PaymentPattern = cpi.PaymentPattern;

						output.Add( cp );
					}
				}
			}

			return output;
		}
		#endregion

		//#region ConditionManifest
		//public static List<MJ.ConditionManifest> FormatConditionManifest( List<MI.ConditionManifest> list, ref List<string> messages )
		//{
		//	if ( list == null || list.Count == 0 )
		//		return null;
		//	var result = new List<MJ.ConditionManifest>();
		//	foreach ( var input in list )
		//	{
		//		var cm = new MJ.ConditionManifest();
		//		cm.Ctid = input.Ctid;
		//		cm.Name = input.Name;
		//		cm.Description = input.Description;

		//		cm.Recommends = FormatConditionProfile( input.RecommendedConditions, ref messages );
		//		cm.Requires = FormatConditionProfile( input.RequiredConditions, ref messages );
		//		cm.EntryConditions = FormatConditionProfile( input.EntryConditions, ref messages );
		//		cm.Corequisite = FormatConditionProfile( input.CorequisiteConditions, ref messages );
		//		result.Add( cm );
		//	}
		//	return result;
		//}
		//#endregion

		#region ConditionProfile
		public static List<MJ.ConditionProfile> FormatConditionProfile( List<MI.ConditionProfile> profiles, ref List<string> messages )
		{
			if ( profiles == null || profiles.Count == 0 )
				return null;
			string status = "";
			bool isUrlPresent = true;
			EntityReferenceHelper helper = new EntityReferenceHelper();

			var list = new List<MJ.ConditionProfile>();

			foreach ( var input in profiles )
			{
				var cp = new MJ.ConditionProfile();
				cp.Name = input.Name;
				cp.Description = input.Description;

				//should only be one SWP
				//foreach ( string subjectWebpage in input.SubjectWebpage )
				//{    }
				if ( !IsUrlValid( input.SubjectWebpage, ref status, ref isUrlPresent ) )
					messages.Add( string.Format( "The Condtion Profile Subject Webpage is invalid ({0}). ", input.SubjectWebpage ) + status );
				else
				{
					if ( isUrlPresent )
					{
						cp.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Condition Profile Subject Webpage", ref messages, false );

					}
				}

				//cp.AudienceLevelType = FormatCredentialAlignmentListFromStrings( input.AudienceLevelType );
				//cp.AudienceType = FormatCredentialAlignmentListFromStrings( input.AudienceType );
				//or audType?
				cp.AudienceLevelType = FormatCredentialAlignmentVocabs( "audienceLevelType", input.AudienceLevelType, ref messages );
				cp.AudienceType = FormatCredentialAlignmentVocabs( "audienceType", input.AudienceType, ref messages );

				cp.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );

				cp.Condition = input.Condition;
				cp.SubmissionOf = input.SubmissionOf;

				cp.AssertedBy = FormatOrganizationReferences( input.AssertedBy, "Asserted By", false, ref messages, false );
				//if ( FormatOrganizationReference( input.AssertedBy, "Asserted By", ref helper, false, ref messages ) )
				//{
				//	if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
				//	{
				//		//currently defined as object. Ultimately will be a list, but should be single
				//		List<OrganizationBase> bys = new List<OrganizationBase>();
				//		bys.Add( helper.OrgBaseList[ 0 ] );
				//		cp.AssertedBy = bys;
				//		//OR
				//		//cp.AssertedByList.Add( helper.OrgBaseList[ 0 ] );
				//		//cp.AssertedBy = helper.OrgBaseList[ 0 ] ;
				//	}
				//}

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
						cp.CreditUnitType = FormatCredentialAlignment( "creditUnitType", input.CreditUnitType, ref messages ) ;
					}
					else
						cp.CreditUnitType = null;

					cp.CreditUnitValue = input.CreditUnitValue;
					cp.CreditHourType = input.CreditHourType;
					cp.CreditHourValue = input.CreditHourValue;
				} else
				{
					cp.CreditUnitType = null;
				}

				cp.AlternativeCondition = FormatConditionProfile( input.AlternativeCondition, ref messages );
				cp.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );

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

				cp.TargetCompetency = FormatCompetencies( input.RequiresCompetency, ref messages );


				list.Add( cp );
			}

			return list;
		}
		#endregion

		#region Connections
		public static List<MJ.ConditionProfile> FormatConnections( List<MI.Connections> requires, ref List<string> messages )
		{
			if ( requires == null || requires.Count == 0 )
				return null;

			var list = new List<MJ.ConditionProfile>();
			EntityReferenceHelper helper = new EntityReferenceHelper();
			foreach ( var item in requires )
			{
				var cp = new Models.Json.ConditionProfile();

				//cp.AssertedBy = FormatOrganizationReferenceToList( item.AssertedBy, "Asserted By", true, ref messages );
				if ( FormatOrganizationReference( item.AssertedBy, "Asserted By", ref helper, false, ref messages ) )
				{
					if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
					{
						//currently defined as object. Ultimately will be a list, but should be single
						List<OrganizationBase> bys = new List<OrganizationBase>();
						bys.Add( helper.OrgBaseList[ 0 ] );
						cp.AssertedBy = bys;
						//cp.AssertedBy = helper.OrgBaseList[ 0 ] ;
                        
					}
				}

				if ( ValidateCreditUnitOrHoursProperties( item.CreditHourValue, item.CreditHourType, item.CreditUnitType, item.CreditUnitValue, item.CreditUnitTypeDescription, ref messages ) )
				{
					cp.CreditUnitTypeDescription = item.CreditUnitTypeDescription;
					//credential alignment object
					if ( !string.IsNullOrWhiteSpace( item.CreditUnitType ) )
					{
						cp.CreditUnitType = FormatCredentialAlignment( "creditUnitType", item.CreditUnitType, ref messages );
					}
					else
						cp.CreditUnitType = null;

					cp.CreditUnitValue = item.CreditUnitValue;
					cp.CreditHourType = item.CreditHourType;
					cp.CreditHourValue = item.CreditHourValue;
				}

				cp.Description = item.Description;
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

				cp.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Process Profile Subject Webpage", ref messages, false );

				//or inputType
				cp.ExternalInputType = FormatCredentialAlignmentVocabs( "externalInputType", input.ExternalInputType, ref messages );
				//foreach ( var type in input.ExternalInputType )
				//                cp.ExternalInputType.Add( FormatCredentialAlignment( type ) );

				//short replacement method
				cp.ProcessMethod = AssignValidUrlAsString( input.ProcessMethod, "Process Method", ref messages );

				cp.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "Process Standards", ref messages );

				cp.ScoringMethodExample = AssignValidUrlAsString( input.ScoringMethodExample, "Scoring Method Example", ref messages );

				cp.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );
				//cp.Region = MapRegions( input.Region, ref messages );
				cp.ProcessingAgent = FormatOrganizationReferences( input.ProcessingAgent, "Processing Agent", false, ref messages, false );

				//targets
				cp.TargetCredential = FormatEntityReferences( input.TargetCredential, MJ.Credential.classType, false, ref messages );
				cp.TargetAssessment = FormatEntityReferences( input.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
				cp.TargetLearningOpportunity = FormatEntityReferences( input.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

                cp.TargetCompetencyFramework = FormatEntityReferences( input.TargetCompetencyFramework, "ceterms:targetCompetencyFramework", false, ref messages ); ;


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
			int cntr = 0;
			foreach ( MI.DurationProfile item in input )
			{
				cntr++;
				cp = new MJ.DurationProfile
				{
					Description = item.Description
				};
				if ( DurationHasValue( item.ExactDuration ) )
				{
					cp.ExactDuration = AsSchemaDuration( item.ExactDuration );
					if ( DurationHasValue( item.MinimumDuration ) || DurationHasValue( item.MaximumDuration ) )
					{
						messages.Add( string.Format( "Duration Profile error - For entry #{0} provide either an exact duration or a range ( a minimum and maximum range), but not both.", cntr ) );
						continue;
					}
				} else if ( DurationHasValue( item.MinimumDuration ) && DurationHasValue( item.MaximumDuration ) )
				{
					cp.MinimumDuration = AsSchemaDuration( item.MinimumDuration );
					cp.MaximumDuration = AsSchemaDuration( item.MaximumDuration );
				} else
				{
					//we can just have a description, but must have both sides of a range
					if ( DurationHasValue( item.MinimumDuration ) || DurationHasValue( item.MaximumDuration ) )
					{
						messages.Add( string.Format( "Duration Profile error - For entry #{0} provide either an exact duration or a range ( a minimum and maximum range), but not both.", cntr ) );
						continue;
					}
				}

				list.Add( cp );
			}

			if ( list.Count > 0 )
				return list;
			else
				return null;
		}
		public static string FormatDurationItem( MI.DurationItem input, string propertyName, ref List<string> messages )
		{
			if ( input == null )
				return null;

			//List<string> list = new List<string>();
			var cp = new MJ.DurationProfile();
			string duration = "";
			//int cntr = 0;
			//foreach ( MI.DurationItem item in input )
			//{	}
				//cntr++;
				//cp = new MJ.DurationProfile{};
				if ( DurationHasValue( input ) )
				{
					duration = AsSchemaDuration( input );
				}
				else
				{
				//ignore
					//messages.Add( string.Format( "Duration Item error - For {0}, please provide at least one value for the duration item.", propertyName ) );
					//continue;
				}
		
			return duration;
		}
		public static string AsSchemaDuration( MI.DurationItem entity )
		{
			string duration = "";
			//first check if a valid ISO8601 value has been provided.
			if ( !string.IsNullOrWhiteSpace( entity.Duration_ISO8601 ) )
			{
                //find a regex validator

                //if valid,return string
                return entity.Duration_ISO8601;

            }

			if ( entity.Years > 0 )
				duration += entity.Years.ToString() + "Y";
			if ( entity.Months > 0 )
				duration += entity.Months.ToString() + "M";
			if ( entity.Weeks > 0 )
				duration += entity.Weeks.ToString() + "W";
			if ( entity.Days > 0 )
				duration += entity.Days.ToString() + "D";
			if ( entity.Hours > 0 || entity.Minutes > 0 )
				duration += "T";
			if ( entity.Hours > 0 )
				duration += entity.Hours.ToString() + "H";
			if ( entity.Minutes > 0 )
				duration += entity.Minutes.ToString() + "M";

			if ( !string.IsNullOrEmpty( duration ) )
			{
				duration = "P" + duration;
				return duration;
			}
			else
				return null;
		}

		public static bool DurationHasValue( MI.DurationItem duration )
		{
			//if offering input of iso 8601 duration, need a check as well
			return ( duration.Years + duration.Months + duration.Weeks + duration.Days + duration.Hours + duration.Minutes ) > 0;
		}
		private static MJ.DurationItem FormatDurationItem( MI.DurationItem duration )
		{
			if ( duration == null )
				return null;
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

		public static List<MJ.FinancialAlignmentObject> MapFinancialAssitance( List<Models.Input.FinancialAlignmentObject> list, ref List<string> messages )
		{
			List<MJ.FinancialAlignmentObject> output = new List<MJ.FinancialAlignmentObject>();
			if ( list == null || list.Count == 0 )
				return null;
			MJ.FinancialAlignmentObject jp = new MJ.FinancialAlignmentObject();
			foreach ( var item in list )
			{
				var fa = new Models.Json.FinancialAlignmentObject
				{
					AlignmentType = item.AlignmentType,
					Framework = item.Framework,
					FrameworkName = item.FrameworkName,
					TargetNode = item.TargetNode,
					TargetNodeDescription = item.TargetNodeDescription,
					TargetNodeName = item.TargetNodeName,
					Weight = item.Weight
				};
				fa.AlignmentDate = MapDate( item.AlignmentDate, "AlignmentDate", ref messages );

				//if ( usingPendingSchemaVersion )
				fa.CodedNotation = item.CodedNotation;


				output.Add( fa );
			}
			return output;
		}

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

		/// <summary>
		/// Map specific jurisdiction roles for an organization
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="helper"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
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

			if ( FormatOrganizationReference( profile.AssertedBy, "Asserted By", ref helper, false, ref messages ) )
			{
				if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
				{
					//currently defined as object. Ultimately will be a list, but should be single
					List<OrganizationBase> bys = new List<OrganizationBase>();
					bys.Add( helper.OrgBaseList[ 0 ] );
					jp.AssertedBy = bys;
					//jp.AssertedBy = helper.OrgBaseList[ 0 ];
				}
			}

			//??are we assuming an error here?
			//if ( jp.AssertedBy == null )
			//             return null;

			return jp;
		}

		public static MJ.JurisdictionProfile MapToJurisdiction( Models.Input.Jurisdiction jp, ref List<string> messages )
		{
			var jpOut = new Models.Json.JurisdictionProfile();
			//make sure there is data here!!!
			jpOut.Description = jp.Description;
			//NEED to handle at least the main jurisdiction
			if ( jp.MainJurisdiction != null &&
				(!string.IsNullOrEmpty( jp.MainJurisdiction.Name) || !string.IsNullOrEmpty( jp.MainJurisdiction.GeoURI ) ))
			{
				jpOut.GlobalJurisdiction = null;
				var gc = new MOPlace();
				if ( MapGeoCoordinatesToPlace( jp.MainJurisdiction, ref gc, ref messages ) )
				{
					//jpOut.MainJurisdiction = ( gc );
					jpOut.MainJurisdiction = new List<Models.Json.Place>();
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
				MOPlace gc = new MOPlace();
				foreach ( var item in jp.JurisdictionException )
				{
					gc = new MOPlace();
					if ( MapGeoCoordinatesToPlace( item, ref gc, ref messages ) )
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

		//public static List<MJ.GeoCoordinates> MapRegions( List<Models.Input.GeoCoordinates> list, ref List<string> messages )
		//{
		//	List<MJ.GeoCoordinates> output = new List<MJ.GeoCoordinates>();
		//	if ( list == null || list.Count == 0 )
		//		return null;

		//	return output;
		//}

		public static bool MapGeoCoordinatesToPlace( MI.Place input, ref MOPlace entity, ref List<string> messages )
		{
			bool isValid = true;
			bool isUrlPresent = true;
			entity = new MOPlace();
			string statusMessage = "";

			if ( string.IsNullOrWhiteSpace( input.Name ) && string.IsNullOrWhiteSpace( input.GeoURI ) )
			{
				//although the name will usually equal the country or region
				messages.Add( "Error - a name must be provided with a jurisidiction GeoCoordinate. The name is typically the country or region within a country, but could also be a continent." );
				isValid = false;
			}
			else
				entity.Name = input.Name;

			//Address a = new Address();

			//we don't know for sure if the url is a useful geonames like one.
			if ( !string.IsNullOrWhiteSpace( input.GeoURI ) )
			{
				if ( IsUrlValid( input.GeoURI, ref statusMessage, ref isUrlPresent ) )
				{
					entity.GeoURI = input.GeoURI;
					//should any additional data be also provided?
					//entity.Country = input.Country;
					entity.Name = input.AddressRegion;
					entity.Latitude = input.Latitude;// != 0 ? input.Latitude.ToString() : "";
					entity.Longitude = input.Longitude;// != 0 ? input.Longitude.ToString() : "";

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
				{
					messages.Add( "Error - a valid geo coded URL, such as from geonames.com, must be provided." );
					isValid = false;
				}
			}

			if ( !isValid )
				entity = null;

			return isValid;
		}
		//public static bool FormatGeoCoordinates( MI.GeoCoordinates input, ref MJ.GeoCoordinates entity, ref List<string> messages )
		//{
		//	bool isValid = true;
		//	bool isUrlPresent = true;
		//	entity = new MJ.GeoCoordinates();
		//	string statusMessage = "";

		//	if ( string.IsNullOrWhiteSpace( input.Name ) )
		//	{
		//		//although the name will usually equal the country or region
		//		messages.Add( "Error - a name must be provided with a jurisidiction GeoCoordinate. The name is typically the country or region within a country, but could also be a continent." );
		//		isValid = false;
		//	}
		//	else
		//		entity.Name = input.Name;

		//	//Address a = new Address();

		//	//we don't know for sure if the url is a useful geonames like one.
		//	if ( !string.IsNullOrWhiteSpace( input.GeoUri ) )
		//	{
		//		if ( IsUrlValid( input.GeoUri, ref statusMessage, ref isUrlPresent ) )
		//		{
		//			entity.GeoURI = input.GeoUri;
		//			//should any additional data be also provided?
		//			//entity.Country = input.Country;
		//			entity.Name = input.Region;
		//			entity.Latitude = input.Latitude;// != 0 ? input.Latitude.ToString() : "";
		//			entity.Longitude = input.Longitude;// != 0 ? input.Longitude.ToString() : "";

		//			//if ( input.Address != null )
		//			//{
		//			//	//entity.Address.StreetAddress	<== actually published
		//			//	a = FormatAddress( input.Address, ref messages );
		//			//	if ( a != null )
		//			//		entity.Address.Add( a );
		//			//}

		//		}
		//		else
		//		{
		//			//must have some geo url
		//			messages.Add( "Error - a valid geo coded URL, such as from geonames.com, must be provided." );
		//			isValid = false;
		//		}

		//	}
		//	else
		//	{
		//		//where no url, can we accept just text?
		//		//==> won't be validated!
		//		//entity.Country = input.Country;
		//		//entity.Region = input.Region;

		//		//entity.Latitude = input.Latitude;
		//		//entity.Longitude = input.Longitude;
		//		//if ( input.Address != null )
		//		//{
		//		//	//entity.Address.StreetAddress	<== actually published
		//		//	a = FormatAddress( input.Address, ref messages );
		//		//	if ( a != null )
		//		//		entity.Address.Add( a );
		//		//}
		//		//else
		//		{
		//			messages.Add( "Error - a valid geo coded URL, such as from geonames.com, must be provided." );
		//			isValid = false;
		//		}

		//	}

		//	if ( !isValid )
		//		entity = null;

		//	return isValid;
		//}

		/// <summary>
		/// Format AvailableAt
		/// 17-10-20 - essentially an address now
		/// 17-11-02 - essentially a Place now
		/// </summary>
		/// <param name="input"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static List<MOPlace> FormatAvailableAtList( List<MIPlace> input, ref List<string> messages )
		{
			//Available At should require an address, not just contact points
			return FormatPlacesList( input, ref messages, true ); 
			//List<MOPlace> list = new List<MOPlace>();
			//MOPlace output = new MOPlace();
			//MJ.ContactPoint cpo = new MJ.ContactPoint();

			//foreach ( var inputItem in input )
			//{
			//	output = new MOPlace();

			//	output = FormatPlace( inputItem, ref messages );

			//	if ( inputItem.ContactPoint != null && inputItem.ContactPoint.Count > 0 )
			//	{
			//		foreach ( var cpi in inputItem.ContactPoint )
			//		{
			//			cpo = new MJ.ContactPoint()
			//			{
			//				Name = cpi.Name,
			//				ContactType = cpi.ContactType
			//			};
			//			cpo.ContactOption = cpi.ContactOption;
			//			cpo.PhoneNumbers = cpi.PhoneNumbers;
			//			cpo.Emails = cpi.Emails;
			//			cpo.SocialMediaPages = AssignValidUrlListAsStringList( cpi.SocialMediaPages, "Social Media", ref messages );

			//			output.ContactPoint.Add( cpo );
			//		}
			//	}

			//	list.Add( output );
			//}

			//return list;
		}

		public static List<MOPlace> FormatPlacesList( List<MIPlace> input, ref List<string> messages, bool addressExpected = false )
		{

			List<MOPlace> list = new List<MOPlace>();
			if ( input == null || input.Count == 0 )
				return null;
			MOPlace output = new MOPlace();	

			foreach ( var item in input )
			{
				output = new MOPlace();
                if ( FormatPlace(item, addressExpected, ref output, ref messages) )
                    list.Add(output);

			}

			return list;
		}

		/// <summary>
		/// Append a Place with only contact points to the address list
		/// </summary>
		/// <param name="input"></param>
		/// <param name="list"></param>
		/// <param name="messages"></param>
		/// <param name="addressExpected"></param>
		public static void AppendPlaceContactPoints( MIPlace input, List<MOPlace> list, ref List<string> messages, bool addressExpected = false )
		{

			if ( input == null || input.ContactPoint == null || input.ContactPoint.Count == 0 )
				return;
			if ( list == null )
				list = new List<MOPlace>();
            MOPlace output = new MOPlace();
            if (FormatPlace( input, addressExpected, ref output, ref messages ))
                list.Add( output );
			//}

			return;
		}
		public static bool FormatPlace( MIPlace input, bool isAddressExpected, ref MOPlace output, ref List<string> messages )
		{
            bool isValid = true;
            if ( input == null  )
                return false;

            //need to handle null or incomplete
            //MOPlace output = new MOPlace();
            MJ.ContactPoint cpo = new MJ.ContactPoint();
            //NOTE: can have a place with just contact point!
            //      - would at least a name be required?
			if ( !string.IsNullOrWhiteSpace( input.Name ) )
			{
				output.Name = input.Name;
			}
			output.Description = input.Description;

			if ( !string.IsNullOrWhiteSpace( input.Address1 ) )
			{
				output.StreetAddress = ( input.Address1 ?? "" );
				if ( !string.IsNullOrWhiteSpace( input.Address2 ) )
				{
					output.StreetAddress += ", " + input.Address2;
				}
			}
			if ( !string.IsNullOrWhiteSpace( input.PostOfficeBoxNumber ) )
			{
				output.PostOfficeBoxNumber = input.PostOfficeBoxNumber;
			}
			if ( !string.IsNullOrWhiteSpace( input.City ) )
			{
				output.City = input.City;
			}
			if ( !string.IsNullOrWhiteSpace( input.AddressRegion ) )
			{
				output.AddressRegion = input.AddressRegion;
			}
			if ( !string.IsNullOrWhiteSpace( input.PostalCode ) )
			{
				output.PostalCode = input.PostalCode;
			}
			if ( !string.IsNullOrWhiteSpace( input.Country ) )
			{
				output.Country = input.Country;
			}
			output.Latitude = input.Latitude;
			output.Longitude = input.Longitude;

			bool hasContactPoints = false;
			if ( input.ContactPoint != null && input.ContactPoint.Count > 0 )
			{
				foreach ( var cpi in input.ContactPoint )
				{
					cpo = new MJ.ContactPoint()
					{
						Name = cpi.Name,
						ContactType = cpi.ContactType
					};
					//cpo.ContactOption = cpi.ContactOption;
					cpo.PhoneNumbers = cpi.PhoneNumbers;
					cpo.Emails = cpi.Emails;
					cpo.SocialMediaPages = AssignValidUrlListAsStringList( cpi.SocialMediaPages, "Social Media", ref messages );

					output.ContactPoint.Add( cpo );
					hasContactPoints = true;
				}
			}
			else
				output.ContactPoint = null;

			bool hasAddress = false;
			if ( ( string.IsNullOrWhiteSpace( output.StreetAddress )
					&& string.IsNullOrWhiteSpace( output.PostOfficeBoxNumber ) )
					|| string.IsNullOrWhiteSpace( input.City )
					|| string.IsNullOrWhiteSpace( input.AddressRegion )
					|| string.IsNullOrWhiteSpace( input.PostalCode ) )
			{
				if ( isAddressExpected )
				{
					messages.Add( "Error - A valid address expected. Please provide a proper address, along with any contact points." );
					return false;
				}
			} else
				hasAddress = true;

			//check for at an address or contact point
			if ( !hasContactPoints && !hasAddress
			)
			{
				messages.Add( "Error - incomplete place/address. Please provide a proper address and/or one or more proper contact points." );
                return false;
            }

			return isValid;
		}
		//[Obsolete]
		//public static Models.Json.Address FormatAddress( PostalAddress address, ref List<string> messages )
		//{
		//	//need to handle null or incomplete
		//	Models.Json.Address output = new Address();

		//	if ( !string.IsNullOrWhiteSpace( address.Name ) ) {
		//		output.Name = address.Name;
		//	}
		//	if ( !string.IsNullOrWhiteSpace( address.Address1 ) )
		//	{
		//		output.StreetAddress = ( address.Address1 ?? "" );
		//		if ( !string.IsNullOrWhiteSpace( address.Address2 ) )
		//		{
		//			output.StreetAddress += ", " + address.Address2;
		//		}
		//	}
		//	if ( !string.IsNullOrWhiteSpace( address.PostOfficeBoxNumber ) )
		//	{
		//		output.PostOfficeBoxNumber = address.PostOfficeBoxNumber;
		//	}
		//	if ( !string.IsNullOrWhiteSpace( address.City ) ) {
		//		output.City = address.City;
		//	}
		//	if ( !string.IsNullOrWhiteSpace( address.AddressRegion ) ) {
		//		output.AddressRegion = address.AddressRegion;
		//	}
		//	if ( !string.IsNullOrWhiteSpace( address.PostalCode ) ) {
		//		output.PostalCode = address.PostalCode;
		//	}
		//	if ( !string.IsNullOrWhiteSpace( address.Country ) )
		//	{
		//		output.Country = address.Country;
		//	}
		//	//check for minimum address
		//	if ( ( string.IsNullOrWhiteSpace( output.StreetAddress )
		//		&& string.IsNullOrWhiteSpace( output.PostOfficeBoxNumber ) )
		//		|| string.IsNullOrWhiteSpace( address.City )
		//		|| string.IsNullOrWhiteSpace( address.AddressRegion )
		//		|| string.IsNullOrWhiteSpace( address.PostalCode )
		//	)
		//	{
		//		messages.Add( "Error - incomplete address. Please provide a proper address." );
		//		output = null;
		//	}

		//	return output;
		//}

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
		public static List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentListFromStrings( List<string> terms )
		{
			List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
			if ( terms == null || terms.Count == 0 )
				return null;
			foreach ( string item in terms )
			{
                if (!string.IsNullOrWhiteSpace( item ))
                    list.Add( FormatCredentialAlignment( item ) );
			}

			return list;
		}

		/// <summary>
		/// Only for simple properties like subjects, with no concept scheme
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MJ.CredentialAlignmentObject FormatCredentialAlignment( string name )
		{
            if (string.IsNullOrWhiteSpace( name ))
                return null;

			MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
			ca.TargetNodeName = name;
			return ca;
		}

		public static List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentVocabs( string ctdlProperty, List<string> terms, ref List<string> messages )
		{
			List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
			if ( terms == null || terms.Count == 0 || string.IsNullOrWhiteSpace( ctdlProperty ))
				return null;
			foreach ( string item in terms )
			{
                if (!string.IsNullOrWhiteSpace( item ))
                    list.Add( FormatCredentialAlignment( ctdlProperty, item, ref messages ) );
			}

			return list;
		}
		public static List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentVocabToList( string vocab, string term, ref List<string> messages )
		{
			List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
			if ( string.IsNullOrWhiteSpace( term ) )
				return null;

			list.Add( FormatCredentialAlignment( vocab, term, ref messages ) );

			return list;
		}
		/// <summary>
		/// Format Vocabulary properties/Concepts as credential alignment objects.
		/// </summary>
		/// <param name="vocabulary"></param>
		/// <param name="name"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static MJ.CredentialAlignmentObject FormatCredentialAlignment( string ctdlProperty, string term, ref List<string> messages )
		{
			MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
			CodeItem code = new CodeItem();

            //TODO add framework
            /*
			{
				@type: "ceterms:CredentialAlignmentObject",
				ceterms:framework: {
				@id: "http://www.credreg.net/organizationType"
				},
				ceterms:targetNode: {
				@id: "orgType:CertificationBody"
				},
				ceterms:frameworkName: "Organization Type",
				ceterms:targetNodeName: "Certification Body"
			},			 
			 * 
			 */
            var ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegTermsJson", "http://credreg.net/ctdl/terms/{0}/json" );
            if ( !string.IsNullOrWhiteSpace( term) && ValidationServices.IsTermValid( ctdlProperty, term, ref code ) )
			{
				//IdProperty item = new IdProperty() { Id = code.SchemaName };
				ca.TargetNode = code.SchemaName;
                ca.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
                ca.TargetNodeName = code.Name;
				ca.TargetNodeDescription = code.Description;
				if ( !string.IsNullOrWhiteSpace( code.ParentSchemaName ) )
					ca.FrameworkName = code.ParentSchemaName.Replace("ceterms:","");
			}
			else
			{
				messages.Add( string.Format( "The {0} type of {1} is invalid.", ctdlProperty, term ) );
				ca = null;
			}

			return ca;
		}


		public static List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentListFromFrameworkItemList( List<FrameworkItem> list, bool includingCodedNotation, ref List<string> messages, string frameworkName = "", string framework = "" )
		{
			if ( list == null || list.Count == 0 )
				return null;

			List<MJ.CredentialAlignmentObject> output = new List<Models.Json.CredentialAlignmentObject>();
			MJ.CredentialAlignmentObject entity = new Models.Json.CredentialAlignmentObject();

			//need to add a framework
			foreach ( FrameworkItem item in list )
			{
				entity = new Models.Json.CredentialAlignmentObject();
				entity = FormatCredentialAlignmentFrameworkItem( item, true, ref messages, frameworkName, framework );
				if ( entity != null )
					output.Add( entity );
			}

			return output;
		}   //

		public static MJ.CredentialAlignmentObject FormatCredentialAlignmentFrameworkItem( FrameworkItem entity, bool includingCodedNotation, ref List<string> messages, string frameworkName = "", string framework = "" )
		{
			bool hasData = false;
            bool isUrlPresent = true;
            string statusMessage = "";

            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();

            

			if ( !string.IsNullOrWhiteSpace( entity.FrameworkName ) )
			{
				ca.FrameworkName = entity.FrameworkName;
				//hasData = true;
			}
			else if ( !string.IsNullOrWhiteSpace( frameworkName ) )
			{
				ca.FrameworkName = frameworkName;
				//hasData = true;
			}
            //need a targetNode, normally - this is the schema name
            //==>< N/A for framework items like industries
            //actually should have. It can be a URI, so do we skip Url checks?
            if ( !string.IsNullOrWhiteSpace( entity.URL ) )
            {
                if ( IsUrlValid( entity.URL, ref statusMessage, ref isUrlPresent, false ) )
                {
                    ca.TargetNode = entity.URL;
                    //demand more data
                    //hasData = true;
                }
            }

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

			if ( !string.IsNullOrWhiteSpace( entity.CodedNotation ) )
			{
				if ( includingCodedNotation )
				{
					ca.CodedNotation = entity.CodedNotation;
					//hasData = true;
				}
			}

            //Framework must be a valid Url
            if ( !string.IsNullOrWhiteSpace( entity.Framework ) )
            {
                if ( IsUrlValid( entity.Framework, ref statusMessage, ref isUrlPresent, false ) )
                    ca.Framework = entity.Framework;
                else
                {
                    messages.Add( string.Format( "The Framework in a Credential Alignment Object must be a valid URI. Framework: {0}, targetNodeName: {1}, Message: {2}.", entity.Framework, ca.TargetNodeName, statusMessage ) );
                }
            }
            else if ( !string.IsNullOrWhiteSpace( framework ) )
            {
                if ( IsUrlValid( entity.Framework, ref statusMessage, ref isUrlPresent ) )
                    ca.Framework = framework;
                else
                {
                    messages.Add( string.Format( "The Framework in a Credential Alignment Object must be a valid URI. Framework: {0}, targetNodeName: {1}, Message: {2}.", framework, ca.TargetNodeName, statusMessage ) );
                }
            }

            if ( !hasData )
				ca = null;
			return ca;
		}
		public static List<MJ.CredentialAlignmentObject> FormatCompetencies( List<MI.CredentialAlignmentObject> entities, ref List<string> messages )
		{
			List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
			MJ.CredentialAlignmentObject cao = new MJ.CredentialAlignmentObject();

			if ( entities == null || entities.Count == 0 )
				return null;
			foreach ( var item in entities )
			{
				cao = FormatCompetency( item, ref messages );
				if ( cao != null )
					list.Add( cao );
			}

			return list;
		}//

		public static MJ.CredentialAlignmentObject FormatCompetency( MI.CredentialAlignmentObject entity, ref List<string> messages )
		{
			bool hasData = false;
			if ( entity == null )
				return null;

			MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
			//if present, the framework must be a valid url
			if ( !string.IsNullOrWhiteSpace( entity.Framework ) )
			{
				ca.Framework = AssignValidUrlAsString( entity.Framework, "Competency Framework", ref messages, false );
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
				ca.CodedNotation = entity.CodedNotation;

			}
			if ( !string.IsNullOrWhiteSpace( entity.TargetNode ) )
			{
				ca.TargetNode = AssignValidUrlAsString( entity.TargetNode, "Competency", ref messages, false );
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
				//if no name, use description
				if ( string.IsNullOrWhiteSpace( entity.TargetNodeName ) )
					ca.TargetNodeName = entity.TargetNodeDescription;
				else
					ca.TargetNodeDescription = entity.TargetNodeDescription;

				hasData = true;
			}

			ca.Weight = entity.Weight;
			//if ( !string.IsNullOrWhiteSpace( alignmentType ) )
			//{
			//    ca.AlignmentType = alignmentType;
			//    //alignmentType is not enough for valid data
			//    //hasData = true;
			//}
			if ( !hasData )
				ca = null;
			return ca;
		}
		#endregion

		#region ID property helpers
		public static List<string> AssignEntityReferenceListAsStringList( List<EntityReference> list, ref List<string> messages )
		{
			List<string> urlList = new List<string>();
			if ( list == null || list.Count == 0 )
				return null;
			int cntr = 0;
			foreach ( EntityReference item in list )
			{
				cntr++;
				if ( !string.IsNullOrWhiteSpace( item.Id ) )
				{
					//will not check if resolvable
					urlList.Add( item.Id );
				}
			}
			if ( cntr == 0 )
				return null;
			else
				return urlList;
		}
		public static string AssignValidUrlAsString( string url, string propertyName, ref List<string> messages, bool isRequired = false )
		{
			string statusMessage = "";
			bool isUrlPresent = true;
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

			return url;
		} //
		public static List<string> AssignValidUrlAsStringList( string url, string propertyName, ref List<string> messages, bool isRequired = false )
		{
			string status = "";
			bool isUrlPresent = true;
			List<string> urlList = new List<string>();
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
					urlList.Add( url );
				}
			}

			return urlList;
		}

		/// <summary>
		/// Handle list of strings that should be valid URIs. 
		/// Note, where the list is for registry URIs that may not have been published yet, use the method: AssignRegistryURIsListAsStringList
		/// </summary>
		/// <param name="list"></param>
		/// <param name="title"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static List<string> AssignValidUrlListAsStringList( List<string> list, string title, ref List<string> messages )
		{
			string status = "";
			bool isUrlPresent = true;
			List<string> urlList = new List<string>();
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
							urlList.Add( url );
						}
					}
				}
			}
			if ( cntr == 0 )
				return null;
			else
				return urlList;
		}

		/// <summary>
		/// This method is for a list of strings that contain registry URIs. These are properly formed URIs but may not yet exist in the registry, so no existance check will be done.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="title"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static List<string> AssignRegistryURIsListAsStringList( List<string> list, string title, ref List<string> messages )
		{
			string status = "";
			bool isUrlPresent = true;
			List<string> urlList = new List<string>();
			if ( list == null || list.Count == 0 )
				return null;
			int cntr = 0;
			foreach ( string url in list )
			{
				cntr++;
				if ( !string.IsNullOrWhiteSpace( url ) )
				{
					//check if valid format, but skip existance check
					if ( !IsUrlValid( url, ref status, ref isUrlPresent, false ) )
						messages.Add( string.Format( "The URL #{0}: {1} for list: {2} is invalid. ", cntr, url, title ) + status );
					else
					{
						if ( isUrlPresent )
						{
							urlList.Add( url );
						}
					}
				}
			}
			if ( cntr == 0 )
				return null;
			else
				return urlList;
		}
		/// <summary>
		/// Validate a URL, and if valid assign to an IdProperty
		/// </summary>
		/// <param name="url">Url to validate</param>
		/// <param name="propertyName">Literal for property name - for messages</param>
		/// <param name="messages"></param>
		/// <param name="isRequired">If true, produce a message if url is missing</param>
		/// <returns>null or an IdProperty</returns>
		public static IdProperty AssignValidUrlAsIdProperty( string url, string propertyName, ref List<string> messages, bool isRequired = false )
		{
			string statusMessage = "";
			bool isUrlPresent = true;
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
			bool isUrlPresent = true;
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
			bool isUrlPresent = true;
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
		public static List<MJ.IdentifierValue> AssignIdentifierValueToList( string value )
		{
			if ( string.IsNullOrWhiteSpace( value ) )
				return null;

			List<MJ.IdentifierValue> list = new List<MJ.IdentifierValue>();
			list.Add( new MJ.IdentifierValue()
			{
				IdentifierValueCode = value
			} );

			return list;
		}
		public static List<MJ.IdentifierValue> AssignIdentifierListToList( List<MI.IdentifierValue> input )
		{
			if ( input == null || input.Count == 0 )
				return null;

			List<MJ.IdentifierValue> list = new List<MJ.IdentifierValue>();
			foreach ( var item in input )
			{
				list.Add( new MJ.IdentifierValue()
				{
					IdentifierValueCode = item.IdentifierValueCode,
					IdentifierType  = item.IdentifierType,
					Name = item.Name,
					Description = item.Description
				} );
			}
			return list;
		}
		public static List<string> AssignStringToList( string value )
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                return null;

            List<string> list = new List<string>();
            list.Add( value );

            return list;
        }
		public static string AssignListToString( List<string> list )
		{
			if ( list == null || list.Count == 0 )
				return null;
			string value = list[ 0 ];

			return value;
		}


        #region Language Map helpers

        public static void AssignLanguageMap( string input, ref LanguageMap output, string title, bool isRequired, ref List<string> messages, string langauge = "en" )
        {

            output = new LanguageMap();
            if ( string.IsNullOrWhiteSpace( input ) )
            {
                if ( isRequired )
                    messages.Add( string.Format( "The property {0} is required, please provide a value.", title ) );
                return;
            }
            output.Add( langauge, input );
      
        }
        public static LanguageMap FormatLanguageMap( MI.LanguageMap input, string title, ref List<string> messages )
        {

            LanguageMap output = new LanguageMap();
            if ( input == null )
                return null;
            int cntr = 0;
            foreach ( var item in input )
            {
                cntr++;
                //some validation
                if ( ValidateLanguageCode( item.Key, title, cntr, ref messages ) )
                    output.Add( item.Key.ToLower(), item.Value );
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

      
        public static void FormatLanguageMapList( List<string> list, ref LanguageMapList output, string title, ref List<string> messages, string langauge = "en" )
        {
            output = new LanguageMapList();
            if ( list == null || list.Count == 0 )
                return;

            output.Add( langauge, list );
            
        }
        /// <summary>
        /// Format list of language maps, like keywords
        /// "ceterms:keyword": {
		///     "en": [ "Keyword one", "Keyword two" ],
		///     "ru": [ "Ключевое слово", "Ключевое слово два" ]
        ///     }
        /// </summary>
        /// <param name="list"></param>
        /// <param name="title"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static LanguageMapList FormatLanguageMapList( MI.LanguageMapList list, string title, ref List<string> messages )
        {

            if ( list == null || list.Count == 0 )
                return null;

            LanguageMapList output = new LanguageMapList();
            int cntr = 0;
            foreach ( var item in list )
            {
                cntr++;
                //some validation of lang code and region
                if ( ValidateLanguageCode( item.Key, title, cntr, ref messages ) )
                    output.Add( item.Key.ToLower(), item.Value );
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

        private static bool ValidateLanguageCode(string languageCode, string title, int row, ref List<string> messages )
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                messages.Add( string.Format("A valid language code was not found for property: {0}, row: {1}", title, row ));
                return false;
            }
            int pos = languageCode.IndexOf( "-" );
            if ( languageCode.Length < 2 || (pos > -1 && pos < 2 ))
            {
                messages.Add( string.Format( "A valid language code must be at least two characters in length (eg. 'en' or 'en-us' Property: {0}, Row: {1}, Language Code: {2}", title, row, languageCode ) );
                return false;
            }

            //quick
            string code = languageCode.ToLower().Substring( 0, 2 );
            if ( "en de es fr ar cs da el fi he it ja nl no pl pt sv uk zh".IndexOf( code ) > -1  )
                return true;
            if ( pos > 2 )
            {
                code = languageCode.ToLower().Substring( 0, 3 );
                if ( "eng deu spa ara ces dan ell fin fra heb ita jnp nld nor pol por swe ukr zho".IndexOf( code ) > -1 )
                    return true;
            }
            //at this point don't have a common language code.
            messages.Add( string.Format( "An unknown/unhandled language code was encountered: Property: {0}, Row: {1}, Language Code: {2}", title, row, languageCode ) );
            isValid = false;

            return isValid;
        }

        #endregion


        #region Helpers and validaton

        public static bool IsUrlValid( string url, ref string statusMessage, ref bool urlPresent, bool doingExistanceCheck = true )
        {
            statusMessage = "";
			urlPresent = true;
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

            if ( !doingExistanceCheck )
				return true;

            var isOk = DoesRemoteFileExists( url, ref statusMessage );
            //optionally try other methods, or again with GET
            if ( !isOk && statusMessage == "999" )
                return true;
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
			//although will greatly slow down batch publishing
            if ( UtilityManager.GetAppKeyValue( "ra.SkippingLinkChecking", false ) )
                return true;

            bool treatingRemoteFileNotExistingAsError = UtilityManager.GetAppKeyValue( "treatingRemoteFileNotExistingAsError", true );
            //consider stripping off https?
            //or if not found and https, try http
            try
            {
                if ( SkippingValidation( url ) )
                    return true;
                SafeBrowsing.Reputation rep = SafeBrowsing.CheckUrl( url );
                if ( rep != SafeBrowsing.Reputation.None )
                {
                    responseStatus = string.Format( "Url ({0}) failed SafeBrowsing check.", url );
                    return false;
                }
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create( url ) as HttpWebRequest;
                //NOTE - do NOT use the HEAD option, as many sites reject that type of request
				//		GET seems to be cause false 404s
                //request.Method = "GET";
                //var agent = HttpContext.Current.Request.AcceptTypes;

                //the following also results in false 404s
                //request.ContentType = "text/html;charset=\"utf-8\";image/*";
                //UserAgent appears OK
                request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_2) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1309.0 Safari/537.17";

				//users may be providing urls to sites that have invalid ssl certs installed.You can ignore those cert problems if you put this line in before you make the actual web request:
				ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback( AcceptAllCertifications );

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
		public static bool AcceptAllCertifications( object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors )
		{
			return true;
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
                    messages.Add( string.Format( "Error - {0} is out of range (prior to 1800-01-01) ", dateName ) );
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

		#region JSON helpers
        public static string LogInputFile( CredentialRequest request, string endpoint, int appLevel = 6 )
        {
            string jsoninput = JsonConvert.SerializeObject( request, ServiceHelper.GetJsonSettings() );
            LoggingHelper.WriteLogFile( appLevel, string.Format("Credential_{0}_{1}_raInput.json", endpoint, request.Credential.Ctid), jsoninput, "", false );
            return jsoninput;
        }
        public static string LogInputFile( object request, string ctid, string entityType, string endpoint, int appLevel = 6 )
        {
            string jsoninput = JsonConvert.SerializeObject( request, ServiceHelper.GetJsonSettings() );
            LoggingHelper.WriteLogFile( appLevel, string.Format( "{0}_{1}_{2}_raInput.json", entityType, endpoint, ctid ), jsoninput, "", false );
            return jsoninput;
        }

        public static JsonSerializerSettings GetJsonSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new EmptyNullResolver(),
				Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
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

		/// <summary>
		/// NOTE: previously inherited from AlphaNumericContractResolver. the latter would sort by property name, which we don't want - must be @context, @id, and @graph
		/// </summary>
		public class EmptyNullResolver : DefaultContractResolver
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

        /// <summary>
        /// The actual validation will be via a call to the accounts api
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public static bool ValidateRequest(RequestHelper helper, ref string statusMessage, bool isDeleteRequest = false)
		{
			bool isValid = true;
            string clientIdentifier = "";
            bool isTokenRequired = UtilityManager.GetAppKeyValue( "requiringHeaderToken", true );
            if ( isDeleteRequest )
                isTokenRequired = true;

            //api key will be passed in the header
            string apiToken = "";
            if (IsAuthTokenValid( isTokenRequired, ref apiToken, ref clientIdentifier, ref statusMessage) == false)
            {
                return false;
            }
            helper.ApiKey = apiToken;
            helper.ClientIdentifier = clientIdentifier ?? "";

            if ( isTokenRequired &&
                (string.IsNullOrWhiteSpace(helper.OwnerCtid) || 
                 !helper.OwnerCtid.ToLower().StartsWith("ce-")  ||
                 helper.OwnerCtid.Length != 39) 
                )
            {
                if (clientIdentifier.ToLower().StartsWith( "cerpublisher" ) == false)
                {
                    statusMessage = "Error - a valid CTID for the related organization must be provided.";
                    return false;
                }
            }
            return isValid;
		}

		public static bool IsAuthTokenValid( bool isTokenRequired, ref string token, ref string clientIdentifier, ref string message )
		{
			bool isValid = true;
            //need to handle both ways. So if a token, and ctid are provided, then use them!
            //bool isTokenRequired = UtilityManager.GetAppKeyValue( "requiringHeaderToken", true );

            try
            {
                HttpContext httpContext = HttpContext.Current;
                clientIdentifier = httpContext.Request.Headers[ "Proxy-Authorization" ];
                string authHeader = httpContext.Request.Headers[ "Authorization" ];
                //registry API uses ApiToken rather than Basic
                if ( !string.IsNullOrWhiteSpace(authHeader)  )
                {
                    LoggingHelper.DoTrace( 4, "$$$$$$$$ Found an authorization header." + authHeader );
                    if (authHeader.ToLower().StartsWith( "apitoken" ))
                    {
                        //Extract credentials
                        authHeader = authHeader.ToLower();
                        token = authHeader.Substring( "apitoken ".Length ).Trim();
                    }
                }
            } catch (Exception ex)
            {
                if ( isTokenRequired )
                {
                    LoggingHelper.LogError( ex, "Exception encountered attempting to get API key from request header. " );
                    isValid = false;
                }
            }

            if ( isTokenRequired && string.IsNullOrWhiteSpace(token ) )
            {
                if (!string.IsNullOrWhiteSpace(clientIdentifier))
                {
                    if (clientIdentifier.ToLower().StartsWith( "cerpublisher" ))
                        return true;
                }
                message = "Error a valid API key must be provided in the header";
                isValid = false;
            }

            return isValid;
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
            if (string.IsNullOrWhiteSpace(keyName))
            {
                LogError( string.Format( "@@@@ Error: Empty string AppKey was encoutered, using default of: {0}", defaultValue ) );
                return defaultValue;
            }
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
            if ( string.IsNullOrWhiteSpace( keyName ) )
            {
                LogError( string.Format( "@@@@ Error: Empty int AppKey was encoutered, using default of: {0}", defaultValue ) );
                return defaultValue;
            }
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
            if ( string.IsNullOrWhiteSpace( keyName ) )
            {
                LogError( string.Format( "@@@@ Error: Empty bool AppKey was encoutered, using default of: {0}",  defaultValue ) );
                return defaultValue;
            }
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

		public static void NotifyOnPublish( string type, string message )
		{
			string subject = string.Format("Registry Assistant - successfully published a {0}", type);
			//string message = "";
			//
			if (UtilityManager.GetAppKeyValue( "notifyOnPublish", false ) )
				NotifyAdmin( subject, message );
		}

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
        /// <summary>
        /// Convert characters often resulting from external programs like Word
        /// NOTE: keep in sync with the Publisher version
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertWordFluff( string text )
        {
            if ( string.IsNullOrWhiteSpace( text ) )
                return "";

            if ( text.IndexOf( '\u2013' ) > -1 ) text = text.Replace( '\u2013', '-' ); // en dash
            if ( text.IndexOf( '\u2014' ) > -1 ) text = text.Replace( '\u2014', '-' ); // em dash
            if ( text.IndexOf( '\u2015' ) > -1 ) text = text.Replace( '\u2015', '-' ); // horizontal bar
            if ( text.IndexOf( '\u2017' ) > -1 ) text = text.Replace( '\u2017', '_' ); // double low line
            if ( text.IndexOf( '\u2018' ) > -1 ) text = text.Replace( '\u2018', '\'' ); // left single quotation mark
            if ( text.IndexOf( '\u2019' ) > -1 ) text = text.Replace( '\u2019', '\'' ); // right single quotation mark
            if ( text.IndexOf( '\u201a' ) > -1 ) text = text.Replace( '\u201a', ',' ); // single low-9 quotation mark
            if ( text.IndexOf( '\u201b' ) > -1 ) text = text.Replace( '\u201b', '\'' ); // single high-reversed-9 quotation mark
            if ( text.IndexOf( '\u201c' ) > -1 ) text = text.Replace( '\u201c', '\"' ); // left double quotation mark
            if ( text.IndexOf( '\u201d' ) > -1 ) text = text.Replace( '\u201d', '\"' ); // right double quotation mark
            if ( text.IndexOf( '\u201e' ) > -1 ) text = text.Replace( '\u201e', '\"' ); // double low-9 quotation mark
            if ( text.IndexOf( '\u2026' ) > -1 ) text = text.Replace( "\u2026", "..." ); // horizontal ellipsis
            if ( text.IndexOf( '\u2032' ) > -1 ) text = text.Replace( '\u2032', '\'' ); // prime
            if ( text.IndexOf( '\u2033' ) > -1 ) text = text.Replace( '\u2033', '\"' ); // double prime
            if ( text.IndexOf('\u2036') > -1 )
                text = text.Replace('\u2036', '\"'); // ??
            if ( text.IndexOf('\u0090') > -1 )
                text = text.Replace('\u0090', 'ê'); // e circumflex

            text = Regex.Replace(text, "[Õ]", "'");
            text = Regex.Replace(text, "[Ô]", "'");
            text = Regex.Replace(text, "[Ò]", "\"");
            text = Regex.Replace(text, "[Ó]", "\"");
            text = Regex.Replace(text, "[—]", "-"); //
            return text.Trim();
        } //
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
        public static String CleanText( string text )
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

		public static String FormatMessage( string message, string parm1 )
		{
			string msg = CurrentEntityType; //+ " " + 
			if ( !string.IsNullOrWhiteSpace( CurrentEntityName ) )
				msg += " - " + CurrentEntityName;

			if ( message.IndexOf( "{0}" ) > -1 )
				return string.Format( message, parm1 );
			else
				return message;
		}
		public static string FormatMessage( string message, string parm1, string parm2 )
		{
			return string.Format( message, parm1, parm2 );
		}
		public static string GetCurrentIP()
		{
			string remoteIP = "";
			try
			{
				remoteIP = HttpContext.Current.Request.ServerVariables[ "REMOTE_HOST" ];
			}
			catch { }
			return remoteIP;
		}
		
		#endregion
	}

}
