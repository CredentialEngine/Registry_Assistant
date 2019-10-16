using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
//using Factories;
using RA.Models;
using Utilities;
using System.Net.Http;
using System.Web;
using System.IO;

namespace RA.Services
{
	public class SupportServices
	{
		public static string thisClassName = "SupportServices";
		public static bool forcingUseOfCEKeys = UtilityManager.GetAppKeyValue( "forcingUseOfCEKeys", false );

		public static string credentialRegistryBaseUrl = GetAppKeyValue( "credentialRegistryBaseUrl" );
		//

		public SupportServices(string entityName, string community)
		{
			EntityName = entityName ?? "";
			Community = community ?? "";
		}
		public string EntityName { get; set; }
		public string Community { get; set; }

		#region History
		//****TODO ****
		//change to an API call
		//Main concern is losing the payload etc
		public void CallAddHistory(string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage, string publisherIdentifier)
		{
			//
			string environment = UtilityManager.GetAppKeyValue( "environment", "unknown" );
			var envFilter = string.IsNullOrWhiteSpace( Community ) ? environment : environment + "." + Community;
			var history = new RegistryPublishingHistory()
			{
				DataOwnerCTID = dataOwnerCTID,
				PublishIdentifier = publisherIdentifier,
				PublishMethodURI = publishMethodURI,
				PublishingEntityType = publishingEntityType,
				CtdlType = ctdlType,
				EntityCtid = entityCtid,
				EntityName = EntityName,
				PublishInput = payloadInput,
				PublishPayload = payloadJSON,
				Environment = envFilter,
				EnvelopeId = crEnvelopeId
			};

			var password = UtilityManager.GetAppKeyValue( "CEAccountSystemStaticPassword", "" );
			var url = UtilityManager.GetAppKeyValue( "publisherAddHistoryUrl" );
			try
			{
				var content = new StringContent( JsonConvert.SerializeObject( history ), System.Text.Encoding.UTF8, "application/json" );
				var rawData = new HttpClient().PostAsync( url, content ).Result.Content.ReadAsStringAsync().Result;

				if ( rawData == null || rawData.IndexOf( "he resource cannot be found" ) > 0
				|| rawData.IndexOf( "\"PublishingApiKey\"" ) == -1 )
				{
					statusMessage = "Error: something failed";


					LoggingHelper.DoTrace( 4, string.Format( "SupportServices.CallAddHistory. Failed. Org Ctid: {0}, publishingEntityType: {1}, Document Ctid: {2}", dataOwnerCTID, publishingEntityType, entityCtid ) );
					return;
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, string.Format( "SupportServices.CallAddHistory. Failed. Org Ctid: {0}, publishingEntityType: {1}, Document Ctid: {2}", dataOwnerCTID, publishingEntityType, entityCtid ) );
				string message = LoggingHelper.FormatExceptions( ex );
				LoggingHelper.DoTrace( 4, string.Format( "SupportServices.CallAddHistory. Failed. Org Ctid: {0}, publishingEntityType: {1}, Document Ctid: {2}", dataOwnerCTID, publishingEntityType, entityCtid ) );
				statusMessage = message;
				return;
			}
		}   //

		public void AddHistory(string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage, string publisherIdentifier, bool wasChanged)
		{
			//
	
		} //

		public static bool GetPublishingOrgByApiKey(string apiKey, ref string publisherCTID, ref List<string> messages)
		{
			bool isTrustedPartner = false;

			return GetPublishingOrgByApiKey( apiKey, ref publisherCTID, ref messages, ref isTrustedPartner ) ;
		} //

		public static bool GetPublishingOrgByApiKey(string apiKey, ref string ctid, ref List<string> messages, ref bool isTrustedPartner)
		{
			bool isValid = true;
			

			return isValid;
		}  //

		/// <summary>
		/// Retrieve org if not previously published, and publishing org is a trusted partner.
		/// Really only need to know if exists, so probably don't have to cache
		/// </summary>
		/// <param name="ctid"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static bool FindOwningOrgByCTID(string ctid, ref bool recordExists, ref List<string> messages)
		{
			bool isValid = true;
			

			return isValid;
		}


		/// <summary>
		/// Check if requested target has been previously published and by what method
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="ctid"></param>
		/// <param name="cer"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static bool ValidateAgainstPastRequest(string entityType, string ctid, ref CER cer, ref List<string> messages)
		{

			bool recordWasFound = false;
			return ValidateAgainstPastRequest( entityType, ctid, ref cer, ref messages, ref recordWasFound );
		}

		/// <summary>
		/// Check if requested target has been previously published and by what method
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="ctid"></param>
		/// <param name="cer"></param>
		/// <param name="messages"></param>
		/// <param name="recordWasFound">True if previously published, else false</param>
		/// <returns></returns>
		public static bool ValidateAgainstPastRequest( string entityType, string ctid, ref CER cer, ref List<string> messages, ref bool recordWasFound)
		{
			

			return true;//??
		}

		/// <summary>
		/// Get last publish event data
		/// Use to vaidate integrity of request
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="ctid"></param>
		/// <param name="recordWasFound">Return True if there was a previous event</param>
		/// <param name="usedCEKeys">Return True if last publish used CE keys</param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static RegistryPublishingHistory GetMostRecentHistory( string entityType, string ctid, ref bool recordWasFound, ref bool usedCEKeys, ref string message, string community="" )
		{
			var lastPublishEvent = new RegistryPublishingHistory();
			

			return lastPublishEvent;
		}
		#endregion

		public static string FormatRegistryUrl( int urlType, string suffix, string community = "")
		{
			var url = "";
			string cmnty = string.IsNullOrWhiteSpace(community) ? "" : community.ToLower() + "/";
			switch (urlType)
			{
				case 1:
					url = credentialRegistryBaseUrl + cmnty + "graph/" + suffix.ToLower();
					break;
				case 2:
					url = credentialRegistryBaseUrl + cmnty + "resources/" + suffix.ToLower();
					break;
				case 3:
					url = credentialRegistryBaseUrl + cmnty + "envelopes/" + suffix.ToLower();
					break;
				default:
					url = "";
					break;
			}
			
			return url;
		}
		#region Activity
		public void AddActivity( SiteActivity request )
		{

		}

		#endregion

		#region look up codes
		public static bool InitializeConceptSchemes()
		{
			LoggingHelper.DoTrace( 1, thisClassName + ".InitializeConceptSchemes***" );
			bool isValid = true;
			List<string> messages = new List<string>();
			ConceptSchemes schemes = new ConceptSchemes();

			isValid = SupportServices.GetConceptSchemes( ref messages, ref schemes );
			return isValid;

		}
		public static bool GetConceptSchemes( ref List<string> messages, ref ConceptSchemes results )
		{
			bool isValid = true;
			var cache = new CachedConceptSchemes();
			results = new ConceptSchemes();
			string key = "ctdlConceptSchemes";
			int cacheMinutes = 1440;
			DateTime maxTime = DateTime.Now.AddMinutes( cacheMinutes * -1 );
			if ( HttpRuntime.Cache[ key ] != null && cacheMinutes > 0 )
			{
				//may want to use application cache
				cache = ( CachedConceptSchemes )HttpRuntime.Cache[ key ];
				try
				{
					if ( cache.LastUpdated > maxTime )
					{
						LoggingHelper.DoTrace( 6, string.Format( thisClassName + ".GetConceptSchemes. Using cached version of ConceptSchemes") );

						results= cache.Schemes;
						return true;
					}
				}
				catch ( Exception ex )
				{
					LoggingHelper.DoTrace( 5, thisClassName + ".GetConceptSchemes === exception " + ex.Message );
					//just fall thru and retrieve
				}
			}

			//get schemes from API - this will need to always be the production password!
			var password = UtilityManager.GetAppKeyValue("apiProductionPublisherIdentifier", "" );
			var getUrl = string.Format( UtilityManager.GetAppKeyValue( "publisherApiGetConceptSchemes" ), password );
			SupportResponse response = new SupportResponse();
			try
			{
				var rawData = new HttpClient().PostAsync( getUrl, null ).Result.Content.ReadAsStringAsync().Result;

				if ( rawData == null || rawData.IndexOf( "The resource cannot be found" ) > 0
				)
				{
					//messages.Add( "Error: the concepts schemes were not found using fallback?????? " );
					LoggingHelper.DoTrace( 2, string.Format( "GetConceptSchemes. the concepts schemes were not found using fallback?????? ") );

					string conceptSchemesLocation = UtilityManager.GetAppKeyValue( "conceptSchemesLocation", "" );
					if ( string.IsNullOrWhiteSpace( conceptSchemesLocation ) )
					{
						LoggingHelper.DoTrace( 2, string.Format( "GetConceptSchemes. the concepts schemes appKey ('conceptSchemesLocation') was  not found using fallback?????? ", conceptSchemesLocation ) );
						return false;
					}
					string publicPath = "";
					if ( conceptSchemesLocation.ToLower().StartsWith( "c:\\" ) )
						publicPath = conceptSchemesLocation;
					else
						publicPath = Path.Combine( HttpRuntime.AppDomainAppPath, conceptSchemesLocation );

					string schemesAsJson = File.ReadAllText( publicPath );
					if ( string.IsNullOrWhiteSpace( schemesAsJson ) )
					{
						LoggingHelper.DoTrace( 2, string.Format( "GetConceptSchemes. the concepts schemes location ({0}) was  not found using fallback?????? ", conceptSchemesLocation ) );
						return false;
					}
					response.ConceptSchemes = new JavaScriptSerializer().Deserialize<ConceptSchemes>( schemesAsJson );

					//return false;
				} else
				{
					response = new JavaScriptSerializer().Deserialize<SupportResponse>( rawData );
				}

				
				if ( response != null && response.ConceptSchemes != null && response.ConceptSchemes.ConceptScheme != null && response.ConceptSchemes.ConceptScheme.Count > 0)
				{
					results = response.ConceptSchemes;
					//add to cache
					if ( key.Length > 0 && cacheMinutes > 0 )
					{
						var newCache = new CachedConceptSchemes()
						{
							Schemes = results,
							LastUpdated = DateTime.Now
						};
						if ( HttpContext.Current != null )
						{
							if ( HttpContext.Current.Cache[ key ] != null )
							{
								HttpRuntime.Cache.Remove( key );
								HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );

								LoggingHelper.DoTrace( 5, thisClassName + ".GetConceptSchemes $$$ Updating cached version " );

							}
							else
							{
								LoggingHelper.DoTrace( 6, string.Format( thisClassName + ".GetConceptSchemes ****** Inserting new cached version : no cached record found" ) );

								System.Web.HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );
							}
						}
					}
					//
				}
				else
				{
					//messages.Add( "Error: was not able to retrieve ConceptSchemes - do we have a fall back to just use old methods.");
					LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".GetConceptSchemes FAILED. Error: was not able to retrieve ConceptSchemes - do we have a fall back to just use old methods.") );
					return false;
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, thisClassName + ".GetConceptSchemes" );
				string message = LoggingHelper.FormatExceptions( ex );
				messages.Add( message );
				return false;
			}


			return isValid;
		}
		//public void ResolveOnetCodes(List<string> codes)
		//{

		//	List<CodeItem> list = CodesManager.SOC_Search( codes );


		//}
		#endregion

		#region === Application Keys Methods ===

		/// <summary>
		/// Gets the value of an application key from web.config. Returns blanks if not found
		/// </summary>
		/// <remarks>This clientProperty is explicitly thread safe.</remarks>
		public static string GetAppKeyValue(string keyName)
		{

			return GetAppKeyValue(keyName, "");
		} //

		/// <summary>
		/// Gets the value of an application key from web.config. Returns the default value if not found
		/// </summary>
		/// <remarks>This clientProperty is explicitly thread safe.</remarks>
		public static string GetAppKeyValue(string keyName, string defaultValue)
		{
			string appValue = "";
			if (string.IsNullOrWhiteSpace(keyName))
			{
				LoggingHelper.LogError(string.Format("@@@@ Error: Empty string AppKey was encoutered, using default of: {0}", defaultValue));
				return defaultValue;
			}
			try
			{
				appValue = System.Configuration.ConfigurationManager.AppSettings[keyName];
				if (appValue == null)
					appValue = defaultValue;
			}
			catch
			{
				appValue = defaultValue;
				LoggingHelper.LogError(string.Format("@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue));
			}

			return appValue;
		} //
		public static int GetAppKeyValue(string keyName, int defaultValue)
		{
			int appValue = -1;
			if (string.IsNullOrWhiteSpace(keyName))
			{
				LoggingHelper.LogError(string.Format("@@@@ Error: Empty int AppKey was encoutered, using default of: {0}", defaultValue));
				return defaultValue;
			}
			try
			{
				appValue = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[keyName]);

				// If we get here, then number is an integer, otherwise we will use the default
			}
			catch
			{
				appValue = defaultValue;
				LoggingHelper.LogError(string.Format("@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue));
			}

			return appValue;
		} //
		public static bool GetAppKeyValue(string keyName, bool defaultValue)
		{
			bool appValue = false;
			if (string.IsNullOrWhiteSpace(keyName))
			{
				LoggingHelper.LogError(string.Format("@@@@ Error: Empty bool AppKey was encoutered, using default of: {0}", defaultValue));
				return defaultValue;
			}
			try
			{
				appValue = bool.Parse(System.Configuration.ConfigurationManager.AppSettings[keyName]);

				// If we get here, then number is an integer, otherwise we will use the default
			}
			catch
			{
				appValue = defaultValue;
				LoggingHelper.LogError(string.Format("@@@@ Error on appKey: {0},  using default of: {1}", keyName, defaultValue));
			}

			return appValue;
		} //
		#endregion
	}
	[Serializable]
	public class GetOrgResult
	{
		public AccountOrganization data { get; set; }
	}
	public class AccountOrganization
	{
		public string CTID { get; set; }
		public string Name { get; set; }
		public bool CanPublish { get; set; }
		public bool IsTrustedPartner { get; set; }
		public List<string> ApprovedPublishingMethods { get; set; } = new List<string>();
		public List<string> ApprovedPublishingRoles { get; set; } = new List<string>();
		public List<string> ApprovedConsumingMethods { get; set; } = new List<string>();

		public string PublicPrivateKey { get; set; }
		public string PublishingApiKey { get; set; }

	}

	[Serializable]
	public class OrganizationAddition : AccountOrganization
	{
		public string ProfileName { get; set; }
		public string FEIN { get; set; }
		public string DUNS { get; set; }
		public string OPEID { get; set; }
		public string Url { get; set; }

		public List<string> OrganizationPublishingRoleUris { get; set; } = new List<string>();
		public List<string> OrganizationPublishingMethodUris { get; set; } = new List<string>();
		public List<string> OrganizationConsumingMethodUris { get; set; } = new List<string>();
		//for these, may want acxess to json class??
		public List<string> OrganizationTypeUris { get; set; } = new List<string>(); //ceterms:OrganizationType concept scheme
		public string OrganizationSectorUri { get; set; } //ceterms:OrganizationSector concept scheme

		#region properties for adding
		public Address MainAddress { get; set; } = new Address();
		public string PrimaryPhoneNumber { get; set; }
		public string PrimaryEmail { get; set; }
		#endregion
	}
	[Serializable]
	public class Address 
	{
		public string Name { get; set; }
		public string StreetAddress { get; set; }
		public string City { get; set; }
		public string StateProvince { get; set; }
		public string Country { get; set; }
		public string PostalCode { get; set; }

	}
	[Serializable]
	public class OrgCache
	{
		public OrgCache()
		{
			lastUpdated = DateTime.Now;
		}
		public DateTime lastUpdated { get; set; }
		public AccountOrganization Organization { get; set; } = new AccountOrganization();

	}
	[Serializable]
	public class StringCache
	{
		public StringCache()
		{
			lastUpdated = DateTime.Now;
		}
		public DateTime lastUpdated { get; set; }
		public string Item { get; set; }

	}
	[Serializable]
	public class CachedConceptSchemes
	{
		public CachedConceptSchemes()
		{
			LastUpdated = DateTime.Now;
		}
		public DateTime LastUpdated { get; set; }
		public ConceptSchemes Schemes { get; set; }

	}
	[Serializable]
	public class SupportResponse
	{
		public SupportResponse()
		{
			Messages = new List<string>();
		}
		public ConceptSchemes ConceptSchemes { get; set; }
		public bool Successful { get; set; }

		public List<string> Messages { get; set; }

	}
}
