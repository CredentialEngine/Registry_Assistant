using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RAResponse = RA.Models.RegistryAssistantResponse;
using RA.Models.Input;

namespace RA.Implementations
{
	public class Services
	{
		public static string serviceUri = "http://credentialfinder.org/ra/";
		//need process to dynamically retrieve the registry url for a target environment, such as sandbox, or production
		//<add key = "credRegistryResourceUrl" value="http://lr-staging.learningtapestry.com/resources/" />
	  
		public static string idUrl = "http://lr-staging.learningtapestry.com/resources/";

		#region JSON helpers
		public static JsonSerializerSettings GetJsonSettings()
		{
			var settings = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ContractResolver = new EmptyNullResolver(),
				Formatting = Formatting.Indented
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

		public class EmptyNullResolver : AlphaNumericContractResolver
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

		#region Mapping helpers
		

		#endregion

		#region API methods
		public static bool FormatRequest( string postBody, string requestType, ref RAResponse response )
		{
			string status = "";
			AssistantRequest req = new AssistantRequest()
			{
				EndpointType = requestType,
				RequestType = "Format",
				Identifier = requestType,
				InputPayload = postBody
			};

			req.EndpointUrl = serviceUri + string.Format( "{0}/format", requestType );

			if ( PostRequest( req ) )
			{
				response.Payload = req.FormattedPayload;
				response.RegistryEnvelopeIdentifier = req.EnvelopeIdentifier;
				return true;
			}
			else
			{
				response.Payload = req.FormattedPayload;
				//status = req.Status;
				return false;
			}
		}
		public static bool PublishRequest( string postBody, string requestType, ref RAResponse response )
		{
			string status = "";
			AssistantRequest req = new AssistantRequest()
			{
				EndpointType = requestType,
				RequestType = "publish",
				Identifier = requestType,
				InputPayload = postBody
			};
			
			req.EndpointUrl = serviceUri + string.Format( "{0}/publish", requestType );

			if ( PostRequest( req ) )
			{
				response.Payload = req.FormattedPayload;
				response.RegistryEnvelopeIdentifier = req.EnvelopeIdentifier;
				return true;
			}
			else
			{
				response.Payload = req.FormattedPayload;
				return false;
			}
		}
		public static bool PublishRequest( AssistantRequest request )
		{

			string serviceUri = "http://credentialfinder.org/ra/";
			request.EndpointUrl = serviceUri + string.Format( "{0}/publish", request.EndpointType );

			return PostRequest( request );
		}
		public static bool PostRequest( AssistantRequest request )
		{
			RAResponse response = new RAResponse();
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );


					var task = client.PostAsync( request.EndpointUrl,
						new StringContent( request.InputPayload, Encoding.UTF8, "application/json" ) );
					task.Wait();
					var result = task.Result;
					var contents = task.Result.Content.ReadAsStringAsync().Result;

					if ( result.IsSuccessStatusCode == false )
					{
						response = JsonConvert.DeserializeObject<RAResponse>( contents );
						//logging???
						string status = string.Join( ",", response.Messages.ToArray() );
						request.FormattedPayload = response.Payload ?? "";
						request.Messages.AddRange( response.Messages );

						//add error logging

					}
					else
					{
						response = JsonConvert.DeserializeObject<RAResponse>( contents );
						//
						//a request with error messages returns a 200 code )=(OK), so check response.Successful
						if ( response.Successful )
						{
							//payload is always returned for reference
							request.FormattedPayload = response.Payload;
							//envelope identifier from registry is useful to store
							request.EnvelopeIdentifier = response.RegistryEnvelopeIdentifier;
							//may have some warnings to display
							request.Messages.AddRange( response.Messages );
						}
						else
						{
							
							request.Messages.AddRange( response.Messages );
							request.FormattedPayload = response.Payload;
							//logging/notifications
							return false;
						}

					}
					return result.IsSuccessStatusCode;
				}
			}
			catch ( Exception exc )
			{
				//logging
				request.Messages.Add( exc.Message );
				return false;

			}
			
		}
		#endregion

		public class AssistantRequest
		{
			public AssistantRequest()
			{
				//response = new RAResponse();
				Messages = new List<string>();
			}
			//input
			public string RequestType { get; set; }
			public string Submitter { get; set; }
			public string InputPayload { get; set; }
			public string EndpointType { get; set; }
			public string EndpointUrl { get; set; }
			public string Identifier { get; set; }
			public string EnvelopeIdentifier { get; set; }

			public string FormattedPayload { get; set; }

			//public string Status { get; set; }
			public List<string> Messages { get; set; }
			//public RAResponse response { get; set; }
		}
	}
}
