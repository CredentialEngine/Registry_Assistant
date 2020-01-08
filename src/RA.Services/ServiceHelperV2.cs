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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utilities;

using RA.Models;
using MI = RA.Models.Input;
using MJ = RA.Models.JsonV2;
using MIPlace = RA.Models.Input.Place;
using MOPlace = RA.Models.JsonV2.Place;

using RA.Models.JsonV2;
using RA.Models.Input;


namespace RA.Services
{
    public class ServiceHelperV2
	{
        #region Properties
        private static string thisClassName = "RA.Services.ServiceHelperV2";
		public static string ceasnContext = "https://credreg.net/ctdlasn/schema/context/json";
		public static string ctdlContext = "https://credreg.net/ctdl/schema/context/json";
		public static string environment = UtilityManager.GetAppKeyValue( "environment", "unknown" );
		public static int GraphTypeUrl = 1;
		public static int ResourceTypeUrl = 2;
		//public string credRegistryResourceUrl = GetAppKeyValue( "credRegistryResourceUrl" );
		//public string credRegistryGraphUrl = GetAppKeyValue( "credRegistryGraphUrl" );
		public string credentialRegistryBaseUrl = GetAppKeyValue( "credentialRegistryBaseUrl" );
		public string credentialFinderDetailUrl = GetAppKeyValue( "credentialFinderDetailUrl" );
		public bool isFrameworkDateCreatedRequired = GetAppKeyValue( "frameworkDateCreatedIsRequired", true );
		public bool isFrameworkCreatorRequired = GetAppKeyValue( "frameworkCreatorIsRequired", true );
		public bool isFrameworkPublisherRequired = GetAppKeyValue( "frameworkPublisherIsRequired", true );
		public bool usingQuantitiveValue = GetAppKeyValue( "usingQuantitiveValue", true );

		public readonly string SystemDefaultLanguageForMaps = "en-US";
        public static string DefaultLanguageForMaps  = "en-US";
        public static string LanguageCodes_2Characters = "en de es fr ar cs da el fi he it ja nl no pl pt sv uk zh";
        public static string LanguageCodes_3Characters = "eng deu spa fra ara ces dan ell fin heb ita jnp nld nor pol por swe ukr zho";
        public bool IsAPublishRequest = true;
        public bool ConvertingFromResourceLinkToGraphLink = true;
        public List<MJ.BlankNode> BlankNodes = new List<MJ.BlankNode>();
		//set to true for scenarios like a deprecated credential, where need allow publishing even if urls are invalid!
		public bool WarnOnInvalidUrls = false;
		public string codeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
        public bool usingSingleDirectCost = UtilityManager.GetAppKeyValue( "usingSingleDirectCost", false );
		public static int MinimumDescriptionLength = UtilityManager.GetAppKeyValue( "minDescriptionTextLength", 25 );
		public static int NavyMinimumDescriptionLength = UtilityManager.GetAppKeyValue( "navyMinDescriptionTextLength", 25 );


		public List<string> warningMessages = new List<string>();

		//this can only be true for a format request
		public bool includingMinDataWithReferenceId = UtilityManager.GetAppKeyValue( "includeMinDataWithReferenceId", false );
		public string CurrentEntityType = "";
		public string CurrentEntityName = "";
		public string Community = "";
		public string CurrentCtid = "";
		public string LastProfileType = "";

		//
		/// <summary>
		/// Session variable for message to display in the system console
		/// </summary>
		public const string SYSTEM_CONSOLE_MESSAGE = "SystemConsoleMessage";

        static bool requiringQAOrgForQAQRoles = GetAppKeyValue("requireQAOrgForQAQRoles", false);
        public bool GeneratingCtidIfNotFound()
		{
			bool generatingCtidIfNotFound = GetAppKeyValue( "generateCtidIfNotFound", true );

			return generatingCtidIfNotFound;
		}
        #endregion
		public ServiceHelperV2 ()
		{
			DefaultLanguageForMaps = SystemDefaultLanguageForMaps;
		}
		#region Code validation
		/// <summary>
		/// Validate CTID
		/// TODO - should we generate if not found
		/// </summary>
		/// <param name="ctid"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool IsCtidValid( string ctid, string property, ref List<string> messages, bool isRequired = true )
		{
			bool isValid = true;

			if ( string.IsNullOrWhiteSpace( ctid ) )
			{
				if ( isRequired )
					messages.Add( "Error - A CTID must be entered for " + property );
				return false;
			}
			//just in case, handle old formats
			//ctid = ctid.Replace( "urn:ctid:", "ce-" );
			if ( ctid.Length != 39 )
			{
				messages.Add( string.Format("Error - Invalid CTID format for {0}. The proper format is ce-UUID. ex. ce-84365aea-57a5-4b5a-8c1c-eae95d7a8c9b (with all lowercase letters)", property ));
				return false;
			}

			if ( !ctid.StartsWith( "ce-" ) )
			{
				//actually we could add this if missing - but maybe should NOT
				messages.Add( "Error - The CTID property must begin with ce-" );
				return false;
			}
			//now we have the proper length and format, the remainder must be a valid guid
			if ( !IsValidGuid( ctid.Substring( 3, 36 ) ) )
			{
				//actually we could add this if missing - but maybe should NOT
				messages.Add( string.Format( "Error - Invalid CTID format for {0}. The proper format is ce-UUID. ex. ce-84365aea-57a5-4b5a-8c1c-eae95d7a8c9b (with all lowercase letters)", property ) );
				return false;
			}

			return isValid;
		}
        #endregion

		public void CheckIfChanged( RequestHelper helper, bool wasEnvelopeUpdated )
		{
			helper.WasChanged = wasEnvelopeUpdated;
			if ( !helper.WasChanged )
				helper.AddWarning( "NOTE: The credential registry envelope was NOT updated as the input data was the same as the data on the current envelope." );
		}

        public string FormatCtid(string ctid, string property, ref List<string> messages )
        {
            //todo determine if will generate where not found
            if ( string.IsNullOrWhiteSpace(ctid) && GeneratingCtidIfNotFound() )
                ctid = GenerateCtid();

            if ( IsCtidValid(ctid, property, ref messages) )
            {
                //can't do this yet, as the registry may treat an existing records as new!!!
                //if ( UtilityManager.GetAppKeyValue( "forceCtidToLowerCase", false ) )
                //{
                    //if ( ctid != ctid.ToLower() )
                    //{
                    //    //perhaps log for delete handling
                    //    LoggingHelper.DoTrace( 2, string.Format("WARNING - ENCOUNTERED CTID THAT WAS UPPER CASE: {0}, setting to lowercase", ctid) );
                    //}
                    ctid = ctid.ToLower();
                //}
				//don't want to arbitrarily to this for a child class, should only be for the parent!!!!
                //CurrentCtid = ctid;
            }

            return ctid;
        }
        public string GenerateCtid()
		{
			string ctid = "ce-" + Guid.NewGuid().ToString().ToLower();

			return ctid;
		}

		public string AddBlankNode(string type, string name, string description, string subjectWebpage)
        {
            string bnodeId = "";
            if ( DoesBNodeExist( name, description, subjectWebpage, ref bnodeId ) )
            {
                return bnodeId;
            }
			MJ.BlankNode bn = new MJ.BlankNode() { BNodeId = GenerateBNodeId(), Type = type, SubjectWebpage = subjectWebpage ?? "" };
            bn.Name = Assign( name, DefaultLanguageForMaps );
            bn.Description = Assign( description, DefaultLanguageForMaps );
            BlankNodes.Add( bn );
            return bn.BNodeId;
        }
        public string AddBlankNode( EntityBase entityBase )
        {
            string bnodeId = "";
            if ( DoesBNodeExist( entityBase.Name, entityBase.Description, entityBase.SubjectWebpage, ref bnodeId ) )
            {
                return bnodeId;
            }
			MJ.BlankNode bn = new MJ.BlankNode() { BNodeId = GenerateBNodeId(), Type = entityBase.Type, SubjectWebpage = entityBase.SubjectWebpage ?? "" };
            bn.Name = Assign( entityBase.Name, DefaultLanguageForMaps );
            bn.Description = Assign( entityBase.Description, DefaultLanguageForMaps );
            BlankNodes.Add( bn );
            return bn.BNodeId;
        }

        public string AddBlankNode( OrganizationBase entityBase )
        {
            string bnodeId = "";
            if ( DoesBNodeExist( entityBase.Name, entityBase.Description, entityBase.SubjectWebpage, ref bnodeId ) )
            {
                return bnodeId;
            }
			MJ.BlankNode bn = new MJ.BlankNode() { BNodeId = GenerateBNodeId(), Type = entityBase.Type, SubjectWebpage = entityBase.SubjectWebpage ?? "" };
            bn.Name = Assign( entityBase.Name, DefaultLanguageForMaps );
            bn.Description = Assign( entityBase.Description, DefaultLanguageForMaps );
            BlankNodes.Add( bn );
            return bn.BNodeId;
        }
        public string GenerateBNodeId()
        {
            string bid = "_:" + Guid.NewGuid().ToString().ToLower();

            return bid;
        }
        public bool DoesBNodeExist( string name, string description, string subjectWebpage, ref string bnodeId)
        {
            bool found = false;
            description = description ?? "";
            subjectWebpage = subjectWebpage ?? "";
            var exists = BlankNodes.FirstOrDefault( a => a.Name.ToString( DefaultLanguageForMaps ).ToLower() == name.ToLower() 
                    && a.SubjectWebpage.ToLower() == subjectWebpage.ToLower()
                    );
            if ( exists != null && (exists.Name.ToString( DefaultLanguageForMaps ) ?? "").Length > 0)
            {
                bnodeId = exists.BNodeId;
                return true;
            }
            return found;
        }

        //public void HandleRequestDefaults( MI.BaseRequest request )
        //{
        //    if ( request.NotConvertingFromResourceLinkToGraphLink == null )
        //        request.NotConvertingFromResourceLinkToGraphLink = false;
        //}
        #region Entity References 
        public List<string> FormatEntityReferencesList( List<EntityReference> input,
               string classSchema,
               bool dataIsRequired, //may not be necessary
               ref List<string> messages )
        {
            if ( input == null || input.Count == 0 )
                return null;

            List<string> output = new List<string>();
            string idUrl = "";
            EntityReferenceHelper helper = new EntityReferenceHelper();
            foreach ( var target in input )
            {
                if ( FormatEntityReference( target, classSchema, ref idUrl, dataIsRequired, ref messages ) )
                {
                    if ( !string.IsNullOrWhiteSpace( idUrl ) )
                    {
                        output.Add( idUrl );
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
        /// For graphs, always return single Id, create a bnode as needed
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="classSchema">can be blank, where used with generic props like offers, approves</param>
        /// <param name="helper"></param>
        /// <param name="dataIsRequired">True if the property is required</param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool FormatEntityReference( EntityReference entity,
                string classSchema,
                ref string idUrl,
                bool dataIsRequired,
                ref List<string> messages )
        {
            bool hasData = false;
            bool isValid = true;
            bool isUrlPresent = true;
            idUrl = "";
            EntityReferenceHelper helper = new EntityReferenceHelper();
            EntityBase entityBase = new EntityBase();
            //just in case
            //helper.EntityBaseList = new List<EntityBase>();
            //default
     

            if ( entity == null || entity.IsEmpty() )
            {
                if ( dataIsRequired )
                    messages.Add( string.Format( "A valid entity reference is required for {0}", classSchema ) );
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
                    //TODO	- should convert from /resource/ to /graph/ as needed
					//		- should these stay as /resource/	???
                    if ( ConvertingFromResourceLinkToGraphLink )
                    {
                        entity.Id = entity.Id.Replace( "/resources/ce-", "/graph/ce-" );
                    }
                    idUrl = entity.Id.ToLower();

                    entityBase.NegateNonIdProperties();
                    
                    entityBase.CtdlId = idUrl;
                    return true;
                }
                else
                {
					//NOTE: for an entity reference require valid even if credential is deprecated
                    messages.Add( string.Format( "Invalid Id property for {0} ({1}). When the Id property is provided for an entity, it must have valid format for a URL", entity.Id, classSchema ) );
                    return false;
                }
            }
            else if ( !string.IsNullOrWhiteSpace( entity.CTID ) )
            {
                if ( !IsCtidValid( entity.CTID, classSchema ?? "entityReference", ref messages ) )
                    return false;
                entityBase.NegateNonIdProperties();
				//maybe this should be resource
                //idUrl = credRegistryGraphUrl + entity.CTID.ToLower();
				idUrl = SupportServices.FormatRegistryUrl( ResourceTypeUrl, entity.CTID.ToLower(), Community);
				return true;
            }

            // ============================================================
            //helper.ReturnedDataType = 2;
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
                messages.Add( string.Format( "Invalid Entity Type of {0} for Name: {1}, SubjectWebpage: {2}. ", entity.Type ?? "missing", entity.Name, entity.SubjectWebpage ) );
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
				//NOTE: for an entity reference require valid even if credential is deprecated
				messages.Add( string.Format( "The Subject Webpage '{0}' for '{1}' is invalid: {2}", entity.SubjectWebpage, entityBase.Name, statusMessage ) );
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
                idUrl = AddBlankNode( entityBase );
                //helper.EntityBaseList.Add( entityBase );
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
        public bool ValidateEntityRefType( string inputType,
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

        #region Organization references
		/// <summary>
		/// Handle where an organization reference could be a list of a single object
		/// </summary>
		/// <param name="orgReference"></param>
		/// <param name="propertyName"></param>
		/// <param name="dataIsRequired"></param>
		/// <param name="messages"></param>
		/// <param name="isQAOrgReference"></param>
		/// <returns></returns>
        public List<string> FormatOrganizationReferences( object orgReference,
                    string propertyName,
                    bool dataIsRequired,
                    ref List<string> messages,
                    bool isQAOrgReference = false )
        {
            if ( orgReference == null )
                return null;
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
			else if ( orgReference.GetType() == typeof( string ) )
			{
				//can be a CTID or URI
				var item = orgReference as string;
				var orgList = new List<string>();
				var result = AssignRegistryResourceURIAsString( item, propertyName, ref messages, false, false );

				return FormatOrganizationReferenceToList( input, propertyName, dataIsRequired, ref messages, isQAOrgReference );
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
        /// TBD - do blank nodes need to accomodate language maps???
        /// </summary>
        /// <param name="input"></param>
        /// <param name="propertyName"></param>
        /// <param name="dataIsRequired"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public List<string> FormatOrganizationReferences( List<OrganizationReference> input,
			   string propertyName,
			   bool dataIsRequired,
			   ref List<string> messages,
				bool isQAOrgReference = false )
		{
			if ( input == null || input.Count == 0 )
				return null;

			List<string> output = new List<string>();
            string idUrl = "";
			EntityReferenceHelper helper = new EntityReferenceHelper();
			foreach ( var target in input )
			{
				if ( FormatOrganizationReference( target, propertyName, ref idUrl, dataIsRequired, ref messages, isQAOrgReference ) )
				{
					if ( !string.IsNullOrWhiteSpace( idUrl ))
					{
						output.Add( idUrl );
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
		public List<string> FormatOrganizationReferenceToList( OrganizationReference input,
			   string propertyName,
			   bool dataIsRequired,
			   ref List<string> messages,
				bool isQAOrgReference = false )
		{
			List<string> output = new List<string>();
			EntityReferenceHelper helper = new EntityReferenceHelper();
            string idUrl = "";
            if ( FormatOrganizationReference( input, propertyName, ref idUrl, dataIsRequired, ref messages, isQAOrgReference ) )
			{
                if ( !string.IsNullOrWhiteSpace( idUrl ) )
                {
                    output.Add( idUrl );
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
        public bool FormatOrganizationReference( OrganizationReference entity,
				string propertyName,
				ref string idUrl,
				bool dataIsRequired,
				ref List<string> messages,
				bool isQAOrgReference = false )
		{
			bool hasData = false;
			bool isValid = true;
            idUrl = "";

            bool isUrlPresent = true;
            EntityReferenceHelper helper = new EntityReferenceHelper();
            OrganizationBase org = new OrganizationBase();
			//org.Context = null;
			//just in case
			//helper.OrgBaseList = new List<OrganizationBase>();
	

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
                    //TODO - should convert from /resource/ to /graph/ as needed
                    if ( ConvertingFromResourceLinkToGraphLink )
                    {
                        entity.Id = entity.Id.Replace( "/resources/ce-", "/graph/ce-" );
                    }
                    idUrl = entity.Id.ToLower();

                    org.NegateNonIdProperties();
                   
                    org.CtdlId = idUrl;
					//17-09-?? per Stuart don't include a type with an @id
					org.Type = null;

					//consider whether a warning should be returned if additional data was included - that is, it will be ignored.
					return true;
				}
				else
				{
					//NOTE: for an org reference require valid even if credential is deprecated
					messages.Add( string.Format( "Invalid Id property for {0} ({1}). When the Id property is provided for an organization, it must be a valid resolvable URL", propertyName, statusMessage ) );
					return false;
				}
			}
			else if ( !string.IsNullOrWhiteSpace( entity.CTID ) )
			{
				if ( !IsCtidValid( entity.CTID, propertyName, ref messages ) )
					return false;
				org.NegateNonIdProperties();
				//???????????????????
                //idUrl = credRegistryGraphUrl + entity.CTID.ToLower();
				//maybe this should be resource
				//idUrl = credRegistryGraphUrl + entity.CTID.ToLower();
				idUrl = SupportServices.FormatRegistryUrl( ResourceTypeUrl, entity.CTID.ToLower(), Community);
				return true;
			}

			// ============================================================

			//helper.ReturnedDataType = 2;
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
			{
				//NOTE: for an org reference require valid even if credential is deprecated
				messages.Add( string.Format( "The Subject Webpage of: '{0}', for property: '{1}' Organization: '{2}' is invalid: {3}", entity.SubjectWebpage, propertyName, org.Name, statusMessage ) );
			}
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
						messages.Add( string.Format( "The SocialMedia URL of: '{0}', for property: '{1}' Organization: '{2}' is invalid: {3}", url, propertyName, org.Name, statusMessage )  );
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
                idUrl = AddBlankNode( org );
                //helper.OrgBaseList.Add( org );
			}

			if ( messages.Count > msgcount )
				isValid = false;

			return isValid;
        }     
        
		public bool ValidateOrgType( string inputType,
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
		public List<MJ.CostProfile> FormatCosts( List<MI.CostProfile> costs, ref List<string> messages )
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

                hold.Name = AssignLanguageMap( ConvertSpecialCharacters( item.Name ), item.Name_Map,  "Cost Name", DefaultLanguageForMaps, ref messages );
                hold.Description = AssignLanguageMap( ConvertSpecialCharacters( item.Description ), item.Description_Map, "Cost Description", DefaultLanguageForMaps, ref messages, true );
                hold.CostDetails = AssignValidUrlAsString( item.CostDetails, "Cost Details", ref messages, true );

				//hold.EndDate = item.EndDate;
				//hold.StartDate = item.StartDate;
				hold.StartDate = MapDate( item.StartDate, "CostProfile StartDate", ref messages );
				hold.EndDate = MapDate( item.EndDate, "CostProfile EndDate", ref messages );


				hold.Condition = AssignLanguageMapList( item.Condition, item.Condition_Map, "Cost Conditions", ref messages );

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
					   if (!string.IsNullOrWhiteSpace( cpi.DirectCostType ) )
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
                        cp.PaymentPattern = AssignLanguageMap( ConvertSpecialCharacters( cpi.PaymentPattern ), cpi.PaymentPattern_Map, "PaymentPattern",  DefaultLanguageForMaps, ref messages );

                        output.Add( cp );
					}
				}
			}

			return output;
		}
		#endregion

		
		#region ConditionProfile
		public List<MJ.ConditionProfile> FormatConditionProfile( List<MI.ConditionProfile> profiles, ref List<string> messages )
		{
			if ( profiles == null || profiles.Count == 0 )
				return null;
			string status = "";
			bool isUrlPresent = true;
			EntityReferenceHelper helper = new EntityReferenceHelper();

			var list = new List<MJ.ConditionProfile>();

			foreach ( var input in profiles )
			{
				var output = new MJ.ConditionProfile
				{
					Name = AssignLanguageMap( ConvertSpecialCharacters( input.Name ), input.Name_Map, "Condition Name", DefaultLanguageForMaps, ref messages, false ),
					Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Condition Description", DefaultLanguageForMaps, ref messages, true )
				};

				//should only be one SWP
				//foreach ( string subjectWebpage in input.SubjectWebpage )
				//{    }
				if ( !IsUrlValid( input.SubjectWebpage, ref status, ref isUrlPresent ) )
				{
					if ( isUrlPresent )
					{
						if ( WarnOnInvalidUrls )
							warningMessages.Add( string.Format( "The Condtion Profile Subject Webpage is invalid ({0}) (warning only). ", input.SubjectWebpage ) + status );
						else
							messages.Add( string.Format( "The Condtion Profile Subject Webpage is invalid ({0}). ", input.SubjectWebpage ) + status );
					}
				}
				else
				{
					if ( isUrlPresent )
					{
						output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Condition Profile Subject Webpage", ref messages, false );

					}
				}

				output.AudienceLevelType = FormatCredentialAlignmentVocabs( "audienceLevelType", input.AudienceLevelType, ref messages );
				output.AudienceType = FormatCredentialAlignmentVocabs( "audienceType", input.AudienceType, ref messages );

				output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );

                output.Condition = AssignLanguageMapList( input.Condition, input.Condition_Map, "Condition Profile Conditions", ref messages );
				//TODO - change SubmissionOf to be of type uri
				MI.LanguageMapList SubmissionOf_Map = new MI.LanguageMapList();
				//MI.LanguageMap SubmissionOfDescription_Map = new MI.LanguageMap();
				//check for non-URLs
				string submissions = "";
				List<string> validUrls = new List<string>();
				foreach ( var item in input.SubmissionOf )
				{
					if ( string.IsNullOrWhiteSpace( item ) )
						continue;

					if ( item.ToLower().StartsWith( "http" ) )
					{
						validUrls.Add( item );
					}
					else
					{
						submissions += item + " " + Environment.NewLine;
					}
				}//
				 //start with handling as a string - see if any API calls with language - unlikely
				 //ONCE IN PRODUCTION CHANGE SubmissionOf DEFINITION BACK FROM object
				if ( UtilityManager.GetAppKeyValue( "usingSubmisionOfAsUrls", false ) ==  false )
				{
					output.SubmissionOf = AssignLanguageMapList( validUrls, SubmissionOf_Map, "Condition Profile SubmissionOf", ref messages );

					if ( !string.IsNullOrWhiteSpace(submissions ))
					{
						//populate anyway
						if ( string.IsNullOrWhiteSpace( input.SubmissionOfDescription ) )
						{
							input.SubmissionOfDescription = submissions;
						}
						else
						{
							input.SubmissionOfDescription += Environment.NewLine + submissions;
						}
					}
					
				} else
				{
					if ( validUrls.Count > 0 )
					{
						output.SubmissionOf = AssignValidUrlListAsStringList( validUrls, "SubmissionOf", ref messages );
					}
					if ( !string.IsNullOrWhiteSpace( submissions ) )
					{
						warningMessages.Add( "August 30, 2019. The ConditionProfile SubmissionOf property is now a list of URIs. Use SubmissionOfDescription to provide text related information for submission requirements. " );

						if ( string.IsNullOrWhiteSpace( input.SubmissionOfDescription ) )
						{
							input.SubmissionOfDescription = submissions;
						} else
						{
							//should check if exists - temporary could be in both
							input.SubmissionOfDescription += Environment.NewLine + submissions;
						}
					}
				}
				//
				output.SubmissionOfDescription = AssignLanguageMap( ConvertSpecialCharacters( input.SubmissionOfDescription ), input.SubmissionOfDescription_Map, "SubmissionOf Description", DefaultLanguageForMaps, ref messages, false );

				//special case where the AssertedBy property could be a list or a single class
				output.AssertedBy = FormatOrganizationReferences( input.AssertedBy, "Asserted By", false, ref messages, false );

				output.Experience = input.Experience;
				output.MinimumAge = input.MinimumAge;
				output.YearsOfExperience = input.YearsOfExperience;
				output.Weight = input.Weight;

				//
				output.CreditUnitType = null;
				output.CreditValue = AssignQuantitiveValue( input.CreditValue, "CreditValue", "ConditionProfile", ref messages );
				//at this point could have had no data, or bad data
				if ( output.CreditValue == null )
				{
					//check legacy
					output.CreditValue = AssignQuantitiveValue( "ConditionProfile", input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref messages );

					//apparantly will still allow just a description. TBD: is it allowed if creditValue is provided?
					output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map, "CreditUnitTypeDescription", DefaultLanguageForMaps, ref messages );
				}

				//bool hasData = false;
				//MJ.QuantitativeValue qv = new MJ.QuantitativeValue();
				//if ( AssignQuantitiveValue( input.CreditValue, "CreditValue", "ConditionProfile", ref qv, ref messages ) )
				//{
				//	output.CreditValue = new List<MJ.QuantitativeValue>
				//	{
				//		qv
				//	};
				//}
				//else if ( !usingQuantitiveValue )
				//{
				//	if ( ValidateCreditUnitOrHoursProperties( input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref hasData, ref messages ) )
				//	{
				//		output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map, "CreditUnitTypeDescription", DefaultLanguageForMaps, ref messages );
				//		//credential alignment object
				//		if ( !string.IsNullOrWhiteSpace( input.CreditUnitType ) )
				//		{
				//			output.CreditUnitType = FormatCredentialAlignment( "creditUnitType", input.CreditUnitType, ref messages );
				//		}

				//		output.CreditUnitValue = input.CreditUnitValue;
				//		output.CreditHourType = AssignLanguageMap( ConvertSpecialCharacters( input.CreditHourType ), input.CreditHourType_Map, "CreditHourType", DefaultLanguageForMaps, ref messages );
				//		output.CreditHourValue = input.CreditHourValue;
				//	}
				//}

				output.AlternativeCondition = FormatConditionProfile( input.AlternativeCondition, ref messages );
				output.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );

				//jurisdictions
				JurisdictionProfile newJp = new JurisdictionProfile();
				foreach ( var jp in input.Jurisdiction )
				{
					newJp = MapToJurisdiction( jp, ref messages );
					if ( newJp != null )
						output.Jurisdiction.Add( newJp );
				}
				foreach ( var jp in input.ResidentOf )
				{
					newJp = MapToJurisdiction( jp, ref messages );
					if ( newJp != null )
						output.ResidentOf.Add( newJp );
				}

				//targets
				output.TargetCredential = FormatEntityReferencesList( input.TargetCredential, MJ.Credential.classType, false, ref messages );
				output.TargetAssessment = FormatEntityReferencesList( input.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
				output.TargetLearningOpportunity = FormatEntityReferencesList( input.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

				output.TargetCompetency = FormatCompetencies( input.TargetCompetency, ref messages );


				list.Add( output );
			}

			return list;
		}
		#endregion

		#region Connections
		public List<MJ.ConditionProfile> FormatConnections( List<MI.Connections> inputList, ref List<string> messages )
		{
			if ( inputList == null || inputList.Count == 0 )
				return null;

			var list = new List<MJ.ConditionProfile>();
			EntityReferenceHelper helper = new EntityReferenceHelper();
			foreach ( var input in inputList )
			{
				var output = new MJ.ConditionProfile();
                string idUrl = "";
                //output.AssertedBy = FormatOrganizationReferenceToList( item.AssertedBy, "Asserted By", true, ref messages );
                if ( FormatOrganizationReference( input.AssertedBy, "Asserted By", ref idUrl, false, ref messages ) )
				{
                    if ( !string.IsNullOrWhiteSpace( idUrl ) )
                    {
                        if ( output.AssertedBy == null )
                            output.AssertedBy = new List<string>();

                        output.AssertedBy.Add( idUrl);
						//output.AssertedBy = helper.OrgBaseList[ 0 ] ;
                        
					}
				}

				output.CreditUnitType = null;
				output.CreditValue = AssignQuantitiveValue( input.CreditValue, "CreditValue", "ConditionProfile", ref messages );
				//at this point could have had no data, or bad data
				if ( output.CreditValue == null )
				{
					//check legacy
					output.CreditValue = AssignQuantitiveValue( "ConditionProfile", input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref messages );

					//apparantly will still allow just a description. TBD: is it allowed if creditValue is provided?
					output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map, "CreditUnitTypeDescription", DefaultLanguageForMaps, ref messages );
				}

				//bool hasData = false;
				//MJ.QuantitativeValue qv = new MJ.QuantitativeValue();
				//if ( AssignQuantitiveValue( input.CreditValue, "CreditValue", "ConditionProfile", ref qv, ref messages ) )
				//{
				//	output.CreditValue = new List<MJ.QuantitativeValue>
				//	{
				//		qv
				//	};
				//}
				//else if ( !usingQuantitiveValue )
				//{
				//	if ( ValidateCreditUnitOrHoursProperties( input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref hasData, ref messages ) )
				//	{
				//		output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map, "CreditUnitTypeDescription", DefaultLanguageForMaps, ref messages );
				//		//credential alignment object
				//		if ( !string.IsNullOrWhiteSpace( input.CreditUnitType ) )
				//		{
				//			output.CreditUnitType = FormatCredentialAlignment( "creditUnitType", input.CreditUnitType, ref messages );
				//		}

				//		output.CreditUnitValue = input.CreditUnitValue;
				//		output.CreditHourType = AssignLanguageMap( ConvertSpecialCharacters( input.CreditHourType ), input.CreditHourType_Map, "CreditHourType", DefaultLanguageForMaps, ref messages );
				//		output.CreditHourValue = input.CreditHourValue;
				//	}
				//}

                output.Name = AssignLanguageMap( ConvertSpecialCharacters( input.Name ), input.Name_Map, "Condition Name", DefaultLanguageForMaps, ref messages, false );
                output.Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Condition Description", DefaultLanguageForMaps, ref messages, true );
                output.Weight = input.Weight;

				//targets
				//must have at least one target
				output.TargetCredential = FormatEntityReferencesList( input.TargetCredential, MJ.Credential.classType, false, ref messages );
				output.TargetAssessment = FormatEntityReferencesList( input.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
				output.TargetLearningOpportunity = FormatEntityReferencesList( input.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

				list.Add( output );
			}

			return list;
		}
		#endregion

		#region ProcessProfile
		public List<MJ.ProcessProfile> FormatProcessProfile( List<MI.ProcessProfile> profiles, ref List<string> messages )
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
                    Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Process Profile Description", DefaultLanguageForMaps, ref messages, true ),

                    ProcessFrequency = AssignLanguageMap( ConvertSpecialCharacters( input.ProcessFrequency ), input.ProcessFrequency_Map, "ProcessFrequency", DefaultLanguageForMaps, ref messages, false ),
                    ProcessMethodDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ProcessMethodDescription ), input.ProcessMethodDescription_Map, "ProcessMethodDescription", DefaultLanguageForMaps, ref messages, false ),
                    ProcessStandardsDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ProcessStandardsDescription ), input.ProcessStandardsDescription_Map, "ProcessStandardsDescription", DefaultLanguageForMaps, ref messages, false ),
                    ScoringMethodDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ScoringMethodDescription ), input.ScoringMethodDescription_Map, "ScoringMethodDescription",DefaultLanguageForMaps,  ref messages, false ),
                    ScoringMethodExampleDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ScoringMethodExampleDescription ), input.ScoringMethodExampleDescription_Map, "ScoringMethodExampleDescription", DefaultLanguageForMaps,  ref messages, false ),
                    VerificationMethodDescription = AssignLanguageMap( ConvertSpecialCharacters( input.VerificationMethodDescription ), input.VerificationMethodDescription_Map, "VerificationMethodDescription", DefaultLanguageForMaps, ref messages, false )
                };

				cp.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Process Profile Subject Webpage", ref messages, false );

				//or inputType
				cp.ExternalInputType = FormatCredentialAlignmentVocabs( "externalInputType", input.ExternalInputType, ref messages );

				//short replacement method
				cp.ProcessMethod = AssignValidUrlAsString( input.ProcessMethod, "Process Method", ref messages, false );

				cp.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "Process Standards", ref messages, false );

				cp.ScoringMethodExample = AssignValidUrlAsString( input.ScoringMethodExample, "Scoring Method Example", ref messages, false );

				cp.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );
				//cp.Region = MapRegions( input.Region, ref messages );
				cp.ProcessingAgent = FormatOrganizationReferences( input.ProcessingAgent, "Processing Agent", false, ref messages, false );

				//targets
				cp.TargetCredential = FormatEntityReferencesList( input.TargetCredential, MJ.Credential.classType, false, ref messages );
				cp.TargetAssessment = FormatEntityReferencesList( input.TargetAssessment, MJ.AssessmentProfile.classType, false, ref messages );
				cp.TargetLearningOpportunity = FormatEntityReferencesList( input.TargetLearningOpportunity, MJ.LearningOpportunityProfile.classType, false, ref messages );

                cp.TargetCompetencyFramework = FormatEntityReferencesList( input.TargetCompetencyFramework, "ceterms:targetCompetencyFramework", false, ref messages ); ;


                output.Add( cp );
			}

			return output;
		}
		#endregion

		#region estimatedDuration
		public List<MJ.DurationProfile> FormatDuration( List<MI.DurationProfile> input, ref List<string> messages )
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
					Description = AssignLanguageMap( item.Description, item.Description_Map, "Duration Profile Description",DefaultLanguageForMaps,  ref messages )
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
		public string FormatDurationItem( MI.DurationItem input, string propertyName, ref List<string> messages )
		{
			if ( input == null )
				return null;

			//List<string> list = new List<string>();
			var cp = new MJ.DurationProfile();
			string duration = "";
			if ( DurationHasValue( input ) )
			{
				duration = AsSchemaDuration( input );
			}
			else
			{
				//ignore
				//messages.Add( string.Format( "Duration Item error - For {0}, please provide at least one value for the duration item.", propertyName ) );
				//continue;
				return null;
			}
		
			return duration;
		}
		public string AsSchemaDuration( MI.DurationItem entity )
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

		public bool DurationHasValue( MI.DurationItem duration )
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

		//public List<MJ.FinancialAlignmentObject> MapFinancialAssistance( List<Models.Input.FinancialAlignmentObject> list, ref List<string> messages )
		//{
		//	List<MJ.FinancialAlignmentObject> output = new List<MJ.FinancialAlignmentObject>();
		//	if ( list == null || list.Count == 0 )
		//		return null;
		//	MJ.FinancialAlignmentObject jp = new MJ.FinancialAlignmentObject();
		//	foreach ( var item in list )
		//	{
		//		var fa = new MJ.FinancialAlignmentObject
		//		{
		//			AlignmentType = item.AlignmentType,
		//			Framework = AssignValidUrlAsString( item.Framework, "Framework", ref messages, false ),
		//			FrameworkName = AssignLanguageMap( item.FrameworkName, item.FrameworkName_Map,"Framework Name", DefaultLanguageForMaps, ref messages ),
		//			TargetNode = AssignValidUrlAsString( item.TargetNode, "TargetNode", ref messages, false ),
		//			TargetNodeDescription = AssignLanguageMap( item.TargetNodeDescription, item.TargetNodeDescription_Map, "TargetNode Description", DefaultLanguageForMaps, ref messages ),
  //                  TargetNodeName = AssignLanguageMap( item.TargetNodeName, item.TargetNodeName_Map, "TargetNode Name", DefaultLanguageForMaps, ref messages ),
  //                  Weight = item.Weight
		//		};
		//		fa.AlignmentDate = MapDate( item.AlignmentDate, "AlignmentDate", ref messages );

		//		//if ( usingPendingSchemaVersion )
		//		fa.CodedNotation = item.CodedNotation;

		//		output.Add( fa );
		//	}
		//	return output;
		//}

		public List<MJ.FinancialAssistanceProfile> MapFinancialAssistance( List<Models.Input.FinancialAssistanceProfile> list, ref List<string> messages )
		{
			List<MJ.FinancialAssistanceProfile> output = new List<MJ.FinancialAssistanceProfile>();
			if ( list == null || list.Count == 0 )
				return null;
			MJ.FinancialAssistanceProfile jp = new MJ.FinancialAssistanceProfile();
			foreach ( var item in list )
			{
				var fa = new MJ.FinancialAssistanceProfile
				{
					Name = AssignLanguageMap( item.Name, item.Name_Map, "FinancialAssistance Name", DefaultLanguageForMaps, ref messages ),
					Description = AssignLanguageMap( item.Description, item.Description_Map, "FinancialDescription", DefaultLanguageForMaps, ref messages ),
					SubjectWebpage = AssignValidUrlAsString( item.SubjectWebpage, "Subject Webpage", ref messages, false ), //??
					
					FinancialAssistanceType = FormatCredentialAlignmentVocabs( "FinancialAssistanceType", item.FinancialAssistanceType, ref messages )
				};
				output.Add( fa );
			}

			return output;
		}
		#region === Jurisdictions/addresses ===
		public List<JurisdictionProfile> MapJurisdictions( List<Models.Input.Jurisdiction> list, ref List<string> messages )
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
		public JurisdictionProfile MapJurisdictionAssertions( MI.JurisdictionAssertedInProfile profile,
			ref EntityReferenceHelper helper,
			ref List<string> messages )
		{

			JurisdictionProfile jp = new JurisdictionProfile();

			jp = MapToJurisdiction( profile.Jurisdiction, ref messages );
			if ( jp == null )
				return null;
            //additional check for asserted by org, and list of assertion types
            //jp.AssertedBy = FormatOrganizationReferenceToList( profile.AssertedBy, "Asserted By", true, ref messages );
            string idUrl = "";
            if ( FormatOrganizationReference( profile.AssertedBy, "Asserted By", ref idUrl, false, ref messages ) )
			{
                if ( !string.IsNullOrWhiteSpace( idUrl ) )
                {
					//currently defined as object. Ultimately will be a list, but should be single
					List<string> bys = new List<string>();
					bys.Add( idUrl );
					jp.AssertedBy = bys;
				}
			}
			return jp;
		}

		public MJ.JurisdictionProfile MapToJurisdiction( Models.Input.Jurisdiction jp, ref List<string> messages )
		{
			var jpOut = new MJ.JurisdictionProfile();
			//make sure there is data here!!!
			jpOut.Description = Assign(jp.Description, DefaultLanguageForMaps );
			//NEED to handle at least the main jurisdiction
			if ( jp.MainJurisdiction != null &&
				(!string.IsNullOrEmpty( jp.MainJurisdiction.Name) || !string.IsNullOrEmpty( jp.MainJurisdiction.GeoURI ) ))
			{
				jpOut.GlobalJurisdiction = null;
				var gc = new MOPlace();
				if ( MapGeoCoordinatesToPlace( jp.MainJurisdiction, ref gc, ref messages ) )
				{
					//jpOut.MainJurisdiction = ( gc );
					jpOut.MainJurisdiction = new List<MJ.Place>();
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

		public List<JurisdictionProfile> JurisdictionProfileAdd( JurisdictionProfile input, List<JurisdictionProfile> output )
		{
			if ( input == null )
				return output;
			if ( output == null )
				output = new List<JurisdictionProfile>();
			output.Add( input );
			return output;
		}


		public bool MapGeoCoordinatesToPlace( MI.Place input, ref MOPlace entity, ref List<string> messages )
		{
			bool isValid = true;
			bool isUrlPresent = true;
			entity = new MOPlace();
			string statusMessage = "";

			if (string.IsNullOrWhiteSpace(input.Name) && string.IsNullOrWhiteSpace(input.GeoURI))
			{
				//although the name will usually equal the country or region
				messages.Add("Error - a name must be provided with a jurisidiction GeoCoordinate. The name is typically the country or region within a country, but could also be a continent.");
				isValid = false;
			}
			else
			{
				entity.Name = AssignLanguageMap(input.Name, input.Name_Map, "Place Name", DefaultLanguageForMaps, ref messages);
			}

			//Address a = new Address();

			//we don't know for sure if the url is a useful geonames like one.
			if ( !string.IsNullOrWhiteSpace( input.GeoURI ) )
			{
				if ( IsUrlValid( input.GeoURI, ref statusMessage, ref isUrlPresent ) )
				{
					entity.GeoURI = input.GeoURI;
					//should any additional data be also provided?
					//entity.Country = input.Country;
					entity.Name = Assign(input.AddressRegion, DefaultLanguageForMaps );
					entity.Latitude = input.Latitude;// != 0 ? input.Latitude.ToString() : "";
					entity.Longitude = input.Longitude;// != 0 ? input.Longitude.ToString() : "";

				}
				else
				{
					//must have some geo url
					if ( isUrlPresent )
					{
						if ( WarnOnInvalidUrls )
							warningMessages.Add( "Warning - a valid geo coded URL, such as from geonames.com, must be provided." );
						else
							messages.Add( "Error - a valid geo coded URL, such as from geonames.com, must be provided." );
					}
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
	
		/// <summary>
		/// Format AvailableAt
		/// 17-10-20 - essentially an address now
		/// 17-11-02 - essentially a Place now
		/// </summary>
		/// <param name="input"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public List<MOPlace> FormatAvailableAtList( List<MIPlace> input, ref List<string> messages )
		{
			//Available At should require an address, not just contact points
			return FormatPlacesList( input, ref messages, true ); 
			
		}

		public List<MOPlace> FormatPlacesList( List<MIPlace> input, ref List<string> messages, bool addressExpected = false )
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

		public bool FormatPlace( MIPlace input, bool isAddressExpected, ref MOPlace output, ref List<string> messages )
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
				//output.Name = Assign(input.Name, DefaultLanguageForMaps );
				output.Name = AssignLanguageMap(input.Name, input.Name_Map, "Place Name", DefaultLanguageForMaps, ref messages);
			}
			output.Description = AssignLanguageMap(ConvertSpecialCharacters(input.Description), input.Description_Map, "Place Description", DefaultLanguageForMaps, ref messages, false, MinimumDescriptionLength);

			if ( !string.IsNullOrWhiteSpace( input.Address1 ) )
			{
                string address = ( input.Address1 ?? "" );
				if ( !string.IsNullOrWhiteSpace( input.Address2 ) )
				{
                    address += ", " + input.Address2;
				}
                output.StreetAddress = Assign( address, DefaultLanguageForMaps );
            }
			if ( !string.IsNullOrWhiteSpace( input.PostOfficeBoxNumber ) )
			{
				output.PostOfficeBoxNumber = input.PostOfficeBoxNumber;
			}
			if ( !string.IsNullOrWhiteSpace( input.City ) )
			{
				output.City = Assign(input.City, DefaultLanguageForMaps );
			}
			if ( !string.IsNullOrWhiteSpace( input.AddressRegion ) )
			{
				output.AddressRegion = Assign(input.AddressRegion, DefaultLanguageForMaps );
			}
			if ( !string.IsNullOrWhiteSpace( input.PostalCode ) )
			{
				output.PostalCode = input.PostalCode;
			}
			if ( !string.IsNullOrWhiteSpace( input.Country ) )
			{
				output.Country = Assign(input.Country, DefaultLanguageForMaps );
			}
			output.Latitude = input.Latitude;
			output.Longitude = input.Longitude;

			bool hasContactPoints = false;
			//check for old Property of ContactPoint, ignore if ContactPoint provided
			//if ( input.ContactPoint == null || input.ContactPoint.Count == 0 )
			//{
			//	if ( input.ContactPointOLD != null && input.ContactPointOLD.Count > 0 )
			//	{
			//		input.ContactPoint = input.ContactPointOLD;
			//		//add a warning
			//		warningMessages.Add( "The Place property of ContactPoint has been renamed to TargetContactPoint. Please update your code to use TargetContactPoint" );
			//		input.ContactPointOLD = null;
			//	}
			//}

			if ( input.ContactPoint != null && input.ContactPoint.Count > 0 )
			{
				foreach ( var cpi in input.ContactPoint )
				{
					cpo = new MJ.ContactPoint()
					{
						Name = Assign(cpi.Name, DefaultLanguageForMaps ),
						ContactType = Assign(cpi.ContactType, DefaultLanguageForMaps )
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
			//do we have any valid address fields?
			if ( ( !string.IsNullOrWhiteSpace( input.Address1 )
					|| !string.IsNullOrWhiteSpace( input.PostOfficeBoxNumber )
					) &&
					( string.IsNullOrWhiteSpace( input.City )
					|| string.IsNullOrWhiteSpace( input.AddressRegion )
					|| string.IsNullOrWhiteSpace( input.PostalCode ) )
					)
			{
				//missing somthing

					messages.Add( string.Format("A valid address is expected. Please provide a proper address. Address1: {0}, Address2: {1}, POBox: {5} City:{2}, Region: {3}, PostalCode: {4}", input.Address1 ?? "", input.Address2 ?? "", input.City ?? "*missing*", input.AddressRegion ?? "*missing*", input.PostalCode ?? "*missing*", input.PostOfficeBoxNumber ?? "" ) );
					return false;
				
			}

			if ( ( string.IsNullOrWhiteSpace( input.Address1 )
					&& string.IsNullOrWhiteSpace( input.PostOfficeBoxNumber ) 
					)
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

		#endregion

		#region Handle Credit Units
		public List<MJ.QuantitativeValue> AssignQuantitiveValue( MI.QuantitativeValue input, string propertyName, string parentEntityType, ref List<string> messages, string conceptScheme = "creditUnitType" )
		{
			if ( input == null )
			{
				return null;
			}
			var list = new List<MJ.QuantitativeValue>();
			var output = new MJ.QuantitativeValue();
			int currentCount = messages.Count();
			//check if empty
			if ( input.Value == 0 && input.MinValue == 0 && input.MaxValue == 0 
				&& string.IsNullOrWhiteSpace( input.Description )
				&& (input.Description_Map == null || input.Description_Map.Count() == 0)
				&& string.IsNullOrWhiteSpace( input.UnitText )
				)
			{ 
				return null;
			}

			//if no values, can have description alone, but not unit text
			if ( input.Value == 0 && input.MinValue == 0 && input.MaxValue == 0 )
			{
				if ( string.IsNullOrWhiteSpace( input.Description )
					&& (input.Description_Map == null || input.Description_Map.Count() == 0 )
					&& !string.IsNullOrWhiteSpace( input.UnitText )
					)
				{
					messages.Add( string.Format( "Where no values are entered for a QuantitativeValue, a description is required, the UnitText is not enough for: '{0}' {1}.", parentEntityType, propertyName ) );
					return null;
				}
				//here we have at least a desc
				output = new MJ.QuantitativeValue
				{
					Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, propertyName + " Description", DefaultLanguageForMaps, ref messages, false )
				};
				//not sure it makes sense to allow units without values?
				if ( !string.IsNullOrWhiteSpace( input.UnitText ) )
					output.UnitText = FormatCredentialAlignment( "creditUnitType", input.UnitText, ref messages );

				list.Add( output );
				return list;
			}

			//have at least one value

			if ( input.Value < 0 )
				messages.Add( string.Format( "The unit value for: '{0}' {1} must be greater than zero.", parentEntityType, propertyName ) );

			if ( input.MinValue < 0 )
				messages.Add( string.Format( "The minimum unit value for: '{0}' {1} must be greater than zero.", parentEntityType, propertyName ) );

			if ( input.MaxValue < 0 || input.MinValue > input.MaxValue)
				messages.Add( string.Format( "When a MaxValue is provided for: '{0}' {1}, it must be greater than zero (and greater than the MinValue.", parentEntityType, propertyName ) );

			if ( input.Value > 0 )
			{
				if ( input.MinValue != 0 || input.MaxValue != 0 )
				{
					messages.Add( string.Format( "When a Value is provided, NEITHER Min Value or Max Values may be provided for: '{0}' {1}", parentEntityType, propertyName ) );
				}
			}
			else if ( input.MinValue <= 0 || input.MaxValue <= 0 ) 
			{
				messages.Add( string.Format( "When specifying a range for credit value, both Min Value and Max Values must be provided for:'{0}' {1}", parentEntityType, propertyName ) );
			}

			if ( currentCount < messages.Count() )
				return null;
			//
			output = new MJ.QuantitativeValue
			{
				Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, propertyName + " Description", DefaultLanguageForMaps, ref messages, false ),
				Value = input.Value,
				MinValue = input.MinValue,
				MaxValue = input.MaxValue
			};
			//not required. but at this point there are values
			if ( string.IsNullOrWhiteSpace( input.UnitText ) )
			{
				//must have desc and/or unitText
				if ( output.Description == null || output.Description.Count == 0 )
				{
					messages.Add( string.Format( "Either UnitText or Credit Unit Description are required when values are present in the QuantitativeValue of: '{0}' for '{1}'", propertyName, parentEntityType ) );
					return null;
				}
			}
			else
			{
				//credential alignment object
				//may only allow english
				//var text = AssignLanguageMap( input.UnitText, input.UnitText_Map, parentEntityType + " Credit Value", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				//NOTE: need to consider concept schemes other than creditUnitType
				output.UnitText = FormatCredentialAlignment( "creditUnitType", input.UnitText, ref messages );
			}
			list.Add( output );
			return list;
		}
		public bool AssignQuantitiveValue( MI.QuantitativeValue input, string propertyName, string parentEntityType, ref MJ.QuantitativeValue output, ref List<string> messages, string conceptScheme = "creditUnitType" )
		{
			output = null;
			if ( input == null )
			{
				return false;
			}
			int currentCount = messages.Count();
			//check if empty
			if ( input.Value == 0 && input.MinValue == 0 && input.MaxValue == 0
				&& string.IsNullOrWhiteSpace( input.Description )
				&& ( input.Description_Map == null || input.Description_Map.Count() == 0 )
				&& string.IsNullOrWhiteSpace( input.UnitText )
				)
			{
				return false;
			}

			//if no values, can have description alone, but not unit text
			if ( input.Value == 0 && input.MinValue == 0 && input.MaxValue == 0 )
			{
				if ( string.IsNullOrWhiteSpace( input.Description )
					&& ( input.Description_Map == null || input.Description_Map.Count() == 0 )
					&& !string.IsNullOrWhiteSpace( input.UnitText )
					)
				{
					messages.Add( string.Format( "Where no values are entered for a QuantitativeValue, a description is required, the UnitText is not enough for: '{0}' {1}.", parentEntityType, propertyName ) );
					return false;
				}
				//here we have at least a desc
				output = new MJ.QuantitativeValue
				{
					Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, propertyName + " Description", DefaultLanguageForMaps, ref messages, false ),
					//not sure it makes sense to allow units without values?
					UnitText = FormatCredentialAlignment( "creditUnitType", input.UnitText, ref messages )
				};
				return true;
			}

			//have at least one value

			if ( input.Value < 0 )
				messages.Add( string.Format( "The unit value for: '{0}' {1} must be greater than zero.", parentEntityType, propertyName ) );

			if ( input.MinValue < 0 )
				messages.Add( string.Format( "The minimum unit value for: '{0}' {1} must be greater than zero.", parentEntityType, propertyName ) );

			if ( input.MaxValue < 0 || input.MinValue > input.MaxValue )
				messages.Add( string.Format( "When a MaxValue is provided for: '{0}' {1}, it must be greater than zero (and greater than the MinValue.", parentEntityType, propertyName ) );

			if ( input.Value > 0 )
			{
				if ( input.MinValue != 0 || input.MaxValue != 0 )
				{
					messages.Add( string.Format( "When a Value is provided, NEITHER Min Value or Max Values may be provided for: '{0}' {1}", parentEntityType, propertyName ) );
					return false;
				}
			}
			else if ( input.MinValue <= 0 || input.MaxValue <= 0 )
			{
				messages.Add( string.Format( "When specifying a range for credit value, both Min Value and Max Values must be provided for:'{0}' {1}", parentEntityType, propertyName ) );
				return false;
			}
			if ( currentCount < messages.Count() )
				return false;
			//
			output = new MJ.QuantitativeValue
			{
				Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, propertyName + " Description", DefaultLanguageForMaps, ref messages, false ),
				Value = input.Value,
				MinValue = input.MinValue,
				MaxValue = input.MaxValue
			};
			//not required. but at this point there are values
			if ( string.IsNullOrWhiteSpace( input.UnitText ) )
			{
				//must have desc and/or unitText
				if ( output.Description == null || output.Description.Count == 0 )
				{
					messages.Add( string.Format( "Either UnitText or Credit Unit Description are required when values are present in the QuantitativeValue of: '{0}' for '{1}'", propertyName, parentEntityType ) );
					return false;
				}
			}
			else
			{
				//credential alignment object
				//may only allow english
				//var text = AssignLanguageMap( input.UnitText, input.UnitText_Map, parentEntityType + " Credit Value", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				//NOTE: need to consider concept schemes other than creditUnitType
				output.UnitText = FormatCredentialAlignment( "creditUnitType", input.UnitText, ref messages );
			}

			return true;
		}
		public List<MJ.QuantitativeValue> AssignQuantitiveValue( string entity, decimal creditHourValue, string creditHourType, string creditUnitType, decimal creditUnitValue, string creditUnitTypeDescription, ref List<string> messages )
		{
			bool hasData = false;
			if ( ValidateCreditUnitOrHoursProperties( creditHourValue, creditHourType, creditUnitType, creditUnitValue, creditUnitTypeDescription, ref hasData, ref messages ) )
			{
				if ( hasData )
				{
					//return warning
					warningMessages.Add( "WARNING: The Credit Unit/Hour properties are obsolete. Use CreditValue instead." );
					//will not have a range
					MI.QuantitativeValue iqv = new MI.QuantitativeValue
					{
						Value = creditUnitValue > 0 ? creditUnitValue : creditHourValue > 0 ? creditHourValue : 0,
						UnitText = creditUnitType ?? "",
						Description = creditUnitTypeDescription ?? "",
						//Description_Map = input.Description_Map
					};
					if ( ( creditHourType ?? "" ).Length > 0 )
					{
						//19-05-01 hour vocabs not set, so set to description
						iqv.Description = creditHourType;
					}

					return AssignQuantitiveValue( iqv, "CreditValue", entity, ref messages );

				}
			}

			return null;
		}
		public bool ValidateCreditUnitOrHoursProperties( decimal creditHourValue, string creditHourType, string creditUnitType, decimal creditUnitValue, string creditUnitTypeDescription, ref bool hasData, ref List<string> messages )
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
			else if ( hasCreditHourData || hasCreditUnitData )
				hasData = true;

			return true;
		}
		#endregion

		#region === CredentialAlignmentObject ===
		/// <summary>
		/// Only for simple properties like subjects, with no concept scheme
		/// </summary>
		/// <param name="terms"></param>
		/// <returns></returns>
		public List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentListFromStrings( List<string> terms )
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
        public List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentListFromStrings( List<string> terms, MI.LanguageMapList map )
        {
            List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
            if ( (terms == null || terms.Count == 0 ) && (map == null || map.Count == 0))
                return null;

            if ( terms != null && terms.Count > 0 )
            {
                foreach ( string item in terms )
                {
                    if ( !string.IsNullOrWhiteSpace( item ) )
                        list.Add( FormatCredentialAlignment( item ) );
                }
            }

            return list;
        }
        /// <summary>
        /// Only for simple properties like subjects, with no concept scheme
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private MJ.CredentialAlignmentObject FormatCredentialAlignment( string name )
		{
            if (string.IsNullOrWhiteSpace( name ))
                return null;

			MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject
			{
				TargetNodeName = Assign( name, DefaultLanguageForMaps )
			};
			return ca;
		}

		/// <summary>
		/// Format CredentialAlignmentObject with only text exists
		/// </summary>
		/// <param name="name"></param>
		/// <param name="inputMap"></param>
		/// <returns></returns>
		//public MJ.CredentialAlignmentObject FormatCredentialAlignment( string name, MI.LanguageMap inputMap  )
		//{
		//	if ( string.IsNullOrWhiteSpace( name ) )
		//		return null;

		//	MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject
		//	{
		//		TargetNodeName = Assign( name, DefaultLanguageForMaps )
		//	};
		//	return ca;
		//}
		/// <summary>
		/// Assign CAO for properties with concepts
		/// </summary>
		/// <param name="ctdlProperty"></param>
		/// <param name="terms"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentVocabs( string ctdlProperty, List<string> terms, ref List<string> messages, string alias = "" )
		{
			List<MJ.CredentialAlignmentObject> list = new List<MJ.CredentialAlignmentObject>();
			if ( terms == null || terms.Count == 0 || string.IsNullOrWhiteSpace( ctdlProperty ))
				return null;
			foreach ( string item in terms )
			{
                if (!string.IsNullOrWhiteSpace( item ))
                    list.Add( FormatCredentialAlignment( ctdlProperty, item, ref messages, alias ) );
			}

			return list;
		}
		public List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentVocabToList( string vocab, string term, ref List<string> messages )
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
		public MJ.CredentialAlignmentObject FormatCredentialAlignment( string ctdlProperty, string term, ref List<string> messages, string alias = "" )
		{
			MJ.CredentialAlignmentObject output = new MJ.CredentialAlignmentObject();
			CodeItem code = new CodeItem();
			if ( string.IsNullOrWhiteSpace( term ))
			{
				return null;
			}
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
			//json
				var ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegTermsJson", "http://credreg.net/ctdl/terms/{0}/json" );

			if ( ValidationServices.IsTermValid( ctdlProperty, term, ref code ) )
			{
				//IdProperty item = new IdProperty() { Id = code.SchemaName };
				output.TargetNode = code.SchemaName;
				output.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
				//*******these can only have one language, as controlled
				output.TargetNodeName = Assign( code.Name, SystemDefaultLanguageForMaps );
				output.TargetNodeDescription = Assign( code.Description, DefaultLanguageForMaps );
				if ( !string.IsNullOrWhiteSpace( code.ParentSchemaName ) )
					output.FrameworkName = Assign( code.ParentSchemaName.Replace( "ceterms:", "" ), DefaultLanguageForMaps );
			}
			else if ( ctdlProperty == "creditUnitType" ) //
			{
				//check for values not on credreg yet
				if ( ValidatePendingCreditUnitValues( term, ref code ) )
				{
					output.TargetNode = code.SchemaName;
					output.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
					//*******these can only have one language, as controlled
					output.TargetNodeName = Assign( code.Name, SystemDefaultLanguageForMaps );
					output.TargetNodeDescription = Assign( code.Description, DefaultLanguageForMaps );
				}
				else
				{
					messages.Add( string.Format( "The {0} type of {1} is invalid.", string.IsNullOrWhiteSpace( alias ) ? ctdlProperty : alias, term ) );
					output = null;
				} 
			}
			else if ( ctdlProperty == "FinancialAssistanceType" && environment != "production") //
			{
				//not on credreg yet - allow for non-production
				code = new CodeItem();
				code.SchemaName = "financialAid:" + term;
				code.ConceptSchemaPlain = term;
				code.Name = term;

				output.TargetNode = code.SchemaName;
				output.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
				//*******these can only have one language, as controlled
				output.TargetNodeName = Assign( code.Name, SystemDefaultLanguageForMaps );
				output.TargetNodeDescription = Assign( code.Description, DefaultLanguageForMaps );

			}
			else
			{
				messages.Add( string.Format( "The {0} type of {1} is invalid.", string.IsNullOrWhiteSpace( alias ) ? ctdlProperty : alias, term ) );
				output = null;
			}

			return output;
		}
		private bool ValidatePendingCreditUnitValues( string term, ref CodeItem code )
		{
			bool isValid = true;
			string validTerms = "Quarter Hours QuarterHours Semester Hours SemesterHours Clock Hours ClockHours";
			string pendingTerms = "Contact Hours ContactHours CreditHour Credit Hour";
			//orginal method will set to null
			code = new CodeItem();

			string label = "";
			term = term.Contains( ':' ) ? term.Split( ':' )[ 1 ] : term;
			if ( "quarter hours quarterhours".IndexOf( term.ToLower() ) > -1 )
			{
				code.SchemaName = "credUnit:QuarterHours";
				code.ConceptSchemaPlain = "QuarterHours";
				code.Name = "Quarter Hours";
			}
			else if ( "semester hours semesterhours".IndexOf( term.ToLower() ) > -1 )
			{
				code.SchemaName = "credUnit:SemesterHours";
				code.ConceptSchemaPlain = "SemesterHours";
				code.Name = "Semester Hours";
			}
			else if ( "clock hours clockhours".IndexOf( term.ToLower() ) > -1 )
			{
				code.SchemaName = "credUnit:ClockHours";
				code.ConceptSchemaPlain = "ClockHours";
				code.Name = "Clock Hours";
			}
			else
				return false;


			return isValid;
		}
		public List<MJ.CredentialAlignmentObject> AppendCredentialAlignmentListFromList( List<string> list, MI.LanguageMap mapList, string frameworkName, string framework, string property, List<MJ.CredentialAlignmentObject> output,  ref List<string> messages )
		{
			MJ.CredentialAlignmentObject entity = new MJ.CredentialAlignmentObject();
			FrameworkItem fi = new FrameworkItem();

			if ( list == null || list.Count == 0 )
			{
				if ( mapList == null || mapList.Count == 0 )
				{
					return output;
				}
				else
				{
					if ( output == null )
						output = new List<MJ.CredentialAlignmentObject>();
					int cntr = 0;
					foreach ( var item in mapList )
					{
						cntr++;
						entity = new MJ.CredentialAlignmentObject();
						fi = new FrameworkItem();
						//some validation of lang code and region
						string lang = item.Key;
						if ( ValidateLanguageCode( property, cntr, true, ref lang, ref messages ) )
						{
							fi.Name_Map.Add( lang.ToLower(), item.Value );
						}
					}
				}
				return output;
			} else
			{
				if ( output == null )
					output = new List<MJ.CredentialAlignmentObject>();
				//need to add a framework
				foreach ( var item in list )
				{
					entity = new MJ.CredentialAlignmentObject();
					//need ability to lookup codes
					fi = new FrameworkItem()
					{
						Framework = framework,
						FrameworkName = frameworkName,
						Name = item
					};
					entity = FormatCredentialAlignmentFrameworkItem( fi, true, ref messages, frameworkName, framework );
					if ( entity != null )
						output.Add( entity );
				}
			}

			return output;
		}   //

			/// <summary>
			/// Format CAO for frameworks like occupations, NAICS, and CIPS
			/// </summary>
			/// <param name="list"></param>
			/// <param name="includingCodedNotation"></param>
			/// <param name="messages"></param>
			/// <param name="frameworkName"></param>
			/// <param name="framework"></param>
			/// <returns></returns>
		public List<MJ.CredentialAlignmentObject> FormatCredentialAlignmentListFromFrameworkItemList( List<FrameworkItem> list, bool includingCodedNotation, ref List<string> messages, string frameworkName = "", string framework = "" )
		{
			if ( list == null || list.Count == 0 )
				return null;

			List<MJ.CredentialAlignmentObject> output = new List<MJ.CredentialAlignmentObject>();
			MJ.CredentialAlignmentObject entity = new MJ.CredentialAlignmentObject();

			//need to add a framework
			foreach ( FrameworkItem item in list )
			{
				entity = new MJ.CredentialAlignmentObject();
				entity = FormatCredentialAlignmentFrameworkItem( item, true, ref messages, frameworkName, framework );
				if ( entity != null )
					output.Add( entity );
			}

			return output;
		}   //

		/// <summary>
		/// Format a Credential Alignment Object
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="includingCodedNotation"></param>
		/// <param name="messages"></param>
		/// <param name="frameworkName">Helper, where if not provided in FrameworkItem, will be used.</param>
		/// <param name="framework">Helper, where if not provided in FrameworkItem, will be used.</param>
		/// <returns></returns>
		public MJ.CredentialAlignmentObject FormatCredentialAlignmentFrameworkItem( FrameworkItem entity, bool includingCodedNotation, ref List<string> messages, string frameworkName = "", string framework = "" )
		{
			bool hasData = false;
            bool isUrlPresent = true;
            string statusMessage = "";

            MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();

			if ( !string.IsNullOrWhiteSpace( entity.FrameworkName ) || ( entity.FrameworkName_Map != null && entity.FrameworkName_Map.Count > 0 ) )
			{
				ca.FrameworkName = AssignLanguageMap( entity.FrameworkName, entity.FrameworkName_Map, "Framework Name", DefaultLanguageForMaps, ref messages );
                //hasData = true;
            }
			else if ( !string.IsNullOrWhiteSpace( frameworkName ) )
			{
				ca.FrameworkName = Assign(frameworkName, DefaultLanguageForMaps );
				//hasData = true;
			}
            //need a targetNode, normally - this is the schema name
            //==>< N/A for framework items like industries
            //actually should have. It can be a URI, so do we skip Url checks?
            if ( !string.IsNullOrWhiteSpace( entity.TargetNode ) )
            {
                if ( IsUrlValid( entity.TargetNode, ref statusMessage, ref isUrlPresent, false ) )
                {
					//should we arbitrarily make all Urls lowercase or just registry Urls?
                    ca.TargetNode = AssignUrl(entity.TargetNode);
                    //demand more data
                    //hasData = true;
                } else
				{
					if ( isUrlPresent )
					{
						messages.Add( string.Format( "The {0} URL ({1}) is invalid: {2}.", "targetNode", entity.TargetNode, statusMessage ) );
					}
				}
            }

            if ( !string.IsNullOrWhiteSpace( entity.Name ) || ( entity.Name_Map != null && entity.Name_Map.Count > 0 ) )
            {
				ca.TargetNodeName = AssignLanguageMap( entity.Name, entity.Name_Map, "Name", DefaultLanguageForMaps, ref messages );
                hasData = true;
			}
			if ( !string.IsNullOrWhiteSpace( entity.Description ) || ( entity.Description_Map != null && entity.Description_Map.Count > 0 ) )
            {
                ca.TargetNodeDescription = AssignLanguageMap( entity.Description, entity.Description_Map, "TargetNodeDescription", DefaultLanguageForMaps, ref messages );
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
				{
					if ( isUrlPresent )
						ca.Framework = AssignUrl(entity.Framework);
				}
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
		public List<MJ.CredentialAlignmentObject> FormatCompetencies( List<MI.CredentialAlignmentObject> entities, ref List<string> messages )
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

		public MJ.CredentialAlignmentObject FormatCompetency( MI.CredentialAlignmentObject entity, ref List<string> messages )
		{
			bool hasData = false;
			if ( entity == null )
				return null;
			int currentCnt = messages.Count();

			MJ.CredentialAlignmentObject ca = new MJ.CredentialAlignmentObject();
			//if present, the framework must be a valid url
			if ( !string.IsNullOrWhiteSpace( entity.Framework ) )
			{
				ca.Framework = AssignValidUrlAsString( entity.Framework, "Competency Framework", ref messages, false );
				if ( !string.IsNullOrWhiteSpace(ca.Framework) )
					hasData = true;
			}
			if ( !string.IsNullOrWhiteSpace( entity.FrameworkName ) || (entity.FrameworkName_Map != null && entity.FrameworkName_Map.Count > 0 ) )
			{
				ca.FrameworkName = AssignLanguageMap( entity.FrameworkName, entity.FrameworkName_Map,  "Framework Name", DefaultLanguageForMaps, ref messages );
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
			if ( !string.IsNullOrWhiteSpace( entity.TargetNodeName ) || ( entity.TargetNodeName_Map != null && entity.TargetNodeName_Map.Count > 0 ) )
			{
                ca.TargetNodeName = AssignLanguageMap( entity.TargetNodeName, entity.TargetNodeName_Map, "Target Node Name", DefaultLanguageForMaps, ref messages );
                hasData = true;
			}
			if ( !string.IsNullOrWhiteSpace( entity.TargetNodeDescription ) || ( entity.TargetNodeDescription_Map != null && entity.TargetNodeDescription_Map.Count > 0 ) )
            {
				//if no name, use description
				if ( string.IsNullOrWhiteSpace( entity.TargetNodeName ) )
					ca.TargetNodeName = AssignLanguageMap( entity.TargetNodeDescription, entity.TargetNodeDescription_Map, "Target Node Description for NodeName", DefaultLanguageForMaps, ref messages );
                else
					ca.TargetNodeDescription = AssignLanguageMap( entity.TargetNodeDescription, entity.TargetNodeDescription_Map, "Target Node Description", DefaultLanguageForMaps, ref messages );

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
			else
			{

			}
			return ca;
		}
		#endregion


		#region === Concepts to URI ===
		public List<string> FormatConceptListFromStringsXXX(string ctdlProperty, List<string> terms, ref List<string> messages, string alias = "")
		{
			List<string> list = new List<string>();
			if ( terms == null || terms.Count == 0 || string.IsNullOrWhiteSpace( ctdlProperty ) )
				return null;
			foreach ( string item in terms )
			{
				if ( !string.IsNullOrWhiteSpace( item ) )
					list.Add( FormatConceptUriXXX( ctdlProperty, item, ref messages, alias ) );
			}


			return list;
		}

		/// <summary>
		/// Format Vocabulary properties/Concepts as URI.
		/// </summary>
		/// <param name="vocabulary"></param>
		/// <param name="name"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public string FormatConceptUriXXX(string ctdlProperty, string term, ref List<string> messages, string alias = "")
		{
			var output = "";
			CodeItem code = new CodeItem();
			if ( string.IsNullOrWhiteSpace( term ) )
			{
				return null;
			}
			//validation is pending - no lookup yet
			//need to be able to retrieve a concept by name?. Need to include the scheme. Could still be duplicates
			//		DB lookup
			/*
			 {
				"@type": "skos:Concept",
				"skos:prefLabel": "Level 1"
			}
			 */

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
			//json
			var ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegTermsJson", "http://credreg.net/ctdl/terms/{0}/json" );

			//if ( ValidationServices.IsTermValid( ctdlProperty, term, ref code ) )
			//{
			//	//IdProperty item = new IdProperty() { Id = code.SchemaName };
			//	output.TargetNode = code.SchemaName;
			//	output.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
			//	//*******these can only have one language, as controlled
			//	output.TargetNodeName = Assign( code.Name, SystemDefaultLanguageForMaps );
			//	output.TargetNodeDescription = Assign( code.Description, DefaultLanguageForMaps );
			//	if ( !string.IsNullOrWhiteSpace( code.ParentSchemaName ) )
			//		output.FrameworkName = Assign( code.ParentSchemaName.Replace( "ceterms:", "" ), DefaultLanguageForMaps );
			//}
			//else if ( ctdlProperty == "creditUnitType" ) //
			//{
			//	//check for values not on credreg yet
			//	if ( ValidatePendingCreditUnitValues( term, ref code ) )
			//	{
			//		output.TargetNode = code.SchemaName;
			//		output.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
			//		//*******these can only have one language, as controlled
			//		output.TargetNodeName = Assign( code.Name, SystemDefaultLanguageForMaps );
			//		output.TargetNodeDescription = Assign( code.Description, DefaultLanguageForMaps );
			//	}
			//	else
			//	{
			//		messages.Add( string.Format( "The {0} type of {1} is invalid.", string.IsNullOrWhiteSpace( alias ) ? ctdlProperty : alias, term ) );
			//		output = null;
			//	}
			//}
			//else if ( ctdlProperty == "FinancialAssistanceType" && environment != "production" ) //
			//{
			//	//not on credreg yet - allow for non-production
			//	code = new CodeItem();
			//	code.SchemaName = "financialAid:" + term;
			//	code.ConceptSchemaPlain = term;
			//	code.Name = term;

			//	output.TargetNode = code.SchemaName;
			//	output.Framework = string.Format( ctdlUrl, code.ConceptSchemaPlain );
			//	//*******these can only have one language, as controlled
			//	output.TargetNodeName = Assign( code.Name, SystemDefaultLanguageForMaps );
			//	output.TargetNodeDescription = Assign( code.Description, DefaultLanguageForMaps );

			//}
			//else
			//{
			//	messages.Add( string.Format( "The {0} type of {1} is invalid.", string.IsNullOrWhiteSpace( alias ) ? ctdlProperty : alias, term ) );
			//	output = null;
			//}

			return output;
		}
		#endregion

		#region ID property helpers
		[Obsolete]
		public List<string> AssignEntityReferenceListAsStringList( List<EntityReference> list, ref List<string> messages )
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



		/// <summary>
		/// Validate a URL and return as a List
		/// </summary>
		/// <param name="url"></param>
		/// <param name="propertyName"></param>
		/// <param name="messages"></param>
		/// <param name="isRequired"></param>
		/// <param name="doingExistanceCheck"></param>
		/// <returns></returns>
		public List<string> AssignValidUrlAsStringList( string url, string propertyName, ref List<string> messages, bool isRequired, bool doingExistanceCheck = true )
		{

			List<string> urlList = new List<string>();
			string output = AssignValidUrlAsString( url, propertyName, ref messages, isRequired, doingExistanceCheck );
			if ( !string.IsNullOrWhiteSpace(output ))
			{
				urlList.Add( url );
			}
			return urlList;
		}

		/// <summary>
		/// Handle list of strings that should be valid URIs. 
		/// Note, where the list is for registry URIs that may not have been published yet, use the method: AssignRegistryResourceURIsListAsStringList
		/// </summary>
		/// <param name="list"></param>
		/// <param name="title"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public List<string> AssignValidUrlListAsStringList( List<string> list, string propertyName, ref List<string> messages, bool doingExistanceCheck = true )
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
				string output = AssignValidUrlAsString( url, propertyName, ref messages, false, doingExistanceCheck );
				if ( !string.IsNullOrWhiteSpace( output ) )
				{
					urlList.Add( output );
				}
			}
			if ( cntr == 0 )
				return null;
			else
				return urlList;
		}

		/// <summary>
		/// Validate a URL and return standardized string.
		/// NOTE this method is not to be used with CTIDs - use AssignRegistryResourceURIAsString
		/// </summary>
		/// <param name="url"></param>
		/// <param name="propertyName"></param>
		/// <param name="messages"></param>
		/// <param name="isRequired"></param>
		/// <param name="doingExistanceCheck">Defaults to true. Set false for registry URIs that may not exists yet.</param>
		/// <returns></returns>
		public string AssignValidUrlAsString( string url, string propertyName, ref List<string> messages, bool isRequired, bool doingExistanceCheck = true )
		{
			string statusMessage = "";
			bool isUrlPresent = true;
			if ( string.IsNullOrWhiteSpace( url ) )
			{
				if ( isRequired )
					messages.Add( string.Format( "The {0} URL is a required property.", propertyName ) );
				return null;
			}
			List<string> temp = new List<string>();
			url = url.Trim();
			//NOTE this method is not to be used with CTIDs
			if ( IsCtidValid( url, propertyName, ref temp ) )
			{
				//NO: don't use this method with CTIDs
				//url = credRegistryGraphUrl + url.ToLower().Trim();
				//return url;
				messages.Add( string.Format( "A CTID is not valid to be provided for the '{0}' property.", propertyName ) );
				return null;
			}

			if ( !IsUrlValid( url, ref statusMessage, ref isUrlPresent, doingExistanceCheck ) )
			{
				if ( isUrlPresent )
				{
					if ( WarnOnInvalidUrls )
						warningMessages.Add( string.Format( "The {0} URL ({1}) is invalid: {2} (warning only).", propertyName, url, statusMessage ) );
					else
						messages.Add( string.Format( "The {0} URL ({1}) is invalid: {2}.", propertyName, url, statusMessage ) );
				}
				return null;
			}
			url = AssignUrl( url.TrimEnd( '/' ) );
			return url;
		} //

		/// <summary>
		/// This method is for a list of strings that contain registry URIs. 
		/// These are properly formed URIs but may not yet exist in the registry, so no existance check will be done. 
		/// If a valid CTID is provided, it will be formatted as a /resource/ URL
		/// </summary>
		/// <param name="list"></param>
		/// <param name="propertyName"></param>
		/// <param name="messages"></param>
		/// <param name="formatAsGraph"></param>
		/// <param name="isRequired"></param>
		/// <returns></returns>
		public List<string> AssignRegistryResourceURIsListAsStringList( List<string> list, string propertyName, ref List<string> messages, bool formatAsGraph = false, bool isRequired = false )
		{
			//string status = "";
			//bool isUrlPresent = true;
			List<string> urlList = new List<string>();
			List<string> temp = new List<string>();
			if ( list == null || list.Count == 0 )
			{
				if ( isRequired )
				{
					messages.Add( string.Format( "The property: '{0}' is required, and has not been provided.", propertyName ) );
				}
				return null;
			}
			int cntr = 0;
			foreach ( string item in list )
			{
				cntr++;
				string output = AssignRegistryResourceURIAsString( item, propertyName, ref messages, formatAsGraph );
				if ( !string.IsNullOrWhiteSpace( output ) )
				{
					urlList.Add( output );
				}

			}

			if ( cntr == 0 || urlList.Count == 0 )
				return null;
			else
				return urlList;
		}

		public string AssignRegistryResourceURIAsString( string url, string propertyName, ref List<string> messages, bool formatAsGraph = false, bool isRequired = false )
		{
			string statusMessage = "";
			bool isUrlPresent = true;
			if ( string.IsNullOrWhiteSpace( url ) )
			{
				if ( isRequired )
					messages.Add( string.Format( "The {0} URL is a required property.", propertyName ) );
				return null;
			}
			List<string> temp = new List<string>();
			url = url.Trim();
			//helper to accept a ctid
			if ( IsCtidValid( url, propertyName, ref temp ) )
			{
				if (formatAsGraph) {
					//url = credRegistryGraphUrl + url.ToLower().Trim();
					url = SupportServices.FormatRegistryUrl(GraphTypeUrl, url, Community);
				}
				else {
					//url = credRegistryResourceUrl + url.ToLower().Trim();
					url = SupportServices.FormatRegistryUrl(ResourceTypeUrl, url, Community);
				}

				return url;
			}

			if ( !IsUrlValid( url, ref statusMessage, ref isUrlPresent, false ) )
			{
				if ( isUrlPresent )
				{
					if ( WarnOnInvalidUrls )
						warningMessages.Add( string.Format( "The {0} URL ({1}) is invalid: {2} (warning only).", propertyName, url, statusMessage ) );
					else
						messages.Add( string.Format( "The {0} URL ({1}) is invalid: {2}.", propertyName, url, statusMessage ) );
				}
				return null;
			}
			url = AssignUrl( url.TrimEnd( '/' ) );
			return url;
		} //

		//force registry urls to lowercase
		public string AssignUrl( string url )
		{
			if ( url.ToLower().IndexOf( "/resources/ce-" ) > -1 
				|| url.ToLower().IndexOf( "/graph/ce-") > -1)
			{
				return url.ToLower();
			} else 
				return url;
		} //

		  /// <summary>
		  /// Validate a URL, and if valid assign to an IdProperty
		  /// </summary>
		  /// <param name="url">Url to validate</param>
		  /// <param name="propertyName">Literal for property name - for messages</param>
		  /// <param name="messages"></param>
		  /// <param name="isRequired">If true, produce a message if url is missing</param>
		  /// <returns>null or an IdProperty</returns>
		[Obsolete]
		public IdProperty AssignValidUrlAsIdProperty( string url, string propertyName, ref List<string> messages, bool isRequired = false )
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
					if ( WarnOnInvalidUrls )
						warningMessages.Add( string.Format( "The {0} URL ({1}) is invalid: {2} (warning only).", propertyName, url, statusMessage ) );
					else
						messages.Add( string.Format( "The {0} URL ({1}) is invalid: {2}.", propertyName, url, statusMessage ) );
				}
				return null;
			}

			idProp = new IdProperty() { Id = url.ToLower() };

			return idProp;
		} //
		[Obsolete]
		public List<IdProperty> AssignValidUrlAsPropertyIdList( string url, string propertyName, ref List<string> messages, bool isRequired = false )
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
			{
				if ( isUrlPresent )
				{
					if ( WarnOnInvalidUrls )
						warningMessages.Add( string.Format( "The {0} URL ({1}) is invalid: {2} (warning only).", propertyName, url, status ) );
					else
						messages.Add( string.Format( "The {0} URL ({1}) is invalid: {2}.", propertyName, url, status ) );
				}
			}
			else
			{
				if ( isUrlPresent )
				{
					IdProperty item = new IdProperty() { Id = url.ToLower() };
					urlId.Add( item );
				}
			}

			return urlId;
		}
		[Obsolete]
		public List<IdProperty> AssignValidUrlAsPropertyIdList( List<string> list, string title, ref List<string> messages )
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
					{
						messages.Add( string.Format( "Warning, the URL #{0}: {1} for list: {2} is invalid: ", cntr, url, title ) + status );
						if ( isUrlPresent )
						{
							if ( WarnOnInvalidUrls )
								warningMessages.Add( string.Format( "The URL #{0}: {1} for list: {2} is invalid: ", cntr, url, title ) + status );
							else
								messages.Add( string.Format( "The URL #{0}: {1} for list: {2} is invalid: ", cntr, url, title ) + status );
						}
					}
					else
					{
						if ( isUrlPresent )
						{
							urlId.Add( new IdProperty() { Id = url.ToLower() } );
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


        public List<MJ.IdentifierValue> AssignIdentifierValueToList( string value )
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
		public List<MJ.IdentifierValue> AssignIdentifierListToList( List<MI.IdentifierValue> input, ref List<string> messages )
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
					Description = AssignLanguageMap( item.Description, item.Description_Map, "Identifier Description", DefaultLanguageForMaps, ref messages)

                } );
			}
			return list;
		}
		public List<string> AssignStringToList( string value )
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                return null;

            List<string> list = new List<string>
            {
                value
            };

            return list;
        }
		public string AssignListToString( List<string> list )
		{
			if ( list == null || list.Count == 0 )
				return null;
			string value = list[ 0 ];

			return value;
		}
		/// <summary>
		/// Handle where an input object could be a string or a list of strings, and output must be a list of strings
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string AssignObjectToString(object input)
		{
			if ( input == null  )
				return null;
			if ( input.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
			{
				var list = input as Newtonsoft.Json.Linq.JArray;
				if ( list != null && list.Count() > 0 )
					return list[ 0 ].ToString();
			}
			else if ( input.GetType() == typeof( System.String ) )
			{
				return input.ToString();
			} else
			{
				//unexpected/unhandled
			}

			return null;
		}
		/// <summary>
		/// Handle where an input object could be a string or a list of strings
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public List<string> AssignObjectToList(object input, string property, ref List<string> messages)
		{
			if ( input == null )
				return null;
			List<string> output = new List<string>();
			if ( input.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
			{
				var list = input as Newtonsoft.Json.Linq.JArray;
				foreach ( var item in list )
				{
					output.Add( item.ToString());
				}
				return output;
			}
			else if ( input.GetType() == typeof( System.String ) )
			{
				output.Add( input.ToString());
				return output;
			}
			else
			{
				//unexpected/unhandled
				var inputType = input.GetType().ToString();
				messages.Add( "Error: unexpected type for " + property + ". It should be a string or a list of strings, and input was a type of: " + inputType );
			}

			return null;
		}
		public List<string> AssignListToList( List<string> list )
		{

			if ( list == null || list.Count == 0 )
				return null;
			List<string> value = new List<string>();
			value.AddRange( list );

			return value;
		}


		#region Language Map helpers
		public MJ.LanguageMap AssignLanguageMap( string input, MI.LanguageMap inputMap, string property, ref List<string> messages, bool isRequired = false )
		{
			return AssignLanguageMap( input, inputMap, property, DefaultLanguageForMaps, ref messages, isRequired );
		}

		public MJ.LanguageMap AssignLanguageMap( string input, MI.LanguageMap inputMap, string propertyName, string language, string parentCtid, bool isRequired, ref List<string> messages )
		{
			MJ.LanguageMap output = new MJ.LanguageMap();

			if ( string.IsNullOrWhiteSpace( input ) )
			{
				if ( inputMap == null || inputMap.Count == 0 )
				{
					if ( isRequired )
						messages.Add( FormatMessage( "Error - A string or language map must be entered for {0} with CTID: '{1}'.", propertyName, parentCtid ) );
				}
				else
				{
					output = AssignLanguageMap( inputMap, propertyName, ref messages );
				}
			}
			else
			{
				output = Assign( input, DefaultLanguageForMaps );
			}
			

			return output;
		}
		public MJ.LanguageMap Assign( string input, string language )
        {
            if ( string.IsNullOrWhiteSpace( input ) )
                return null;
            MJ.LanguageMap output = new MJ.LanguageMap();
            output = new MJ.LanguageMap
            {
                { language, input }
            };
            return output;
        }

        /// <summary>
        /// Format string as a language map with single value with default language of 'en'
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <param name="isRequired"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public MJ.LanguageMap AssignLanguageMap( string input, string property, bool isRequired, ref List<string> messages )
        {
            return AssignLanguageMap( input, property, isRequired, DefaultLanguageForMaps, ref messages );
        }

        public MJ.LanguageMap AssignLanguageMap( string input, string property, bool isRequired, string language, ref List<string> messages )
        {

            MJ.LanguageMap output = new MJ.LanguageMap();
            if ( string.IsNullOrWhiteSpace( input ) )
            {
                if ( isRequired )
                    messages.Add( string.Format( "The property {0} is required, please provide a value.", property ) );
                return null;
            }

            if ( string.IsNullOrWhiteSpace( language ) )
                language = DefaultLanguageForMaps;

            output.Add( language, input );

            return output;
      
        }
        public MJ.LanguageMap AssignLanguageMap( MI.LanguageMap input, string title, ref List<string> messages )
        {

            MJ.LanguageMap output = new MJ.LanguageMap();
            if ( input == null )
                return null;
            int cntr = 0;
            foreach ( var item in input )
            {
                cntr++;
                //some validation
                string lang = item.Key;
                if ( ValidateLanguageCode( title, cntr, true, ref lang, ref messages ) )
                    output.Add( lang.ToLower(), item.Value );
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }



        public MJ.LanguageMap AssignLanguageMap( string input, MI.LanguageMap inputMap, string property, string language, ref List<string> messages, bool isRequired = false, int minimumLength = 0 )
        {

            MJ.LanguageMap output = null;
            if ( string.IsNullOrWhiteSpace( input ) )
            {
                if ( inputMap == null || inputMap.Count == 0 )
                {
                    if ( isRequired )
                        messages.Add( FormatMessage( "A value or LanguageMap must be entered for property: '{0}'.", property ) );
                }
                else
                {
                    output = AssignLanguageMap( inputMap, property, ref messages );
                }
            }
            else
            {
				if ( minimumLength > 0 && input.Length > 0 && input.Length < minimumLength )
				{
					messages.Add( string.Format( "The property: {0} must be a minimum length of {1} characters.", property, minimumLength ) );
				}
				output = Assign( input, DefaultLanguageForMaps );
            }

            return output;
        }

		public MJ.LanguageMap AssignLanguageMap( List<LanguageItem> input, string property, string language, string parentCtid, ref List<string> messages, bool isRequired = false )
		{

			MJ.LanguageMap output = new MJ.LanguageMap();
			if ( input == null )
			{
				if ( isRequired )
					messages.Add( FormatMessage( "A value must be entered for property: '{0}' with CTID: '{0}'.", property, parentCtid ) );
				return null;
			}
			int cntr = 0;
			foreach ( var item in input )
			{
				cntr++;
				//some validation
				string lang = item.Language.Trim();
				if ( ValidateLanguageCode( property, cntr, true, ref lang, ref messages ) )
					output.Add( lang.ToLower(), item.Value );
			}
			if ( output.Count == 0 )
				return null;
			else
				return output;
		}

		/// <summary>
		/// Format list of strings as a language map list with single entry with default language of 'en'
		/// </summary>
		/// <param name="list"></param>
		/// <param name="property"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public MJ.LanguageMapList FormatLanguageMapList( List<string> list, string property, ref List<string> messages )
        {

            if ( list == null || list.Count == 0 )
                return null;

            MJ.LanguageMapList output = new MJ.LanguageMapList();
            output.Add( DefaultLanguageForMaps, list );
            return output;
        }

        /// <summary>
        /// Format list of language maps, like keywords
        /// "ceterms:keyword": {
		///     "en": [ "Keyword one", "Keyword two" ],
		///     "ru": [ "Ключевое слово", "Ключевое слово два" ]
        ///     }
        /// </summary>
        /// <param name="list"></param>
        /// <param name="property"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public MJ.LanguageMapList AssignLanguageMapList( MI.LanguageMapList list, string property, ref List<string> messages )
        {

            if ( list == null || list.Count == 0 )
                return null;

            MJ.LanguageMapList output = new MJ.LanguageMapList();
            int cntr = 0;
            foreach ( var item in list )
            {
                cntr++;
                //some validation of lang code and region
                string lang = item.Key;
                if ( ValidateLanguageCode( property, cntr, true, ref lang, ref messages ) )
                    output.Add( lang.ToLower(), item.Value );
            }
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

		/// <summary>
		/// Assign a LanguageMapList from either a list of strings, or a LanguageMapList
		/// </summary>
		/// <param name="list"></param>
		/// <param name="mapList"></param>
		/// <param name="property"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
        public MJ.LanguageMapList AssignLanguageMapList( List<string> list, MI.LanguageMapList mapList, string property, ref List<string> messages )
        {
            return AssignLanguageMapList( list, mapList, property, DefaultLanguageForMaps, ref messages );
        }

		/// <summary>
		/// Assign a LanguageMapList from either a list of strings or a LanguageMapList
		/// </summary>
		/// <param name="list"></param>
		/// <param name="mapList"></param>
		/// <param name="property"></param>
		/// <param name="language"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
        public MJ.LanguageMapList AssignLanguageMapList( List<string> list, MI.LanguageMapList mapList, string property, string language, ref List<string> messages )
        {

            MJ.LanguageMapList output = new MJ.LanguageMapList();
            if ( list == null || list.Count == 0 )
            {
                if ( mapList == null || mapList.Count == 0 )
                    return null;
                else
                {
                    int cntr = 0;
                    foreach ( var item in mapList )
                    {
                        cntr++;
                        //some validation of lang code and region
                        string lang = item.Key;
                        if ( ValidateLanguageCode( property, cntr, true, ref lang, ref messages ) )
                            output.Add( lang.ToLower(), item.Value );
                    }
                }
            } else
            {
                //need to check for and remove empty strings
                List<string> input = new List<string>();
                foreach (var item in list)
                {
                    if ( !string.IsNullOrWhiteSpace( item ) )
                        input.Add( item );
                }
                if (input.Count > 0)
                    output.Add( DefaultLanguageForMaps, input );
            }
                
            if ( output.Count == 0 )
                return null;
            else
                return output;
        }

		/// <summary>
		/// Assign a language map list from a list of language items
		/// </summary>
		/// <param name="list"></param>
		/// <param name="property"></param>
		/// <param name="language"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public MJ.LanguageMapList AssignLanguageMapList( List<LanguageItem> list, string property, string language, ref List<string> messages )
		{
			if ( list == null || list.Count == 0 )
				return null;

			MJ.LanguageMapList output = new MJ.LanguageMapList();
			int cntr = 0;
			foreach ( var item in list )
			{
				cntr++;
				if ( item == null )
					continue;
				//some validation of lang code and region
				string lang = item.Language ?? "".Trim();
				List<string> values = new List<string>() { item.Value ?? "".Trim() };
				if ( ValidateLanguageCode( property, cntr, true, ref lang, ref messages ) )
					output.Add( lang.ToLower(), values );
			}
			if ( output.Count == 0 )
				return null;
			else
				return output;
		}

		public bool HasData(MJ.LanguageMap input)
		{
			if ( input != null && input.Count > 0 &&  input.ToString().Length > 0 )
				return true;
			else
				return false;
		}

		public List<string> PopulateInLanguage( List<string> input, string entityType, string entityName, bool hasDefaultLanguage, ref List<string> messages)
        {
            List<string> output = new List<string>();
            if ( input != null && input.Count > 0 )
            {
                int cntr = 0;
                foreach ( var item in input )
                {
                    if ( string.IsNullOrWhiteSpace( item ) )
                        continue;

                    cntr++;
                    string lang = item;
                    if ( ValidateLanguageCode( "InLanguage", cntr, false, ref lang, ref messages ) )
                    {
                        output.Add( lang );
                        if ( cntr == 1 && !hasDefaultLanguage )
                        {
                            DefaultLanguageForMaps = lang;
                            hasDefaultLanguage = true;
                        }
                    }
                }
                //output.InLanguage = input.InLanguage;
                if ( !hasDefaultLanguage )
                {
                    //should not happen unless first item in list had error, or was empty
                    DefaultLanguageForMaps = SystemDefaultLanguageForMaps;
                    if ( output.Count() == 0 )
                        output.Add( DefaultLanguageForMaps );
                }
            }
            else
            {
                //make configurable. Default to english if not found
                if ( UtilityManager.GetAppKeyValue( "ra.RequiringLanguage", false ) && !hasDefaultLanguage )
                {
                    messages.Add( string.Format( "At least one language (InLanguage) must be provided for {0}: '{1}'", entityType, entityName ) );
                }
                else
                {
                    output.Add( DefaultLanguageForMaps );
                }
            }
            return output;
        }
		public List<string> PopulateInLanguage( string input, string entityType, string entityName, bool hasDefaultLanguage, ref List<string> messages )
		{
			List<string> output = new List<string>();
			if ( string.IsNullOrWhiteSpace( input ) )
			{
				//make configurable. Default to english if not found
				if ( UtilityManager.GetAppKeyValue( "ra.RequiringLanguage", false ) && !hasDefaultLanguage )
				{
					messages.Add( string.Format( "At least one language (InLanguage) must be provided for {0}: '{1}'", entityType, entityName ) );
				}
				else
				{
					if (!string.IsNullOrWhiteSpace( DefaultLanguageForMaps ) )
						output.Add( DefaultLanguageForMaps );
				}
				return output;
			}

			string lang = input.Trim();
			if ( ValidateLanguageCode( "InLanguage", 1, false, ref lang, ref messages ) )
			{
				output.Add( lang );
				if ( !hasDefaultLanguage )
				{
					DefaultLanguageForMaps = lang;
					hasDefaultLanguage = true;
				}
			}

			//output.InLanguage = input.InLanguage;
			if ( !hasDefaultLanguage )
				{
					//should not happen unless first item in list had error, or was empty
					DefaultLanguageForMaps = SystemDefaultLanguageForMaps;
					if ( output.Count() == 0 )
						output.Add( DefaultLanguageForMaps );
				}
		
			return output;
		}
		public static bool ValidateLanguageCode(string languageCode, string property, ref List<string> messages )
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                messages.Add( string.Format("A valid language code was not found for property: {0}", property ));
                return false;
            }
            languageCode = languageCode.Trim();
            if ( languageCode.ToLower() == "english" )
                languageCode = "en-US";
            else if ( languageCode.ToLower() == "spanish" )
                languageCode = "es";
            int pos = languageCode.IndexOf( "-" );
            if ( languageCode.Length < 2 || (pos > -1 && pos < 2 ))
            {
                messages.Add( string.Format( "A valid language code must be at least two characters in length (eg. 'en' or 'en-us' Property: {0}, Language Code: {1}", property, languageCode ) );
                return false;
            }

            //quick
            string code = languageCode.ToLower().Substring( 0, 2 );
            if ( LanguageCodes_2Characters.IndexOf( code ) > -1  )
                return true;
            if ( pos > 2 )
            {
                code = languageCode.ToLower().Substring( 0, 3 );
                if ( LanguageCodes_3Characters.IndexOf( code ) > -1 )
                    return true;
            }
            //at this point don't have a common language code.
            messages.Add( string.Format( "An unknown/unhandled language code was encountered: Property: {0},  Language Code: {1}", property, languageCode ) );
            isValid = false;

            return isValid;
        }

        public static bool ValidateLanguageCode( string property, int row, bool langIsExpected, ref string languageCode, ref List<string> messages )
        {
            bool isValid = true;
            if ( string.IsNullOrWhiteSpace( languageCode ) )
            {
                if ( langIsExpected )
                {
                    messages.Add( string.Format( "A valid language code was not found for property: {0}, row: {1}", property, row ) );
                    return false;
                }
                else
                    return true;
            }
            languageCode = languageCode.Trim();
            if ( languageCode.ToLower() == "english" )
                languageCode = "en-US";
            else if ( languageCode.ToLower() == "spanish" )
                languageCode = "es";

            int pos = languageCode.IndexOf( "-" );
            if ( languageCode.Length < 2 || ( pos > -1 && pos < 2 ) )
            {
                messages.Add( string.Format( "A valid language code must be at least two characters in length (eg. 'en' or 'en-us' Property: {0}, Row: {1}, Language Code: {2}", property, row, languageCode ) );
                return false;
            }

            //quick
            string code = languageCode.ToLower().Substring( 0, 2 );
            if ( LanguageCodes_2Characters.IndexOf( code ) > -1 )
                return true;
            if ( pos > 2 )
            {
                code = languageCode.ToLower().Substring( 0, 3 );
                if ( LanguageCodes_3Characters.IndexOf( code ) > -1 )
                    return true;
            }
            //at this point don't have a common language code.
            messages.Add( string.Format( "An unknown/unhandled language code was encountered: Property: {0}, Row: {1}, Language Code: {2}", property, row, languageCode ) );
            isValid = false;

            return isValid;
        }

        public MJ.LanguageMap ConvertLanguageMap( MI.LanguageMap map )
        {
            MJ.LanguageMap output = new MJ.LanguageMap();
            if ( map == null )
                return null;

            foreach ( var item in map )
            {
                output.Add( item.Key, item.Value );
            }

            return output;
        }
        //raw convert, should use FormatLanguageMapList
        public MJ.LanguageMapList ConvertLanguageMapList( MI.LanguageMapList map )
        {
            MJ.LanguageMapList output = new MJ.LanguageMapList();
            if ( map == null || map.Count == 0 )
                return null;

            foreach ( var item in map )
            {
                output.Add( item.Key, item.Value );
            }

            return output;
        }
        public string GetFirstItemValue( MI.LanguageMap map )
        {
            if ( map == null || map.Count == 0 )
                return "";
            string output = "";
            foreach ( var item in map )
            {
                output = item.Value;
                break;
            }

            return output;
        }
        public string GetFirstItemValue( MJ.LanguageMap map )
        {
            if ( map == null || map.Count == 0 )
                return "";
            string output = "";
            foreach ( var item in map )
            {
                output = item.Value;
                break;
            }

            return output;
        }
        #endregion


        #region Helpers and validaton

        public bool IsUrlValid( string url, ref string statusMessage, ref bool urlPresent, bool doingExistanceCheck = true )
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
			//hack for pattern like https://https://www.sscc.edu
			if ( url.LastIndexOf( "//" ) > url.IndexOf( "//" ) )
			{
				statusMessage = "Invalid format, contains multiple sets of '//'";
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
        public bool DoesRemoteFileExists( string url, ref string responseStatus )
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
				//testing
				request.AllowAutoRedirect = true;
				request.Timeout = 10000;  //10 seconds
				request.KeepAlive = false;
				request.Accept = "*/*";

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
						var urlStatusCode = ( int )response.StatusCode;
						if ( urlStatusCode == 301 )
						{
							//for ( int i = 0; i < response.Headers.Count; ++i )
							//	Console.WriteLine( "\nHeader Name:{0}, Value :{1}", response.Headers.Keys[ i ], response.Headers[ i ] );
							string location = response.Headers.GetValues( "Location" ).FirstOrDefault();
							if ( !string.IsNullOrWhiteSpace( location ) )
							{
								string clearUrl = url.Replace( "http://", "" ).Replace( "https://", "" ).Trim( '/' );
								string clearLoc = location.Replace( "http://", "" ).Replace( "https://", "" ).Trim( '/' );
								//L: http://www.tesu.edu/about/mission
								//U: http://www.tesu.edu/about/mission.cfm
								if ( location.Replace( "https", "http" ).Trim( '/' ) == url.Trim( '/' )
									|| location.ToLower().Trim( '/' ) == url.ToLower().Trim( '/' )
									|| url.ToLower().IndexOf( location.ToLower() ) == 0 //redirect just trims an extension
									|| clearLoc.ToLower().IndexOf( clearUrl.ToLower() ) > 0
									)
								{
									return true;
								}
								else if ( location.Replace( "mobile.twitter", "twitter" ).ToLower().Trim( '/' ) == url.ToLower().Trim( '/' )
									|| location == "https://www.linkedin.com/error_pages/unsupported-browser.html"
									)
								{
									//Redirect to: https://www.linkedin.com/error_pages/unsupported-browser.html
									return true;
								}

							}

						}
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
                    LoggingHelper.LogError( string.Format( thisClassName + ".DoesRemoteFileExists url: {0}. Exception Message:{1}; URL: {2}", url, wex.Message, GetWebUrl() ), true, "SKIPPING - Exception on URL Checking" );

                    return true;
                }

                LoggingHelper.LogError( string.Format( thisClassName + ".DoesRemoteFileExists url: {0}. Exception Message:{1}", url, wex.Message ), true, "Exception on URL Checking" );
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
                    LoggingHelper.LogError( string.Format( thisClassName + ".DoesRemoteFileExists url: {0}. Exception Message:{1}", url, ex.Message ), true, "SKIPPING - Exception on URL Checking" );

                    return true;
                }

                LoggingHelper.LogError( string.Format( thisClassName + ".DoesRemoteFileExists url: {0}. Exception Message:{1}", url, ex.Message ), true, "Exception on URL Checking" );
                //Any exception will returns false.
                responseStatus = ex.Message;
                return false;
            }
        }
		public bool AcceptAllCertifications( object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors )
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


        public int StringToInt( string value, int defaultValue )
        {
            int returnValue = defaultValue;
            if ( Int32.TryParse( value, out returnValue ) == true )
                return returnValue;
            else
                return defaultValue;
        }


        public bool StringToDate( string value, ref DateTime returnValue )
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
        public bool IsInteger( string stringToTest )
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
        public bool IsDate( string stringToTest, bool doingReasonableCheck = true )
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

        public string MapDate( string date, string dateName, ref List<string> messages, bool doingReasonableCheck = true )
        {
            if ( string.IsNullOrWhiteSpace( date ) )
                return null;

            DateTime newDate = new DateTime();

            if ( DateTime.TryParse( date, out newDate ) )
            {
                if ( doingReasonableCheck && newDate < new DateTime( 1800, 1, 1 ) )
                    messages.Add( string.Format( "Error - {0} is out of range (prior to 1800-01-01) ", dateName ) );
				else
				{
					//check if input was just two parts
					var parts = date.Trim().Split(' ' );
					if (parts.Count() == 2)
					{
						//just return input
						return date;
					} else
					{
						var parts2 = date.Trim().Split( '-' );
						if ( parts2.Count() == 2 )
						{
							//just return input
							return date;
						}
					}
				}
            }
            else
            {
                messages.Add( string.Format( "Error - {0} is invalid ", dateName ) );
                return null;
            }
            return newDate.ToString( "yyyy-MM-dd" );

        } //end

		public string MapDateTime(string date, string dateName, ref List<string> messages, bool doingReasonableCheck = true)
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
			return newDate.ToString( "yyyy-MM-dd hh:mm:ss" );

		} //end
		  /// <summary>
		  /// Only check for valid year
		  /// </summary>
		  /// <param name="date"></param>
		  /// <param name="dateName"></param>
		  /// <param name="messages"></param>
		  /// <param name="doingReasonableCheck"></param>
		  /// <returns></returns>
		public string MapYear( string date, string dateName, ref List<string> messages, bool doingReasonableCheck = true )
		{
			if ( string.IsNullOrWhiteSpace( date ) )
				return null;

			var year = 0;
			if ( Int32.TryParse( date, out year ) )
			{
				if ( year < 1800 || year > DateTime.Now.Year )
				{
					messages.Add( string.Format( "Error - {0} is out of range (prior to 1800 or greater than the current year) ", dateName ) );
					return null;
				}
			}
			else
			{
				messages.Add( string.Format( "Error - {0} is not a valid year.", dateName ) );
				return null;
			}
			return date;
		} //end

		public string MapIsicV4(string field)
		{
			if ( string.IsNullOrWhiteSpace( field ) )
				return null;
			/*
			 * The ISIC is subdivided in a hierarchical, four-level structure. The categories at the highest level are called sections. The two-digit of the code identify the division, the third digit identifies the group and the fourth digit identifies the class.
			Example
			Section	C	Manufacturing
			Division	13	Manufacture of Textiles
			Group		139	Manufacture of Other Textiles
			Class		1393 Manufacture of Carpets and Rugs

			It seems that sometimes C1393 would be enough???
			Does that mean allowing C13, C139
			 */

			return field.Trim();
		} //end

		public bool IsValidGuid( Guid field )
        {
            if ( ( field == null || field == Guid.Empty ) )
                return false;
            else
                return true;
        }
        public bool IsValidGuid( string field )
        {
            Guid guidOutput;
            if ( ( field == null || field.ToString() == Guid.Empty.ToString() ) )
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
        public bool HasErrorMessage( DataSet wsDataset )
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
        public List<int> CommaSeparatedListToIntegerList( string csl )
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
        public List<string> CommaSeparatedListToStringList( string csl )
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

		public void AddUnHandledWarning(List<string> list, string property)
		{
			if ( list != null && list.Count() > 0 )
			{
				warningMessages.Add( "Warning: " + property + " is not handled yet." );
			}

		}

		#region JSON helpers
		//public static string LogInputFile( CredentialRequest request, string endpoint, int appLevel = 6 )
		//{
		//    string jsoninput = JsonConvert.SerializeObject( request, GetJsonSettings() );
		//    LoggingHelper.WriteLogFile( appLevel, string.Format("Credential_{0}_{1}_raInput.json", endpoint, request.Credential.Ctid), jsoninput, "", false );
		//    return jsoninput;
		//}
		public static string LogInputFile( object request, string ctid, string entityType, string endpoint, int appLevel = 6 )
        {
            string jsoninput = JsonConvert.SerializeObject( request, GetJsonSettings() );
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
				LoggingHelper.LogError( string.Format( "@@@@ Error: Empty string AppKey was encoutered, using default of: {0}", defaultValue ) );
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
				LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
		public static int GetAppKeyValue( string keyName, int defaultValue )
        {
            int appValue = -1;
            if ( string.IsNullOrWhiteSpace( keyName ) )
            {
				LoggingHelper.LogError( string.Format( "@@@@ Error: Empty int AppKey was encoutered, using default of: {0}", defaultValue ) );
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
				LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
		public static bool GetAppKeyValue( string keyName, bool defaultValue )
        {
            bool appValue = false;
            if ( string.IsNullOrWhiteSpace( keyName ) )
            {
				LoggingHelper.LogError( string.Format( "@@@@ Error: Empty bool AppKey was encoutered, using default of: {0}",  defaultValue ) );
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
				LoggingHelper.LogError( string.Format( "@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue ) );
            }

            return appValue;
        } //
		#endregion

		#region Error Logging - OBSOLETE HANDLED BY COMMON PROJECT ==========
		/// <summary>
		/// Format an exception and message, and then log it
		/// </summary>
		/// <param name="ex">Exception</param>
		/// <param name="message">Additional message regarding the exception</param>
		//public static void LogError( Exception ex, string message, string subject = "Registry Assistant Application Exception encountered" )
  //      {
  //          bool notifyAdmin = false;
  //          LogError( ex, message, notifyAdmin, subject );
  //      }

		/// <summary>
		/// Format an exception and message, and then log it
		/// </summary>
		/// <param name="ex">Exception</param>
		/// <param name="message">Additional message regarding the exception</param>
		/// <param name="notifyAdmin">If true, an email will be sent to admin</param>
		//public static void LogError( Exception ex, string message, bool notifyAdmin, string subject = "Registry Assistant Application Exception encountered" )
  //      {

  //          string sessionId = "unknown";
  //          string remoteIP = "unknown";
  //          string path = "unknown";
  //          //string queryString = "unknown";
  //          string url = "unknown";
  //          string parmsString = "";

  //          try
  //          {
  //              if ( UtilityManager.GetAppKeyValue( "notifyOnException", "no" ).ToLower() == "yes" )
  //                  notifyAdmin = true;

  //              string serverName = GetAppKeyValue( "serverName", "unknown" );

  //          }
  //          catch
  //          {
  //              //eat any additional exception
  //          }

  //          try
  //          {
  //              string errMsg = message +
  //                  "\r\nType: " + ex.GetType().ToString() + ";" +
  //                  "\r\nSession Id - " + sessionId + "____IP - " + remoteIP +
  //                  "\r\nException: " + ex.Message.ToString() + ";" +
  //                  "\r\nStack Trace: " + ex.StackTrace.ToString() +
  //                  "\r\nServer\\Template: " + path +
  //                  "\r\nUrl: " + url;

  //              if ( parmsString.Length > 0 )
  //                  errMsg += "\r\nParameters: " + parmsString;

  //              LogError( errMsg, notifyAdmin );
  //          }
  //          catch
  //          {
  //              //eat any additional exception
  //          }

  //      } //


		/// <summary>
		/// Write the message to the log file.
		/// </summary>
		/// <remarks>
		/// The message will be appended to the log file only if the flag "logErrors" (AppSetting) equals yes.
		/// The log file is configured in the web.config, appSetting: "error.log.path"
		/// </remarks>
		/// <param name="message">Message to be logged.</param>
		//public static void LogError( string message, string subject = "Registry Assistant Application Exception encountered" )
  //      {

  //          if ( GetAppKeyValue( "notifyOnException", "no" ).ToLower() == "yes" )
  //          {
  //              LogError( message, true, subject );
  //          }
  //          else
  //          {
  //              LogError( message, false, subject );
  //          }

  //      } //
		  /// <summary>
		  /// Write the message to the log file.
		  /// </summary>
		  /// <remarks>
		  /// The message will be appended to the log file only if the flag "logErrors" (AppSetting) equals yes.
		  /// The log file is configured in the web.config, appSetting: "error.log.path"
		  /// </remarks>
		  /// <param name="message">Message to be logged.</param>
		  /// <param name="notifyAdmin"></param>
		//public static void LogError( string message, bool notifyAdmin, string subject = "Registry Assistant Application Exception encountered" )
  //      {
  //          if ( GetAppKeyValue( "logErrors" ).ToString().Equals( "yes" ) )
  //          {
  //              try
  //              {
		//			string datePrefix1 = System.DateTime.Today.ToString( "u" ).Substring( 0, 10 );
		//			string datePrefix = System.DateTime.Today.ToString( "yyyy-dd" );
		//			string logFile = UtilityManager.GetAppKeyValue( "path.error.log", "" );
		//			if ( !string.IsNullOrWhiteSpace( logFile ) )
		//			{
		//				string outputFile = logFile.Replace( "[date]", datePrefix );

		//				if ( File.Exists( outputFile ) )
		//				{
		//					if ( File.GetLastWriteTime( outputFile ).Month != DateTime.Now.Month )
		//						File.Delete( outputFile );
		//				}
		//				else
		//				{
		//					System.IO.FileInfo f = new System.IO.FileInfo( outputFile );
		//					f.Directory.Create(); // If the directory already exists, this method does nothing.
		//										  //just incase, create folders
		//										  //FileSystemHelper.CreateDirectory( outputFile );
		//				}

		//				StreamWriter file = File.AppendText( outputFile );
		//				file.WriteLine( DateTime.Now + ": " + message );
		//				file.WriteLine( "---------------------------------------------------------------------" );
		//				file.Close();

		//				if ( notifyAdmin )
		//				{
		//					if ( ShouldMessagesBeSkipped( message ) == false )
		//						EmailManager.NotifyAdmin( subject, message );
		//				}
		//			}
		//		}
  //              catch ( Exception ex )
  //              {
  //                  //eat any additional exception
  //                  DoTrace( 5, thisClassName + ".LogError(string message, bool notifyAdmin). Exception: " + ex.Message );
  //              }
  //          }
  //      } //

		public void NotifyOnPublish( string type, string message )
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
        public bool NotifyAdmin( string subject, string message )
        {
            string emailTo = UtilityManager.GetAppKeyValue( "systemAdminEmail", "cwd-mparsons@siu.edu" );
            //work on implementing some specific routing based on error type


            return EmailManager.NotifyAdmin( emailTo, subject, message );
        }


		/// <summary>
		/// Handle trace requests - typically during development, but may be turned on to track code flow in production.
		/// </summary>
		/// <param name="message">Trace message</param>
		/// <remarks>This is a helper method that defaults to a trace level of 10</remarks>
		//public void DoTrace( string message )
		//{
		//    //default level to 8
		//    //should get app key value
		//    int appTraceLevel = UtilityManager.GetAppKeyValue( "appTraceLevel", 8 );
		//    if ( appTraceLevel < 8 )
		//        appTraceLevel = 8;
		//    DoTrace( appTraceLevel, message, true );
		//}

		/// <summary>
		/// Handle trace requests - typically during development, but may be turned on to track code flow in production.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		//public static void DoTrace( int level, string message )
		//      {
		//          DoTrace( level, message, true );
		//      }

		/// <summary>
		/// Handle trace requests - typically during development, but may be turned on to track code flow in production.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="showingDatetime">If true, precede message with current date-time, otherwise just the message> The latter is useful for data dumps</param>
		//public static void DoTrace( int level, string message, bool showingDatetime )
		//      {
		//          //TODO: Future provide finer control at the control level
		//          string msg = "";
		//          int appTraceLevel = 0;
		//          //bool useBriefFormat = true;

		//          try
		//          {
		//              appTraceLevel = GetAppKeyValue( "appTraceLevel", 1 );

		//              //Allow if the requested level is <= the application thresh hold
		//              if ( level <= appTraceLevel )
		//              {
		//                  if ( showingDatetime )
		//                      msg = "\n " + System.DateTime.Now.ToString() + " - " + message;
		//                  else
		//                      msg = "\n " + message;


		//                  string datePrefix = System.DateTime.Today.ToString( "u" ).Substring( 0, 10 );
		//                  string logFile = GetAppKeyValue( "path.trace.log", "C:\\VOS_LOGS.txt" );
		//                  string outputFile = logFile.Replace( "[date]", datePrefix );

		//                  StreamWriter file = File.AppendText( outputFile );

		//                  file.WriteLine( msg );
		//                  file.Close();

		//              }
		//          }
		//          catch
		//          {
		//              //ignore errors
		//          }

		//      }

		#endregion

		#region Common Utility Methods
		/// <summary>
		/// Convert characters often resulting from external programs like Word
		/// NOTE: keep in sync with the Publisher version in BaseFactory
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ConvertSpecialCharacters( string text )
		{
			//, ref bool hasChanged
			//hasChanged = false;
			if ( string.IsNullOrWhiteSpace( text ) )
				return "";
			string orginal = text.Trim();
			if ( ContainsUnicodeCharacter( text ) )
			{

				if ( text.IndexOf( '\u2013' ) > -1 )
					text = text.Replace( '\u2013', '-' ); // en dash
				if ( text.IndexOf( '\u2014' ) > -1 )
					text = text.Replace( '\u2014', '-' ); // em dash
				if ( text.IndexOf( '\u2015' ) > -1 )
					text = text.Replace( '\u2015', '-' ); // horizontal bar
				if ( text.IndexOf( '\u2017' ) > -1 )
					text = text.Replace( '\u2017', '_' ); // double low line
				if ( text.IndexOf( '\u2018' ) > -1 )
					text = text.Replace( '\u2018', '\'' ); // left single quotation mark
				if ( text.IndexOf( '\u2019' ) > -1 )
					text = text.Replace( '\u2019', '\'' ); // right single quotation mark
				if ( text.IndexOf( '\u201a' ) > -1 )
					text = text.Replace( '\u201a', ',' ); // single low-9 quotation mark
				if ( text.IndexOf( '\u201b' ) > -1 )
					text = text.Replace( '\u201b', '\'' ); // single high-reversed-9 quotation mark
				if ( text.IndexOf( '\u201c' ) > -1 )
					text = text.Replace( '\u201c', '\"' ); // left double quotation mark
				if ( text.IndexOf( '\u201d' ) > -1 )
					text = text.Replace( '\u201d', '\"' ); // right double quotation mark
				if ( text.IndexOf( '\u201e' ) > -1 )
					text = text.Replace( '\u201e', '\"' ); // double low-9 quotation mark
				if ( text.IndexOf( '\u201f' ) > -1 )
					text = text.Replace( '\u201f', '\"' ); // ???
				if ( text.IndexOf( '\u2026' ) > -1 )
					text = text.Replace( "\u2026", "..." ); // horizontal ellipsis
				if ( text.IndexOf( '\u2032' ) > -1 )
					text = text.Replace( '\u2032', '\'' ); // prime
				if ( text.IndexOf( '\u2033' ) > -1 )
					text = text.Replace( '\u2033', '\"' ); // double prime
				if ( text.IndexOf( '\u2036' ) > -1 )
					text = text.Replace( '\u2036', '\"' ); // ??
				if ( text.IndexOf( '\u0090' ) > -1 )
					text = text.Replace( '\u0090', 'ê' ); // e circumflex
			}
			text = text.Replace( "â€™", "'" );
			text = text.Replace( "â€\"", "-" );
			text = text.Replace( "\"â€ú", "-" );
			text = text.Replace( "â€¢", "-" );
			text = text.Replace( "Ã¢â‚¬Â¢", "-" );
			text = text.Replace( "ÃƒÂ¢Ã¢,Â¬Ã¢\"Â¢s", "'s" );
			text = text.Replace( "Ãƒ,Ã,Â ", " " );

			//
			//
			//don't do this as \r is valid
			//text = text.Replace( "\\\\r", "" );

			text = text.Replace( "\u009d", " " ); //
			text = text.Replace( "Ã,Â", "" ); //
			text = text.Replace( ".Â", " " ); //

			text = Regex.Replace( text, "’", "'" );
			text = Regex.Replace( text, "“", "'" );
			text = Regex.Replace( text, "”", "'" );
			//BIZARRE
			text = Regex.Replace( text, "Ã¢â,¬â\"¢", "'" );
			text = Regex.Replace( text, "–", "-" );

			text = Regex.Replace( text, "[Õ]", "'" );
			text = Regex.Replace( text, "[Ô]", "'" );
			text = Regex.Replace( text, "[Ò]", "\"" );
			text = Regex.Replace( text, "[Ó]", "\"" );
			text = Regex.Replace( text, "[Ñ]", " -" ); //Ñ
			text = Regex.Replace( text, "[Ž]", "é" );
			text = Regex.Replace( text, "[ˆ]", "à" );
			text = Regex.Replace( text, "[Ð]", "-" );
			//
			text = text.Replace( "‡", "á" ); //Ã³

			text = text.Replace( "ÃƒÂ³", "ó" ); //
			text = text.Replace( "Ã³", "ó" ); //
											  //é
			text = text.Replace( "ÃƒÂ©", "é" ); //
			text = text.Replace( "Ã©", "é" ); //

			text = text.Replace( "ÃƒÂ¡", "á" ); //
			text = text.Replace( "Ã¡", "á" ); //Ã¡
			text = text.Replace( "ÃƒÂ", "à" ); //
											   //
			text = text.Replace( "ÃƒÂ±", "ñ" ); //
			text = text.Replace( "Â±", "ñ" ); //"Ã±"
											  //
			text = text.Replace( "ÃƒÂ-", "í" ); //???? same as à
			text = text.Replace( "ÃƒÂ­­", "í" ); //"Ã­as" "gÃ­a" "gÃ­as"
			text = text.Replace( "gÃ­as", "gías" ); //"Ã­as" "gÃ­a" "gÃ­as"
			text = text.Replace( "’", "í" ); //


			text = text.Replace( "ÃƒÂº", "ú" ); //"Ãº"
			text = text.Replace( "Âº", "ú" ); //"Ãº"
			text = text.Replace( "œ", "ú" ); //

			text = text.Replace( "quÕˆ", "qu'à" ); //
			text = text.Replace( "qu'ˆ", "qu'à" ); //
			text = text.Replace( "ci—n ", "ción " );
			//"Â¨"
			text = text.Replace( "Â¨", "®" ); //

			text = text.Replace( "teor'as", "teorías" ); // 
			text = text.Replace( "log'as", "logías" ); //
			text = text.Replace( "ense–anza", "enseñanza" ); //
															 //
			text = text.Replace( "Ã¢â,¬Ãº", "\"" ); //
			text = text.Replace( "Ã¢â,¬Â", "\"" ); //
												   //

			//not sure if should do this arbitrarily here?
			if ( text.IndexOf( "Ã" ) > -1 || text.IndexOf( "Â" ) > -1 )
			{
				//string queryString = GetWebUrl();
				//LoggingHelper.DoTrace( 1, string.Format("@#@#@# found text containing Ã or Â, setting to blank. URL: {0}, Text:\r{1}", queryString, text ) );
				text = text.Replace( "Ã", "" ); //
				text = text.Replace( ",Â", "," ); //
				text = text.Replace( "Â", "" ); //

			}


			text = text.Replace( "ou�ll", "ou'll" ); //
			text = text.Replace( "�s", "'s" ); // 
			text = text.Replace( "�", "" ); // 
			text = Regex.Replace( text, "[—]", "-" ); //

			text = Regex.Replace( text, "[�]", " " ); //could be anything
													  //covered above

			//text = Regex.Replace(text, "[«»\u201C\u201D\u201E\u201F\u2033\u2036]", "\"");
			//text = Regex.Replace(text, "[\u2026]", "...");

			//

			if ( orginal != text.Trim() )
			{
				//should report any changes
				//hasChanged = true;
				//text = orginal;
			}
			return text.Trim();
		} //
		public static bool ContainsUnicodeCharacter( string input )
		{
			const int MaxAnsiCode = 255;

			return input.Any( c => c > MaxAnsiCode );
		}
		public string HandleApostrophes( string strValue )
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
        public String CleanText( string text )
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

		public String FormatMessage( string message, string parm1 )
		{
			string msg = CurrentEntityType; //+ " " + 
			if ( !string.IsNullOrWhiteSpace( CurrentEntityName ) )
				msg += " - " + CurrentEntityName;

			if ( message.IndexOf( "{0}" ) > -1 )
				return string.Format( message, parm1 );
			else
				return message;
		}
		public string FormatMessage( string message, string parm1, string parm2 )
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