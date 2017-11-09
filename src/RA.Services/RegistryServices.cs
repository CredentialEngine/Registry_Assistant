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
using MetadataRegistry;

namespace RA.Services
{
	public class RegistryServices
	{

		#region Publishing

		/// <summary>
		/// Publish a document to the Credential Registry
		/// </summary>
		/// <param name="payload"></param>
		/// <param name="submitter"></param>
		/// <param name="statusMessage"></param>
		/// <param name="crEnvelopeId"></param>
		/// <returns></returns>
		public bool Publish( string payload,
									string submitter,
									string identifier,
									ref string statusMessage,
									ref string crEnvelopeId )
		{
			var successful = true;
			var result = Publish( payload, submitter, identifier, ref successful, ref statusMessage, ref crEnvelopeId );
			return successful;
		}


		//Used for demo page, and possibly other cases where the raw response is desired
		public string Publish( string payload,
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

			try
			{
				//TODO - add handling where keys are stored in the registry
				if ( GetKeys( ref publicKeyPath, ref privateKeyPath, ref status ) == false )
				{
					valid = false;
					//no, the proper error is returned from GetKeys
					//status = "Error getting CER Keys";
					return status;
				}

				//todo - need to add signer and other to the content
				//note for new, DO NOT INCLUDE an EnvelopeIdentifier property 
				//		-this is necessary due to a bug, and hopefully can change back to a single call

				LoggingHelper.DoTrace( 5, "RegistryServices.Publish - payload: \r\n" + payload );

				if ( string.IsNullOrWhiteSpace( crEnvelopeId ) )
				{
					Envelope envelope = new Envelope();
					//envelope = RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload );
					//OR
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload, envelope );

					postBody = JsonConvert.SerializeObject( envelope );

					LoggingHelper.DoTrace( 7, "RegistryServices.Publish - ADD envelope: \r\n" + postBody );
				}
				else
				{
					UpdateEnvelope envelope = new UpdateEnvelope();
					RegistryHandler.CreateEnvelope( publicKeyPath, privateKeyPath, payload, envelope );

					//now embed 
					envelope.EnvelopeIdentifier = crEnvelopeId;
					postBody = JsonConvert.SerializeObject( envelope );

					LoggingHelper.DoTrace( 7, "RegistryServices.Publish - update envelope: \r\n" + postBody );
				}

				//Do publish
				string serviceUri = UtilityManager.GetAppKeyValue( "credentialRegistryPublishUrl" );
				var skippingValidation = forceSkipValidation ? true : UtilityManager.GetAppKeyValue( "skippingValidation" ) == "yes";
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
						var contents = task.Result.Content.ReadAsStringAsync().Result;

						if ( response.IsSuccessStatusCode == false )
						{
							RegistryResponseContent contentsJson = JsonConvert.DeserializeObject<RegistryResponseContent>( contents );
							//
							valid = false;
							string queryString = GetRequestContext();

							LoggingHelper.LogError( identifier + " RegistryServices.Publish Failed:"
								+ "\n\rURL:\n\r " + queryString
								+ "\n\rERRORS:\n\r " + string.Join( ",", contentsJson.Errors.ToArray() )
								+ "\n\rRESPONSE:\n\r " + JsonConvert.SerializeObject( response )
								+ "\n\rCONTENTS:\n\r " + JsonConvert.SerializeObject( contents ),
								false, "CredentialRegistry publish failed for " + identifier );

							status = string.Join( ",", contentsJson.Errors.ToArray() );

							LoggingHelper.WriteLogFile( 5, identifier + "_payload_failed.json", payload, "", false );
							//LoggingHelper.WriteLogFile( 7, identifier + "_envelope_failed", postBody, "", false );
							//statusMessage =contents.err contentsJson.Errors.ToString();
						}
						else
						{
							valid = true;
							UpdateEnvelope ue = JsonConvert.DeserializeObject<UpdateEnvelope>( contents );
							crEnvelopeId = ue.EnvelopeIdentifier;

							//LoggingHelper.DoTrace( 7, "response: " + JsonConvert.SerializeObject( contents ) );

							LoggingHelper.WriteLogFile( 6, identifier + "_payload_Successful.json", payload, "", false );
							//LoggingHelper.WriteLogFile( 7, identifier + "_envelope_Successful", postBody, "", false );
						}

						return contents;
					}
				}
				catch ( Exception ex )
				{
					LoggingHelper.LogError( ex, "RegistryServices.Publish - POST" );
					valid = false;
					status = "Failed on Registry Publish: " + LoggingHelper.FormatExceptions( ex );
					return status;
				}
				//Set return values
				//no cr id returned?

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "RegistryServices.Publish" );

				valid = false;
				crEnvelopeId = "";
				status = "Failed during Registry preperations: " + LoggingHelper.FormatExceptions( ex );
				return status;
			}
		}
		//


		//
		private static string GetRequestContext()
		{
			string queryString = "batch";
			try
			{
				queryString = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
			}
			catch ( Exception exc )
			{
				return queryString;
			}
			return queryString;
		}
		private static bool PostRequest( string postBody, string serviceUri, ref string response )
		{
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );


					var task = client.PostAsync( serviceUri,
						new StringContent( postBody, Encoding.UTF8, "application/json" ) );
					task.Wait();
					var result = task.Result;
					response = JsonConvert.SerializeObject( result );
					var contents = task.Result.Content.ReadAsStringAsync();

					if ( result.IsSuccessStatusCode == false )
					{
						//logging???

						LoggingHelper.LogError( "RegistryServices.PostRequest Failed\n\r" + response + "\n\r" + contents );
					}
					else
					{
						//no doc id?
						LoggingHelper.DoTrace( 6, "result: " + response );
					}
					return result.IsSuccessStatusCode;
				}
			}
			catch ( Exception exc )
			{
				LoggingHelper.LogError( exc, "RegistryServices.PostRequest" );
				return false;

			}

		}
		#endregion

		#region Deletes
		public bool CredentialRegistry_Delete( string crEnvelopeId, string ctid, string requestedBy, ref string statusMessage )
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


					//client.DefaultRequestHeaders.
					//	Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );


					//var task = client.PutAsync( serviceUri,
					//	new StringContent( postBody, Encoding.UTF8, "application/json" ) );
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
				statusMessage = "Error - the encoding key was not found";
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
}
