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
using RA.Models.Input;


namespace RA.Services
{
	public class RegistryServices
	{
		public const string RA_PUBLISH_METHOD = "publishMethod:RegistryAssistant";
		public const string CE_MANAGED_PUBLISH_METHOD = "publishMethod:ManualEntry";
		public const string CE_BULK_UPLOAD_METHOD = "publishMethod:BulkUpload";
		public const string CE_DIRECT_PUBLISH_METHOD = "ManualPublish";
		public RegistryServices( string entityType, string ctdlType, string ctid)
		{
            PublishingEntityType = entityType;
            CtdlType = ctdlType;
            EntityCtid = ctid ?? "";
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
		public string SerializedInput { get; set; } = "";
        public bool IsManagedRequest { get; set; }
		//true if originates from publisher
		public bool IsPublisherRequest { get; set; }

		public bool OverrodeOriginalRequest { get; set; }
		public bool SkippingValidation { get; set; }
  
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
		public string SelfPublish( string payload,
				string submitter,
				string dataOwnerCTID,
				string identifier,
				ref bool valid,
				ref string status,
				ref string crEnvelopeId,
				bool forceSkipValidation = false )
		{
			valid = true;
			return "NOT INCLUDED";
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
			return "NOT INCLUDED";
		}
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
		public bool DeleteRequest( DeleteRequest request, string requestType, ref List<string> messages )
		{
			bool isValid = true;
			//NOT INCLUDED
			return isValid;
		} //
		public bool CustomDelete( EnvelopeDelete request, string requestType, ref List<string> messages )
		{
			bool isValid = true;
			//NOT INCLUDED
			return isValid;
		} //

		public bool ManagedDelete( string dataOwnerCTID, string ctid, string apiKey, ref List<string> messages )
        {
            bool valid = true;
			//NOT INCLUDED
			return valid;

		}


		#endregion
		#region Helpers
	
		public bool HasValidPublisherToken()
		{
			if ( PublisherAuthorizationToken != null && PublisherAuthorizationToken.Length >= 32 )
			{
				return true;
			}
			return false;
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
        //public string serializedInput { get; set; }
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
