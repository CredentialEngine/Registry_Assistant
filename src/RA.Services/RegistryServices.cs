using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Utilities;
//using JWT;
//using J2 = Utilities.JsonWebToken2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CredentialRegistry;

namespace RA.Services
{
	public class RegistryServices
	{
		public RegistryServices( string entityType, string ctdlType, string ctid)
		{
            PublishingEntityType = entityType;
            CtdlType = ctdlType;
            EntityCtid = ctid;
        }
        public RegistryServices( string entityType, string ctdlType, string ctid, string serializedInput )
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
		public string PublishingForOrgCtid { get; set; }
        public string PublishingEntityType { get; set; }
        public string CtdlType { get; set; }
        public string EntityCtid { get; set; }
        public string SerializedInput { get; set; } = "";
        public bool IsManagedRequest { get; set; }
        public bool SkippingValidation { get; set; }
        public bool IsManagedRequestOld { get; set; }
		#region Publishing

		/// <summary>
		/// Publish a document to the Credential Registry
		/// </summary>
		/// <param name="payload">Serialized version of request object</param>
		/// <param name="submitter"></param>
		/// <param name="statusMessage"></param>
		/// <param name="crEnvelopeId"></param>
		/// <returns></returns>
		public bool Publish( string payload,
									string submitter,
									string identifier,
									ref string statusMessage,
									ref string crEnvelopeId)
		{
			var successful = true;
			if ( IsManagedRequest )
				ManagedPublishThroughAccounts( payload,
					this.PublisherAuthorizationToken,
					this.PublishingForOrgCtid,
					submitter, identifier, ref successful, ref statusMessage, ref crEnvelopeId );
			else
				SelfPublish( payload, submitter, identifier, ref successful, ref statusMessage, ref crEnvelopeId, SkippingValidation );
			
				
			return successful;
		} //

		/// <summary>
		/// Publish using
		/// </summary>
		/// <param name="payload"></param>
		/// <param name="submitter"></param>
		/// <param name="identifier"></param>
		/// <param name="valid"></param>
		/// <param name="status"></param>
		/// <param name="crEnvelopeId"></param>
		/// <param name="forceSkipValidation"></param>
		/// <returns></returns>
		public string SelfPublish( string payload,
				string submitter,
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

				

				if ( string.IsNullOrWhiteSpace( crEnvelopeId ) )
				{
					Envelope envelope = new Envelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload, envelope );
                    if ( environment != "sandbox" )
                    {
                        envelope.EnvelopeCetermsCtid = EntityCtid;
                        envelope.EnvelopeCtdlType = CtdlType;
                    }
					postBody = JsonConvert.SerializeObject( envelope, new ServiceHelperV2().GetJsonSettings() );

					LoggingHelper.DoTrace( 7, "RegistryServices.SelfPublish - ADD envelope: \r\n" + postBody );
				}
				else
				{
					UpdateEnvelope envelope = new UpdateEnvelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload, envelope );

					//now embed 
					envelope.EnvelopeIdentifier = crEnvelopeId;
                    if ( environment != "sandbox" )
                    {
                        envelope.EnvelopeCetermsCtid = EntityCtid;
                        envelope.EnvelopeCtdlType = CtdlType;
                    }
                    postBody = JsonConvert.SerializeObject( envelope, new ServiceHelperV2().GetJsonSettings() );

					LoggingHelper.DoTrace( 5, string.Format( "RegistryServices.SelfPublish - updating existing envelopeId: {0}. update envelope: \r\n", crEnvelopeId )  );
					LoggingHelper.DoTrace( 7, "RegistryServices.SelfPublish - update envelope: \r\n" + postBody );
				}


                //Do publish
                string serviceUri = UtilityManager.GetAppKeyValue( "credentialRegistryPublishUrl" );
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
				try
				{
					using ( var client = new HttpClient() )
					{
						client.DefaultRequestHeaders.
							Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					

						var task = client.PostAsync( serviceUri,
							new StringContent( postBody, Encoding.UTF8, "application/json" ) );
						task.Wait();
						var response = task.Result;
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
							LoggingHelper.DoTrace( 7, "contents after successful publish.\r\n" + contents );
							UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>( contents );
							crEnvelopeId = ue.EnvelopeIdentifier;
                            LoggingHelper.DoTrace( 5, "Returned EnvelopeId - " + crEnvelopeId );

                            LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", payload, "", false );
                            //LoggingHelper.WriteLogFile( 7, identifier + "_envelope_Successful", postBody, "", false );

                            if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", false ) )
                            {
                                HistoryServices mgr = new HistoryServices();
                                mgr.Add( "N/A", payload, "Self Publish", PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status );
                            }
                        }

						return contents;
					}
				}
				catch ( Exception ex )
				{
					
					LoggingHelper.LogError( ex, string.Format( "RegistryServices.SelfPublish - On POST. identifier: {0}, crEnvelopeId: {1}", identifier, crEnvelopeId ) );
					valid = false;
					status = "Failed on Registry Publish: " + LoggingHelper.FormatExceptions( ex );
					messages.Add( status );
					return "";
				}
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
			AccountPublishRequest apr = new AccountPublishRequest();
			apr.payloadJSON = payload;
			apr.apiKey = apiKey;
			apr.dataOwnerCTID = dataOwnerCTID;
            apr.publishMethodURI = "publishMethod:RegistryAssistant";
            apr.publishingEntityType = PublishingEntityType;
            apr.ctdlType = CtdlType;
            apr.entityCtid = EntityCtid;
            apr.serializedInput = SerializedInput;

            LoggingHelper.DoTrace( 6, "RegistryServices.ManagedPublishThroughAcccounts - dataOwnerCTID: \r\n" + dataOwnerCTID );
			
			//get accounts url
			string serviceUri = UtilityManager.GetAppKeyValue( "accountsPublishApi" );
            bool skippingValidation = SkippingValidation ? true : UtilityManager.GetAppKeyValue("skippingValidation") == "yes";
            if ( payload.ToLower().IndexOf( "@graph" ) > 0 )
                skippingValidation = true;
            if ( skippingValidation )
            {
                if ( serviceUri.ToLower().IndexOf("skip_validation") > 0 )
                {
                    //assume OK, or check to change false to true
                    serviceUri = serviceUri.Replace("skip_validation=false", "");
                    apr.skipValidation = "true";
                }
                else
				{
                    //append
                    apr.skipValidation = "true";
                }
            }

            //already serialized!
            string postBody = JsonConvert.SerializeObject( apr, new ServiceHelperV2().GetJsonSettings() );
            string contents = "";

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
							valid = true;//maybe
							LoggingHelper.DoTrace( 7, "contents after successful managed publish.\r\n" + contents );
							UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>( contents );
							crEnvelopeId = ue.EnvelopeIdentifier;
                            LoggingHelper.DoTrace( 5, "EnvelopeId - " + crEnvelopeId );
                            LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", payload, "", false );


                            if ( UtilityManager.GetAppKeyValue( "loggingPublishingHistory", false ) ) {
                                HistoryServices mgr = new HistoryServices();
                                mgr.Add( dataOwnerCTID, payload, apr.publishMethodURI, PublishingEntityType, CtdlType, EntityCtid, SerializedInput, crEnvelopeId, ref status );
                            }
                        }

					}

					return contents;
				}
			}
			catch ( Exception ex )
			{
                
				LoggingHelper.LogError( ex, string.Format("RegistryServices.ManagedPublishThroughAcccounts. identifier: {0}, apiKey: {1}, contents: ",identifier, apiKey) + contents );
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
        public string ManagedPublish(string payload,
                string authorizationToken,
                string publishingOrgUid,
                string submitter,
                string identifier,
                ref bool valid,
                ref string status,
                ref string crEnvelopeId,
                bool forceSkipValidation = false)
        {
            valid = true;
            var postBody = "";
            List<string> messages = new List<string>();



            //todo - need to add signer and other to the content
            //note for new, DO NOT INCLUDE an EnvelopeIdentifier property 
            //		-this is necessary due to a bug, and hopefully can change back to a single call

            LoggingHelper.DoTrace(6, "RegistryServices.ManagedPublish(OLD) - payload: \r\n" + payload);
            //already serialized!
            //postBody = JsonConvert.SerializeObject( payload );
            postBody = payload;


            //Do publish
            string serviceUri = string.Format(UtilityManager.GetAppKeyValue("managedRegistryUrl"), publishingOrgUid);

            string contents = "";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "ApiToken " + authorizationToken);

                    var task = client.PostAsync(serviceUri,
                        new StringContent(postBody, Encoding.UTF8, "application/json"));
                    task.Wait();
                    var response = task.Result;
                    //should get envelope_id from contents?
                    contents = task.Result.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode == false)
                    {
                        RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>(contents);
                        //
                        valid = false;
                        string queryString = GetRequestContext();
                        //now null:
                        //+ "\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )
                        LoggingHelper.LogError(identifier + " RegistryServices.Publish Failed:"
                            + "\n\rURL:\n\r " + queryString
                            + "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject(response)
                            + "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject(contents),
                            false, "CredentialRegistry publish failed for " + identifier);
                        if (contentsJson.Errors != null && contentsJson.Errors.Count > 0)
                        {
                            status = string.Join(",", contentsJson.Errors.ToArray());
                            messages.AddRange(contentsJson.Errors);
                        }
                        else
                        {
                            status = contents;
                            messages.Add(status);
                        }

                        LoggingHelper.WriteLogFile(5, identifier + "_payload_failed.json", payload, "", false);
                        //LoggingHelper.WriteLogFile( 7, identifier + "_envelope_failed", postBody, "", false );
                        //statusMessage =contents.err contentsJson.Errors.ToString();
                    }
                    else
                    {
                        valid = true;
                        LoggingHelper.DoTrace(6, "contents after successful managed publish.\r\n" + contents);
                        UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>(contents);
                        crEnvelopeId = ue.EnvelopeIdentifier;

                        LoggingHelper.WriteLogFile(6, identifier + "_payload_Successful.json", payload, "", false);
                        //LoggingHelper.WriteLogFile( 7, identifier + "_envelope_Successful", postBody, "", false );
                    }

                    return contents;
                }
            }
            catch (Exception ex)
            {

                LoggingHelper.LogError(ex, "RegistryServices.Publish - POST");
                valid = false;
                status = "Failed on Registry Publish: " + LoggingHelper.FormatExceptions(ex);
                messages.Add(status);
                return status;
            }

        }
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
        public bool ManagedDelete( string dataOwnerCTID, string ctid, string apiKey, ref string statusMessage )
        {
            bool valid = true;
            AccountPublishRequest apr = new AccountPublishRequest();
            apr.apiKey = apiKey;
            apr.dataOwnerCTID = dataOwnerCTID;
            apr.publishMethodURI = "publishMethod:RegistryAssistant";
            apr.publishingEntityType = PublishingEntityType;
            apr.entityCtid = ctid;
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
                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent( postBody, Encoding.UTF8, "application/json" ),
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri( serviceUri )
                    };
                    var task = client.SendAsync( request );
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
                        statusMessage = string.Join( ",", contentsJson.Errors.ToArray() );
                        return false;
                    }
                    else
                    {
                        if ( contents.ToLower().IndexOf( "{\"data\":" ) == 0 && contents.ToLower().IndexOf( "error:" ) > -1 )
                        {
                            valid = false;
                            statusMessage = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );
                            return false;
                        }
                        else if ( contents.ToLower().IndexOf( "error" ) > 0 && contents.ToLower().IndexOf( "{" ) > -1 )
                        {
                            valid = false;
                            contents = contents.Replace( "[[", "[" ).Replace( "]]", "]" );
                            RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
                            if ( contentsJson.Errors != null && contentsJson.Errors.Count > 0 )
                            {
                                statusMessage = string.Join( ",", contentsJson.Errors.ToArray() );
                                //remove extraneous message of:@type : did not match one of the following values
                                if ( statusMessage.IndexOf( ", \"@type : did not match one of the following values" ) > 50 )
                                {
                                    statusMessage = statusMessage.Substring( 0, statusMessage.IndexOf( ", \"@type : did not match one of the following values" ) ) + "]";
                                }

                            }
                            else
                            {
                                statusMessage = UtilityManager.ExtractNameValue( contents, "Error", ":", "\"" );

                            }
                            return false;
                        }
                        else
                        {
                            //not sure what will be returned upon successful delete - apparantly nothing
                            statusMessage = contents;
                            LoggingHelper.DoTrace( 6, string.Format( "Delete Successful. Type: {0}, CTID: {1}, OwnerCtid: {2}, contents: {3}", PublishingEntityType, EntityCtid, dataOwnerCTID, contents ) );

                        }

                    }

                    return valid;
                }
            }
            catch ( Exception exc )
            {
                LoggingHelper.LogError( exc, "RegistryServices.ManagedDelete" );
                statusMessage = exc.Message;
                return false;

            }

        }


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
			if ( !DeleteRequest( postBody, serviceUri, ref response ) )
			{
				//failed
				//not sure what to use for a statusMessage message
				statusMessage = response;
				return false;
			}

			return true;
		}



		private static bool DeleteRequest( string postBody, string serviceUri, ref string response )
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

		#endregion
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
        public string publishIdentifier { get; set; }
        public string publishMethodURI { get; set; } = "publishMethod:RegistryAssistant";
        public string publishingEntityType { get; set; }
        public string ctdlType { get; set; }
        public string entityCtid { get; set; }
        public string serializedInput { get; set; }
        public string skipValidation { get; set; }
    }

    public class AccountPublishResponse
    {
        public AccountPublishResponse()
        {
            Messages = new List<string>();
        }
        public string ResponseType { get; set; } = "accountResponse";
        public bool Successful { get; set; } = true;

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
}
