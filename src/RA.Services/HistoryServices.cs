using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Factories;
using Utilities;

namespace RA.Services
{
    public class HistoryServices
    {
        

        public void Add( string dataOwnerCTID, string payloadJSON, string publishMethodURI, string publishingEntityType, string ctdlType, string entityCtid, string payloadInput, string crEnvelopeId, ref string statusMessage )
        {
            //
            string environment = UtilityManager.GetAppKeyValue( "environment", "unknown" );
            try
            {
                RegistryPublishManager mgr = new RegistryPublishManager();
                mgr.Add( environment, dataOwnerCTID, payloadJSON, publishMethodURI, publishingEntityType, ctdlType, entityCtid, payloadInput, crEnvelopeId, ref statusMessage );
            } catch (Exception ex)
            {
                //eat any errors
                LoggingHelper.DoTrace( 2, "HistoryServices.Add() " + ex.Message );
            }

        }
    }
}
