using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using CredentialRegistry;
//using JWT;
//using J2 = Utilities.JsonWebToken2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RA.Models;
using RA.Models.BusObj;
using RA.Models.Input;
using Utilities;

namespace RA.Services
{
	public class RegistryServices
	{
		#region Properites/Constructors
		public const string RA_PUBLISH_METHOD = "publishMethod:RegistryAssistant";
		public const string CE_PUBLISH_METHOD_MANUAL_ENTRY = "publishMethod:ManualEntry";
		public const string CE_BULK_UPLOAD_METHOD = "publishMethod:BulkUpload";
		public const string CE_PUBLISH_METHOD_USING_CEKEYS = "ManualPublish";
		public const string CE_TRUSTED_PARTNER_ROLE = "publishRole:TrustedPartner";

		public static string REGISTRY_ACTION_DELETE = "Registry Delete";
		public static string REGISTRY_ACTION_PURGE = "Registry Purge";
		public static string REGISTRY_ACTION_TRANSFER = "Transfer of Owner";
		public static string REGISTRY_ACTION_REMOVE_ORG = "RemoveOrganization";

		public string thisClassName = "RegistryServices";
		public string currentEnvironment = UtilityManager.GetAppKeyValue( "environment" );

		public RegistryServices(string entityType, string ctdlType, string ctid)
		{
			PublishingEntityType = entityType;
			CtdlType = ctdlType;
			EntityCtid = ctid ?? "";
		}
		public RegistryServices(string entityType, string ctdlType, string ctid, string serializedInput)
		{
			PublishingEntityType = entityType;
			CtdlType = ctdlType;
			EntityCtid = ctid;
			SerializedInput = serializedInput;
		}
		
		/// <summary>
		/// AKA API Key
		/// </summary>
		public string PublisherAuthorizationToken { get; set; }
		/// <summary>
		/// Data Owner CTID
		/// </summary>
		public string PublishingForOrgCtid { get; set; } 

		/// <summary>
		/// CTID for the publishing org. This will be derived using the apiKey (PublisherAuthorizationToken)
		/// These will typically be the same, unless part of a third party trx
		/// </summary>
		public string PublishingByOrgCtid { get; set; } = "";
		public string PublishingEntityType { get; set; }
		public string CtdlType { get; set; }
		public string EntityCtid { get; set; }
		public string EntityName { get; set; }
		public string EntityNameForHistory { get; set; }
		public string Community { get; set; } = "";
		public string SerializedInput { get; set; } = "";
		/// <summary>
		/// true = keys managed by credential registry, goes through accounts site
		/// </summary>
		public bool IsManagedRequest { get; set; }
		public bool LastActionWasTransferOfOwner { get; set; }
		/// <summary>
		/// true if originates from publisher
		/// </summary>
		public bool IsPublisherRequest { get; set; }

		public bool HasBeenPreviouslyPublished { get; set; }
		public bool OverrodeOriginalRequest { get; set; }
		public bool SkippingValidation { get; set; }
		public bool WasChanged { get; set; }
		public string EnvelopeUrl { get; set; }
		public string GraphUrl { get; set; }

		#endregion

		#region Publishing
		/// <summary>
		/// Initiate a publish request, using a managed request or self published
		/// </summary>
		/// <param name="request">Request including payload, and configuration properties</param>
		/// <param name="submitter">NO LONGER USED</param>
		/// <param name="identifier"></param>
		/// <param name="statusMessage"></param>
		/// <param name="crEnvelopeId"></param>
		/// <returns></returns>
		public bool Publish(RequestHelper request,
									string submitter,
									string identifier,
									ref string statusMessage,
									ref string crEnvelopeId)
		{
			var successful = true;
			if (!string.IsNullOrWhiteSpace(Community))
			{
				if (Community.ToLower() != "navy")
				{
					statusMessage = "Error - an invalid community was provided: " + Community;
					return false;
				}
			}
			else
				Community = "";

			if ( IsManagedRequest )
			{
				//if ( PublishingEntityType.StartsWith("Pathway") || UtilityManager.GetAppKeyValue( "doingDirectManagedPublishing", false))

				DoDirectManagedPublish( request,
					this.PublisherAuthorizationToken,
					this.PublishingForOrgCtid,
					identifier, ref successful, ref statusMessage, ref crEnvelopeId );
				//else
				//	DoManagedPublishThroughAccounts( request,
				//	this.PublisherAuthorizationToken,
				//	this.PublishingForOrgCtid,
				//	identifier, ref successful, ref statusMessage, ref crEnvelopeId );
			}
			else
				DoPublishUsingCERegistryKeys( request, this.PublishingForOrgCtid, identifier, ref successful, ref statusMessage, ref crEnvelopeId, SkippingValidation );

			if (successful)
			{
				//use envelopeId
				request.EnvelopeUrl = SupportServices.FormatRegistryUrl(3, crEnvelopeId, Community);
				//use ctid
				request.GraphUrl = SupportServices.FormatRegistryUrl(1, EntityCtid, Community);
			}
			return successful;
		} //

		/// <summary>
		/// Publish using old approach using self managed keys
		/// NOTE: should include apiKey for reference is was provided
		/// </summary>
		/// <param name="payload"></param>
		/// <param name="identifier"></param>
		/// <param name="valid"></param>
		/// <param name="status"></param>
		/// <param name="crEnvelopeId"></param>
		/// <param name="forceSkipValidation"></param>
		/// <returns></returns>
		private string DoPublishUsingCERegistryKeys(RequestHelper request,
				string dataOwnerCTID,
				string identifier,
				ref bool valid,
				ref string status,
				ref string crEnvelopeId,
				bool forceSkipValidation = false )
		{
			valid = true;
			var publicKeyPath = "";
			var privateKeyPath = "";
			var postBody = "";
			//List<string> messages = new List<string>();
            string environment = UtilityManager.GetAppKeyValue( "environment" );
            LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.DoPublishUsingCERegistryKeys. crEnvelopeId: {0}", crEnvelopeId ));
			//LoggingHelper.DoTrace( 7, "              - payload: \r\n" + payload );
			try
			{
				//TODO - add handling where keys are stored in the registry
				if ( GetKeys( ref publicKeyPath, ref privateKeyPath, ref status ) == false )
				{
					valid = false;
					//no, the proper error is returned from GetKeys
					//status = "Error getting CER Keys";
					request.AddError( status );
					return "";
				}

				//todo - need to add signer and other to the content
				//note for new, DO NOT INCLUDE an EnvelopeIdentifier property 

				//get publish API
				string serviceUri = UtilityManager.GetAppKeyValue( "credentialRegistryPublishUrl" );
				string queryString = GetRequestContext();
				var domainWarning = "";
				EntityNameForHistory = EntityName;
				if ( environment == "production" && queryString.IndexOf( "//credentialengine.org/assistant" ) > -1 )
				{
					domainWarning = UtilityManager.GetAppKeyValue( "domainWarning" );
					EntityNameForHistory += " (**old domain**)";
				}
				var defaultCommunity = UtilityManager.GetAppKeyValue( "defaultCommunity", "ce-registry" );
				//just one for now
				if ( !string.IsNullOrWhiteSpace( Community ) )
				{
					//Warning: assumes default community name
					serviceUri = serviceUri.Replace( "ce-registry", Community );
					defaultCommunity = Community;
				}
				#region Envelope related steps
				if ( string.IsNullOrWhiteSpace( crEnvelopeId ) )
				{
					Envelope envelope = new Envelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, request.Payload, envelope, defaultCommunity );
                    envelope.EnvelopeCetermsCtid = EntityCtid;
                    envelope.EnvelopeCtdlType = CtdlType;
					envelope.documentOwnedBy = dataOwnerCTID;
					if (!string.IsNullOrWhiteSpace( PublishingByOrgCtid ) )
						envelope.documentPublishedBy = PublishingByOrgCtid;
					//
					postBody = JsonConvert.SerializeObject( envelope, ServiceHelperV2.GetJsonSettings() );

					//LoggingHelper.DoTrace( 7, "RegistryServices.DoPublishUsingCERegistryKeys - ADD envelope: \r\n" + postBody );
				}
				else
				{
					UpdateEnvelope envelope = new UpdateEnvelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, request.Payload, envelope, defaultCommunity );

					//now embed 
					envelope.EnvelopeIdentifier = crEnvelopeId;
					envelope.EnvelopeCetermsCtid = EntityCtid;
					envelope.EnvelopeCtdlType = CtdlType;
					envelope.documentOwnedBy = dataOwnerCTID;
					if ( !string.IsNullOrWhiteSpace( PublishingByOrgCtid ) )
						envelope.documentPublishedBy = PublishingByOrgCtid;
					//
					postBody = JsonConvert.SerializeObject( envelope, ServiceHelperV2.GetJsonSettings() );

					LoggingHelper.DoTrace( 5, string.Format( "RegistryServices.DoPublishUsingCERegistryKeys - updating existing envelopeId: {0}. update envelope: \r\n", crEnvelopeId )  );
					//LoggingHelper.DoTrace( 7, "RegistryServices.DoPublishUsingCERegistryKeys - update envelope: \r\n" + postBody );
				}


				var skippingValidation = forceSkipValidation ? true : UtilityManager.GetAppKeyValue( "skippingValidation" ) == "yes";
                if ( request.Payload.ToLower().IndexOf( "@graph" ) > 0 )
                    skippingValidation = true;

                if ( skippingValidation )
				{
					if ( serviceUri.ToLower().IndexOf( "skip_validation" ) > 0 )
					{
						//assume OK, or check to change false to true
						serviceUri = serviceUri.Replace( "skip_validation=false", "skip_validation=true" );
					}
					else
					{
						//append
						serviceUri += "&skip_validation=true";
					}
				}
				//20-10-27 - getting a Can't find organization error with CE: ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73
				//			- latter not ce-6b62f237-3354-4e2c-b96d-3db5c10e91e3
				if ( UtilityManager.GetAppKeyValue( "includingMetadataBYParameters", false ) )
				{
					serviceUri += string.Format( "&owned_by={0}&published_by={1}", dataOwnerCTID, PublishingByOrgCtid );
					LoggingHelper.DoTrace( 6, "***DoPublishUsingCERegistryKeys - new registry url: " + serviceUri );

				}

				#endregion

				string contents = "";
				DateTime started = DateTime.Now;
				try
				{
					if ( HttpContext.Current != null )
					{
						if (HttpContext.Current.Server.ScriptTimeout < 1000)
							HttpContext.Current.Server.ScriptTimeout = 1000;
					}
					
					using ( var client = new HttpClient() )
					{
						client.DefaultRequestHeaders.
							Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
						if( postBody.Length > 1000000 )
						{
							client.Timeout = new TimeSpan( 0, 30, 0 );

							LoggingHelper.DoTrace( 1, string.Format( "RegistryServices.DoPublishUsingCERegistryKeys(). *******publish note. Larger payload: {0} bytes. ", postBody.Length ) );
						}
						//var content = new StringContent( postBody, Encoding.UTF8, "application/json" );
						var task = client.PostAsync( serviceUri,
							new StringContent( postBody, Encoding.UTF8, "application/json" ) );
						task.Wait();
						var response = task.Result;
						TimeSpan duration = DateTime.Now.Subtract( started );
						if ( duration.TotalSeconds > 2 )
							LoggingHelper.DoTrace( 1, string.Format( " *******publish took a little longer: elapsed: {0:N2} seconds ", duration.TotalSeconds ) );
						//should get envelope_id from contents?
						contents = task.Result.Content.ReadAsStringAsync().Result;

						if ( response.IsSuccessStatusCode == false )
						{
                            if (contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "{" ) > -1)
                            {
                                //handle double [[
                                contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
                                RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                                if (contentsJson.Errors != null && contentsJson.Errors.Count > 0)
                                {
                                    status = string.Join( ",", contentsJson.Errors.ToArray() );
                                    request.SetMessages( contentsJson.Errors );
                                } else
								{
									//what
									status = contents;
								}
                            }
                            else
                            {
                                status = contents;
								request.AddError( status );
                            }
                            // RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                            //
                            valid = false;
							
                            // +"\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )

                            LoggingHelper.LogError( identifier + " RegistryServices.Publish Failed:"
								+ "\n\rURL:\n\r " + queryString
								+ "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject( response )
								+ "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject( contents ),
								false, "CredentialRegistry publish failed for " + identifier );

							//messages.AddRange( contentsJson.Errors );
							LoggingHelper.WriteLogFile( 5, identifier + "_payload_failed.json", request.Payload, "", false );
							LoggingHelper.WriteLogFile( 7, identifier + "_envelope_failed", postBody, "", false );
							//statusMessage =contents.err contentsJson.Errors.ToString();
						}
						else
						{
							valid = true;
							//LoggingHelper.DoTrace( 7, "contents after successful publish.\r\n" + contents );
							UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>( contents );
							//when doing pub of large framework the first time, this value ended up false?
							if ( HasBeenPreviouslyPublished )
								WasChanged = ue.Changed;
							else
								WasChanged = true;

							crEnvelopeId = ue.EnvelopeIdentifier;
							var envelopeUpdatedAt = ue.NodeHeader.UpdatedAt;
							LoggingHelper.DoTrace( 5, string.Format( "Returned EnvelopeId: {0}, Elapsed: {1:N2}, WasChanged: {2}. ", crEnvelopeId, duration.TotalSeconds, ue.Changed ) + domainWarning );

							LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", request.Payload, "", false );
							//LoggingHelper.WriteLogFile( 7, identifier + "_envelope_Successful", postBody, "", false );
							SupportServices ss = new SupportServices();
							if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", false ) )
                            {
								//initialize with entity name - why here?
								//should this be allowed if envelope was not updated?
								// 20-02
                                SupportServices mgr = new SupportServices( EntityNameForHistory, Community );
                                mgr.AddHistory( string.IsNullOrWhiteSpace(dataOwnerCTID) ? "missing" : dataOwnerCTID, request.Payload, CE_PUBLISH_METHOD_USING_CEKEYS, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status, PublishingByOrgCtid, WasChanged, !HasBeenPreviouslyPublished );
                            }

							ActivityLog activity = new ActivityLog()
							{
								Application = "RegistryAssistant",
								ActivityType = PublishingEntityType,
								Activity = "Credential Registry",
								Event = "ManualPublish",   //temp?
								ActivityObjectCTID = EntityCtid,
								PublishingOrganizationCTID = request.OwnerCtid,
								DataOwnerCTID = request.OwnerCtid,
								IPAddress = request.IPAddress       //this will need to be provided for each publish type
							};
							activity.Comment = string.Format( "Organization: '{0}' published the '{1}': '{2}'.", request.OwnerCtid, PublishingEntityType, EntityNameForHistory );
							//else
							//	activity.Comment = string.Format( "Third Party Organization: '{0}' published the '{1}': '{2}' for Organization: '{3}'", trxCheck.PublishingOrganization, PublishingEntityType, EntityNameForHistory, trxCheck.OwningOrganization );
							ss.AddActivity( activity, ref status );
						}

						if ( !string.IsNullOrWhiteSpace( domainWarning ) )
						{
							LoggingHelper.DoTrace( 5, string.Format( "*********** old domain encountered ******** DataOwnerCtid: {0}, PublishingEntityType: {1}, EntityCtid: {2}", PublishingForOrgCtid, PublishingEntityType, EntityCtid ) );
							request.AddWarning( domainWarning );
							status += domainWarning;
						}
						return contents;
					}
				}
				catch ( Exception ex )
				{
					TimeSpan duration = DateTime.Now.Subtract( started );
					if ( duration.TotalSeconds > 2 )
						LoggingHelper.DoTrace( 1, string.Format( " *******publish exception. Elapsed: {0:N2} seconds ", duration.TotalSeconds ) );

					var ipAddress = GetSourceIPAddress();
					LoggingHelper.LogError( ex, string.Format( "RegistryServices.DoPublishUsingCERegistryKeys - On POST. identifier: {0}, dataOwnerCTID: {1}, ipAddress: {2}", identifier, dataOwnerCTID, ipAddress ) );
					valid = false;
					status = "Self Publish Exception: " + LoggingHelper.FormatExceptions( ex );
					request.AddError( status );
					return "";
				}
				//finally
				//{
				//	TimeSpan rowDuration = DateTime.Now.Subtract( started );
				//}
				//Set return values
				//no cr id returned?

			}
			catch ( Exception ex )
			{
				string extraMsg = "";
				if (ex.Message.IndexOf( "The system cannot find the file specified" ) > -1 )
				{
					/*likely due to the application pool issue: 
					 * Go to IIS/Application Pools
					 * Select the pool being used for the assistant API and click on Advanced Settings
					 * Under Process Model, set Load User Profile to be true
					 * 
					 */
					extraMsg = " **Likely due to an application pool issue: Go to IIS, Select AppPool for Assistant, then Advanced Settings. Under Process Model, set Load User Profile to be true";
				}
                LoggingHelper.LogError( ex, string.Format( "RegistryServices.DoPublishUsingCERegistryKeys - Outside try/catch. identifier: {0}, crEnvelopeId: {1}", identifier, crEnvelopeId ) + extraMsg );

                valid = false;
				crEnvelopeId = "";
				status = "Self Publish Failed during Registry preparations: " + LoggingHelper.FormatExceptions( ex ) + extraMsg;
				request.AddError( status );
				return "";
			}
			finally
			{
				LoggingHelper.DoTrace( 6, "RegistryServices.DoPublishUsingCERegistryKeys. Exited. " );
			}
		}
		//

		public string DoDirectManagedPublish( RequestHelper request,
				string apiKey,
				string dataOwnerCTID,
				string identifier,
				ref bool valid,
				ref string status,
				ref string crEnvelopeId )
		{
			valid = true;

			//List<string> messages = new List<string>();
			LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.DoDirectManagedPublish. Entered. dataOwnerCTID: {0}, Type:{1}, CTID: {2}", dataOwnerCTID, PublishingEntityType, EntityCtid ) );
			//Now, need set the publish method to manual entry of credentials, or competencies. 
			//TODO - replace use of this:
			ManagedPublishRequest apr = new ManagedPublishRequest
			{
				payloadJSON = request.Payload,
				apiKey = apiKey,
				dataOwnerCTID = dataOwnerCTID,
				publishMethodURI = RA_PUBLISH_METHOD,  //not necessarily
				publishingEntityType = PublishingEntityType,
				ctdlType = CtdlType,
				entityCtid = EntityCtid,
				community = Community
				//serializedInput = SerializedInput
			};
			if ( OverrodeOriginalRequest )
			{
				//meand the request came from the manual publisher and there was either a previous request via the api or moving forward all new publishing is to be managed. 
				apr.publishMethodURI = CE_PUBLISH_METHOD_MANUAL_ENTRY;
			} else if ( LastActionWasTransferOfOwner )
			{
				//in most case this will be the proper action - it may not really matter
				apr.publishMethodURI = CE_PUBLISH_METHOD_MANUAL_ENTRY;
			}
			EntityNameForHistory = EntityName;
			//			
			string queryString = GetRequestContext();
			string environment = UtilityManager.GetAppKeyValue( "environment" );

			//always skipping so skip the code
			bool skippingValidation = SkippingValidation ? true : UtilityManager.GetAppKeyValue( "skippingValidation" ) == "yes";
			if ( request.Payload.ToLower().IndexOf( "@graph" ) > 0 )
				skippingValidation = true;
			if ( skippingValidation )
			{
				apr.skipValidation = "true";
			}

			//string postBody = JsonConvert.SerializeObject( apr, ServiceHelperV2.GetJsonSettings() );
			
			string contents = "";
			List<string> messages = new List<string>();
			DateTime started = DateTime.Now;
			SupportServices supportServices = new SupportServices( EntityNameForHistory, Community );
			// ===========================================================================
			//the method will also have to check actions other than publishing
			var trxCheck = supportServices.ValidateRegistryRequest( apiKey, request.OwnerCtid, apr.publishMethodURI, ref messages );
			if ( trxCheck.Successful == false )
			{
				valid = false;
				request.SetMessages( messages );
				return "";
			}
			var isThirdPartyEvent = false;
			if ( trxCheck.OwningOrganization != trxCheck.PublishingOrganization )
				isThirdPartyEvent = true;
			LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.DoDirectManagedPublish - Publishing Org CTID: {0}, dataOwnerCTID: {1}, PublishingEntityType: {2}, EntityCtid: {3}, publishMethodURI: {4} ", trxCheck.PublishingOrganizationCTID, dataOwnerCTID, PublishingEntityType, EntityCtid, apr.publishMethodURI ) );
			//
			var summary = "";
			if ( isThirdPartyEvent )
				summary=string.Format( "Publisher: {0}, CTID:'{1}', DataOwner: {2}, CTID: '{3}', Publish Method: '{4}', Publishing EntityType: '{5}', Entity Ctid:'{6}', Entity Name: '{7}'. ", trxCheck.PublishingOrganization, PublishingByOrgCtid, trxCheck.OwningOrganization, dataOwnerCTID, apr.publishMethodURI, PublishingEntityType, EntityCtid, EntityNameForHistory );
			else
				summary=string.Format( "DataOwner: {0}, Owner CTID:'{1}', PublishingByOrgCtid: '{2}, Publish Method: '{3}', Publishing EntityType: '{4}', Entity Ctid:'{5}', Entity Name: '{6}'. ", trxCheck.OwningOrganization, dataOwnerCTID, PublishingByOrgCtid, apr.publishMethodURI,  PublishingEntityType,  EntityCtid,EntityNameForHistory );
			ActivityLog activity = new ActivityLog()
			{
				Application = "RegistryAssistant",
				ActivityType = PublishingEntityType,
				Activity = "Credential Registry",
				//Event = "Publish from" + apr.publishMethodURI,   //temp? - should distiguish add/update?-done later
				ActivityObjectCTID = EntityCtid,
				PublishingOrganizationCTID = trxCheck.PublishingOrganizationCTID,
				DataOwnerCTID = request.OwnerCtid,
				IPAddress = request.IPAddress       
			};
			//
			var registryPublishEndpoint = UtilityManager.GetAppKeyValue( "CredentialRegistryPublishEndpoint" );
			if ( !string.IsNullOrWhiteSpace( Community ) )
			{
				//this will work
				registryPublishEndpoint += "&community_name=" + Community;
			}
			//add documentPublishedBy 
			if ( !string.IsNullOrWhiteSpace( trxCheck.PublishingOrganizationCTID ) )
			{
				//add publisher CTID. It will be added to the envelope
				if ( UtilityManager.GetAppKeyValue( "includingMetadataBYParameters", false ) )
					registryPublishEndpoint += "&published_by=" + trxCheck.PublishingOrganizationCTID;
			}
			var registryAuthorizationToken = UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" );
			//
			var owningOrgParm = trxCheck.OwningOrganizationRegistryIdentifier;
			//testing something-not sure if there is a plan to use ctid instead of internal registry identifier
			//20-11-20 - noticed in the sandbox that it works with the ctid!!!!!!!!!!!!!!!!
			//	it seems that the useOrgCTIDinCERPublishEndpoint was to use ctid and not organization_id 
			if ( ( UtilityManager.GetAppKeyValue( "useOrgCTIDinCERPublishEndpoint", false ) ) )
			{
				owningOrgParm = request.OwnerCtid; //in registry: ce-af04a49e-5b80-4258-beb7-f556cee3b92b	from publisher: ce-b67e212e-2eb7-401e-a076-bf566102f268
			}

			string upperCaseCTID = "";
			if ( ShouldUpperCaseOwnerCTIDBeUsed( owningOrgParm, ref upperCaseCTID ) )
			{
				owningOrgParm = upperCaseCTID;
			}
			try
			{
				//Format request =====================================================

				using ( var client = new HttpClient() )
				{

					var url = string.Format( registryPublishEndpoint, owningOrgParm );

					LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.DoDirectManagedPublish(). ****using url: {0}. ", url ) );
					var content = new StringContent( request.Payload, Encoding.UTF8, "application/json" );
					if ( request.Payload.Length > 1000000 )
					{
						client.Timeout = new TimeSpan( 0, 30, 0 );
						if ( HttpContext.Current != null )
						{
							if ( HttpContext.Current.Server.ScriptTimeout < 1000 )
								HttpContext.Current.Server.ScriptTimeout = 1000;
						}
						LoggingHelper.DoTrace( 1, string.Format( "RegistryServices.DoDirectManagedPublish(). *******publish note. Larger payload: {0} bytes. ", request.Payload.Length ) );
					}
					client.DefaultRequestHeaders.Add( "Authorization", "Token " + registryAuthorizationToken );
					//TODO - check the use of this:
					if ( trxCheck.PublishingOrganizationCTID != ( dataOwnerCTID ?? "" ).ToLower() )
					{
						//20-10-18 - check if this is an issue with non-unique CTID
						client.DefaultRequestHeaders.Add( "Secondary-Token", "Token " + trxCheck.PublishingOrganizationRegistryIdentifier );
					}

					var result = client.PostAsync( url, content ).Result;
					var resultContent = result.Content.ReadAsStringAsync().Result;
					TimeSpan duration = DateTime.Now.Subtract( started );
					if ( duration.TotalSeconds > 5 )
						LoggingHelper.DoTrace( 1, string.Format( " *******publish took a little longer: elapsed: {0:N2} seconds ", duration.TotalSeconds ) );

					CheckRegistryResponse( result.ToString() );
					bool responseStatusCode = result.IsSuccessStatusCode;
					if ( ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
						|| contents.ToLower().IndexOf( "<html>" ) > 0
						|| ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "error" ) < 15 && contents.ToLower().IndexOf( "{" ) > -1 )
						)
						responseStatusCode = false;
					//RegistryResponse rrue = JsonConvert.DeserializeObject<RegistryResponse>( result.ToString() );
					//don't return a bad response, especially with only returning a string
					//20-08-20 mp - why -need to be clear if should not
					if ( responseStatusCode == false )
					{
						//Response.StatusCode = ( int ) result.StatusCode; //Test this
						// resultContent = resultContent + " ( " + result.StatusCode + " )";
						LoggingHelper.DoTrace( 5, "RegistryServices.DoDirectManagedPublish: Failed " + Environment.NewLine + resultContent + Environment.NewLine + JsonConvert.SerializeObject( result ) );

						if ( resultContent.ToLower().IndexOf( "{\"data\":" ) == 0 && resultContent.ToLower().IndexOf( "error:" ) > -1 )
						{
							valid = false;
							status = UtilityManager.ExtractNameValue( resultContent, "Error", ":", "\"" );
							request.AddError( status );
						}
						else if ( resultContent.ToLower().IndexOf( "<html>" ) > 0 )
						{
							int pos = resultContent.IndexOf( "title" );
							int next = resultContent.IndexOf( "</title>" );
							if ( pos > 0 && next > pos )
							{
								status = resultContent.Substring( pos + 5, resultContent.Length - next );
							}
							else
							{
								status = resultContent;
							}
						}
						else if ( resultContent.ToLower().IndexOf( "error" ) > 0 && resultContent.ToLower().IndexOf( "error" ) < 15 && resultContent.ToLower().IndexOf( "{" ) > -1 )
						{
							valid = false;
							resultContent = resultContent.Replace( "[[", "[" ).Replace( "]]", "]" );
							RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( resultContent );
							if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
							{
								status = string.Join( ",", contentsJson.Errors.ToArray() );
								//remove extraneous message of:@type : did not match one of the following values
								if ( status.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
								{
									status = status.Substring( 0, status.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]";
								}
								
								if ( status.IndexOf( "Couldn't find Organization" ) > -1 )
								{
									request.AddError( "The Credential Registry returned the message: \"Couldn't find Organization\". This usually means that the Owning Organization has not been properly registered and approved in the CE Accounts site (but could be a bug)." );
								} //
								else if ( status.IndexOf( "Resource CTID must be unique" ) > -1 )
								{
									request.AddError( "The Resource CTID must be unique. The CTID has been used previously by a different Owning Organization. You may need to contact Credential Engine to resolve this issue." );
								}
								else
								{
									request.SetMessages( contentsJson.Errors );
								}
							}
							else
							{
								status = UtilityManager.ExtractNameValue( resultContent, "Error", ":", "\"" );
								request.AddError( status );
							}

						}
						var msgs = string.Join( ";", request.GetAllErrorMessages().ToArray() );
						activity.Comment = string.Format( "Publish attempt failed for Organization: '{0}', type: '{1}' Name: '{2}', reason(s): {3}.", trxCheck.OwningOrganization, PublishingEntityType, EntityNameForHistory, msgs );
						supportServices.AddActivity( activity, ref status );
						//20-08-20 - getting a 401 error and response ends up being successful but envelope not changed
						return resultContent;//Note: resultContent is not used in caller!!
					}
					else
					{
						LoggingHelper.DoTrace( 6, "RegistryServices.DoDirectManagedPublish: Succeeded. " + summary);
						
						valid = true;//
						LoggingHelper.DoTrace( 7, "contents after successful managed publish.\r\n" + resultContent );

						UpdateEnvelope ue = new UpdateEnvelope();
						//
						ue = JsonConvert.DeserializeObject<UpdateEnvelope>( resultContent );
						//verify if HasBeenPreviouslyPublished is accurate
						if ( HasBeenPreviouslyPublished )
							WasChanged = ue.Changed;
						else
							WasChanged = true;
						crEnvelopeId = ue.EnvelopeIdentifier;
						var envelopeUpdatedAt = ue.NodeHeader.UpdatedAt;

						LoggingHelper.DoTrace( 5, string.Format( "Returned EnvelopeId: {0}, Elapsed: {1:N2}, WasChanged: {2}", crEnvelopeId, duration.TotalSeconds, ue.Changed ) );
						LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", request.Payload, "", false );
							

						if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", true ) )
						{
							//SupportServices mgr = new SupportServices( EntityNameForHistory, Community );
							supportServices.AddHistory( dataOwnerCTID, request.Payload, apr.publishMethodURI, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status, PublishingByOrgCtid, ue.Changed, !HasBeenPreviouslyPublished);
						}
						//set Event
						activity.Event = string.Format( "{0} using {1}", ( HasBeenPreviouslyPublished ? "Updated" : "Added" ), (apr.publishMethodURI.IndexOf( "RegistryAssistant" ) > -1 ? "RegistryAssistant" : " RegistryAssistant via " + apr.publishMethodURI ) );
						if ( isThirdPartyEvent == false)
							activity.Comment = string.Format( "Organization: '{0}' published the '{1}': '{2}'.", trxCheck.OwningOrganization, PublishingEntityType, EntityNameForHistory );
						else
							activity.Comment = string.Format( "Third Party Organization: '{0}' published the '{1}': '{2}' for Organization: '{3}'", trxCheck.PublishingOrganization, PublishingEntityType, EntityNameForHistory, trxCheck.OwningOrganization );

						supportServices.AddActivity( activity, ref status );

						return resultContent;

					
						//TODO - start logging somewhere 

					}
				}	
			}
			catch ( Exception ex )
			{

				//return JsonConvert.SerializeObject( JsonResponse( null, false, "Error: " + ex.Message + " was encountered attempting to publish.", null ).Data );
				request.AddError( "Error: " + ex.Message + " was encountered attempting to publish." );
				valid = false;
			}

			return contents;
		}
		private bool ShouldUpperCaseOwnerCTIDBeUsed( string ctid, ref string upperCaseCTID )
		{
			var candidates = UtilityManager.GetAppKeyValue( "orgCTIDsToUseUpperCase" );
			if ( candidates.IndexOf( ctid.ToLower() ) > -1 )
			{
				upperCaseCTID = ctid.ToUpper().Replace( "CE-", "ce-" );
				return true;
			}

			return false;
		}
		private void CheckRegistryResponse(string result )
		{

			try
			{
				CredentialRegistryResponse rrue = JsonConvert.DeserializeObject<CredentialRegistryResponse>( result.ToString() );

			} catch (Exception ex)
			{

			}
		}
		//

		public string DoManagedPublishThroughAccounts(RequestHelper request,
				string apiKey,
				string dataOwnerCTID,
				string identifier,
				ref bool valid,
				ref string status,
				ref string crEnvelopeId)
		{
			valid = true;
			//List<string> messages = new List<string>();
			LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.DoManagedPublishThroughAccounts. Entered. dataOwnerCTID: {0}", dataOwnerCTID ) );
			//Now, need set the publish method to manual entry of credentials, or competencies. 
			//or have the account system figure it out!
			ManagedPublishRequest apr = new ManagedPublishRequest
			{
				payloadJSON = request.Payload,
				apiKey = apiKey,
				dataOwnerCTID = dataOwnerCTID,
				publishMethodURI = RA_PUBLISH_METHOD,  //not necessarily
				publishingEntityType = PublishingEntityType,
				ctdlType = CtdlType,
				entityCtid = EntityCtid,
				community = Community
				//serializedInput = SerializedInput
			};
			if (OverrodeOriginalRequest)
			{
				//meand the request came from the manual publisher and there was either a previous request via the api or moving forward all new publishing is to be managed. 
				apr.publishMethodURI = CE_PUBLISH_METHOD_MANUAL_ENTRY;
			}
			//			
			List<string> messages = new List<string>();
			bool skippingValidation = SkippingValidation ? true : UtilityManager.GetAppKeyValue("skippingValidation") == "yes";
            if ( request.Payload.ToLower().IndexOf( "@graph" ) > 0 )
                skippingValidation = true;
            if ( skippingValidation )
            {
				apr.skipValidation = "true";
			}


			//
			//var serializer = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
			//var p2 = serializer.Serialize( apr );

			string postBody = JsonConvert.SerializeObject( apr, ServiceHelperV2.GetJsonSettings() );


			//get accounts url
			string serviceUri = UtilityManager.GetAppKeyValue( "accountsPublishApi" );

			string queryString = GetRequestContext();
			string environment = UtilityManager.GetAppKeyValue( "environment" );
			var domainWarning = "";
			EntityNameForHistory = EntityName;
			if ( environment=="development" && System.DateTime.Now.Day == 18 )
			{
				//serviceUri = "https://localhost:44384/registry/publishStream/";
			}
			if ( environment == "production" && queryString.IndexOf( "//credentialengine.org/assistant" ) > -1 )
			{
				domainWarning = UtilityManager.GetAppKeyValue( "domainWarning" );
				EntityNameForHistory += " (**old domain**)";
			}
			SupportServices supportServices = new SupportServices( EntityNameForHistory, Community );
			//
			//this step is redundant for now (will also be done in accounts). However will be moot once we stop using the accounts method.
			var trxCheck = supportServices.ValidateRegistryRequest( apiKey, request.OwnerCtid, apr.publishMethodURI, ref messages );
			if ( trxCheck.Successful == false )
			{
				valid = false;
				request.SetMessages( messages );
				return "";
			}
			var isThirdPartyEvent = false;
			if ( trxCheck.OwningOrganization != trxCheck.PublishingOrganization )
				isThirdPartyEvent = true;

			string contents = "";
			DateTime started = DateTime.Now;
			LoggingHelper.DoTrace( 6, string.Format("RegistryServices.ManagedPublishThroughAcccounts - dataOwnerCTID: {0}, url: {1}", dataOwnerCTID, serviceUri) );
			var summary = "";
			if( isThirdPartyEvent )
				summary = string.Format( "Publisher CTID:'{0}', DataOwner CTID: '{1}', Publish Method: '{2}', Publishing EntityType: '{3}', Entity Ctid:'{4}', Entity Name: '{5}'. ", PublishingByOrgCtid, dataOwnerCTID, apr.publishMethodURI, PublishingEntityType, EntityCtid, EntityNameForHistory );
			else
				summary = string.Format( "DataOwner CTID:'{0}', Publish Method: '{1}', Publishing EntityType: '{2}', Entity Ctid:'{3}', Entity Name: '{4}'. ", PublishingByOrgCtid, dataOwnerCTID, apr.publishMethodURI, PublishingEntityType, EntityCtid, EntityNameForHistory );

			//prepare activity
			ActivityLog activity = new ActivityLog()
			{
				Application = "RegistryAssistant",
				ActivityType = PublishingEntityType,
				Activity = "Credential Registry",
				Event = "ManagedPublish",   //temp?
				ActivityObjectCTID = EntityCtid,
				PublishingOrganizationCTID = trxCheck.PublishingOrganizationCTID,
				DataOwnerCTID = request.OwnerCtid,
				IPAddress = request.IPAddress       //this will need to be provided for each publish type
			};
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					//client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
					if( postBody.Length > 1000000 )
					{
						client.Timeout = new TimeSpan( 0, 30, 0 );

						LoggingHelper.DoTrace( 1, string.Format( "RegistryServices.DoManagedPublishThroughAccounts(). *******publish note. Larger request.Payload: {0} bytes, using stream endpoint. ", postBody.Length ) );
						//use stream endpoint - maybe always use stream - makes sense to only have one??
						//all use stream
						//serviceUri = UtilityManager.GetAppKeyValue( "accountsPublishStreamApi" );
					}
					

					var task = client.PostAsync( serviceUri,
						new StringContent( postBody, Encoding.UTF8, "application/json" ) );
					task.Wait();
					var response = task.Result;

					TimeSpan duration = DateTime.Now.Subtract( started );
					if ( duration.TotalSeconds > 3 )
						LoggingHelper.DoTrace( 1, string.Format( " *******publish took a little longer: elapsed: {0:N2} seconds ", duration.TotalSeconds ) );
					//should get envelope_id from contents?
					//the accounts endpoint will return the registry response verbatim 
					contents = task.Result.Content.ReadAsStringAsync().Result;
					bool responseStatusCode = response.IsSuccessStatusCode;
					if ( (contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
						|| contents.ToLower().IndexOf( "<html>" ) > 0
						|| ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "error" ) < 15 && contents.ToLower().IndexOf( "{" ) > -1 )
						)
						responseStatusCode = false;

					if ( responseStatusCode == false )
					{
                        //note the accounts publish will always return successful otherwise any error messages get lost
                        if ( contents.ToLower().IndexOf( "accountresponse" ) > 0  )
                        {
                            AccountPublishResponse acctResponse = JsonConvert.DeserializeObject<AccountPublishResponse>( contents );
                            if ( acctResponse.Messages != null && acctResponse.Messages.Count > 0 )
                            {
                                status = string.Join( ",", acctResponse.Messages.ToArray() );
                                request.SetMessages( acctResponse.Messages );
                            }
                        }
						else if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
						{
							valid = false;
							status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
							request.AddError( status );
						}
						else if ( contents.ToLower().IndexOf( "<html>" ) > 0 )
						{
							int pos = contents.IndexOf( "title" );
							int next = contents.IndexOf( "</title>" );
							if ( pos > 0 && next > pos )
							{
								status = contents.Substring( pos + 6, next - (pos + 6) );
							}
							else
							{
								status = contents;
							}
							request.AddError( status );

						}
						if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "error" ) < 15 && contents.ToLower().IndexOf( "{" ) > -1 )
						{
							//handle double [[
							valid = false;
							contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
							RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
							if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
							{
								status = string.Join( ",", contentsJson.Errors.ToArray() );
								//remove extraneous message of:@type : did not match one of the following values
								if ( status.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
								{
									status = status.Substring( 0, status.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]";
								}
								request.SetMessages( contentsJson.Errors );
							}
							else
							{
								status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
								request.AddError( status );
							}
						}
                        else
                        {
                            status = contents;
							request.AddError( status );
                        }
						
						//
						valid = false;
						//
						var msgs = string.Join(";", request.GetAllErrorMessages().ToArray());
						activity.Comment = string.Format( "Publish attempt failed for Organization: '{0}', type: '{1}' Name: '{2}', reason(s): {3}.", trxCheck.OwningOrganization, PublishingEntityType, EntityNameForHistory, msgs );
						supportServices.AddActivity( activity, ref status );
						//now null:
						//+ "\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )
						LoggingHelper.LogError( identifier + " RegistryServices.Publish Failed:"
							+ "\n\rURL:\n\r " + queryString
							+ "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject( response )
							+ "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject( contents ),
							false, "CredentialRegistry publish failed for " + identifier );
						//if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
						//{
						//	status = string.Join( ",", contentsJson.Errors.ToArray() );
						//	messages.AddRange( contentsJson.Errors );
						//}
						//else
						//{
						//	status = contents;
						//	messages.Add( status );
						//}

						LoggingHelper.WriteLogFile( 5, identifier + "_payload_failed.json", request.Payload, "", false );
					}
					else
					{
                        //accounts publisher can return errors like: {"data":null,"valid":false,"status":"Error: Owning organization not found.","extra":null}
      //                  if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
      //                  {
      //                      valid = false;
      //                      status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
						//	request.AddError( status );
						//}
						
      //                  else
						//if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "error" ) < 15 && contents.ToLower().IndexOf( "{" ) > -1 )
      //                  {
      //                      valid = false;
      //                      contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
      //                      RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
      //                      if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
      //                      {
      //                          status = string.Join( ",", contentsJson.Errors.ToArray() );
      //                          //remove extraneous message of:@type : did not match one of the following values
      //                          if (status.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
      //                          {
      //                              status = status.Substring( 0, status.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]";
      //                          }
      //                          request.SetMessages( contentsJson.Errors );
      //                      }
      //                      else
      //                      {
      //                          status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
						//		request.AddError( status );
      //                      }

						//}
      //                  else
                        {
							valid = true;//summary
							LoggingHelper.DoTrace( 5, "Successful publish through accounts api.\r\n" + summary );
							LoggingHelper.DoTrace( 7, "contents after successful managed publish.\r\n" + contents );

							UpdateEnvelope ue = new UpdateEnvelope();
							AccountApiResponse aar = JsonConvert.DeserializeObject<AccountApiResponse>( contents );
							if (aar.Successful)
							{
								if ( aar.ResultContent.IndexOf( "{\"errors\":[" ) > -1 )
								{
									status = aar.ResultContent.Replace( "[[", "[" ).Replace( "]]", "]" );
									request.AddError( status );
									valid = false;
								}
								else
								{
									ue = JsonConvert.DeserializeObject<UpdateEnvelope>( aar.ResultContent );
									//verify
									if ( HasBeenPreviouslyPublished )
										WasChanged = ue.Changed;
									else
										WasChanged = true;

									crEnvelopeId = ue.EnvelopeIdentifier;
									var envelopeUpdatedAt = ue.NodeHeader.UpdatedAt;

									LoggingHelper.DoTrace( 5, string.Format( "Returned EnvelopeId: {0}, Elapsed: {1:N2}, WasChanged: {2}", crEnvelopeId, duration.TotalSeconds, ue.Changed ) );

									LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", request.Payload, "", false );

									SupportServices ss = new SupportServices();
									if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", true ) )
									{
										//SupportServices mgr = new SupportServices( EntityNameForHistory, Community );
										supportServices.AddHistory( dataOwnerCTID, request.Payload, apr.publishMethodURI, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status, PublishingByOrgCtid, ue.Changed, !HasBeenPreviouslyPublished );
									}

									
									if ( trxCheck.OwningOrganization == trxCheck.PublishingOrganization )
										activity.Comment = string.Format( "Organization: '{0}' published the '{1}': '{2}'.", trxCheck.OwningOrganization, PublishingEntityType, EntityNameForHistory );
									else
										activity.Comment = string.Format( "Third Party Organization: '{0}' published the '{1}': '{2}' for Organization: '{3}'", trxCheck.PublishingOrganization, PublishingEntityType, EntityNameForHistory, trxCheck.OwningOrganization );
									supportServices.AddActivity( activity, ref status );
								}
							} else
							{
								//force this to prevent misleading message
								WasChanged = true;
								valid = false;
								request.SetMessages( aar.Messages );
								status = string.Join( ",", request.GetAllMessages().ToArray() );
							}
                        }
					}

					if ( !string.IsNullOrWhiteSpace( domainWarning ) )
					{
						LoggingHelper.DoTrace( 5, string.Format( "*********** old domain encountered ******** DataOwnerCtid: {0}, PublishingEntityType: {1}, EntityCtid: {2}", PublishingForOrgCtid, PublishingEntityType, EntityCtid ) );
						request.AddWarning( domainWarning );
						status += domainWarning;
						
					}
					return contents;
				}
			}
			catch ( Exception ex )
			{
				TimeSpan duration = DateTime.Now.Subtract( started );
				if ( duration.TotalSeconds > 2 )
					LoggingHelper.DoTrace( 1, string.Format( " *******Managed publish exception. Elapsed: {0:N2} seconds ", duration.TotalSeconds ) );

				var ipAddress = GetSourceIPAddress();
				LoggingHelper.LogError( ex, string.Format( "RegistryServices.ManagedPublishThroughAcccounts. identifier: {0}, apiKey: {1}, ipAddress: {2}, contents: ", identifier, apiKey, ipAddress ) + contents );
				valid = false;
				status = "Managed Publish Exception: " + LoggingHelper.FormatExceptions( ex );
				request.AddError( status );
				return status;
			}
			finally
			{
				LoggingHelper.DoTrace( 6, "RegistryServices.DoManagedPublishThroughAccounts. Exited. " );
			}

		}

		/*
		 * not sure of intent for this?
		public string ManagedPublishThroughAccountsNEW(string payload,
		string apiKey,
		string dataOwnerCTID,
		string identifier,
		ref bool valid,
		ref string status,
		ref string crEnvelopeId)
		{
			valid = true;
			List<string> messages = new List<string>();
			//Now, need set the publish method to manual entry of credentials, or competencies. 
			//or have the account system figure it out!
			AccountPublishRequest apr = new AccountPublishRequest
			{
				payloadJSON = payload,
				apiKey = apiKey,
				dataOwnerCTID = dataOwnerCTID,
				publishMethodURI = RA_PUBLISH_METHOD,  //not necessarily
				publishingEntityType = PublishingEntityType,
				ctdlType = CtdlType,
				entityCtid = EntityCtid,
				community = Community
				//serializedInput = SerializedInput
			};
			if( OverrodeOriginalRequest )
			{
				//meand the request came from the manual publisher and there was either a previous request via the api or moving forward all new publishing is to be managed. 
				apr.publishMethodURI = CE_PUBLISH_METHOD_MANUAL_ENTRY;
			}
			//



			bool skippingValidation = SkippingValidation ? true : UtilityManager.GetAppKeyValue( "skippingValidation" ) == "yes";
			if( payload.ToLower().IndexOf( "@graph" ) > 0 )
				skippingValidation = true;
			if( skippingValidation )
			{
				apr.skipValidation = "true";
			}

			//var serializer = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
			//var p2 = serializer.Serialize( apr );

			string postBody = JsonConvert.SerializeObject( apr, ServiceHelperV2.GetJsonSettings() );
			//get accounts url
			string serviceUri = UtilityManager.GetAppKeyValue( "accountsPublishApi" );
			string environment = UtilityManager.GetAppKeyValue( "environment" );
			//https://localhost:44320/publish/publishRequest
			if( DateTime.Now.ToString( "yyyy-MM-dd" ) == "2019-11-14" && environment == "development" )
			{
				//serviceUri = "https://localhost:44320/registry/publishStream";
			}

			if( postBody.Length > 1000000 )
			{
				LoggingHelper.DoTrace( 1, string.Format( "RegistryServices.DoManagedPublishThroughAccounts(). *******publish note. Larger payload: {0} bytes, using stream endpoint. ", postBody.Length ) );
				//use stream endpoint - maybe always use stream - makes sense to only have one??
				serviceUri = UtilityManager.GetAppKeyValue( "accountsPublishStreamApi" );
			}
			string contents = "";
			DateTime started = DateTime.Now;
			LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.ManagedPublishThroughAcccountsNew - dataOwnerCTID: {0}, url: {1}", dataOwnerCTID, serviceUri ) );
			try
			{
				using( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					//client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );

					var task = client.PostAsync( serviceUri,
						new StringContent( postBody, Encoding.UTF8, "application/json" ) );
					task.Wait();
					var response = task.Result;

					TimeSpan duration = DateTime.Now.Subtract( started );
					if( duration.TotalSeconds > 3 )
						LoggingHelper.DoTrace( 1, string.Format( " *******publish took a little longer: elapsed: {0:N2} seconds ", duration.TotalSeconds ) );
					//should get envelope_id from contents?
					//the accounts endpoint will return the registry response verbatim 
					contents = task.Result.Content.ReadAsStringAsync().Result;

					if( response.IsSuccessStatusCode == false )
					{
						//note the accounts publish will always return successful otherwise any error messages get lost
						if( contents.ToLower().IndexOf( "accountresponse" ) > 0 )
						{
							AccountPublishResponse acctResponse = JsonConvert.DeserializeObject<AccountPublishResponse>( contents );
							if( acctResponse.Messages != null && acctResponse.Messages.Count > 0 )
							{
								status = string.Join( ",", acctResponse.Messages.ToArray() );
								messages.AddRange( acctResponse.Messages );
							}
						}
						else if( contents.ToLower().IndexOf( "<html>" ) > 0 )
						{
							int pos = contents.IndexOf( "title" );
							int next = contents.IndexOf( "</title>" );
							if( pos > 0 && next > pos )
							{
								status = contents.Substring( pos + 6, next - (pos + 6) );
							}
							else
							{
								status = contents;
							}
						}
						else if( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "{" ) > -1 )
						{
							//handle double [[
							contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
							RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
							if( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
							{
								status = string.Join( ",", contentsJson.Errors.ToArray() );
								messages.AddRange( contentsJson.Errors );
							}
						}
						else
						{
							status = contents;
							messages.Add( status );
						}
						//
						valid = false;
						string queryString = GetRequestContext();
						//now null:
						//+ "\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )
						LoggingHelper.LogError( identifier + " RegistryServices.Publish Failed:"
							+ "\n\rURL:\n\r " + queryString
							+ "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject( response )
							+ "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject( contents ),
							false, "CredentialRegistry publish failed for " + identifier );
						//if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
						//{
						//	status = string.Join( ",", contentsJson.Errors.ToArray() );
						//	messages.AddRange( contentsJson.Errors );
						//}
						//else
						//{
						//	status = contents;
						//	messages.Add( status );
						//}

						LoggingHelper.WriteLogFile( 5, identifier + "_payload_failed.json", payload, "", false );
					}
					else
					{
						//accounts publisher can return errors like: {"data":null,"valid":false,"status":"Error: Owning organization not found.","extra":null}
						if( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
						{
							valid = false;
							status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
							messages.Add( status );
						}
						else if( contents.ToLower().IndexOf( "<html>" ) > 0 )
						{
							int pos = contents.IndexOf( "title" );
							int next = contents.IndexOf( "</title>" );
							if( pos > 0 && next > pos )
							{
								status = contents.Substring( pos + 5, contents.Length - next );
							}
							else
							{
								status = contents;
							}
						}
						else if( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "error" ) < 15 && contents.ToLower().IndexOf( "{" ) > -1 )
						{
							valid = false;
							contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
							RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
							if( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
							{
								status = string.Join( ",", contentsJson.Errors.ToArray() );
								//remove extraneous message of:@type : did not match one of the following values
								if( status.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
								{
									status = status.Substring( 0, status.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]";
								}
								messages.AddRange( contentsJson.Errors );
							}
							else
							{
								status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
								messages.Add( status );
							}

						}
						else
						{
							valid = true;//
							LoggingHelper.DoTrace( 7, "contents after successful managed publish.\r\n" + contents );

							UpdateEnvelope ue = new UpdateEnvelope();
							AccountApiResponse aar = JsonConvert.DeserializeObject<AccountApiResponse>( contents );
							if( aar.Successful )
							{
								ue = JsonConvert.DeserializeObject<UpdateEnvelope>( aar.ResultContent );
								//check if this can be false on an add
								//verify
								if ( HasBeenPreviouslyPublished )
									WasChanged = ue.Changed;
								else
									WasChanged = true;
								crEnvelopeId = ue.EnvelopeIdentifier;
								var envelopeUpdatedAt = ue.NodeHeader.UpdatedAt;
							}

							LoggingHelper.DoTrace( 5, string.Format( "Returned EnvelopeId: {0}, Elapsed: {1:N2}, WasChanged: {2}", crEnvelopeId, duration.TotalSeconds, ue.Changed ) );

							LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", payload, "", false );

							if( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", true ) )
							{
								//TODO record the updated_at date from the envelope - or pass envelope
								SupportServices mgr = new SupportServices( EntityName, Community );
								mgr.AddHistory( dataOwnerCTID, payload, apr.publishMethodURI, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status, PublishingByOrgCtid, ue.Changed );
							}
						}
					}
					return contents;
				}
			}
			catch( Exception ex )
			{
				TimeSpan duration = DateTime.Now.Subtract( started );
				if( duration.TotalSeconds > 2 )
					LoggingHelper.DoTrace( 1, string.Format( " *******Managed publish exception. Elapsed: {0:N2} seconds ", duration.TotalSeconds ) );

				var ipAddress = GetSourceIPAddress();
				LoggingHelper.LogError( ex, string.Format( "RegistryServices.ManagedPublishThroughAcccounts. identifier: {0}, apiKey: {1}, ipAddress: {2}, contents: ", identifier, apiKey, ipAddress ) + contents );
				valid = false;
				status = "Managed Publish Exception: " + LoggingHelper.FormatExceptions( ex );
				messages.Add( status );
				return status;
			}

		}
		*/
		//

		public static string GetRequestContext()
		{
			string queryString = "batch";
			try
			{
				queryString = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
			}
			catch ( Exception exc )
			{
				return "";
			}
			return queryString;
		}

		#endregion

		#region Deletes
		/// <summary>
		/// Handle request to delete a document
		/// </summary>
		/// <param name="request"></param>
		/// <param name="requestType"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool DeleteRequest( DeleteRequest request, string requestType, ref List<string> messages )
		{
			//=================================
			//2020-06 made this the default
			//if ( UtilityManager.GetAppKeyValue( "usingAssistantDirectDelete", false ) )
				return DoDirectDelete( request, ref messages );

			//=================================
			//bool isValid = true;
			//messages = new List<string>();
			//string statusMessage = "";
			//RA.Models.RequestHelper helper = new Models.RequestHelper();

			//try
			//{
			//	//envelopeId is not applicable for managed keys!, could have a separate endpoint?
			//	if ( request == null
			//		|| string.IsNullOrWhiteSpace( request.CTID )
			//		|| string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
			//		)
			//	{
			//		messages.Add( "Error - please provide a valid delete request with a CTID, and the owning organization." );
			//		return false;
			//	}

			//	helper.OwnerCtid = request.PublishForOrganizationIdentifier;
			//	if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
			//	{
			//		messages.Add( statusMessage );
			//		return false;
			//	}


			//	// check if delete method should change
			//	bool recordWasFound = false;
			//	bool usedCEKeys = false;
			//	string message = "";
			//	//May need to include community in look up!
			//	var result = SupportServices.GetMostRecentHistory(  request.CTID, ref recordWasFound, ref usedCEKeys, ref message, request.Community ?? "" );
			//	if ( recordWasFound )
			//	{
			//		//found previous (if not found, can't delete?)
			//		if ( result.DataOwnerCTID.ToLower() != helper.OwnerCtid.ToLower() )
			//		{
			//			//don't allow but may be moot if validating the apiKey owner ctid combination
			//			messages.Add( string.Format( "Error: Suspcious request. The provided data owner CTID is different from the data owner CTID used for previous requests. This condition is not allowed. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid ) );
			//			return false;
			//		}
			//		else
			//		if ( usedCEKeys ) //only allow override outside of production for now && env != "production"
			//		{
			//			//need to require the api key, and to validate latter
			//			LoggingHelper.DoTrace( 5, requestType + " Delete. Last publish was CE request. Overriding to envelopeDelete." );
			//			if ( helper.IsPublisherRequest )
			//			{
			//				//not really necessary?
			//				EnvelopeDelete edRequest = new EnvelopeDelete()
			//				{
			//					RegistryEnvelopeId = result.EnvelopeId,
			//					CTID = request.CTID,
			//					PublishForOrganizationIdentifier = request.PublishForOrganizationIdentifier, 
			//					Community= request.Community ?? ""
			//				};

			//				return DeleteByEnvelopeIdentifier( edRequest, requestType, ref messages );
			//			}
			//			else
			//			{
			//				//could allow
			//				if ( helper.ApiKey != null && helper.ApiKey.Length == 32 )
			//				{
			//					//could encrypt api key and save in history. Except the api key could change. 
			//				}

			//				messages.Add( string.Format( "Terribly sorry, this {2} was published by the CE Publisher app, you don't have the necessary privileges to delete the record. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid, requestType ) );
			//				LoggingHelper.DoTrace( 5, string.Format( "Terribly sorry, this {2} was published by the CE Publisher app, you don't have the necessary privileges to delete the record. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid, requestType ) );
			//				return false;
			//			}
			//		}
			//		else
			//		{
			//			//continue?
			//		}
			//	} 
			//	else
			//	{
			//		//just in case, fall thru and attempt managed delete
			//		//for dev could have been done in the sandbox server?
			//		LoggingHelper.DoTrace( 5, string.Format( "RegistryServices.DeleteRequest. No history was found. Falling thru to a managed delete. RequestType: {0}, CTID: {1}, Data Owner CTID '{2}'.", requestType, request.CTID, helper.OwnerCtid ) );

			//	}
					
			//	messages = new List<string>();
			//	RegistryServices cer = new RegistryServices( requestType, "", request.CTID );
			//	isValid = cer.ManagedDelete( request, helper.ApiKey, ref messages );
			//	//if ( !string.IsNullOrWhiteSpace( statusMessage ) )
			//	if (!isValid)
			//	{
			//		if (messages != null && messages.Count() == 1 && messages[0] == "No matching resource found" )
			//		{
			//			//try envelope delete
			//			messages.Add( "Perhaps you should try delete by envelope using '/envelopeDelete'." );
			//		}
			//	}
			//	//	messages.Add( statusMessage );
				
			//}
			//catch ( Exception ex )
			//{
			//	messages.Add( ex.Message );
			//	isValid = false;
			//}
			//return isValid;
		} //
		/// <summary>
		/// Custom delete using delete envelope
		/// THIS WILL BE REMOVED BY JUST TAKING APPROACH TO DO A PURGE
		/// </summary>
		/// <param name="request"></param>
		/// <param name="requestType"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		private bool DeleteByEnvelopeIdentifier( EnvelopeDelete request, string requestType, ref List<string> messages )
		{
			bool isValid = true;
			messages = new List<string>();
			string statusMessage = "";
			RA.Models.RequestHelper helper = new Models.RequestHelper();

			try
			{
				//envelopeId is not applicable for managed keys!, could have a separate endpoint?
				if ( request == null
					|| string.IsNullOrWhiteSpace( request.CTID )
					|| string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
					|| string.IsNullOrWhiteSpace( request.RegistryEnvelopeId )
					)
				{
					messages.Add( "Error - please provide a valid delete request with a CTID, the envelope Id, and the owning organization." );
					return false;
				}
				//ACTUALLY not used directly here, will need mechanism to ensure a valid request. 
				helper.OwnerCtid = request.PublishForOrganizationIdentifier;
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
				{
					messages.Add( statusMessage );
				}
				else
				{
					//only valid token at this time is from CER
					RegistryServices cer = new RegistryServices( requestType, "", request.CTID );
					EnvelopeDelete edrequest = new EnvelopeDelete()
					{
						CTID = request.CTID,
						RegistryEnvelopeId = request.RegistryEnvelopeId,
						Community = request.Community ?? ""
					};
					isValid = cer.CredentialRegistry_SelfManagedKeysDelete( edrequest, "registry assistant", ref statusMessage );

				}
			}
			catch ( Exception ex )
			{
				messages.Add( ex.Message );
				isValid = false;
			}
			return isValid;
		} //
		//public bool ManagedDelete(string dataOwnerCTID, string ctid, string apiKey, ref List<string> messages)
		//{
		//	DeleteRequest request = new DeleteRequest()
		//	{
		//		CTID = ctid,
		//		PublishForOrganizationIdentifier = dataOwnerCTID,
		//		Community = ""
		//	};
		//	return ManagedDelete( request, apiKey, ref messages );
		//} //
		public bool ManagedDelete(DeleteRequest request, string apiKey, ref List<string> messages)
		{
			//2020-06 made this the default
			//if ( UtilityManager.GetAppKeyValue( "usingAssistantDirectDelete", false ) )
				return DoDirectDelete( request, ref messages );


			//bool valid = true;
			//AccountDeleteRequest apr = new AccountDeleteRequest
			//{
			//	apiKey = apiKey,
			//	dataOwnerCTID = request.PublishForOrganizationIdentifier,
			//	publishMethodURI = RA_PUBLISH_METHOD,
			//	publishingEntityType = PublishingEntityType,
			//	entityCtid = request.CTID,
			//	community = request.Community ?? ""
			//};
			//var url = string.Format( "?apiKey={0}&dataOwnerCTID={1}&publishingEntityType={2}&entityCtid={3}&community={4}", apiKey, request.PublishForOrganizationIdentifier, PublishingEntityType, request.CTID, request.Community);

			//string postBody = JsonConvert.SerializeObject( apr );
			//string environment = UtilityManager.GetAppKeyValue( "environment" );
			//string serviceUri = UtilityManager.GetAppKeyValue( "accountsDeleteApi" );
			//if ( DateTime.Now.ToString( "yyyy-MM-dd" ) == "2020-04-17" && environment == "development" )
			//{
			//	//serviceUri = "https://localhost:44384/registry/delete";
			//}
			//serviceUri += url;
			//string contents = "";
   //         try
   //         {
   //             using ( var client = new HttpClient() )
   //             {
   //                 client.DefaultRequestHeaders.
   //                     Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
			//		if ( !string.IsNullOrWhiteSpace( apiKey ) )
			//		{
			//			client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );
			//		}
			//		if ( environment == "development" )
			//		{
			//			client.Timeout = new TimeSpan( 0, 30, 0 );
			//		}
			//		//account site will do actual delete, so not sure if this should be a delete request
			//		HttpRequestMessage requestMsg = new HttpRequestMessage
   //                 {
   //                     Content = new StringContent( postBody, Encoding.UTF8, "application/json" ),
   //                     Method = HttpMethod.Delete,
   //                     RequestUri = new Uri( serviceUri )
   //                 };
   //                 var task = client.SendAsync( requestMsg );

			//		task.Wait();
   //                 var response = task.Result;
   //                 contents = task.Result.Content.ReadAsStringAsync().Result;

   //                 //will always return successful from accounts
   //                 if ( response.IsSuccessStatusCode == false )
   //                 {
   //                     //logging???
   //                     //response = contents.Result;
   //                     LoggingHelper.LogError( "RegistryServices.DeleteRequest Failed\n\r" + response + "\n\rError: " + JsonConvert.SerializeObject( contents ) );
			//			if ( contents.IndexOf("Message\":") > -1)
			//			{
			//				messages.Add( contents );
			//				return false;
			//			}
   //                     RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
   //                     messages.Add( string.Join( ",", contentsJson.Errors.ToArray()) );
   //                     return false;
   //                 }
   //                 else
   //                 {
   //                     if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
   //                     {
   //                         valid = false;
   //                         messages.Add( UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" ));
   //                         return false;
   //                     }
   //                     else if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "{" ) > -1 )
   //                     {
   //                         valid = false;
   //                         contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
   //                         RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
   //                         if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
   //                         {
   //                             string message = string.Join( ",", contentsJson.Errors.ToArray() );
   //                             //remove extraneous message of:@type : did not match one of the following values
   //                             if ( message.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
   //                             {
   //                                 messages.Add( message.Substring( 0, message.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]");
   //                             }
   //                         }
   //                         else
   //                         {
   //                             messages.Add( UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" ));
   //                         }
   //                         return false;
   //                     }
   //                     else
   //                     {
			//				AccountApiResponse aar = JsonConvert.DeserializeObject<AccountApiResponse>( contents );
			//				if ( aar.Successful )
			//				{
			//					//not sure what will be returned upon successful delete - apparantly nothing
			//					messages.Add( contents );
			//					LoggingHelper.DoTrace( 5, string.Format( "Delete Successful. Type: {0}, CTID: {1}, OwnerCtid: {2}, contents: {3}", PublishingEntityType, EntityCtid, request.PublishForOrganizationIdentifier, contents ) );

			//					if ( UtilityManager.GetAppKeyValue( "loggingDeleteHistory", false ) )
			//					{
			//						//20-03-12 - start logging deletes
			//						string status = "";
			//						SupportServices mgr = new SupportServices( "Deleted", request.Community );
			//						mgr.AddHistory( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) ? "missing" : request.PublishForOrganizationIdentifier, "", REGISTRY_ACTION_DELETE, PublishingEntityType, PublishingEntityType, EntityCtid, "", "none", ref status, apiKey, true );
			//					}
			//				} else
			//				{
			//					valid = false;
			//					messages.AddRange( aar.Messages );
			//				}	
   //                     }
   //                 }
   //                 return valid;
   //             }
   //         }
   //         catch ( Exception exc )
   //         {
			//	var ipAddress = GetSourceIPAddress();
			//	LoggingHelper.LogError( exc, string.Format( "RegistryServices.ManagedDelete - On POST. ctid: {0}, dataOwnerCTID: {1}, ipAddress: {2}", request.CTID, request.PublishForOrganizationIdentifier, ipAddress ) );
			//	valid = false;
   //             messages.Add( exc.Message );
   //             return false;
   //         }
		}

		/// <summary>
		/// Call registry delete directly (not thru accounts)
		/// </summary>
		/// <param name="request"></param>
		/// <param name="apiKey"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool DoDirectDelete(DeleteRequest request, ref List<string> messages)
		{
			bool isValid = true;
			string statusMessage = "";
			RA.Models.RequestHelper helper = new Models.RequestHelper();

			if ( request == null
				|| string.IsNullOrWhiteSpace( request.CTID )
				|| string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier )
				)
			{
				messages.Add( "Error - please provide a valid delete request with a CTID, and the owning organization." );
				//return false;
			}
			else
			{
				if ( string.IsNullOrWhiteSpace( request.CTID ) )
					messages.Add( "Error: The Entity CTID must be provided." );
				//

				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
					messages.Add( "Error: The PublishForOrganizationIdentifier (Data Owner CTID) must be provided." );
			}
			helper.OwnerCtid = request.PublishForOrganizationIdentifier;
			//validateRequest will populate the API key
			if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage, true ) )
			{
				messages.Add( statusMessage );
				//return false;
			}
			else if ( string.IsNullOrWhiteSpace( helper.ApiKey ) )
			{
				messages.Add( "Error: The Publisher Organization apiKey must be provided." );
				//return false;
			}
			//
			var summary = string.Format( "DataOwner CTID:'{0}', EntityType: '{1}', Entity Ctid:'{2}'. ", request.PublishForOrganizationIdentifier, PublishingEntityType, request.CTID );
			if ( messages.Any() )
			{
				LoggingHelper.DoTrace( 2, "Delete request failed: " + summary + ". " + string.Join( ":", messages.ToArray() ) );
				return false;
			}
			//
			//May need to include community in look up!
			//should probably still check history. Can't let just anyone delete stuff
			bool recordWasFound = false;
			bool usedCEKeys = false;
			string message = "";
			//20-09-28 mp - should get history first. Need to consider in the sandbox, the owning org doesn't have to have an account
			//			- OR, this could be an opportunity to do JIT registration!!
			var history = SupportServices.GetMostRecentHistory( request.CTID, ref recordWasFound, ref usedCEKeys, ref message, request.Community ?? "" );
			if ( recordWasFound )
			{
				//found previous (if not found, can't delete?)
				if ( history.DataOwnerCTID.ToLower() != helper.OwnerCtid.ToLower() )
				{
					//don't allow but may be moot if validating the apiKey owner ctid combination
					messages.Add( string.Format( "Error: Suspcious request. The provided data owner CTID is different from the data owner CTID used for previous requests. This condition is not allowed. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid ) );
					return false;
				}
				else if ( usedCEKeys ) //should no longer matter - TBD
				{
					//check env. If sandbox, may require an envelope delete - leads to making a change to use CE registry keys?

					//messages.Add( string.Format( "Terribly sorry, this {2} was published by the CE Publisher app, you don't have the necessary privileges to delete the record. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid, requestType ) );
					//LoggingHelper.DoTrace( 5, string.Format( "Terribly sorry, this {2} was published by the CE Publisher app, you don't have the necessary privileges to delete the record. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid, requestType ) );
					//return false;

				}
				else
				{
					//continue?
					if (history.PublishMethodURI == RegistryServices.REGISTRY_ACTION_TRANSFER)
					{

					}
				}
			}

			//validate request 
			var trxCheck = new SupportServices().ValidateRegistryRequest( helper.ApiKey, request.PublishForOrganizationIdentifier, RegistryServices.REGISTRY_ACTION_DELETE, ref messages );
			if ( trxCheck.Successful == false )
			{
				return false;
			}
			var isThirdPartyEvent = false;
			if ( trxCheck.OwningOrganization != trxCheck.PublishingOrganization )
				isThirdPartyEvent = true;
			//
			ActivityLog activity = new ActivityLog()
			{
				Application = "RegistryAssistant",
				ActivityType = PublishingEntityType,	//don't have the type at this time? - actually will when called from individual controllers. No name
				Activity = "Credential Registry",
				Event = "Delete",   //temp?
				ActivityObjectCTID = request.CTID,
				PublishingOrganizationCTID = trxCheck.PublishingOrganizationCTID,
				DataOwnerCTID = request.PublishForOrganizationIdentifier,
				IPAddress = GetSourceIPAddress() 
			};
			//
			

			//not found, log and try anyway

			// =========================================================
			//Lookup key
			var registryAuthorizationToken = UtilityManager.GetAppKeyValue( "CredentialRegistryAuthorizationToken" );
			string registryEndpoint = UtilityManager.GetAppKeyValue( "CredentialRegistryDeleteEndpoint" );
			//	https://credentialengineregistry.org/{0}resources/documents/{1}"
			if ( !string.IsNullOrWhiteSpace( request.Community ) )
			{
				//adjust to include in endpoint
				request.Community += "/";
			}

			if ( DateTime.Now.ToString( "yyyy-MM-dd" ) == "2020-02-11" && currentEnvironment == "development" )
			{
				//serviceUri = "https://localhost:44384/registry/delete";
			}
			

			//

			//should we first get the record for references?
			string ctdlType = "";
			var ro = new RegistryObject();
			//look up document (good to get for stats)


			var document = RegistryServices.GetResource( request.CTID, ref ctdlType, ref statusMessage, request.Community );
			if ( document == null || document.Length == 0 || ( document.IndexOf( "\"errors\":" ) > -1 && document.IndexOf( "\"errors\":" ) < 100 ) )
			{
				//need a better check that this - maybe just continue
				//|| document.IndexOf( "\"errors\":" ) > -1
				messages.Add( "Error: The requested document was not found, so you could say the Delete was successful, or maybe check the input CTID! " + statusMessage );
				//TODO - LOG ACTIVITY FOR REFERENCE
				return false;
			}
			else
			{
				//20-06-12 mp - this was already done in: var document = RegistryServices.GetResource
				ro = RegistryServices.GetResourceObject( document );
			}
			var entityName = ro.Name ?? "Missing Name".ToString();
			var requestUrl = string.Format( registryEndpoint, request.Community, request.CTID );
			

			//var summary = "";
			if ( isThirdPartyEvent )
				summary = string.Format( "Publisher (CTID):'{0}' ({1}) for DataOwner (CTID): '{2}' ({3}), deleted EntityType: '{4}', Entity Ctid:'{5}', Entity Name: '{6}'. ", trxCheck.PublishingOrganization, trxCheck.PublishingOrganizationCTID, trxCheck.OwningOrganization, request.PublishForOrganizationIdentifier, PublishingEntityType, request.CTID, entityName );
			else
				summary = string.Format( "DataOwner (CTID):'{0}' ({1}), deleted EntityType: '{2}', Entity Ctid:'{3}', Entity Name: '{4}'. ", trxCheck.OwningOrganization, request.PublishForOrganizationIdentifier, PublishingEntityType, request.CTID, entityName );
			//
			LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".Delete request for " + summary ) );
			//
			SupportServices supportServices = new SupportServices( entityName, Community );

			try
			{
				using ( var client = new HttpClient() )
				{

					client.DefaultRequestHeaders.Add( "Authorization", "Token " + registryAuthorizationToken );
					//expected format is a delete:
					/*
						DELETE /resources/documents/{ctid}
						DELETE /{community_name}/resources/documents/{ctid}?purge=false
					 * */
					HttpRequestMessage requestMsg = new HttpRequestMessage
					{
						Method = HttpMethod.Delete,
						RequestUri = new Uri( requestUrl )
					};

					var result = client.SendAsync( requestMsg ).Result;
					//result will contain the envelope if successful
					var resultContent = result.Content.ReadAsStringAsync().Result;
					//don't return a bad response, especially with only returning a string
					if ( result.IsSuccessStatusCode == false )
					{
						//Response.StatusCode = ( int ) result.StatusCode; //Test this
						// resultContent = resultContent + " ( " + result.StatusCode + " )";
						LoggingHelper.DoTrace( 5, thisClassName + ".DirectDelete. Failed using url:\n\r " + requestUrl + Environment.NewLine + resultContent );
						if ( resultContent.IndexOf( "No matching resource found" ) > -1 )
						{
							messages.Add( "No matching resource found" );
						}
						else
							messages.Add( resultContent );
						return false;
					}
					else
					{
						LoggingHelper.DoTrace( 7, thisClassName + ".DirectDelete.: Succeeded " + Environment.NewLine + resultContent );

						//not sure what will be returned upon successful delete - apparantly nothing
						messages.Add( resultContent );
						LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".DirectDelete. Successful. Type: {0}, CTID: {1}, OwnerCtid: {2}, contents: {3}", PublishingEntityType, EntityCtid, request.PublishForOrganizationIdentifier, resultContent ) );

						string status = "";

						supportServices.AddHistory( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) ? "missing" : request.PublishForOrganizationIdentifier, "", REGISTRY_ACTION_DELETE, PublishingEntityType, PublishingEntityType, EntityCtid, "", "none", ref status, helper.ApiKey, true, false );
						//
						activity.Comment = string.Format( "Organization: '{0}' deleted the '{1}': '{2}'.", trxCheck.OwningOrganization, PublishingEntityType, entityName );
						supportServices.AddActivity( activity, ref status );
					}

					return true;
				}
			}
			catch ( Exception exc )
			{
				var ipAddress = GetSourceIPAddress();
				LoggingHelper.LogError( exc, string.Format( thisClassName + ".DirectDelete - On POST. ctid: {0}, dataOwnerCTID: {1}, ipAddress: {2}", request.CTID, request.PublishForOrganizationIdentifier, ipAddress ) );
				messages.Add( exc.Message );
				return false;

			}
		}

		public bool CredentialRegistry_SelfManagedKeysDelete(EnvelopeDelete request, string throughApiKey, ref string statusMessage)
		{
			string publicKeyPath = "";
			string privateKeyPath = "";
			if ( GetKeys( ref publicKeyPath, ref privateKeyPath, ref statusMessage ) == false )
			{
				return false;
			}
			//could do a get so have info for history
			//crEnvelopeId, 
			DeleteEnvelope envelope = RegistryHandler.CreateDeleteEnvelope( publicKeyPath, privateKeyPath, request.CTID, throughApiKey );
			//handle community
			string serviceUri = string.Format( UtilityManager.GetAppKeyValue( "credentialRegistryGet" ), request.RegistryEnvelopeId );
			if(!string.IsNullOrWhiteSpace(request.Community))
			{
				serviceUri = serviceUri.Replace( "ce-registry", request.Community );
			}
			string postBody = JsonConvert.SerializeObject( envelope );
			string response = "";
			if ( !DoDelete( postBody, serviceUri, ref response ) )
			{
				//failed
				//not sure what to use for a statusMessage message
				statusMessage = response;
				return false;
			} else if ( UtilityManager.GetAppKeyValue( "loggingDeleteHistory", false ) )
			{
				//20-03-12 - start logging deletes
				string status = "";
				SupportServices mgr = new SupportServices( "Deleted", request.Community );
				mgr.AddHistory( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) ? "missing" : request.PublishForOrganizationIdentifier, "", REGISTRY_ACTION_DELETE, PublishingEntityType, PublishingEntityType, EntityCtid, "", request.RegistryEnvelopeId ?? "none", ref status, throughApiKey, true, false );
			}

			return true;
		}

		/*
        public bool CredentialRegistry_SelfManagedKeysDelete( string crEnvelopeId, string ctid, string requestedBy, ref string statusMessage )
		{
			string publicKeyPath = "";
			string privateKeyPath = "";
			if ( GetKeys( ref publicKeyPath, ref privateKeyPath, ref statusMessage ) == false )
			{
				return false;
			}
			//crEnvelopeId, 
			DeleteEnvelope envelope = RegistryHandler.CreateDeleteEnvelope( publicKeyPath, privateKeyPath, ctid, requestedBy );

			string serviceUri = string.Format( UtilityManager.GetAppKeyValue( "credentialRegistryGet" ), crEnvelopeId );

			string postBody = JsonConvert.SerializeObject( envelope );
			string response = "";
			if ( !DoDelete( postBody, serviceUri, ref response ) )
			{
				//failed
				//not sure what to use for a statusMessage message
				statusMessage = response;
				return false;
			}

			return true;
		}
		*/


		private static bool DoDelete( string postBody, string serviceUri, ref string response )
		{
			try
			{
				using ( var client = new HttpClient() )
				{
					HttpRequestMessage request = new HttpRequestMessage
					{
						Content = new StringContent( postBody, Encoding.UTF8, "application/json" ),
						Method = HttpMethod.Delete,
						RequestUri = new Uri( serviceUri )
					};
					var task = client.SendAsync( request );
					task.Wait();
					var result = task.Result;
					response = JsonConvert.SerializeObject( result );
					var contents = task.Result.Content.ReadAsStringAsync();

					if ( result.IsSuccessStatusCode == false )
					{
						//logging???
						//response = contents.Result;
						LoggingHelper.LogError( "RegistryServices.DeleteRequest Failed\n\r" + response + "\n\rError: " + JsonConvert.SerializeObject( contents ) );

						RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents.Result );
						response = string.Join( "<br/>", contentsJson.Errors.ToArray() );
					}
					else
					{
						//RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents.Result );
						//response = string.Join( ",", contentsJson.Errors.ToArray() );
						LoggingHelper.DoTrace( 6, "result: " + response );
					}


					return result.IsSuccessStatusCode;

				}
			}
			catch ( Exception exc )
			{
				LoggingHelper.LogError( exc, "RegistryServices.DeleteRequest" );
				response = exc.Message;
				return false;

			}

		}

		#endregion
		#region Helpers
		private static bool GetKeys( ref string publicKeyPath, ref string privateKeyPath, ref string statusMessage )
		{
			bool isValid = true;
			//TODO - validate files exist - some issue on test server???

			string privateKeyLocation = UtilityManager.GetAppKeyValue( "privateKeyLocation", "" );
			if ( string.IsNullOrWhiteSpace( privateKeyLocation ) )
			{
				statusMessage = "Error - missing application key of privateKeyLocation";
				return false;
			}
			string publicKeyLocation = UtilityManager.GetAppKeyValue( "pemKeyLocation", "" );
			if ( string.IsNullOrWhiteSpace( publicKeyLocation ) )
			{
				statusMessage = "Error - missing application key of pemKeyLocation";
				return false;
			}
			//processing for dev env - where full path will vary by machine
			//could use a common location like @logs\keys??
			//if it works, then adjust the value stored in appkeys
			//doens't work = mike.parsons\appData\roaming
			//var fileName = Path.Combine( Environment.GetFolderPath( 	Environment.SpecialFolder.ApplicationData ), publicKeyLocation );
			//NOTE: cannot use HostingEnvironment.MapPath here
			//var fullPath = System.Web.Hosting.HostingEnvironment.MapPath( publicKeyLocation );

			if ( publicKeyLocation.ToLower().StartsWith( "c:\\" ) )
				publicKeyPath = publicKeyLocation;
			else
				publicKeyPath = Path.Combine( HttpRuntime.AppDomainAppPath, publicKeyLocation );
			//publicKeyData = File.ReadAllText( signingKeyPath );

			if ( privateKeyLocation.ToLower().StartsWith( "c:\\" ) )
				privateKeyPath = privateKeyLocation;
			else
				privateKeyPath = Path.Combine( HttpRuntime.AppDomainAppPath, privateKeyLocation );

			LoggingHelper.DoTrace( 4, string.Format( "files: private: {0}, \r\npublic: {1}", privateKeyPath, publicKeyPath ) );
			if ( !System.IO.File.Exists( privateKeyPath ) )
			{
				statusMessage = "Error - the privateKeyPath (encoding key) was not found";
				isValid = false;
			}
			if ( !System.IO.File.Exists( publicKeyPath ) )
			{
				statusMessage = "Error - the public key was not found";
				isValid = false;
			}
			return isValid;
		}

		public bool HasValidPublisherToken()
		{
			if ( PublisherAuthorizationToken != null && PublisherAuthorizationToken.Length >= 32 )
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Simple validation, until more communities
		/// </summary>
		/// <returns></returns>
		public bool ValidateCommunity()
		{
			if (!string.IsNullOrWhiteSpace(Community))
			{
				if (Community.ToLower() != "navy")
				{
					Community = "";
					return false;
				}
			}
			return true;
		}
		#endregion

		#region Reading 
		/// <summary>
		/// Retrieve a resource from the registry by ctid - using /graph
		/// </summary>
		/// <param name="ctid"></param>
		/// <param name="ctdlType"></param>
		/// <param name="statusMessage"></param>
		/// <param name="community"></param>
		/// <returns></returns>
		public static string GetResourceGraph( string ctid, ref string ctdlType, ref string statusMessage, string community = "")
		{
			if ( !string.IsNullOrWhiteSpace( community ) && community.IndexOf("/") == -1 )
			{
				community += "/";
			}
			string url = UtilityManager.GetAppKeyValue( "credentialRegistryBaseUrl" ) + community + "graph/" + ctid;
			
			return GetResourceByUrl( url, ref ctdlType, ref statusMessage );
		}
		public static bool DoesResourceExistByCTID( string ctid, ref string registryURL, ref string statusMessage, string community = "" )
		{
			if ( !string.IsNullOrWhiteSpace( community ) )
				community += "/";
			registryURL = UtilityManager.GetAppKeyValue( "credentialRegistryBaseUrl" ) + community + "resources/" + ctid;
			var ctdlType = "";
			var payload = GetResourceByUrl( registryURL, ref ctdlType, ref statusMessage );

			if ( string.IsNullOrWhiteSpace( payload ) )
				return false;
			else
				return true;
		}
		public static bool DoesResourceExist( string registryURL, ref string statusMessage )
		{
			var ctdlType = "";
			var payload = GetResourceByUrl( registryURL, ref ctdlType, ref statusMessage );

			if ( string.IsNullOrWhiteSpace( payload ) )
				return false;
			else
				return true;
		}
		public static string GetResource(string ctid, ref string ctdlType, ref string statusMessage, string community = "")
		{
			if ( !string.IsNullOrWhiteSpace( community ) )
				community += "/";
			string url = UtilityManager.GetAppKeyValue( "credentialRegistryBaseUrl" ) + community + "resources/" + ctid;
			return GetResourceByUrl( url, ref ctdlType, ref statusMessage );
		}
		public static string GetEnvelope(string envelopeIdentifier, ref string ctdlType, ref string statusMessage, string community = "")
		{
			if ( !string.IsNullOrWhiteSpace( community ) )
				community += "/";
			string url = UtilityManager.GetAppKeyValue( "credentialRegistryBaseUrl" ) + community + "envelopes/" + envelopeIdentifier;
			return GetResourceByUrl( url, ref ctdlType, ref statusMessage );
		}
		/// <summary>
		/// Retrieve a resource from the registry by resource url
		/// </summary>
		/// <param name="resourceId">Url to a resource in the registry</param>
		/// <param name="statusMessage"></param>
		/// <returns></returns>
		public static string GetResourceByUrl( string resourceUrl, ref string ctdlType, ref string statusMessage )
		{
			string payload = "";
			statusMessage = "";
			ctdlType = "";
			string ceApiKey = UtilityManager.GetAppKeyValue( "ceApiKey" );
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					if ( !string.IsNullOrWhiteSpace( ceApiKey ) )
					{
						client.DefaultRequestHeaders.Add( "Authorization", "Token " + ceApiKey );
					}
					var task = client.GetAsync( resourceUrl );
					task.Wait();
					var response1 = task.Result;
					payload = task.Result.Content.ReadAsStringAsync().Result;

					//just in case, likely the caller knows the context
					if ( !string.IsNullOrWhiteSpace( payload ) 
							&& payload.Length > 100
							//&& payload.IndexOf("\"errors\":") == -1
							)
					{
						ctdlType = RegistryServices.GetResourceType( payload );
					} else
					{
						//nothing found, or error/not found
						LoggingHelper.DoTrace( 1, "RegistryServices.GetResourceByUrl. Did not find: " + resourceUrl );
						statusMessage = "The referenced resource was not found in the credential registry: " + resourceUrl;
						payload = "";
					}
					//

				}
			}
			catch ( Exception exc )
			{
				if ( exc.Message.IndexOf( "(404) Not Found" ) > 0 )
				{
					//need to surface these better
					statusMessage = "The referenced resource was not found in the credential registry: " + resourceUrl;
				}
				else
				{
					LoggingHelper.LogError( exc, "RegistryServices.GetResource" );
					statusMessage = exc.Message;
				}
			}
			return payload;
		}
		private static string GetResourceType( string payload )
		{
			string ctdlType = "";
			RegistryObject ro = new RegistryObject( payload );
			ctdlType = ro.CtdlType;
			//ctdlType = ctdlType.Replace( "ceterms:", "" );
			return ctdlType;

		}
		public static RegistryObject GetResourceObject(string payload)
		{
			string ctdlType = "";
			RegistryObject ro = new RegistryObject( payload );
			return ro;
		}

		public static List<ReadEnvelope> QuickSearch(string registryEntityType, int pageNbr, int pageSize, ref int pTotalRows, ref string statusMessage, string community)
		{
			var list = Search( registryEntityType, "", "", "", pageNbr, pageSize, ref pTotalRows, ref statusMessage, community );

			return list;
		}

		public static List<ReadEnvelope> Search(string type, string fts, string startingDate, string endingDate, int pageNbr, int pageSize, ref int pTotalRows, ref string statusMessage, string community, string sortOrder = "asc" )
		{

			string document = "";
			string filter = "";
			//includes the question mark
			string serviceUri = GetRegistrySearchUrl( community );
			//from=2016-08-22T00:00:00&until=2016-08-31T23:59:59
			//resource_type=credential
			if ( !string.IsNullOrWhiteSpace( type ) )
				filter = string.Format( "resource_type={0}", type.ToLower() );

			SetPaging( pageNbr, pageSize, ref filter );
			SetDateFilters( startingDate, endingDate, ref filter );
			SetSortOrder( ref filter );

			string ceApiKey = UtilityManager.GetAppKeyValue( "ceApiKey" );
			serviceUri += filter.Length > 0 ? filter : "";
			//future proof

			List<ReadEnvelope> list = new List<ReadEnvelope>();
			try
			{
				WebRequest request = WebRequest.Create( serviceUri );

				// If required by the server, set the credentials.
				request.Credentials = CredentialCache.DefaultCredentials;

				var hdr = new WebHeaderCollection
				{
					{ "Authorization", "Token  " + ceApiKey }
				};
				request.Headers.Add( hdr );

				HttpWebResponse response = ( HttpWebResponse )request.GetResponse();
				Stream dataStream = response.GetResponseStream();

				// Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader( dataStream );
				// Read the content.
				document = reader.ReadToEnd();

				// Cleanup the streams and the response.
				reader.Close();
				dataStream.Close();
				response.Close();

				//Link contains links for paging
				var hdr2 = response.GetResponseHeader( "Link" );
				Int32.TryParse( response.GetResponseHeader( "Total" ), out pTotalRows );
				//20-07-02 mp - seems the header name is now X-Total
				if ( pTotalRows == 0 )
				{
					Int32.TryParse( response.GetResponseHeader( "X-Total" ), out pTotalRows );
				}

				//map to the list
				list = JsonConvert.DeserializeObject<List<ReadEnvelope>>( document );

			}
			catch ( Exception exc )
			{
				LoggingHelper.LogError( exc, "RegistryServices.Search" );
			}
			return list;
		}//
		private static void SetSortOrder( ref string where, string sortOrder = "asc" )
		{
			string AND = "";
			if ( where.Length > 0 )
				AND = "&";
			if ( string.IsNullOrWhiteSpace( sortOrder ) )
				sortOrder = "asc";
			//this is the default anyway - maybe not, seems like dsc is the default
			where = where + AND + "sort_by=updated_at&sort_order=" + sortOrder;
		}

		private static void SetDateFilters( string startingDate, string endingDate, ref string where )
		{
			string AND = "";
			if ( where.Length > 0 )
				AND = "&";

			string date = FormatDateFilter( startingDate );
			if ( !string.IsNullOrWhiteSpace( date ) )
			{
				where = where + AND + string.Format( "from={0}", startingDate );
				AND = "&";
			}

			date = FormatDateFilter( endingDate );
			if ( !string.IsNullOrWhiteSpace( date ) )
			{
				where = where + AND + string.Format( "until={0}", endingDate );
				AND = "&";
			}
			//if ( !string.IsNullOrWhiteSpace( endingDate ) && endingDate.Length == 10 )
			//{
			//	where = where + AND + string.Format( "until={0}T23:59:59", endingDate );
			//}
		}
		private static string FormatDateFilter( string date )
		{
			string formatedDate = "";
			if ( string.IsNullOrWhiteSpace( date ) )
				return "";

			//start by checking for just properly formatted date
			if ( !string.IsNullOrWhiteSpace( date ) && date.Length == 10 )
			{
				//apparently this is not necessary!!
				formatedDate = string.Format( "{0}T00:00:00", date );
			}
			else if ( !string.IsNullOrWhiteSpace( date ) )
			{
				//check if in proper format - perhaps with time provided
				if ( date.IndexOf( "T" ) > 8 )
				{
					formatedDate = string.Format( "{0}", date );
				}
				else
				{
					//not sure how to handle unexpected date except to ignore
					//might be better to send actual DateTime field
				}
			}

			return formatedDate;
		}
		public static string GetRegistrySearchUrl(string community = "")
		{
			if ( string.IsNullOrWhiteSpace( community ) )
			{
				community = UtilityManager.GetAppKeyValue( "defaultCommunity" );
			}
			string serviceUri = UtilityManager.GetAppKeyValue( "credentialRegistrySearch" );
			//"https://sandbox.credentialengineregistry.org/{0}/search?"
			serviceUri = string.Format( serviceUri, community );
			return serviceUri;
		}
		private static void SetPaging(int pageNbr, int pageSize, ref string where)
		{
			string AND = "";
			if ( where.Length > 0 )
				AND = "&";

			if ( pageNbr > 0 )
			{
				where = where + AND + string.Format( "page={0}", pageNbr );
				AND = "&";
			}
			if ( pageSize > 0 )
			{
				where = where + AND + string.Format( "per_page={0}", pageSize );
				AND = "&";
			}
		}
		private static void SetSortOrder(ref string where)
		{
			string AND = "";
			if ( where.Length > 0 )
				AND = "&";
			//this is the default anyway
			where = where + AND + "sort_by=updated_at&sort_order=asc";
		}
		/// <summary>
		/// Generic handling of Json object - especially for unexpected types
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static Dictionary<string, object> JsonToDictionary( string json )
		{
			var result = new Dictionary<string, object>();
			var obj = JObject.Parse( json );
			foreach ( var property in obj )
			{
				result.Add( property.Key, JsonToObject( property.Value ) );
			}
			return result;
		}
		public static object JsonToObject( JToken token )
		{
			switch ( token.Type )
			{
				case JTokenType.Object:
				{
					return token.Children<JProperty>().ToDictionary( property => property.Name, property => JsonToObject( property.Value ) );
				}
				case JTokenType.Array:
				{
					var result = new List<object>();
					foreach ( var obj in token )
					{
						result.Add( JsonToObject( obj ) );
					}
					return result;
				}
				default:
				{
					return ( ( JValue )token ).Value;
				}
			}
		}
		#endregion

		public static string GetSourceIPAddress()
		{
			string ipAddress = "unknown";
			try
			{
				ipAddress = HttpContext.Current.Request.ServerVariables[ "HTTP_X_FORWARDED_FOR" ];
				if ( !string.IsNullOrEmpty( ipAddress ) )
				{
					string[] addresses = ipAddress.Split( ',' );
					if ( addresses.Length != 0 )
					{
						return addresses[ 0 ];
					}
				}
				if ( ipAddress == null || ipAddress == "" || ipAddress.ToLower() == "unknown" )
				{
					ipAddress = HttpContext.Current.Request.ServerVariables[ "REMOTE_ADDR" ];
				}
			}
			catch ( Exception ex )
			{

			}

			return ipAddress;
		} //
	}
	public class PublishRequest
	{
		public PublishRequest()
		{
			Messages = new List<string>();
		}
		//input
		public string RequestType { get; set; }
		public string AuthorizationToken { get; set; }
		public string PublishingOrgUid { get; set; }
		public string Submitter { get; set; }
		public string InputPayload { get; set; }
		
		public string Identifier { get; set; }
		public string EnvelopeIdentifier { get; set; }

		public string FormattedPayload { get; set; }

		public List<string> Messages { get; set; }
	}

	public class ManagedPublishRequest
	{
		public ManagedPublishRequest()
		{
		}
        public string clientIdentifier { get; set; }
        public string dataOwnerCTID { get; set; }
		public string apiKey { get; set; }
		public string payloadOverflow { get; set; }
		public string publishIdentifier { get; set; }
        public string publishMethodURI { get; set; } 
        public string publishingEntityType { get; set; }
        public string ctdlType { get; set; }
        public string entityCtid { get; set; }
		public string community { get; set; } = "";
		public string skipValidation { get; set; } = "true";

		public string payloadJSON { get; set; }

		//
	}

	public class AccountPublishResponse
    {
        public AccountPublishResponse()
        {
            Messages = new List<string>();
        }
        public string ResponseType { get; set; } = "accountResponse";
        public bool Successful { get; set; } = true;
		public bool WasEnvelopeUpdated { get; set; } = true;

		public List<string> Messages { get; set; }

    }

	public class AccountDeleteRequest
	{
		public AccountDeleteRequest()
		{
		}
		
		public string apiKey { get; set; }
		public string dataOwnerCTID { get; set; }
		public string publishMethodURI { get; set; }
		public string publishingEntityType { get; set; }
		public string entityCtid { get; set; }
		public string community { get; set; } = "";


		//
	}
	public class AccountConsumeRequest
    {
        public AccountConsumeRequest()
        {
        }
        public string ApiKey { get; set; }

		//TBD
		public string ConsumeMethodURI { get; set; } = "consumingMethod:SearchApi";
		public int Skip { get; set; }
		public int Take { get; set; }
		public JObject CTDLQuery { get; set; } //Used for statistics/logging purposes when the query is passed to the accounts system
		public string GremlinQuery { get; set; } //Used to actually perform the query when the query is proxied through the accounts system
		public List<KeyValuePair<string, string>> DirectQuery { get; set; } //Use with original full text search
		public List<string> DescriptionSetCTIDs { get; set; } //Use when getting description sets for a set of known search result CTIDs
		public bool DescriptionSetIncludeData { get; set; } //Determines whether to just get the URIs or include the data too when getting description sets by CTIDs
		public int DescriptionSetRelatedURIsLimit { get; set; } //Determines how many URIs for each description set branch to get
		public int DescriptionSetRelatedItemsLimit { get; set; } //Determines how many items for each description set branch to get
		public bool IsGremlinOnly { get; set; } //Helps the account system figure out how to handle this query
		public string SPARQLQuery { get; set; }
		public bool ShouldLogRequest { get; set; } //Avoid logging queries unnecessarily
	}

    public class AccountConsumeResponse
    {
        public AccountConsumeResponse()
        {
            Messages = new List<string>();
        }
        public string ResponseType { get; set; } = "accountResponse";
        public bool Successful { get; set; } = true;
        public List<string> Messages { get; set; }

    }

	public class RegistryObject
	{
		public RegistryObject() { }

		public RegistryObject( string payload )
		{
			if ( !string.IsNullOrWhiteSpace( payload ) && (payload.IndexOf("errors") == -1 || payload.IndexOf( "errors" ) > 50 ) )
			{
				dictionary = RegistryServices.JsonToDictionary( payload );
				if ( payload.IndexOf( "@graph" ) > 0 && payload.IndexOf( "@graph\": null" ) == -1 )
				{
					IsGraphObject = true;
					//get the graph object
					object graph = dictionary[ "@graph" ];
					//serialize the graph object
					var glist = JsonConvert.SerializeObject( graph );
					//parse graph in to list of objects
					JArray graphList = JArray.Parse( glist );

					var main = graphList[ 0 ].ToString();
					BaseObject = JsonConvert.DeserializeObject<RegistryBaseObject>( main );
					CtdlType = BaseObject.CdtlType;
					Ctid = BaseObject.Ctid;

					var n = new ServiceHelperV2().AssignObjectToString( BaseObject.Name );
					//not important to fully resolve yet
					if ( BaseObject.Name != null )
						Name = BaseObject.Name.ToString();
					else if ( CtdlType == "ceasn:CompetencyFramework" )
					{
						//var n = JsonConvert.DeserializeObject<LanguageMap>( BaseObject.CompetencyFrameworkName.ToString() );
						Name = ( BaseObject.CompetencyFrameworkName ?? "" ).ToString();
					}
					else
						Name = "?????";
				}
				else
				{
					//check if old resource or standalone resource
					BaseObject = JsonConvert.DeserializeObject<RegistryBaseObject>( payload );
					CtdlType = BaseObject.CdtlType;
					Ctid = BaseObject.Ctid;
					if ( BaseObject.Name != null )
						Name = BaseObject.Name.ToString();
					else
					{
						Name = "no name property for this document";
					}
				}
				if ( (Name ??"").IndexOf("{") == 0)
				{
					var n = JsonConvert.DeserializeObject<LanguageMap>( Name );
					Name = n.ToString();
					//Name = n.;
				}
				CtdlType = CtdlType.Replace( "ceterms:", "" );
				CtdlType = CtdlType.Replace( "ceasn:", "" );
			}
		}

		Dictionary<string, object> dictionary = new Dictionary<string, object>();

		public bool IsGraphObject { get; set; }
		public RegistryBaseObject BaseObject { get; set; } = new RegistryBaseObject();
		public string CtdlType { get; set; } = "";
		public string CtdlId { get; set; } = "";
		public string Ctid { get; set; } = "";
		public string Name { get; set; }
	}
	public class RegistryBaseObject
	{
		[JsonProperty( "@id" )]
		public string CtdlId { get; set; }

		/// <summary>
		/// Type  of CTDL object
		/// </summary>
		[JsonProperty( "@type" )]
		public string CdtlType { get; set; }

		[JsonProperty( PropertyName = "ceterms:ctid" )]
		public string Ctid { get; set; }

		[JsonProperty( PropertyName = "ceterms:name" )]
		public object Name { get; set; }

		[JsonProperty( PropertyName = "ceterms:description" )]
		public object Description { get; set; }

		[JsonProperty( PropertyName = "ceasn:name" )]
		public object CompetencyFrameworkName { get; set; }

		[JsonProperty( PropertyName = "ceasn:description" )]
		public object FrameworkDescription { get; set; }


		[JsonProperty( PropertyName = "ceterms:subjectWebpage" )]
		public string SubjectWebpage { get; set; }

	}

	public class AccountApiResponse
	{
		public AccountApiResponse()
		{
			Messages = new List<string>();
			ResultContent = "";
		}

		/// True if action was successfull, otherwise false
		public bool Successful { get; set; }
		/// <summary>
		/// List of error or warning messages
		/// </summary>
		public List<string> Messages { get; set; }

		/// <summary>
		/// Payload of request to registry, containing properties formatted as CTDL - JSON-LD
		/// </summary>
		public string ResultContent { get; set; }
	}


	public class CredentialRegistryResponse
	{
		public Version Version { get; set; }
		public Content Content { get; set; }
		public int StatusCode { get; set; }
		public string ReasonPhrase { get; set; }
		public List<Header> Headers { get; set; }
		public RequestMessage RequestMessage { get; set; }
		public bool IsSuccessStatusCode { get; set; }
	}

	public class Version
	{
		public int Major { get; set; }
		public int Minor { get; set; }
		public int Build { get; set; }
		public int Revision { get; set; }
		public int MajorRevision { get; set; }
		public int MinorRevision { get; set; }
	}

	public class Header
	{
		public string Key { get; set; }
		public List<string> Value { get; set; }
	}

	public class Content
	{
		public List<Header> Headers { get; set; }
	}



	public class RequestMessage
	{
		public Version Version { get; set; }
		public Content Content { get; set; }
		//public Method Method { get; set; }
		public string RequestUri { get; set; }
		public List<Header> Headers { get; set; }
		public Properties Properties { get; set; }
	}

	//public class Method
	//{
	//	public string Method { get; set; }
	//}

	public class Properties
	{
	}
}
