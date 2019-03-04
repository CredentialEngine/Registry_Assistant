using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
//using Factories;
using RA.Models;
using Utilities;

namespace RA.Services
{
	/// <summary>
	/// PENDING - NOT IMPLEMENTED FOR THIS VERSION
	/// </summary>
    public class HistoryServices
    {
        

        public void Add( string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage )
        {
            //
            string environment = UtilityManager.GetAppKeyValue( "environment", "unknown" );
            try
            {
               // RegistryPublishManager mgr = new RegistryPublishManager();
                //mgr.Add( environment, dataOwnerCTID, payloadJSON, publishMethodURI, publishingEntityType, ctdlType, entityCtid, payloadInput, crEnvelopeId, ref statusMessage );
            } catch (Exception ex)
            {
                //eat any errors
                LoggingHelper.DoTrace( 2, "HistoryServices.Add() " + ex.Message );
            }

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
			message = "";
			recordWasFound = false;
			//note may need to use sandbox if value is development
			string environment = UtilityManager.GetAppKeyValue( "environment" );
			//returns object serialized as a string
			//var result = RegistryPublishManager.GetMostRecentHistory( ctid, environment );
			//if ( result == "")
			//{
			//	return lastPublishEvent;
			//}
			//try
			//{
			//	lastPublishEvent = JsonConvert.DeserializeObject<RegistryPublishingHistory>( result );
			//	if ( lastPublishEvent == null || string.IsNullOrWhiteSpace( lastPublishEvent.EntityCtid))
			//	{
			//		//actually may be OK if first publish effort
			//		//messages.Add( string.Format( "A registry {0}", ctid ));
			//		return lastPublishEvent;
			//	}
			//	//must match type
			//	recordWasFound = true;
			//	if ( lastPublishEvent.PublishMethodURI == RegistryServices.CE_PUBLISH_METHOD )
			//		usedCEKeys = true;

			//	//may want to get the ownedBy CTID and compare to input

			//} catch (Exception ex)
			//{
			//	LoggingHelper.LogError( ex, "HistoryServices.GetMostRecentHistory" );
			//	//no message, just 
			//	message = string.Format("Error encounterd checking history for Type: {0}, CTID: {1}", ctid, ex.Message );
			//}

			return lastPublishEvent;
		}
		
	}
}
