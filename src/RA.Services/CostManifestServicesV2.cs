using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using CER = RA.Services.RegistryServices;
using EntityRequest = RA.Models.Input.CostManifestRequest;
using InputEntity = RA.Models.Input.CostManifest;
using OutputEntity = RA.Models.JsonV2.CostManifest;
using OutputGraph = RA.Models.JsonV2.GraphContainer;

using Utilities;

namespace RA.Services
{
    public class CostManifestServicesV2 : ServiceHelperV2
    {
        static string status = "";
        static bool isUrlPresent = true;

        /// <summary>
        /// Publish an Cost Manifest to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="helper"></param>
        public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
            isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;
            string submitter = "";
			List<string> messages = new List<string>();

			var output = new OutputEntity();
            OutputGraph og = new OutputGraph();

            if ( ToMap( request, output, ref helper ) )
            {
                og.Graph.Add( output );
                //TODO - is there other info needed, like in context?
                if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

                CER cer = new CER( "CostManifest", output.Type, output.Ctid, helper.SerializedInput )
                {
                    PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					PublishingForOrgCtid = helper.OwnerCtid
                };

				//
				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
				{
					cer.IsManagedRequest = true;
					//get publisher org
					string publisherCTID = "";
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;
					}
					else
					{
						//should be an error message returned
						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "CostManifest Services.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.Ctid, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				//
				if ( !SupportServices.ValidateAgainstPastRequest( "CostManifest", output.Ctid, ref cer, ref messages ) )
				{
					isValid = false;
					//helper.SetMessages( messages );
					//return; //===================
				}
				else
				{

					string identifier = "CostManifest_" + request.CostManifest.Ctid;
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;

						string msg = string.Format( "<p>Published CostManifest: {0}</p><p>CostDetails  webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", request.CostManifest.Name, output.CostDetails, output.Ctid, crEnvelopeId );
						NotifyOnPublish( "CostManifest", msg );
					}
					else
					{
						helper.AddError( status );
						isValid = false;
					}
				}
            }
            else
            {
                helper.HasErrors = true;
                isValid = false;
                og.Graph.Add( output );
                //TODO - is there other info needed, like in context?
                if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }
			helper.SetWarningMessages( warningMessages );
			helper.SetMessages( messages );
		}
        //

        public string FormatAsJson( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
            OutputGraph og = new OutputGraph();
            var output = new OutputEntity();
            helper.Payload = "";
            isValid = true;
            IsAPublishRequest = false;

            RA.Models.RequestHelper reqStatus = new Models.RequestHelper();
            reqStatus.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );

            if ( ToMap( request, output, ref helper ) )
            {
                og.Graph.Add( output );
                if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }

                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }
            else
            {
                isValid = false;
                //do payload anyway
                og.Graph.Add( output );
                //TODO - is there other info needed, like in context?
                if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }
			helper.SetWarningMessages( warningMessages );
			return helper.Payload;
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
        public bool ToMap( EntityRequest request, OutputEntity output, ref RA.Models.RequestHelper helper )
        {
            CurrentEntityType = "CostManifest";
            InputEntity input = request.CostManifest;
            bool isValid = true;
			Community = request.Community ?? "";

			List<string> messages = new List<string>();
            if ( !string.IsNullOrWhiteSpace( request.DefaultLanguage ) )
            {
                //validate
                if ( ValidateLanguageCode( request.DefaultLanguage, "request.DefaultLanguage", ref messages ) )
                {
                    DefaultLanguageForMaps = request.DefaultLanguage;
                }
            }
            else
                DefaultLanguageForMaps = SystemDefaultLanguageForMaps;

            try
            {
                HandleRequiredFields( input, output, ref messages );

                HandleLiteralFields( input, output, ref messages );

                //TBD - are estimated costs required?
                if ( input.EstimatedCost == null || input.EstimatedCost.Count == 0 )
                {
                    //messages.Add( "Error - An Cost Manifest must have at least one cost profile." );
                }
                else
                {
                    output.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );
                }
            }
            catch ( Exception ex )
            {
				LoggingHelper.LogError( ex, "CostManifestServices.ToMap" );
                messages.Add( ex.Message );
            }
            if ( messages.Count > 0 )
            {
                isValid = false;
                helper.SetMessages( messages );
            }

            return isValid;
        }

        public bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
            string statusMessage = "";
			CurrentCtid = output.Ctid = FormatCtid( input.Ctid, "Cost Manifest", ref messages );
            output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.Ctid, Community);

            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                if ( input.Name_Map == null || input.Name_Map.Count == 0 )
                {
                    messages.Add( FormatMessage( "Error - A Name or Name_Map must be entered for Cost Manifest with CTID: '{0}'.", input.Ctid ) );
                }
                else
                {
                    output.Name = AssignLanguageMap( input.Name_Map, "Cost Manifest Name", ref messages );
                    CurrentEntityName = GetFirstItemValue( output.Name );
                }
            }
            else
            {
                output.Name = Assign( input.Name, DefaultLanguageForMaps );
                CurrentEntityName = input.Name;
            }
            output.Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

            if ( !IsUrlValid( input.CostDetails, ref statusMessage, ref isUrlPresent ) )
                messages.Add( "The CostDetails is invalid" );
            else
                if ( isUrlPresent )
                output.CostDetails = input.CostDetails;

            output.CostManifestOf = FormatOrganizationReferenceToList( input.CostManifestOf, "Owning Organization", true, ref messages );


            return isValid;
        }

        public void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            output.StartDate = MapDate( input.StartDate, "StartDate", ref messages );
            output.EndDate = MapDate( input.EndDate, "EndDate", ref messages );
        }

    }
}
