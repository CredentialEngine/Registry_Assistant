using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Script.Serialization;


using RA.Models;
using Utilities;
using CER = RA.Services.RegistryServices;

namespace RA.Services
{
	public class SupportServices
	{
		public static string thisClassName = "SupportServices";
		public static bool forcingUseOfCEKeys = UtilityManager.GetAppKeyValue( "forcingUseOfCEKeys", false );
		public SupportServices( string entityName)
		{
			EntityName = entityName ?? "";
		}
		public string EntityName { get; set; }

		#region History
		//****TODO ****

		public void AddHistory( string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage, string publisherIdentifier )
		{
			//
			//string environment = UtilityManager.GetAppKeyValue( "environment", "unknown" );
			//try
			//{
			//	RegistryPublishManager mgr = new RegistryPublishManager();
			//	if ( mgr.Add( environment, dataOwnerCTID, payloadJSON, publishMethodURI, publishingEntityType, ctdlType, entityCtid, EntityName, payloadInput, crEnvelopeId, publisherIdentifier, ref statusMessage ) < 1)
			//	{
			//		//log and notify
			//	}
			//}
			//catch ( Exception ex )
			//{
			//	//eat any errors
			//	LoggingHelper.DoTrace( 2, "SupportServices.AddHistory() " + ex.Message );
			//}
		} //

		public static bool GetPublishingOrgByApiKey( string apiKey, ref string ctid, ref List<string> messages )
		{
			bool isValid = true;
			
			return isValid;
		}

		public static bool ValidateAgainstPastRequest( string entityType, string ctid, ref CER cer, ref List<string> messages )
		{
			bool recordWasFound = false;
			bool usedCEKeys = false;
			string message = "";
			var lastPublishEvent = GetMostRecentHistory( entityType, ctid, ref recordWasFound, ref usedCEKeys, ref message );

			if ( recordWasFound ) //found previous
			{
				//***** issue, if always send the org api key from the publisher. 
				LoggingHelper.DoTrace( 5, string.Format( "ValidateAgainstPastRequest. {0} publish. Found a previous publish for CTID: {1}, Used CEKeys: {2}, PublishMethodURI: {3} ", entityType, ctid, usedCEKeys, lastPublishEvent.PublishMethodURI ) );

				if ( lastPublishEvent.DataOwnerCTID.ToLower() != cer.PublishingForOrgCtid.ToLower() )
				{
					//don't allow but may be moot if validating the apiKey owner ctid combination
					//we should move the latter here then
					messages.Add( string.Format( "Suspcious request. The provided data owner CTID is different from the data owner CTID used for previous requests. This condition is not allowed. Entity type: ({0}), CTID '{1}', DataOwnerCTID: '{2}', PublishingForOrgCtid: '{3}'.", entityType, ctid, lastPublishEvent.DataOwnerCTID, cer.PublishingForOrgCtid) );

					LoggingHelper.DoTrace( 5, messages[ messages.Count() - 1 ] );
					//isValid = false;
					return false;

				}
				else if ( usedCEKeys )
				//want to always the original publishing keys - which will always be proper in the last
				{
					if ( cer.IsManagedRequest )
					{
						LoggingHelper.DoTrace( 5, entityType + " publish. Received a managed request but OVERRIDING to CE Self-Publish." );
						cer.IsManagedRequest = false;   //should record override
						cer.OverrodeOriginalRequest = true;
					}
				}
				else //if ( !usedCEKeys )
				{
					cer.IsManagedRequest = true;   //should record override
												   
					//19-03-25 - this is very likely for anything new after this date.
					//			- and will likely be managed
					//if ( !cer.IsManagedRequest )
					//{
					//}
					if ( lastPublishEvent.PublishMethodURI == RegistryServices.CE_MANAGED_PUBLISH_METHOD )
					{
						cer.OverrodeOriginalRequest = true;
					}

					LoggingHelper.DoTrace( 5, entityType + " publish. Received a CE Publish request but OVERRIDING to Managed request and cer.OverrodeOriginalRequest = " + cer.OverrodeOriginalRequest );
					//this should not happen. Means used publisher 
					//- actually now enabling:
					//	- done via publisher, using 
					//import to log this in publisher activity log
	
				}
			}
			else
			{
				//eventually will always do managed
				if (  !cer.IsManagedRequest ) //Duh if PublisherAuthorizationToken present, will be managed!!
				{
					//but only if an api key was provide
					if ( cer.HasValidPublisherToken() )
					{
						//|| ServiceHelperV2.environment != "staging"
						if ( !forcingUseOfCEKeys  )
						{
							LoggingHelper.DoTrace( 5, entityType + " publish. Received a CE Publish request but OVERRIDING to Managed request - first publish event for this entity." );
							cer.OverrodeOriginalRequest = true;
							cer.IsManagedRequest = true;
						}
					}
				} else 
				{
					//was there a case for doing something here?
					/*need to set the publishing method, where
					 * - inititated from publisher
					 * - first time
					 * - apikey is now provided by publisher, so need to handle
					 */
					if ( cer.IsPublisherRequest )
					{
						LoggingHelper.DoTrace( 5, entityType + " publish. Received a Managed request that originated from CE Publish. This is the first publish event for this entity. OVERRIDING to force publish type of publishMethod:ManualEntry." );
						cer.OverrodeOriginalRequest = true;
					}
					//TEMP
					if ( forcingUseOfCEKeys && ServiceHelperV2.environment == "staging" )
					{
						//force to Manual
						cer.OverrodeOriginalRequest = true;
						cer.IsManagedRequest = false;

						LoggingHelper.DoTrace( 5, entityType + " publish. HOLD ON - FOR NOW, ALL STAGING PUBLISHING WILL USE SELF VERSION." );
					}
				}
					
			}

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
		public static RegistryPublishingHistory GetMostRecentHistory( string entityType, string ctid, ref bool recordWasFound, ref bool usedCEKeys, ref string message ) 
		{
			var lastPublishEvent = new RegistryPublishingHistory();
			

			return lastPublishEvent;
		}
		#endregion


		#region Activity
		public void AddActivity(SiteActivity request)
		{

		}

		#endregion

		#region look up codes
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

			//get schemes from API
			var password = UtilityManager.GetAppKeyValue( "apiPublisherIdentifier", "" );
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
	public class AccountOrganization
	{
		public string CTID { get; set; }
		public string Name { get; set; }
		public bool CanPublish { get; set; }
		public List<string> ApprovedPublishingMethods { get; set; } = new List<string>();
		public List<string> ApprovedPublishingRoles { get; set; } = new List<string>();
		public List<string> ApprovedConsumingMethods { get; set; } = new List<string>();
		public string PublicPrivateKey { get; set; }
		public string PublishingApiKey { get; set; }
	}

	[Serializable]
	public class OrgCache
	{
		public OrgCache()
		{
			lastUpdated = DateTime.Now;
		}
		public DateTime lastUpdated { get; set; }
		public AccountOrganization org { get; set; } = new AccountOrganization();

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
