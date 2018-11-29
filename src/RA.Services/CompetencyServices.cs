using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RJ = RA.Models.JsonV2;
using RA.Models.Input;
using CASSEntityRequest = RA.Models.Input.CASSCompetencyFrameworkRequest;
using EntityRequest = RA.Models.Input.CompetencyFrameworkRequest;
using InputEntity = RA.Models.Input.CompetencyFramework;
using OutputEntity = RA.Models.JsonV2.CompetencyFramework;
using OutputGraph = RA.Models.JsonV2.CompetencyFrameworksGraph;
using OutputCompetency = RA.Models.JsonV2.Competency;
using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
    public class CompetencyServices : ServiceHelper
    {
        static string status = "";
        static List<string> warnings = new List<string>();
        #region from CASS
        /// <summary>
        /// A request from CASS will come already formatted
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="helper"></param>
        public void PublishFromCASS( CASSEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
            isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;

            //submitter is not a person for this api, rather the organization
            //may want to do a lookup via the api key?
            string submitter = "";
            List<string> messages = new List<string>();
            var output = new OutputGraph();
            OutputCompetency oc = new OutputCompetency();
            List<OutputCompetency> compList = new List<RJ.Competency>();

            CompetencyFrameworksGraph input = request.CompetencyFrameworkGraph;// 
            if ( ToMapFromCASS( input, request.CTID, output, ref messages ) )
            {

                //RJ.CompetencyFrameworkInput framework = GetFramework( input.Graph );
                //if ( framework == null || string.IsNullOrWhiteSpace( framework.CTID ) )
                //{
                //    messages.Add( "Error - A ceasn:CompetencyFramework document was not found." );
                //}
                //else
                //{
                //TODO - if from CASS, just pass thru, with minimum validation
                //output.Graph = input.Graph;
                //if ( string.IsNullOrWhiteSpace( request.CTID ) || framework.CTID != request.CTID )
                //{
                //    LoggingHelper.DoTrace( 4, string.Format( "CompetencyServices.PublishFromCASS. DIFFERENCES IN CTIDs. request.CTID: {0}, framework.CTID: {1}", ( request.CTID ?? "" ), framework.CTID ) );
                //    output.CTID = framework.CTID;
                //    request.CTID = framework.CTID;
                //}
                //else
                //    output.CTID = request.CTID;
                //output.CtdlId = idBaseUrl + output.CTID;

                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

                //will need to extract a ctid?
                CER cer = new CER( "CompetencyFramework", output.Type, output.CTID, helper.SerializedInput );
                cer.PublisherAuthorizationToken = helper.ApiKey;
                cer.PublishingForOrgCtid = helper.OwnerCtid;
                cer.SkippingValidation = true;

                if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
                    cer.IsManagedRequest = true;

                string identifier = "CompetencyFramework_" + output.CTID;
                if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
                {
                    //for now need to ensure envelopid is returned
                    helper.RegistryEnvelopeId = crEnvelopeId;

                    string msg = string.Format( "<p>Published Competency Framework</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
                    NotifyOnPublish( "CompetencyFramework", msg );
                }
                else
                {
                    messages.Add( status );
                    isValid = false;
                }
                //}
            }
            else
            {
                helper.HasErrors = true;
                isValid = false;
                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }
            helper.SetMessages( messages );
            return;
        }


        /// <summary>
        /// Input from CASS should already be properly formatted.
        /// Ensure a framework exists
        /// </summary>
        /// <param name="input"></param>
        /// <param name="requestCTID"></param>
        /// <param name="output"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ToMapFromCASS( CompetencyFrameworksGraph input, string requestCTID ,OutputGraph output, ref List<string> messages )
        {
            CurrentEntityType = "CASSCompetencyFramework";
            bool isValid = true;

            //TODO - if from CASS, just pass thru, with minimum validation
            output.Graph = input.Graph;
            int competenciesCount = 0;
            try
            {
                RJ.CompetencyFrameworkInput framework = GetFramework( input.Graph, ref competenciesCount );
                if ( framework == null || string.IsNullOrWhiteSpace( framework.CTID ) )
                {
                    messages.Add( "A ceasn:CompetencyFramework document was not found." );
                }
                else
                {
                    if ( !string.IsNullOrWhiteSpace( requestCTID ) )
                    {
                        output.CTID = requestCTID;
                    }
                    else
                    if ( !string.IsNullOrWhiteSpace( framework.CTID ) )
                    {
                        //LoggingHelper.DoTrace( 4, string.Format( "CompetencyServices.PublishFromCASS. DIFFERENCES IN CTIDs. request.CTID: {0}, framework.CTID: {1}", ( requestCTID ?? "" ), framework.CTID ) );
                        output.CTID = framework.CTID;
                    }
                    else
                    {
                        messages.Add( "A CTID for the competency framework was not found in the request object or competency framework." );
                    }
                    if ( competenciesCount == 0 )
                        messages.Add( "No documents of type ceasn:Competency were found." );
                }
                output.CtdlId = credRegistryGraphUrl + output.CTID;
            } catch (Exception ex)
            {
                LogError( ex, "CompetencyServices.ToMapFromCASS" );
                messages.Add( ex.Message );
            }
            if ( messages.Count > 0 )
                isValid = false;

            return isValid;
        }
        private RJ.CompetencyFrameworkInput GetFramework(object graph, ref int competenciesCount )
        {
            //string ctid = "";
            competenciesCount = 0;
            if (graph == null)
            {
                return null;
            }
			RJ.CompetencyFrameworkInput entity = new RJ.CompetencyFrameworkInput();
            Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray ) graph;
            foreach (var token in jarray)
            {
                if (token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ))
                {
                    if (token.ToString().IndexOf( "ceasn:CompetencyFramework" ) > -1)
                    {
                        entity = ( ( Newtonsoft.Json.Linq.JObject ) token ).ToObject<RJ.CompetencyFrameworkInput>();

                        //RJ.CompetencyFrameworkInput cf = ( RJ.CompetencyFrameworkInput ) JsonConvert.DeserializeObject( token.ToString() );
                        if (competenciesCount == 0 && jarray.Count > 1)
                        {
                            //18-09-25 the competency framework is now first in the export document
                            competenciesCount = jarray.Count - 1;
                        }
                        return entity;
                    } else if (token.ToString().IndexOf( "ceasn:Competency" ) > -1)
                    {
                        competenciesCount++;
                        //ignore
                        //var c1 = token.ToString().Replace( "exactMatch", "exactAlignment" );
                        //var c2 = ( ( Newtonsoft.Json.Linq.JObject ) c1 ).ToObject<RJ.CompetencyInput>();

                    }
                }

                else
                {
                    //error
                }
            }
            //no ctid found, so????
            return entity;
        }

        //private void OldStuff( CASSEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        //{
        //    isValid = true;
        //    string crEnvelopeId = request.RegistryEnvelopeId;

        //    //submitter is not a person for this api, rather the organization
        //    //may want to do a lookup via the api key?
        //    string submitter = "";
        //    List<string> messages = new List<string>();
        //    var output = new OutputGraph();
        //    OutputCompetency oc = new OutputCompetency();
        //    List<OutputCompetency> compList = new List<RJ.Competency>();

        //    #region NON-CASS
        //    var outputEntity = new OutputEntity();
        //    //List<object> deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>( input.Graph );
        //    var o = input.Graph;
        //    Newtonsoft.Json.Linq.JArray o2 = ( Newtonsoft.Json.Linq.JArray ) o;
        //    foreach ( var token in o2 )
        //    {
        //        if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
        //        {

        //            if ( token.ToString().IndexOf( "ceasn:Competency" ) > -1 )
        //            {
        //                var c1 = token.ToString().Replace( "exactMatch", "exactAlignment" );
        //                var c2 = ( ( Newtonsoft.Json.Linq.JObject ) c1 ).ToObject<RJ.CompetencyInput>();

        //            }
        //            else if ( token.ToString().IndexOf( "ceasn:CompetencyFramework" ) > -1 )
        //            {
        //                RJ.CompetencyFrameworkInput cf = ( RJ.CompetencyFrameworkInput ) JsonConvert.DeserializeObject( token.ToString() );
        //                if ( ToMap( request.CompetencyFramework, outputEntity, ref messages ) )
        //                {
        //                }
        //            }
        //        }

        //        else
        //        {
        //            //error
        //        }
        //    }
        //    #endregion
        //}
        #endregion

        /// <summary>
        /// Publish an CompetencyFramework to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
            isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;

            //submitter is not a person for this api, rather the organization
            //may want to do a lookup via the api key?
            string submitter = "";
            List<string> messages = new List<string>();
            var output = new OutputEntity();

            /*
             * current approach:
             * - multiple publish
             * - framework
             * - one envelope for each competency
             * 
             */ 
            if ( ToMap( request.CompetencyFramework, output, ref messages ) )
            {
                if ( warnings.Count > 0 )
                    messages.AddRange( warnings );

                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

                CER cer = new CER( "CompetencyFramework", output.Type, output.Ctid, helper.SerializedInput );
                cer.PublisherAuthorizationToken = helper.ApiKey;
                cer.PublishingForOrgCtid = helper.OwnerCtid;

                if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
                    cer.IsManagedRequest = true;

                string identifier = "CompetencyFramework_" + request.CompetencyFramework.Ctid;

                if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
                {
                    //for now need to ensure envelopid is returned
                    helper.RegistryEnvelopeId = crEnvelopeId;

                    string msg = string.Format( "<p>Published CompetencyFramework: {0}</p><p>sourcewebpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.name, output.source, output.Ctid, crEnvelopeId );
                    //NotifyOnPublish( "CompetencyFramework", msg );
                }
                else
                {
                    messages.Add( status );
                    isValid = false;
                }
            }
            else
            {
                isValid = false;
                if ( !string.IsNullOrWhiteSpace( status ) )
                    messages.Add( status );
                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }

            helper.SetMessages( messages );
        }
        public string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
        {
            return FormatAsJson( request.CompetencyFramework, ref isValid, ref messages );
        }
        //
        public string FormatAsJson( InputEntity input, ref bool isValid, ref List<string> messages )
        {
            var output = new OutputEntity();
            string payload = "";
            isValid = true;
            IsAPublishRequest = false;

            if ( ToMap( input, output, ref messages ) )
            {
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }
            else
            {
                isValid = false;
                //do payload anyway
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }

            return payload;
        }
        public bool ToMap( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            CurrentEntityType = "CompetencyFramework";
            bool isValid = true;
            RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();

            try
            {
                output.alignFrom = AssignValidUrlListAsStringList( input.alignFrom, "Framework alignFrom", ref messages);
                output.alignTo = AssignValidUrlListAsStringList( input.alignTo, "Framework alignTo", ref messages);

                output.author = input.author;
                //language map  TBD
                output.conceptKeyword = FormatLanguageMapList( input.conceptKeyword, "Framework conceptKeyword", ref messages );

                output.conceptTerm = AssignValidUrlListAsStringList( input.conceptTerm, "Framework conceptTerm", ref messages );

                output.creator = AssignValidUrlListAsStringList( input.creator, "Framework creator", ref messages );

                //??????????????????
                //output.CtdlId = AssignValidUrlAsPropertyIdList( input.creator, "Framework creator", ref messages );
                if ( IsCtidValid( input.Ctid, ref messages ) )
                {
                    //input.Ctid = input.Ctid.ToLower();
                    output.Ctid = input.Ctid;
                    output.CtdlId = idBaseUrl + output.Ctid;
                    CurrentCtid = input.Ctid;
                }


                output.dateCopyrighted = input.dateCopyrighted;
                output.dateCreated = input.dateCreated;
                output.dateValidFrom = input.dateValidFrom;
                output.dateValidUntil = input.dateValidUntil;

                output.derivedFrom = AssignValidUrlAsString( input.derivedFrom, "Framework derivedFrom", ref messages );
                output.description = FormatLanguageMap( input.description, "Framework description", ref messages );
                output.name = FormatLanguageMap( input.name, "Framework name", ref messages );

                output.educationLevelType = AssignValidUrlListAsStringList( input.educationLevelType, "Framework educationLevelType", ref messages );
                output.hasTopChild = AssignValidUrlListAsStringList( input.hasTopChild, "Framework hasTopChild", ref messages );

                output.identifier = AssignValidUrlListAsStringList( input.identifier, "Framework identifier", ref messages );
                output.inLanguage = input.inLanguage;
                output.license = AssignValidUrlAsString( input.license, "Framework license", ref messages );
                //output.localSubject = FormatLanguageMapList( input.localSubject, "Framework localSubject", ref messages );

                output.publicationStatusType = AssignValidUrlAsString( input.publicationStatusType, "Framework publicationStatusType", ref messages );
                output.publisher = AssignValidUrlListAsStringList( input.publisher, "Framework publisher", ref messages );
                output.publisherName = FormatLanguageMapList( input.publisherName, "Framework publisherName", ref messages );
                output.repositoryDate = input.repositoryDate;
                //
                output.rights = AssignValidUrlListAsStringList( input.rights, "Framework rights", ref messages );
                output.rightsHolder = AssignValidUrlAsString( input.rightsHolder, "Framework rightsHolder", ref messages );
                output.source = AssignValidUrlListAsStringList( input.source, "Framework source", ref messages );

                output.tableOfContents = FormatLanguageMap( input.tableOfContents, "Framework tableOfContents", ref messages );

                HandleCompetencies( input, output, ref messages );
            }
            catch ( Exception ex )
            {
                LogError( ex, "CompetencyServices.ToMap" );
                messages.Add( ex.Message );
            }

            if ( messages.Count > 0 )
                isValid = false;

            return isValid;
        }

        public void HandleCompetencies( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            if ( input.Competencies == null || input.Competencies.Count == 0 )
                return;

            //add each top competency
            foreach (var item in input.Competencies)
            {
                output.competency.Add( new OutputCompetency()
                {
                    competencyText = ServiceHelper.FormatLanguageMap( item.competencyText, "CompetencyText", ref messages )
                } );
            }
        }
    }
}
