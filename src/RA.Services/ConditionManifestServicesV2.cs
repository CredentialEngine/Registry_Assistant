using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using CER = RA.Services.RegistryServices;
using EntityRequest = RA.Models.Input.ConditionManifestRequest;
using InputEntity = RA.Models.Input.ConditionManifest;
using OutputEntity = RA.Models.JsonV2.ConditionManifest;
using OutputGraph = RA.Models.JsonV2.GraphContainer;

using Utilities;

namespace RA.Services
{
	/// <summary>
	/// ConditionManifest Services
	/// </summary>
	public class ConditionManifestServicesV2 : ServiceHelperV2
	{
		static string status = "";
        /// <summary>
        /// Publish a Condition Manifest to the Credential Registry
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

                CER cer = new CER( "ConditionManifest", output.Type, output.Ctid, helper.SerializedInput )
                {
                    PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					PublishingForOrgCtid = helper.OwnerCtid
                };

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
						LoggingHelper.DoTrace( 4, string.Format( "ConditionManifest Services.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.Ctid, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "ConditionManifest", output.Ctid, ref cer, ref messages ) )
				{
					isValid = false;
					//helper.SetMessages( messages );
					//return; //===================
				}
				else
				{

					string identifier = "ConditionManifest_" + request.ConditionManifest.Ctid;
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;

						string msg = string.Format( "<p>Published ConditionManifest: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", request.ConditionManifest.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId );
						NotifyOnPublish( "ConditionManifest", msg );
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

		public string FormatAsJson( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
            OutputGraph og = new OutputGraph();
            var output = new OutputEntity();
            helper.Payload = "";
            isValid = true;
            IsAPublishRequest = false;
            //RA.Models.RequestStatus helper = new Models.RequestStatus();
            //do this in controller
            helper.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );


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

		public bool ToMap( EntityRequest request, OutputEntity output, ref RA.Models.RequestHelper helper )
		{
			CurrentEntityType = "ConditionManifest";
            InputEntity input = request.ConditionManifest;
            bool isValid = true;
			Community = request.Community ?? "";

			List<string> messages = new List<string>();
            //too late to fully implement the possibility of returning just warnings
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

				output.Recommends = FormatConditionProfile( input.RecommendedConditions, ref messages );
                output.Renewal = FormatConditionProfile( input.RenewedConditions, ref messages );
                output.Requires = FormatConditionProfile( input.Requires, ref messages );
				output.EntryConditions = FormatConditionProfile( input.EntryConditions, ref messages );
				output.Corequisite = FormatConditionProfile( input.CorequisiteConditions, ref messages );

				//probably should require at least one condition?
				if ( ( output.Recommends == null || output.Recommends.Count == 0 ) &&
                    ( output.Renewal == null || output.Renewal.Count == 0 ) &&
                    ( output.Requires == null || output.Requires.Count == 0 ) &&
					( output.EntryConditions == null || output.EntryConditions.Count == 0 ) &&
					( output.Corequisite == null || output.Corequisite.Count == 0 )
					)
				{
					//not approved
					//messages.Add( "Error - An Condition Manifest must have at least one conditon profile." );
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "ConditionManifestServices.ToMap" );
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
			///string property = "";

			CurrentCtid = output.Ctid = FormatCtid(input.Ctid, "Condition Manifest", ref messages);
            output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.Ctid, Community);

            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                if ( input.Name_Map == null || input.Name_Map.Count == 0 )
                {
                    messages.Add( FormatMessage( "Error - A Name or Name_Map must be entered for Condition Manifest with CTID: '{0}'.", input.Ctid ) );
                }
                else
                {
                    output.Name = AssignLanguageMap( input.Name_Map, "Condition Manifest Name", ref messages );
                    CurrentEntityName = GetFirstItemValue( output.Name );
                }
            }
            else
            {
                output.Name = Assign( input.Name, DefaultLanguageForMaps );
                CurrentEntityName = input.Name;
            }
            output.Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

            output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

			output.ConditionManifestOf = FormatOrganizationReferenceToList( input.ConditionManifestOf, "Owning Organization", true, ref messages );

			return isValid;
		}

	}
}
