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
using RA.Models.Input;
using Utilities;

namespace RA.Services
{
	public class RegistryServices
	{
		public const string RA_PUBLISH_METHOD = "publishMethod:RegistryAssistant";
		public const string CE_MANAGED_PUBLISH_METHOD = "publishMethod:ManualEntry";
		public const string CE_BULK_UPLOAD_METHOD = "publishMethod:BulkUpload";
		public const string CE_DIRECT_PUBLISH_METHOD = "ManualPublish";
		public const string CE_TRUSTED_PARTNER_ROLE = "publishRole:TrustedPartner";

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
		//public RegistryServices( string entityType, string ctdlType, string ctid, string entityName, string serializedInput )
		//{
		//	PublishingEntityType = entityType;
		//	CtdlType = ctdlType;
		//	EntityCtid = ctid;
		//	EntityName = entityName;
		//	SerializedInput = serializedInput;
		//}
		/// <summary>
		/// AKA API Key
		/// </summary>
		public string PublisherAuthorizationToken { get; set; }
		public string PublishingForOrgCtid { get; set; } //data owner

		/// <summary>
		/// CTID for the publishing org. This will be derived using the apiKey (PublisherAuthorizationToken)
		/// These will typically be the same, unless part of a third party trx
		/// </summary>
		public string PublishingByOrgCtid { get; set; } = "";
		public string PublishingEntityType { get; set; }
		public string CtdlType { get; set; }
		public string EntityCtid { get; set; }
		public string EntityName { get; set; }
		public string Community { get; set; } = "";
		public string SerializedInput { get; set; } = "";
		/// <summary>
		/// true = keys managed by credential registry, goes through accounts site
		/// </summary>
		public bool IsManagedRequest { get; set; }
		/// <summary>
		/// true if originates from publisher
		/// </summary>
		public bool IsPublisherRequest { get; set; }

		public bool OverrodeOriginalRequest { get; set; }
		public bool SkippingValidation { get; set; }
		public bool WasChanged { get; set; }
		public string EnvelopeUrl { get; set; }
		public string GraphUrl { get; set; }

		#region Publishing

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

			if (IsManagedRequest)
				ManagedPublishThroughAccounts(request.Payload,
					this.PublisherAuthorizationToken,
					this.PublishingForOrgCtid,
					submitter, identifier, ref successful, ref statusMessage, ref crEnvelopeId);
			else
				SelfPublish(request.Payload, submitter, this.PublishingForOrgCtid, identifier, ref successful, ref statusMessage, ref crEnvelopeId, SkippingValidation);

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
		/// Publish a document to the Credential Registry
		/// </summary>
		/// <param name="payload">Serialized version of request object</param>
		/// <param name="submitter"></param>
		/// <param name="statusMessage"></param>
		/// <param name="crEnvelopeId"></param>
		/// <returns></returns>
		[Obsolete]
		private bool Publish( string payload,
									string submitter,
									string identifier,
									ref string statusMessage,
									ref string crEnvelopeId)
		{
			var successful = true;
			if ( !string.IsNullOrWhiteSpace( Community ) )
			{
				if ( Community.ToLower() != "navy" )
				{
					statusMessage = "Error - an invalid community was provided: " + Community;
					return false;
				}
			}
			else
				Community = "";

			if ( IsManagedRequest )
				ManagedPublishThroughAccounts( payload,
					this.PublisherAuthorizationToken,
					this.PublishingForOrgCtid,
					submitter, identifier, ref successful, ref statusMessage, ref crEnvelopeId );
			else
				SelfPublish( payload, submitter, this.PublishingForOrgCtid, identifier, ref successful, ref statusMessage, ref crEnvelopeId, SkippingValidation );
			
				
			return successful;
		} //

		/// <summary>
		/// Publish using
		/// NOTE: should include apiKey for reference is was provided
		/// </summary>
		/// <param name="payload"></param>
		/// <param name="submitter"></param>
		/// <param name="identifier"></param>
		/// <param name="valid"></param>
		/// <param name="status"></param>
		/// <param name="crEnvelopeId"></param>
		/// <param name="forceSkipValidation"></param>
		/// <returns></returns>
		private string SelfPublish( string payload,
				string submitter,
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
			List<string> messages = new List<string>();
            string environment = UtilityManager.GetAppKeyValue( "environment" );
            LoggingHelper.DoTrace( 6, string.Format( "RegistryServices.SelfPublish. crEnvelopeId: {0}", crEnvelopeId ));
			//LoggingHelper.DoTrace( 7, "              - payload: \r\n" + payload );
			try
			{
				//TODO - add handling where keys are stored in the registry
				if ( GetKeys( ref publicKeyPath, ref privateKeyPath, ref status ) == false )
				{
					valid = false;
					//no, the proper error is returned from GetKeys
					//status = "Error getting CER Keys";
					messages.Add( status );
					return "";
				}

				//todo - need to add signer and other to the content
				//note for new, DO NOT INCLUDE an EnvelopeIdentifier property 
				//		-this is necessary due to a bug, and hopefully can change back to a single call
				/*
				 *	Sandbox: https://reindeer.credential-registry.sandbox.c66.me/
					Staging: https://fox.credential-registry.staging.c66.me/
				*/
				//get publish API
				string serviceUri = UtilityManager.GetAppKeyValue( "credentialRegistryPublishUrl" );
				var defaultCommunity = UtilityManager.GetAppKeyValue( "defaultCommunity", "ce-registry" );
				//just one for now
				if ( !string.IsNullOrWhiteSpace( Community ) )
				{
					//Warning: assumes default community name
					serviceUri = serviceUri.Replace( "ce-registry", Community );
					defaultCommunity = Community;
				}
				if ( string.IsNullOrWhiteSpace( crEnvelopeId ) )
				{
					Envelope envelope = new Envelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload, envelope, defaultCommunity );
                    envelope.EnvelopeCetermsCtid = EntityCtid;
                    envelope.EnvelopeCtdlType = CtdlType;
                   
					postBody = JsonConvert.SerializeObject( envelope, ServiceHelperV2.GetJsonSettings() );

					//LoggingHelper.DoTrace( 7, "RegistryServices.SelfPublish - ADD envelope: \r\n" + postBody );
				}
				else
				{
					UpdateEnvelope envelope = new UpdateEnvelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload, envelope, defaultCommunity );

					//now embed 
					envelope.EnvelopeIdentifier = crEnvelopeId;
					envelope.EnvelopeCetermsCtid = EntityCtid;
					envelope.EnvelopeCtdlType = CtdlType;
					
					postBody = JsonConvert.SerializeObject( envelope, ServiceHelperV2.GetJsonSettings() );

					LoggingHelper.DoTrace( 5, string.Format( "RegistryServices.SelfPublish - updating existing envelopeId: {0}. update envelope: \r\n", crEnvelopeId )  );
					//LoggingHelper.DoTrace( 7, "RegistryServices.SelfPublish - update envelope: \r\n" + postBody );
				}


				var skippingValidation = forceSkipValidation ? true : UtilityManager.GetAppKeyValue( "skippingValidation" ) == "yes";
                if ( payload.ToLower().IndexOf( "@graph" ) > 0 )
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
				string contents = "";
				DateTime started = DateTime.Now;
				try
				{
					HttpContext.Current.Server.ScriptTimeout = 300;
					
					using ( var client = new HttpClient() )
					{
						client.DefaultRequestHeaders.
							Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

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
                                    messages.AddRange( contentsJson.Errors );
                                }
                            }
                            else
                            {
                                status = contents;
                                messages.Add( status );
                            }
                            // RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                            //
                            valid = false;
							string queryString = GetRequestContext();
                            // +"\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )

                            LoggingHelper.LogError( identifier + " RegistryServices.Publish Failed:"
								+ "\n\rURL:\n\r " + queryString
								+ "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject( response )
								+ "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject( contents ),
								false, "CredentialRegistry publish failed for " + identifier );

							//messages.AddRange( contentsJson.Errors );
							LoggingHelper.WriteLogFile( 5, identifier + "_payload_failed.json", payload, "", false );
							LoggingHelper.WriteLogFile( 7, identifier + "_envelope_failed", postBody, "", false );
							//statusMessage =contents.err contentsJson.Errors.ToString();
						}
						else
						{
							valid = true;
							//LoggingHelper.DoTrace( 7, "contents after successful publish.\r\n" + contents );
							UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>( contents );
							WasChanged = ue.Changed;
							crEnvelopeId = ue.EnvelopeIdentifier;
                            LoggingHelper.DoTrace( 5, string.Format( "Returned EnvelopeId: {0}, Elapsed: {1:N2}, WasChanged: {2}", crEnvelopeId, duration.TotalSeconds, ue.Changed ) );

							LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", payload, "", false );
                            //LoggingHelper.WriteLogFile( 7, identifier + "_envelope_Successful", postBody, "", false );

                            if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", false ) )
                            {
								//initialize with entity name - why here?
								//should this be allowed if envelope was not updated?
                                SupportServices mgr = new SupportServices( EntityName, Community );
                                mgr.AddHistory( string.IsNullOrWhiteSpace(dataOwnerCTID) ? "missing" : dataOwnerCTID, payload, CE_DIRECT_PUBLISH_METHOD, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status, PublishingByOrgCtid, ue.Changed );
                            }
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
					LoggingHelper.LogError( ex, string.Format( "RegistryServices.SelfPublish - On POST. identifier: {0}, dataOwnerCTID: {1}, ipAddress: {2}", identifier, dataOwnerCTID, ipAddress ) );
					valid = false;
					status = "Failed on Registry Publish: " + LoggingHelper.FormatExceptions( ex );
					messages.Add( status );
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

                LoggingHelper.LogError( ex, string.Format( "RegistryServices.SelfPublish - Outside try/catch. identifier: {0}, crEnvelopeId: {1}", identifier, crEnvelopeId ) );

                valid = false;
				crEnvelopeId = "";
				status = "Failed during Registry preparations: " + LoggingHelper.FormatExceptions( ex );
				messages.Add( status );
				return "";
			}
		}
		//

		public string ManagedPublishThroughAccounts( string payload,
				string apiKey,
				string dataOwnerCTID,
				string submitter,
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
			if (OverrodeOriginalRequest)
			{
				//meand the request came from the manual publisher and there was either a previous request via the api or moving forward all new publishing is to be managed. 
				apr.publishMethodURI = CE_MANAGED_PUBLISH_METHOD;
			}
			//
			LoggingHelper.DoTrace( 6, "RegistryServices.ManagedPublishThroughAcccounts - dataOwnerCTID: \r\n" + dataOwnerCTID );
			
			//get accounts url
			string serviceUri = UtilityManager.GetAppKeyValue( "accountsPublishApi" );
			string environment = UtilityManager.GetAppKeyValue( "environment" );
			//https://localhost:44320/publish/
			if ( DateTime.Now.ToString("yyyy-MM-dd") == "2019-07-30" && environment=="development")
			{
				//serviceUri = "https://localhost:52345/organization/p2/";
			}
            bool skippingValidation = SkippingValidation ? true : UtilityManager.GetAppKeyValue("skippingValidation") == "yes";
            if ( payload.ToLower().IndexOf( "@graph" ) > 0 )
                skippingValidation = true;
            if ( skippingValidation )
            {
				apr.skipValidation = "true";
			}

			//var serializer = new JavaScriptSerializer() { MaxJsonLength = 86753090 };
			//var p2 = serializer.Serialize( apr );

			string postBody = JsonConvert.SerializeObject( apr, ServiceHelperV2.GetJsonSettings() );
			if (postBody.Length > 1000000)
			{
				LoggingHelper.DoTrace( 1, string.Format( " *******publish note. Larger payload: {0} bytes ", postBody.Length ) );
			}
            string contents = "";
			DateTime started = DateTime.Now;
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					//client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + apiKey );

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

					if ( response.IsSuccessStatusCode == false )
					{
                        //note the accounts publish will always return successful otherwise any error messages get lost
                        if ( contents.ToLower().IndexOf( "accountresponse" ) > 0  )
                        {
                            AccountPublishResponse acctResponse = JsonConvert.DeserializeObject<AccountPublishResponse>( contents );
                            if ( acctResponse.Messages != null && acctResponse.Messages.Count > 0 )
                            {
                                status = string.Join( ",", acctResponse.Messages.ToArray() );
                                messages.AddRange( acctResponse.Messages );
                            }
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
						}
						else if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "{" ) > -1 )
                        {
                            //handle double [[
                            contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
                            RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                            if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
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
                        if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
                        {
                            valid = false;
                            status = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
                            messages.Add( status );
						}
						else if ( contents.ToLower().IndexOf( "<html>" ) > 0 )
						{
							int pos = contents.IndexOf( "title" );
							int next = contents.IndexOf( "</title>" );
							if (pos > 0 && next > pos)
							{
								status= contents.Substring(pos+5, contents.Length - next);
							} else
							{
								status = contents;
							}
						}
                        else if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "error" ) < 15 && contents.ToLower().IndexOf( "{" ) > -1 )
                        {
                            valid = false;
                            contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
                            RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                            if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
                            {
                                status = string.Join( ",", contentsJson.Errors.ToArray() );
                                //remove extraneous message of:@type : did not match one of the following values
                                if (status.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
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
							UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>( contents );
							WasChanged = ue.Changed;
							crEnvelopeId = ue.EnvelopeIdentifier;
							LoggingHelper.DoTrace( 5, string.Format( "Returned EnvelopeId: {0}, Elapsed: {1:N2}, WasChanged: {2}", crEnvelopeId, duration.TotalSeconds, ue.Changed ) );

							LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", payload, "", false );


                            if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", true ) ) {
                                SupportServices mgr = new SupportServices( EntityName, Community );
                                mgr.AddHistory( dataOwnerCTID, payload, apr.publishMethodURI, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status, PublishingByOrgCtid, ue.Changed );
                            }
                        }

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
				status = "Failed on Registry Publish: " + LoggingHelper.FormatExceptions( ex );
				messages.Add( status );
				return status;
			}

		}
        //
        /// <summary>
        /// Publish where registry will manage the keys
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="submitter"></param>
        /// <param name="identifier"></param>
        /// <param name="valid"></param>
        /// <param name="status"></param>
        /// <param name="crEnvelopeId">Will this is stil be applicable</param>
        /// <param name="forceSkipValidation"></param>
        /// <returns></returns>
    //    public string ManagedPublish(string payload,
    //            string authorizationToken,
    //            string publishingOrgUid,
    //            string submitter,
    //            string identifier,
    //            ref bool valid,
    //            ref string status,
    //            ref string crEnvelopeId,
    //            bool forceSkipValidation = false)
    //    {
    //        valid = true;
    //        var postBody = "";
    //        List<string> messages = new List<string>();



    //        //todo - need to add signer and other to the content
    //        //note for new, DO NOT INCLUDE an EnvelopeIdentifier property 
    //        //		-this is necessary due to a bug, and hopefully can change back to a single call

    //        LoggingHelper.DoTrace(6, "RegistryServices.ManagedPublish(OLD) - payload: \r\n" + payload);
    //        //already serialized!
    //        //postBody = JsonConvert.SerializeObject( payload );
    //        postBody = payload;


    //        //Do publish
    //        string serviceUri = string.Format(UtilityManager.GetAppKeyValue("managedRegistryUrl"), publishingOrgUid);

    //        string contents = "";

    //        try
    //        {
    //            using (var client = new HttpClient())
    //            {
    //                client.DefaultRequestHeaders.
    //                    Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    //                client.DefaultRequestHeaders.Add("Authorization", "ApiToken " + authorizationToken);

    //                var task = client.PostAsync(serviceUri,
    //                    new StringContent(postBody, Encoding.UTF8, "application/json"));
    //                task.Wait();
    //                var response = task.Result;
    //                //should get envelope_id from contents?
    //                contents = task.Result.Content.ReadAsStringAsync().Result;

    //                if (response.IsSuccessStatusCode == false)
    //                {
    //                    RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>(contents);
    //                    //
    //                    valid = false;
    //                    string queryString = GetRequestContext();
    //                    //now null:
    //                    //+ "\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )
    //                    LoggingHelper.LogError(identifier + " RegistryServices.Publish Failed:"
    //                        + "\n\rURL:\n\r " + queryString
    //                        + "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject(response)
    //                        + "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject(contents),
    //                        false, "CredentialRegistry publish failed for " + identifier);
    //                    if (contentsJson.Errors != null && contentsJson.Errors.Count > 0)
    //                    {
    //                        status = string.Join(",", contentsJson.Errors.ToArray());
    //                        messages.AddRange(contentsJson.Errors);
    //                    }
    //                    else
    //                    {
    //                        status = contents;
    //                        messages.Add(status);
    //                    }

    //                    LoggingHelper.WriteLogFile(5, identifier + "_payload_failed.json", payload, "", false);
    //                    //LoggingHelper.WriteLogFile( 7, identifier + "_envelope_failed", postBody, "", false );
    //                    //statusMessage =contents.err contentsJson.Errors.ToString();
    //                }
    //                else
    //                {
    //                    valid = true;
    //                    LoggingHelper.DoTrace(6, "contents after successful managed publish.\r\n" + contents);
    //                    UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>(contents);
    //                    crEnvelopeId = ue.EnvelopeIdentifier;

    //                    LoggingHelper.WriteLogFile(6, identifier + "_payload_Successful.json", payload, "", false);
    //                    //LoggingHelper.WriteLogFile( 7, identifier + "_envelope_Successful", postBody, "", false );
    //                }

    //                return contents;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
				//var ipAddress = GetSourceIPAddress();
				//LoggingHelper.LogError( ex, string.Format( "RegistryServices.Publish - On POST. identifier: {0}, dataOwnerCTID: {1}, ipAddress: {2}", identifier, dataOwnerCTID, ipAddress ) );
				//LoggingHelper.LogError(ex, "RegistryServices.Publish - POST");
    //            valid = false;
    //            status = "Failed on Registry Publish: " + LoggingHelper.FormatExceptions(ex);
    //            messages.Add(status);
    //            return status;
    //        }

    //    }
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
		public bool DeleteRequest( DeleteRequest request, string requestType, ref List<string> messages )
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
					)
				{
					messages.Add( "Error - please provide a valid delete request with a CTID, and the owning organization." );
					return false;
				}

				helper.OwnerCtid = request.PublishForOrganizationIdentifier;
				if ( !AuthorizationServices.ValidateRequest( helper, ref statusMessage, true ) )
				{
					messages.Add( statusMessage );
					return false;
				}
				else
				{
					// check if delete method should change
					bool recordWasFound = false;
					bool usedCEKeys = false;
					string message = "";
					//May need to include community in look up!
					var result = SupportServices.GetMostRecentHistory( requestType, request.CTID, ref recordWasFound, ref usedCEKeys, ref message, request.Community ?? "" );
					if ( recordWasFound )
					{
						//found previous (if not found, can't delete?)
						if ( result.DataOwnerCTID.ToLower() != helper.OwnerCtid.ToLower() )
						{
							//don't allow but may be moot if validating the apiKey owner ctid combination
							messages.Add( string.Format( "Suspcious request. The provided data owner CTID is different from the data owner CTID used for previous requests. This condition is not allowed. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid ) );
							return false;
						}
						else
						if ( usedCEKeys ) //only allow override outside of production for now && env != "production"
						{
							//need to require the api key, and to validate latter
							LoggingHelper.DoTrace( 5, requestType + " Delete. Last publish was CE request. Overriding to envelopeDelete." );
							if ( helper.IsPublisherRequest )
							{
								EnvelopeDelete edRequest = new EnvelopeDelete()
								{
									RegistryEnvelopeId = result.EnvelopeId,
									CTID = request.CTID,
									PublishForOrganizationIdentifier = request.PublishForOrganizationIdentifier, 
									Community= request.Community ?? ""
								};

								return CustomDelete( edRequest, requestType, ref messages );
							}
							else
							{
								//could allow
								if ( helper.ApiKey != null && helper.ApiKey.Length == 32 )
								{
									//could encrypt api key and save in history. Except the api key could change. 
								}

								messages.Add( string.Format( "Terribly sorry, this {2} was published by the CE Publisher app, you don't have the necessary privileges to delete the record. CTID: ({0}), Data Owner CTID '{1}'.", request.CTID, helper.OwnerCtid, requestType ) );
								return false;
							}
						}
						else
						{
							//continue?
						}
					} //just in case, fall thru and attempt managed delete
					LoggingHelper.DoTrace( 5, string.Format( "RegistryServices.DeleteRequest. requestType: {0}, CTID: {1}, Data Owner CTID '{2}'.", requestType, request.CTID, helper.OwnerCtid ) );

					RegistryServices cer = new RegistryServices( requestType, "", request.CTID );
					isValid = cer.ManagedDelete( request, helper.ApiKey, ref messages );
					//if ( !string.IsNullOrWhiteSpace( statusMessage ) )
					//	messages.Add( statusMessage );
				}
			}
			catch ( Exception ex )
			{
				messages.Add( ex.Message );
				isValid = false;
			}
			return isValid;
		} //
		/// <summary>
		/// Custom delete using delete envelope
		/// </summary>
		/// <param name="request"></param>
		/// <param name="requestType"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		private bool CustomDelete( EnvelopeDelete request, string requestType, ref List<string> messages )
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
				if ( !AuthorizationServices.ValidateRequest( helper, ref statusMessage, true ) )
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

            bool valid = true;
			AccountPublishRequest apr = new AccountPublishRequest
			{
				apiKey = apiKey,
				dataOwnerCTID = request.PublishForOrganizationIdentifier,
				publishMethodURI = RA_PUBLISH_METHOD,
				publishingEntityType = PublishingEntityType,
				entityCtid = request.CTID,
				community = request.Community ?? ""
			};

			string postBody = JsonConvert.SerializeObject( apr );
            string serviceUri = UtilityManager.GetAppKeyValue( "accountsDeleteApi" );

            string contents = "";
            try
            {
                using ( var client = new HttpClient() )
                {
                    client.DefaultRequestHeaders.
                        Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
                    //account site will do actual delete, so not sure if this should be a delete request
                    HttpRequestMessage requestMsg = new HttpRequestMessage
                    {
                        Content = new StringContent( postBody, Encoding.UTF8, "application/json" ),
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri( serviceUri )
                    };
                    var task = client.SendAsync( requestMsg );
                    task.Wait();
                    var response = task.Result;
                    contents = task.Result.Content.ReadAsStringAsync().Result;

                    //will always return successful from accounts
                    if ( response.IsSuccessStatusCode == false )
                    {
                        //logging???
                        //response = contents.Result;
                        LoggingHelper.LogError( "RegistryServices.DeleteRequest Failed\n\r" + response + "\n\rError: " + JsonConvert.SerializeObject( contents ) );

                        RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                        messages.Add( string.Join( ",", contentsJson.Errors.ToArray()) );
                        return false;
                    }
                    else
                    {
                        if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
                        {
                            valid = false;
                            messages.Add( UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" ));
                            return false;
                        }
                        else if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "{" ) > -1 )
                        {
                            valid = false;
                            contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
                            RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                            if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
                            {
                                string message = string.Join( ",", contentsJson.Errors.ToArray() );
                                //remove extraneous message of:@type : did not match one of the following values
                                if ( message.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
                                {
                                    messages.Add( message.Substring( 0, message.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]");
                                }

                            }
                            else
                            {
                                messages.Add( UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" ));

                            }
                            return false;
                        }
                        else
                        {
                            //not sure what will be returned upon successful delete - apparantly nothing
                            messages.Add( contents );
                            LoggingHelper.DoTrace( 5, string.Format( "Delete Successful. Type: {0}, CTID: {1}, OwnerCtid: {2}, contents: {3}", PublishingEntityType, EntityCtid, request.PublishForOrganizationIdentifier, contents ) );

                        }

                    }

                    return valid;
                }
            }
            catch ( Exception exc )
            {
				var ipAddress = GetSourceIPAddress();
				LoggingHelper.LogError( exc, string.Format( "RegistryServices.ManagedDelete - On POST. ctid: {0}, dataOwnerCTID: {1}, ipAddress: {2}", request.CTID, request.PublishForOrganizationIdentifier, ipAddress ) );
				valid = false;
                messages.Add( exc.Message );
                return false;

            }
		}


		public bool CredentialRegistry_SelfManagedKeysDelete(EnvelopeDelete request, string requestedBy, ref string statusMessage)
		{
			string publicKeyPath = "";
			string privateKeyPath = "";
			if ( GetKeys( ref publicKeyPath, ref privateKeyPath, ref statusMessage ) == false )
			{
				return false;
			}
			//crEnvelopeId, 
			DeleteEnvelope envelope = RegistryHandler.CreateDeleteEnvelope( publicKeyPath, privateKeyPath, request.CTID, requestedBy );
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
		/// Retrieve a resource from the registry by ctid
		/// </summary>
		/// <param name="ctid"></param>
		/// <param name="statusMessage"></param>
		/// <returns></returns>
		public static string GetResourceByCtid( string ctid, ref string ctdlType, ref string statusMessage )
		{
			string url = UtilityManager.GetAppKeyValue( "credRegistryGraphUrl" ) + ctid;
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
			//NOTE - getting by ctid means no envelopeid
			try
			{
				// Create a request for the URL.         
				WebRequest request = WebRequest.Create( resourceUrl );
				request.Credentials = CredentialCache.DefaultCredentials;
				HttpWebResponse response = ( HttpWebResponse )request.GetResponse();
				Stream dataStream = response.GetResponseStream();
				StreamReader reader = new StreamReader( dataStream );
				// Read the content.
				payload = reader.ReadToEnd();

				// Cleanup the streams and the response.

				reader.Close();
				dataStream.Close();
				response.Close();
				//just in case, likely the caller knows the context
				ctdlType = RegistryServices.GetResourceType( payload );
			}
			catch ( Exception exc )
			{
				if ( exc.Message.IndexOf( "(404) Not Found" ) > 0 )
				{
					//need to surface these better
					statusMessage = "ERROR - resource was (still) not found in registry: " + resourceUrl;
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

			//string template = "@type";
			//string template2 = "ceterms:";
			//string template3 = "ceasn:";

			////get first @type, then ceterms
			//int startPos = payload.IndexOf( template );
			//if ( startPos > 0 )
			//{
			//    int begin = startPos + template.Length;
			//    if ( payload.IndexOf( template3, begin ) > -1 )
			//    {
			//        int template2Position = payload.IndexOf( template3, begin );
			//        if ( template2Position > begin )
			//        {
			//            //now get type
			//            int endPos = payload.IndexOf( "\"", template2Position );
			//            if ( endPos > template2Position )
			//            {
			//                type = payload.Substring( template2Position + template3.Length, endPos - ( template2Position + template3.Length ) );
			//            }
			//        }
			//    }
			//    else
			//    {
			//        int template2Position = payload.IndexOf( template2, begin );
			//        if ( template2Position > begin )
			//        {
			//            //now get type
			//            int endPos = payload.IndexOf( "\"", template2Position );
			//            if ( endPos > template2Position )
			//            {
			//                type = payload.Substring( template2Position + template2.Length, endPos - ( template2Position + template2.Length ) );
			//            }
			//        }
			//    }
			//}

			//return ctdlType;

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
			string ip = "unknown";
			try
			{
				ip = HttpContext.Current.Request.ServerVariables[ "HTTP_X_FORWARDED_FOR" ];
				if ( ip == null || ip == "" || ip.ToLower() == "unknown" )
				{
					ip = HttpContext.Current.Request.ServerVariables[ "REMOTE_ADDR" ];
				}
			}
			catch ( Exception ex )
			{

			}

			return ip;
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

	public class AccountPublishRequest
	{
		public AccountPublishRequest()
		{
		}
        public string clientIdentifier { get; set; }
        public string dataOwnerCTID { get; set; }
		public string apiKey { get; set; }
		public string payloadJSON { get; set; }
		public string payloadOverflow { get; set; }
		public string publishIdentifier { get; set; }
        public string publishMethodURI { get; set; } 
        public string publishingEntityType { get; set; }
        public string ctdlType { get; set; }
        public string entityCtid { get; set; }
		public string community { get; set; } = "";
		public string skipValidation { get; set; } = "true";

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
		public bool IsGremlinOnly { get; set; } //Helps the account system figure out how to handle this query

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
		public RegistryObject( string payload )
		{
			if ( !string.IsNullOrWhiteSpace( payload ) )
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
					//not important to fully resolve yet
					if ( BaseObject.Name != null )
						Name = BaseObject.Name.ToString();
					else if ( CtdlType == "ceasn:CompetencyFramework" )
					{
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
					Name = BaseObject.Name.ToString();
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
}
