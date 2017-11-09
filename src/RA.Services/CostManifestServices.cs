using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using RA.Models.Json;

using EntityRequest = RA.Models.Input.CostManifestRequest;
using InputEntity = RA.Models.Input.CostManifest;
using OutputEntity = RA.Models.Json.CostManifest;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
    public class CostManifestServices : ServiceHelper
    {
        static string status = "";
        static bool isUrlPresent = true;

        /// <summary>
        /// Publish an Cost Manifest to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        public static void Publish( EntityRequest request, ref bool isValid, ref List<string> messages, ref string payload, ref string registryEnvelopeId )
        {
            isValid = true;
			registryEnvelopeId = "";
			string submitter = "";

            var output = new OutputEntity();
            if ( ToMap( request.CostManifest, output, ref messages ) )
            {
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

                CER cer = new CER();
                string identifier = "CostManifest_" + request.CostManifest.Ctid;

                if ( cer.Publish( payload, submitter, identifier, ref status, ref registryEnvelopeId ) )
                {
					string msg = string.Format( "<p>Published CostManifest: {0}</p><p>CostDetails  webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.CostDetails, output.Ctid, registryEnvelopeId );
					NotifyOnPublish( "CostManifest", msg );
				}
                else
                {
                    messages.Add( status );
                    isValid = false;
                }
            }
            else
            {
                isValid = false;
            }
        }
        //
        //Used for demo page - NA 6/5/2017
        public static string DemoPublish( OutputEntity ctdlFormattedEntity, ref bool isValid, ref List<string> messages, ref string rawResponse, bool forceSkipValidation = false )
        {
            isValid = true;
            var crEnvelopeId = "";
            var payload = JsonConvert.SerializeObject( ctdlFormattedEntity, ServiceHelper.GetJsonSettings() );
            var identifier = "CostManifest_" + ctdlFormattedEntity.Ctid;
            rawResponse = new CER().Publish( payload, "", identifier, ref isValid, ref status, ref crEnvelopeId, forceSkipValidation );

            return crEnvelopeId;
        }
        //

        public static string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
        {
            return FormatAsJson( request.CostManifest, ref isValid, ref messages );
        }
        //
        public static string FormatAsJson( InputEntity input, ref bool isValid, ref List<string> messages )
        {
            var output = new OutputEntity();
            string payload = "";
            isValid = true;

            RA.Models.RequestStatus reqStatus = new Models.RequestStatus();
            reqStatus.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );

            if ( ToMap( input, output, ref messages ) )
            {
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }
            else
            {
                isValid = false;
                //do payload anyway
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }

            return payload;
        }

        /// <summary>
        /// Map and validate:
        /// - trim all strings
        /// - if empty/default values, set to null
        /// - use codeManager to look up codes
        /// - each code should be checked for proper prefix - supply if missing
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="messages"></param>
        /// <returns></returns>        
        public static bool ToMap( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
			try
			{
				HandleRequiredFields( input, output, ref messages );

				HandleLiteralFields( input, output, ref messages );

				//TBD - are estimated costs required?
				if ( input.EstimatedCosts == null || input.EstimatedCosts.Count == 0 )
				{
					messages.Add( "Error - An Cost Manifest must have at least one cost profile." );
				}
				else
				{
					output.EstimatedCost = FormatCosts( input.EstimatedCosts, ref messages );
				}
			}
			catch ( Exception ex )
			{
				LogError( ex, "CostManifestServices.ToMap" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
                isValid = false;

            return isValid;
        }

        public static bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
            string statusMessage = "";

            //todo determine if will generate where not found
            if ( string.IsNullOrWhiteSpace( input.Ctid ) && GeneratingCtidIfNotFound() )
                input.Ctid = GenerateCtid();

            if ( IsCtidValid( input.Ctid, ref messages ) )
            {
                output.Ctid = input.Ctid;
                output.CtdlId = idUrl + output.Ctid;
            }
            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                messages.Add( "Error - An Cost Manifest name must be entered." );
            }
            else
                output.Name = input.Name;

            if ( string.IsNullOrWhiteSpace( input.Description ) )
                messages.Add( "Error - An Cost Manifest description must be entered." );
            else
                output.Description = input.Description;

            if ( !IsUrlValid( input.CostDetails, ref statusMessage, ref isUrlPresent ) )
                messages.Add( "The CostDetails is invalid" );
            else
                if ( isUrlPresent )
					output.CostDetails = input.CostDetails;

           output.CostManifestOf = FormatOrganizationReferenceToList( input.OwningOrganization, "Owning Organization", true, ref messages );


            return isValid;
        }

        public static void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            output.StartDate = MapDate( input.StartDate, "StartDate", ref messages );
            output.EndDate = MapDate( input.EndDate, "EndDate", ref messages );
        }

    }
}
