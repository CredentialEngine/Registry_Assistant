using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
//using Factories;
using RA.Models;
using RA.Models.BusObj;
using Utilities;
using System.Web;
using System.IO;
using System.Web.Management;

namespace RA.Services
{
	public class SupportServices
	{
		public static string thisClassName = "SupportServices";
		public static bool forcingUseOfCEKeys = UtilityManager.GetAppKeyValue( "forcingUseOfCEKeys", false );

		public static string credentialRegistryBaseUrl = UtilityManager.GetAppKeyValue( "credentialRegistryBaseUrl" );
		//
		public SupportServices()
		{
			EntityName = "";
			Community = "";
		}
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
		public void CallAddHistory(string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage, string publisherIdentifier, bool wasChanged, bool isAddTransaction = false )
		{
			//
			
		}   //

		public void AddHistory(string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage, string publisherIdentifier, bool wasChanged, bool isAddTransaction )
		{
			
	
		} //


		public void AddActivityForFormat( RA.Models.RequestHelper helper, string publishingEntityType, string entityName, string entityCtid, ref string statusMessage )
		{
		

		} //


		public bool AddActivity(  ActivityLog activity, ref string statusMessage )
		{

			return true;
		} //
		/// <summary>
		/// Validate a publishing related transaction
		/// </summary>
		/// <param name="publisherApikey"></param>
		/// <param name="dataOwnerCTID"></param>
		/// <param name="publishMethodURI"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public PublishRequestValidationResponse ValidateRegistryRequest(string publisherApikey, string dataOwnerCTID, string publishMethodURI, ref List<string> messages)
		{
			//, bool allowCaching = true
			PublishRequestValidationResponse response = new PublishRequestValidationResponse();
			//not sure if can cache, give some thought though
			string key = "validationKey_" + publisherApikey + "_" + dataOwnerCTID + "_" + publishMethodURI;
			int cacheMinutes = 1440;
			DateTime maxTime = DateTime.Now.AddMinutes( cacheMinutes * -1 );
			string environment = UtilityManager.GetAppKeyValue( "environment" );

			//no point in caching deletes/purges/transfers
			if ( HttpRuntime.Cache[ key ] != null && cacheMinutes > 0  )
			{
				var cache = ( ValidateRequestCache ) HttpRuntime.Cache[ key ];
				try
				{
					if( cache.lastUpdated > maxTime )
					{
						response = cache.Response;
						LoggingHelper.DoTrace( 6, string.Format( "===ValidatePublishingRequest === Using cached version for: {0}", key ) );

						return response;
					}
				}
				catch( Exception ex )
				{
					LoggingHelper.DoTrace( 6, thisClassName + ".ValidatePublishingRequest === exception " + ex.Message );
				}
			}

			//get Org
			var password = UtilityManager.GetAppKeyValue( "CEAccountSystemStaticPassword", "" );
			var url = UtilityManager.GetAppKeyValue( "ceAccountValidateRegistryRequest" );
			//var accountsApiUrl = string.Format( UtilityManager.GetAppKeyValue( "ceAccountValidateRegistryRequest" ), dataOwnerCTID, publishMethodURI, password );
			//	https://localhost:44384/registry/validateRequest?dataOwnerCTID={0}&amp;requestMethod={1}&amp;password={2}
			if ( environment == "development" )
			{
				//https://localhost:44384/
				//url = "https://localhost:44384/registry/validateRequest?dataOwnerCTID={0}&amp;requestMethod={1}&amp;password={2}";
			}
			var accountsApiUrl = string.Format(url, dataOwnerCTID, publishMethodURI, password );
			try
			{
				using( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + publisherApikey );

					if ( environment == "development" )
					{
						client.Timeout = new TimeSpan( 0, 30, 0 );
					}

					var rawData = client.PostAsync( accountsApiUrl, null ).Result.Content.ReadAsStringAsync().Result;
					if( rawData == null || rawData.IndexOf( "The resource cannot be found" ) > 0 )
					{
						messages.Add( "Error: unexpected response in ValidatePublishingRequest. The endpoint returned null." );
						return response;
					}
					response = new JavaScriptSerializer().Deserialize<PublishRequestValidationResponse>( rawData );

					if( response != null && response.Successful )
					{
						//note this method doesn't determine validity, just returns the results. The caller will make that detemination
						if( response.PublisherIsTrustedPartner )
						{
							LoggingHelper.DoTrace( 5, "SupportServices.ValidatePublishingRequest() Found a trusted partner: " + response.PublishingOrganization );
						}
						//add to cache
						if( key.Length > 0 && cacheMinutes > 0 )
						{
							var newCache = new ValidateRequestCache()
							{
								Response = response,
								lastUpdated = DateTime.Now
							};
							if( HttpContext.Current != null )
							{
								if( HttpContext.Current.Cache[ key ] != null )
								{
									HttpRuntime.Cache.Remove( key );
									HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );

									LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".ValidatePublishingRequest $$$ Updating cached version of {0}", key ) );

								}
								else
								{
									LoggingHelper.DoTrace( 6, string.Format( thisClassName + ".ValidatePublishingRequest ****** Inserting new cached version of Organization CTID: {0}, keypart: {1}", key, publisherApikey.Length > 20 ? publisherApikey.Substring( 0, 9 ) : "no apikey found" ) );

									System.Web.HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );
								}
							}
						}
						//
					}
					else
					{
						if (response.Messages != null && response.Messages.Count() > 0)
						{
							messages.AddRange( response.Messages );
						} else
						{
							messages.Add( "Error: unable to validate the publishing transaction for the provided publisher and data owner." );
							if ( !string.IsNullOrWhiteSpace( rawData ) )
								messages.Add( rawData );
						}
						
						LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".ValidatePublishingRequest FAILED. request key: {0}", key ) );
						return response;
					}
				}
			}
			catch( Exception ex )
			{
				LoggingHelper.LogError( ex, thisClassName + ".ValidatePublishingRequest: " + key );
				string message = LoggingHelper.FormatExceptions( ex );
				messages.Add( message );
				return response;
			}

			return response;
		}  //

		public PublishRequestValidationResponse ValidateDeleteRequest(string publisherApikey, string dataOwnerCTID, string publishMethodURI, ref List<string> messages)
		{
			//
			PublishRequestValidationResponse response = new PublishRequestValidationResponse();
			//not sure if can cache, give some thought though
			string key = "validationKey_" + publisherApikey + "_" + dataOwnerCTID;
			int cacheMinutes = 1440;
			DateTime maxTime = DateTime.Now.AddMinutes( cacheMinutes * -1 );
			if ( HttpRuntime.Cache[ key ] != null && cacheMinutes > 0 )
			{
				var cache = ( ValidateRequestCache )HttpRuntime.Cache[ key ];
				try
				{
					if ( cache.lastUpdated > maxTime )
					{
						response = cache.Response;
						LoggingHelper.DoTrace( 6, string.Format( "===ValidatePublishingRequest === Using cached version for: {0}", key ) );

						return response;
					}
				}
				catch ( Exception ex )
				{
					LoggingHelper.DoTrace( 6, thisClassName + ".ValidatePublishingRequest === exception " + ex.Message );
				}
			}

			//get Org
			var password = UtilityManager.GetAppKeyValue( "CEAccountSystemStaticPassword", "" );
			var accountsApiUrl = UtilityManager.GetAppKeyValue( "ceAccountValidateRegistryRequest" ) + string.Format( "?dataOwnerCTID={0}&publishMethodURI={1}&password={2}", dataOwnerCTID, publishMethodURI, password );
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					client.DefaultRequestHeaders.Add( "Authorization", "ApiToken " + publisherApikey );

					var rawData = client.PostAsync( accountsApiUrl, null ).Result.Content.ReadAsStringAsync().Result;
					if ( rawData == null || rawData.IndexOf( "The resource cannot be found" ) > 0 )
					{
						messages.Add( "Error: unexpected response in ValidatePublishingRequest. The endpoint returned null." );
						return response;
					}
					response = new JavaScriptSerializer().Deserialize<PublishRequestValidationResponse>( rawData );

					if ( response != null && response.Successful )
					{
						//note this method doesn't determine validity, just returns the results. The caller will make that detemination
						if ( response.PublisherIsTrustedPartner )
						{
							LoggingHelper.DoTrace( 5, "SupportServices.ValidatePublishingRequest() Found a trusted partner: " + response.PublishingOrganization );
						}
						//add to cache
						if ( key.Length > 0 && cacheMinutes > 0 )
						{
							var newCache = new ValidateRequestCache()
							{
								Response = response,
								lastUpdated = DateTime.Now
							};
							if ( HttpContext.Current != null )
							{
								if ( HttpContext.Current.Cache[ key ] != null )
								{
									HttpRuntime.Cache.Remove( key );
									HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );

									LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".ValidatePublishingRequest $$$ Updating cached version of {0}", key ) );

								}
								else
								{
									LoggingHelper.DoTrace( 6, string.Format( thisClassName + ".ValidatePublishingRequest ****** Inserting new cached version of Organization CTID: {0}, keypart: {1}", key, publisherApikey.Length > 20 ? publisherApikey.Substring( 0, 9 ) : "no apikey found" ) );

									System.Web.HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );
								}
							}
						}
						//
					}
					else
					{
						if ( response.Messages != null && response.Messages.Count() > 0 )
						{
							messages.AddRange( response.Messages );
						}
						else
						{
							messages.Add( "Error: unable to validate the publishing transaction for the provided publisher and data owner." );
						}

						LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".ValidatePublishingRequest FAILED. request key: {0}", key ) );
						return response;
					}
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, thisClassName + ".ValidatePublishingRequest: " + key );
				string message = LoggingHelper.FormatExceptions( ex );
				messages.Add( message );
				return response;
			}

			return response;
		}  //
		public static bool GetPublishingOrgByApiKey(string apiKey, ref string publisherCTID, ref List<string> messages)
		{
			bool isTrustedPartner = false;

			return GetPublishingOrgByApiKey( apiKey, ref publisherCTID, ref messages, ref isTrustedPartner ) ;
		} //

		public static bool GetPublishingOrgByApiKey(string apiKey, ref string ctid, ref List<string> messages, ref bool isTrustedPartner, bool usingCache = true)
		{
			//
			bool isValid = true;
			string key = "organizationApikey_" + apiKey;
			int cacheMinutes = 1440;
			DateTime maxTime = DateTime.Now.AddMinutes( cacheMinutes * -1 );
			if ( HttpRuntime.Cache[ key ] != null && cacheMinutes > 0 && usingCache )
			{
				//may only need to cache ctid?
				//===>> now need to also store if a trustedPartner! May want to skip when called from org methods
				//may want the org - not to check for third party, or trusted partner
				//var cache2 = ( OrgCache )HttpRuntime.Cache[ key ];
				var cache = ( StringCache )HttpRuntime.Cache[ key ];
				try
				{
					if ( cache.lastUpdated > maxTime )
					{
						//ctid = cache2.org.CTID;
						ctid = cache.Item;
						LoggingHelper.DoTrace( 6, string.Format( "===OrganizationServices.GetPublishingOrgByApiKey === Using cached version of Organization CTID: {0}", ctid ) );

						return true;
					}
				}
				catch ( Exception ex )
				{
					LoggingHelper.DoTrace( 6, thisClassName + ".GetPublishingOrgByApiKey === exception " + ex.Message );
				}
			}

			//get Org
			var password = UtilityManager.GetAppKeyValue( "CEAccountSystemStaticPassword", "" );
			var getUrl = string.Format( UtilityManager.GetAppKeyValue( "ceAccountOrganizationByApiKey" ), apiKey, password );
			try
			{
				var rawData = new HttpClient().PostAsync( getUrl, null ).Result.Content.ReadAsStringAsync().Result;

				if ( rawData == null || rawData.IndexOf( "The resource cannot be found" ) > 0
				|| rawData.IndexOf( "\"PublishingApiKey\"" ) == -1 )
				{
					messages.Add( "Error: invalid apiKey: " + apiKey );
					LoggingHelper.DoTrace( 2, string.Format( "GetPublishingOrgByApiKey. Error attempting to call method to return an org for apiKey: {0}. \r\n{1}", apiKey, rawData.ToString() ) );
					return false;
				}

				var results = new JavaScriptSerializer().Deserialize<GetOrgResult>( rawData );
				if ( results != null && results.data != null && results.data.CTID != null )
				{
					ctid = results.data.CTID.ToLower();
					if ( results.data.ApprovedPublishingRoles.Contains( RegistryServices.CE_TRUSTED_PARTNER_ROLE ) )
					{
						//should already be set
						//results.data.IsTrustedPartner = true;
						isTrustedPartner = true;
						LoggingHelper.DoTrace( 5, "SupportServices.GetPublishingOrgByApiKey() Found a trusted partner: " + results.data.Name );
					}
					//add to cache
					if ( key.Length > 0 && cacheMinutes > 0 )
					{
						var newCache = new StringCache()
						{
							Item = ctid,
							lastUpdated = DateTime.Now
						};
						if ( HttpContext.Current != null )
						{
							if ( HttpContext.Current.Cache[ key ] != null )
							{
								HttpRuntime.Cache.Remove( key );
								HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );

								LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".GetPublishingOrgByApiKey $$$ Updating cached version of Organization CTID: {0}", ctid ) );

							}
							else
							{
								LoggingHelper.DoTrace( 6, string.Format( thisClassName + ".GetPublishingOrgByApiKey ****** Inserting new cached version of Organization CTID: {0}, keypart: {1}", ctid, apiKey.Length > 20 ? apiKey.Substring( 0, 9 ) : "no apikey found" ) );

								System.Web.HttpRuntime.Cache.Insert( key, newCache, null, DateTime.Now.AddMinutes( cacheMinutes ), TimeSpan.Zero );
							}
						}
					}
					//
				}
				else
				{
					messages.Add( "Error: was not able to find an organization for the provided apiKey: " + apiKey );
					LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".GetPublishingOrgByApiKey FAILED. NO ORG RETURNED! Organization apiKey: {0}", apiKey ) );
					return false;
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, thisClassName + ".GetPublishingOrgByApiKey: " + apiKey );
				string message = LoggingHelper.FormatExceptions( ex );
				messages.Add( message );
				return false;
			}


			return isValid;
		}  //

		/// <summary>
		/// Retrieve org if not previously published, and publishing org is a trusted partner.
		/// Really only need to know if exists, so probably don't have to cache
		/// </summary>
		/// <param name="ctid"></param>
		/// <param name="messages"></param>
		/// <returns>Should only return false on an error</returns>
		public static bool FindOwningOrgByCTID(string dataOwnerCtid, ref bool recordExists, ref List<string> messages)
		{
			bool isValid = true;
			var password = UtilityManager.GetAppKeyValue( "CEAccountSystemStaticPassword", "" );
			//http://localhost:52345/Organization/GetByCTID?ctid={0}&amp;password={1}
			var url = UtilityManager.GetAppKeyValue( "ceAccountOrganizationByCTID" );
			var getUrl = string.Format( url, dataOwnerCtid, password );
			try
			{
				var rawData = new HttpClient().PostAsync( getUrl, null ).Result.Content.ReadAsStringAsync().Result;

				if ( rawData == null )
				{
					//messages.Add( "Error: invalid apiKey: " + ctid );
					return false;
				}

				var results = new JavaScriptSerializer().Deserialize<GetOrgResult>( rawData );
				if ( results != null && results.data != null && results.data.CTID != null )
				{
					//dataOwnerCtid = results.data.CTID.ToLower();
					recordExists = true;
					//
				}
				else
				{
					//may not be an error!
					//messages.Add( "Error: was not able to find an organization for the provided CTID: " + ctid );
					LoggingHelper.DoTrace( 5, string.Format( thisClassName + ".FindOwningOrgByCTID FAILED. NO ORG RETURNED! Organization ctid: {0}", dataOwnerCtid ) );
					return false;
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, thisClassName + ".FindOwningOrgByCTID: " + dataOwnerCtid );
				string message = LoggingHelper.FormatExceptions( ex );
				messages.Add( message );
				return false;
			}


			return isValid;
		}

		public static bool AddOrganizationToAccounts(RA.Models.Input.Organization r, string publisherApikey, ref List<string> messages)
		{
			bool isValid = true;
			int cnt = messages.Count();
			var password = UtilityManager.GetAppKeyValue( "CEAccountSystemStaticPassword", "" );
			//http://localhost:52345/Organization/Add/
			var accountsApiUrl = UtilityManager.GetAppKeyValue( "ceAccountAddOrganization" );

			if (string.IsNullOrWhiteSpace(publisherApikey))
			{
				messages.Add("Error: The API Key for the trusted organization must be provided.");
			}
			OrganizationAddition org = new OrganizationAddition()
			{
				//PublisherApikey = publisherApikey,
				Name = r.Name,
				Url = r.SubjectWebpage,
				//max length = 50, can't use;  r.Name.Replace( " ", "_" ).Replace("'","-"),
				ProfileName = "profile_" + Guid.NewGuid().ToString().ToLower(), //
				CTID = (r.Ctid ?? "").ToLower(),
				FEIN = r.Fein,
				DUNS = r.Duns,
				OPEID = r.OpeId
			};
			org.OrganizationPublishingRoleUris.Add( "publishRole:CredentialOrganization" );
			org.OrganizationPublishingMethodUris.Add( "publishMethod:RegistryAssistant" );
			//may need to check for or add prefix agentSector
			if ( r.AgentSectorType.ToLower().IndexOf("agentsector") == -1)
			{
				org.OrganizationSectorUri = "agentSector:" + r.AgentSectorType;
			} else
				org.OrganizationSectorUri = r.AgentSectorType;
			//may need to check for orgType
			foreach(var item in r.AgentType)
			{
				if ( item.ToLower().IndexOf( "orgtype" ) == -1 )
				{
					org.OrganizationTypeUris.Add( "orgType:" + item);
				}
				else
					org.OrganizationTypeUris.Add(item);
			}
			
			//
			if ( r.Email != null && r.Email.Count() > 0 )
			{
				org.PrimaryEmail = r.Email[ 0 ];
			} else
			{
				//temp - since some orgs don't have an email, use a default for now (or for staging)
				if ( UtilityManager.GetAppKeyValue( "environment" ) != "production")
				{
					org.PrimaryEmail = UtilityManager.GetAppKeyValue( "systemAdminEmail" ); //
				} else 
					messages.Add( "Error: In order to add an organization to the CE Account site, an email must be provided." );
			}

			if ( r.Address != null && r.Address.Count() > 0 )
			{
				var address = r.Address[ 0 ];
				org.StreetAddress = r.Address[ 0 ].Address1;
				org.City = r.Address[ 0 ].City;
				org.StateProvince = r.Address[ 0 ].AddressRegion;
				org.PostalCode = r.Address[ 0 ].PostalCode;
				org.Country = r.Address[ 0 ].Country;
				//org.MainAddress.StreetAddress = r.Address[ 0 ].Address1;
				//org.MainAddress.City = r.Address[ 0 ].City;
				//org.MainAddress.StateProvince = r.Address[ 0 ].AddressRegion;
				//org.MainAddress.PostalCode = r.Address[ 0 ].PostalCode;
				//org.MainAddress.Country = r.Address[ 0 ].Country;

				//check for a phone number
				//==> contact point could be a separate 'address'
				if (address.ContactPoint != null && address.ContactPoint.Count() > 0)
				{
					var cp = address.ContactPoint[ 0 ];
					if ( cp.PhoneNumbers != null && cp.PhoneNumbers.Count() > 0 )
						org.PrimaryPhoneNumber = cp.PhoneNumbers[ 0 ];
					else
					{
						messages.Add( "Error: In order to add an organization to the CE Account site, a primary phone number must be included in the ContactPoint class within the Address property." );
					}
				} 
				else if (r.Address.Count() > 1 
					&& ( r.Address[ 1 ].ContactPoint != null && r.Address[ 1 ].ContactPoint.Count() > 0 ) )
				{
					var cp = r.Address[ 1 ].ContactPoint[ 0 ];
					if ( cp.PhoneNumbers != null && cp.PhoneNumbers.Count() > 0 )
						org.PrimaryPhoneNumber = cp.PhoneNumbers[ 0 ];
					else
					{
						messages.Add( "Error: In order to add an organization to the CE Account site, a primary phone number must be included in the ContactPoint class within the Address property." );
					}
				}
				else
				{
					messages.Add( "Error: In order to add an organization to the CE Account site, a primary phone number must be included in the ContactPoint class within the Address property." );
				}
			}
			else
			{
				messages.Add( "Error: In order to add an organization to the CE Account site, an address must be provided." );
			}
			if ( messages.Count() > cnt )
				return false;
			//call accounts
			string contents = "";
			string postBody = JsonConvert.SerializeObject( org, ServiceHelperV2.GetJsonSettings() );
			try
			{
				using ( var client = new HttpClient() )
				{
					client.DefaultRequestHeaders.
						Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
					client.DefaultRequestHeaders.Add("Authorization", "ApiToken " + publisherApikey);

					var task = client.PostAsync( accountsApiUrl,
						new StringContent( postBody, Encoding.UTF8, "application/json" ) );
					task.Wait();
					var response = task.Result;
					contents = task.Result.Content.ReadAsStringAsync().Result;

					//may have to do additional checks
					Dictionary<string, object> dictionary = RegistryServices.JsonToDictionary( contents );
					if ( dictionary.ContainsKey( "valid" ) )
					{
						var valid = dictionary[ "valid" ].ToString();
						if ( valid.ToLower() == "false" )
						{
							var status = dictionary[ "status" ].ToString();
							messages.Add( status );
							return false;
						}
					}
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, string.Format( thisClassName + ".AddOrganizationToAccounts FAILED. Publisher apiKey: {0}, target orgCtid", publisherApikey, r.Ctid ) );
				string message = LoggingHelper.FormatExceptions( ex );
				messages.Add( message );
			}
			//
			if ( messages.Count() > cnt )
				isValid = false; 

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
		/// <param name="recordWasFound">True if previously published, else false. TODO replace with CER.HasBeenPreviouslyPublished</param>
		/// <returns></returns>
		public static bool ValidateAgainstPastRequest( string entityType, string ctid, ref CER cer, ref List<string> messages, ref bool recordWasFound)
		{
			bool usedCEKeys = false;
		
			return true;//??
		}

		/// <summary>
		/// Get last publish event data
		/// Use to vaidate integrity of request
		/// 2020-03-03 NOTE: must handle transfer of ownership
		/// </summary>
		/// <param name="entityType">NOT USED</param>
		/// <param name="ctid"></param>
		/// <param name="recordWasFound">Return True if there was a previous event</param>
		/// <param name="usedCEKeys">Return True if last publish used CE keys</param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static RegistryPublishingHistory GetMostRecentHistory( string ctid, ref bool recordWasFound, ref bool usedCEKeys, ref string message, string community="" )
		{
			var lastPublishEvent = new RegistryPublishingHistory();
			
			return lastPublishEvent;
		}
		#endregion

		public static string FormatRegistryUrl( int urlType, string suffix, string community = "")
		{
			var url = "";
			if ( string.IsNullOrWhiteSpace( suffix ) )
				return url;

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

	}
	[Serializable]
	public class GetOrgResult
	{
		public AccountOrganization data { get; set; }
	}
	[Serializable]
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

	//[Serializable]
	public class OrganizationAddition : AccountOrganization
	{
		public string PublisherApikey { get; set; }
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
		public string PrimaryPhoneNumber { get; set; }
		public string PrimaryEmail { get; set; }
		//public Address MainAddress { get; set; } = new Address();
		//
		public string StreetAddress { get; set; }
		public string City { get; set; }
		public string StateProvince { get; set; }
		public string Country { get; set; }
		public string PostalCode { get; set; }

		#endregion
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
	public class ValidateRequestCache
	{
		public ValidateRequestCache()
		{
			lastUpdated = DateTime.Now;
		}
		public DateTime lastUpdated { get; set; }
		public PublishRequestValidationResponse Response { get; set; } = new PublishRequestValidationResponse();

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

	public class AddOrganizationResponse
	{
		public AddOrganizationResponse()
		{
			Messages = new List<string>();
		}
		public bool Successful { get; set; } = true;
		public List<string> Messages { get; set; }

	}

	public class PublishRequestValidationResponse
	{
		public PublishRequestValidationResponse()
		{
			Messages = new List<string>();
		}

		/// True if action was successfull, otherwise false
		public bool Successful { get; set; }
		public string RegistryAuthorizationToken { get; set; }
		public string PublishingOrganization { get; set; }
		public string PublishingOrganizationRegistryIdentifier { get; set; }
		public string PublishingOrganizationCTID { get; set; }
		public bool OwningOrganizationExists { get; set; }
		public string OwningOrganization { get; set; }
		public string OwningOrganizationRegistryIdentifier { get; set; }
		public bool IsSuperPublisher { get; set; }
		public bool PublisherIsTrustedPartner { get; set; }
		/// <summary>
		/// List of error or warning messages
		/// </summary>
		public List<string> Messages { get; set; }

	}


	public class PublisherApiResponse
	{
		public PublisherApiResponse()
		{
			Messages = new List<string>();
		}
		public bool Successful { get; set; }

		public List<string> Messages { get; set; }

	}
	public class PublisherLastHistoryResponse
	{
		public PublisherLastHistoryResponse()
		{
		}
		public string data { get; set; }
		public bool valid { get; set; }

		public string status { get; set; }
		public object extra { get; set; }
	}
}

