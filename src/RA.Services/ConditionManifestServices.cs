using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
using EntityRequest = RA.Models.Input.ConditionManifestRequest;
using InputEntity = RA.Models.Input.ConditionManifest;
using OutputEntity = RA.Models.Json.ConditionManifest;
using RA.Models.Json;
using RA.Models.Input;

using Utilities;

namespace RA.Services
{
	/// <summary>
	/// ConditionManifest Services
	/// </summary>
	public class ConditionManifestServices : ServiceHelper
	{
		static string status = "";
		static bool isUrlPresent = true;

        /// <summary>
        /// Publish a Condition Manifest to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="helper"></param>
        public static void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;
			string submitter = "";

			var output = new OutputEntity();
			if ( ToMap( request.ConditionManifest, output, ref helper ) )
			{
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

                CER cer = new CER( "ConditionManifest", output.Type, output.Ctid, helper.SerializedInput); 
                cer.PublisherAuthorizationToken = helper.ApiKey;
				cer.PublishingForOrgCtid = helper.OwnerCtid;

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;

				string identifier = "ConditionManifest_" + request.ConditionManifest.Ctid;
				if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
					//for now need to ensure envelopid is returned
					helper.RegistryEnvelopeId = crEnvelopeId;

					string msg = string.Format( "<p>Published ConditionManifest: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId );
					NotifyOnPublish( "ConditionManifest", msg );
				}
				else
				{
					helper.AddError( status );
					isValid = false;
				}
			}
			else
			{
				helper.HasErrors = true;
				isValid = false;
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}
		}

		public static string FormatAsJson( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
            return FormatAsJson( request.ConditionManifest, ref isValid, helper );
		}

		private static string FormatAsJson( InputEntity input, ref bool isValid, RA.Models.RequestHelper helper )
		{
			var output = new OutputEntity();
			helper.Payload = "";
			isValid = true;
            IsAPublishRequest = false;
            //RA.Models.RequestStatus helper = new Models.RequestStatus();
            //do this in controller
            helper.CodeValidationType = UtilityManager.GetAppKeyValue( "conceptSchemesValidation", "warn" );
			

			if ( ToMap( input, output, ref helper ) )
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			else
			{
				isValid = false;
				//do payload anyway
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}

			return helper.Payload;
		}

		public static bool ToMap( InputEntity input, OutputEntity output, ref RA.Models.RequestHelper helper )
		{
			CurrentEntityType = "ConditionManifest";
			bool isValid = true;
			List<string> messages = new List<string>();
			//too late to fully implement the possibility of returning just warnings

			try
			{
				HandleRequiredFields( input, output, ref messages );

				output.Recommends = FormatConditionProfile( input.RecommendedConditions, ref messages );
                output.Renewal = FormatConditionProfile( input.RenewedConditions, ref messages );
                output.Requires = FormatConditionProfile( input.RequiredConditions, ref messages );
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
				LogError( ex, "ConditionManifestServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
			{
				isValid = false;
				helper.SetMessages( messages );
			}
			return isValid;
		}

		public static bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			bool isValid = true;
            ///string property = "";

            output.Ctid = FormatCtid(input.Ctid, ref messages);
            output.CtdlId = idBaseUrl + output.Ctid;

            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
			{
				messages.Add( "Error - An Condition Manifest name must be entered." );
			}
			else
			{
				output.Name = input.Name;
				CurrentEntityName = input.Name;
			}
			if ( string.IsNullOrWhiteSpace( input.Description ) )
			{
				messages.Add( "Error - An Condition Manifest description must be entered." );
			}
			else
                output.Description = ConvertWordFluff( input.Description );

            output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

			output.ConditionManifestOf = FormatOrganizationReferenceToList( input.ConditionManifestOf, "Owning Organization", true, ref messages );

			return isValid;
		}

	}
}
