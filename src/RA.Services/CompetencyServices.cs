using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RJ = RA.Models.JsonV2;
using RA.Models.Input;
using CASSEntityRequest = RA.Models.Input.CompetencyFrameworkGraphRequest;
using EntityRequest = RA.Models.Input.CompetencyFrameworkRequest;
using InputEntity = RA.Models.Input.CompetencyFramework;
using OutputEntity = RA.Models.JsonV2.CompetencyFramework;
using OutputGraph = RA.Models.JsonV2.CompetencyFrameworksGraph;
using OutputCompetency = RA.Models.JsonV2.Competency;

using ServiceHelper = RA.Services.ServiceHelperV2;
using GraphContainer = RA.Models.JsonV2.GraphContainer;
using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class CompetencyServices : ServiceHelperV2
	{
		static string status = "";
		static List<string> warnings = new List<string>();
		List<OutputCompetency> outputCompetencies = new List<RJ.Competency>();

		#region graph publish
		/// <summary>
		/// A request from CASS will come already formatted
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="helper"></param>
		public void PublishGraph( CASSEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
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

			CompetencyFrameworkGraph input = request.CompetencyFrameworkGraph;// 
			if ( ToMapFromGraph( input, request.CTID, output, ref messages ) )
			{

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
		public bool ToMapFromGraph( CompetencyFrameworkGraph input, string requestCTID, OutputGraph output, ref List<string> messages )
		{
			CurrentEntityType = "CASSCompetencyFramework";
			bool isValid = true;

			//TODO - if from CASS, just pass thru, with minimum validation
			output.Graph = input.Graph;
			int competenciesCount = 0;
			try
			{
				RJ.CompetencyFramework framework = GetFramework( input.Graph, ref competenciesCount );
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
			}
			catch ( Exception ex )
			{
				LogError( ex, "CompetencyServices.ToMapFromCASS" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
		private RJ.CompetencyFramework GetFramework( object graph, ref int competenciesCount )
		{
			//string ctid = "";
			competenciesCount = 0;
			if ( graph == null )
			{
				return null;
			}
			RJ.CompetencyFramework entity = new RJ.CompetencyFramework();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "ceasn:CompetencyFramework" ) > -1 )
					{
						entity = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.CompetencyFramework>();

						//RJ.CompetencyFrameworkInput cf = ( RJ.CompetencyFrameworkInput ) JsonConvert.DeserializeObject( token.ToString() );
						if ( competenciesCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the competency framework is now first in the export document
							competenciesCount = jarray.Count - 1;
						}
						return entity;
					}
					else if ( token.ToString().IndexOf( "ceasn:Competency" ) > -1 )
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
			GraphContainer og = new GraphContainer();

			/*
             * current approach:
             * - multiple publish
             * - framework
             * - one envelope for each competency
             * 
             */
			if ( ToMap( request, output, ref messages ) )
			{
				if ( warnings.Count > 0 )
					messages.AddRange( warnings );

				og.Graph.Add( output );
				//add competencies
				if ( outputCompetencies != null && outputCompetencies.Count > 0 )
				{
					foreach ( var item in outputCompetencies )
					{
						og.Graph.Add( item );
					}
				}

				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

				CER cer = new CER( "CompetencyFramework", output.Type, output.CTID, helper.SerializedInput );
				cer.PublisherAuthorizationToken = helper.ApiKey;
				cer.PublishingForOrgCtid = helper.OwnerCtid;

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;

				string identifier = "CompetencyFramework_" + request.CompetencyFramework.Ctid;

				if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
					//for now need to ensure envelopid is returned
					helper.RegistryEnvelopeId = crEnvelopeId;

					string msg = string.Format( "<p>Published CompetencyFramework: {0}</p><p>sourcewebpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.name, output.source, output.CTID, crEnvelopeId );
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

		//
		public string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
		{
			var output = new OutputEntity();
			string payload = "";
			isValid = true;
			IsAPublishRequest = false;

			if ( ToMap( request, output, ref messages ) )
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
		public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "CompetencyFramework";
			bool isValid = true;
			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.CompetencyFramework;
			bool hasDefaultLanguage = false;
			if ( !string.IsNullOrWhiteSpace( request.DefaultLanguage ) )
			{
				//validate
				if ( ValidateLanguageCode( request.DefaultLanguage, "request.DefaultLanguage", ref messages ) )
				{
					DefaultLanguageForMaps = request.DefaultLanguage;
					hasDefaultLanguage = true;
				}
			}
			output.inLanguage = input.inLanguage;
			output.inLanguage = PopulateInLanguage( input.inLanguage, "Competency Framework", "Competency Framework", hasDefaultLanguage, ref messages );
			try
			{
				//??????????????????
				//output.CtdlId = AssignValidUrlAsPropertyIdList( input.creator, "Framework creator", ref messages );
				if ( IsCtidValid( input.Ctid, ref messages ) )
				{
					//input.Ctid = input.Ctid.ToLower();
					output.CTID = input.Ctid;
					output.CtdlId = idBaseUrl + output.CTID;
					CurrentCtid = input.Ctid;
				}

				output.name = AssignLanguageMap( input.name, input.name_map, "Competency Framework", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				output.description = AssignLanguageMap( input.description, input.description_map, "Competency Framework Description", DefaultLanguageForMaps, CurrentCtid, false, ref messages );

				output.alignFrom = AssignValidUrlListAsStringList( input.alignFrom, "Framework alignFrom", ref messages );
				output.alignTo = AssignValidUrlListAsStringList( input.alignTo, "Framework alignTo", ref messages );

				output.author = input.author;
				//language map  TBD
				output.conceptKeyword = AssignLanguageMapList( input.conceptKeyword, input.conceptKeyword_map, "Framework conceptKeyword", ref messages );

				output.conceptTerm = AssignValidUrlListAsStringList( input.conceptTerm, "Framework conceptTerm", ref messages );

				output.creator = AssignValidUrlListAsStringList( input.creator, "Framework creator", ref messages );

				output.dateCopyrighted = MapDate(input.dateCopyrighted, "dateCopyrighted", ref messages);
				output.dateCreated = MapDate( input.dateCreated, "dateCreated", ref messages );
				output.dateModified = MapDate( input.dateModified, "dateModified", ref messages );
				output.dateValidFrom = MapDate( input.dateValidFrom, "dateValidFrom", ref messages );
				output.dateValidUntil = MapDate( input.dateValidUntil, "dateValidUntil", ref messages );

				output.derivedFrom = AssignValidUrlAsString( input.derivedFrom, "Framework derivedFrom", ref messages, false );


				output.educationLevelType = AssignValidUrlListAsStringList( input.educationLevelType, "Framework educationLevelType", ref messages );
				output.hasTopChild = AssignValidUrlListAsStringList( input.hasTopChild, "Framework hasTopChild", ref messages );
				if ( output.hasTopChild == null || output.hasTopChild.Count == 0 )
				{
					messages.Add( "Error: at least one competency must be referenced in the hasTopChild property of a competency framework." );
				}
				output.identifier = AssignValidUrlListAsStringList( input.identifier, "Framework identifier", ref messages );
				
				output.license = AssignValidUrlAsString( input.license, "Framework license", ref messages, false );
				//output.localSubject = FormatLanguageMapList( input.localSubject, "Framework localSubject", ref messages );

				output.publicationStatusType = AssignValidUrlAsString( input.publicationStatusType, "Framework publicationStatusType", ref messages, false );
				output.publisher = AssignValidUrlListAsStringList( input.publisher, "Framework publisher", ref messages );
				output.publisherName = AssignLanguageMapList( input.publisherName_map, "Framework publisherName", ref messages );
				output.repositoryDate = input.repositoryDate;
				//
				//output.rights = AssignValidUrlAsString( input.rights, "Framework rights", ref messages );
				output.rights = AssignLanguageMap( input.rights, input.rights_map, "Competency Framework Rights", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				//output.rights = AssignLanguageMap( input.rights_map, "Framework rights", ref messages );

				output.rightsHolder = AssignValidUrlAsString( input.rightsHolder, "Framework rightsHolder", ref messages, false );
				output.source = AssignValidUrlListAsStringList( input.source, "Framework source", ref messages );

				output.tableOfContents = AssignLanguageMap( input.tableOfContents_map, "Framework tableOfContents", ref messages );

				if ( request.Competencies == null || request.Competencies.Count == 0 )
				{
					messages.Add( "Error: at least one competency must be included with a competency framework." );
				}
				else
				{
					OutputCompetency competency = new OutputCompetency();
					int compCntr = 0;
					//add each top competency
					foreach ( var item in request.Competencies )
					{
						competency = new OutputCompetency();
						compCntr++;
						if ( ToMapCompetency( item, competency, hasDefaultLanguage, compCntr, ref messages ) )
						{
							outputCompetencies.Add( competency );
						}
					}
				}
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

		public bool ToMapCompetency( RA.Models.Input.Competency input, OutputCompetency output, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			//
			output.Ctid = FormatCtid( input.Ctid, ref messages );
			output.CtdlId = idBaseUrl + output.Ctid;
			//establish language. make a common method
			output.inLanguage = PopulateInLanguage( input.inLanguage, "Competency", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );
			//
			output.competencyText = AssignLanguageMap( input.competencyText, input.competencyText_map, "competencyText", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
			//
			output.comment = AssignLanguageMapList( input.comment, input.comment_map, "comment", DefaultLanguageForMaps, ref messages );


			output.complexityLevel = AssignValidUrlListAsStringList( input.complexityLevel, "complexityLevel", ref messages, false );
			output.competencyText = AssignLanguageMap( input.competencyText, input.competencyText_map, "competencyText", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

			output.alignFrom = AssignValidUrlListAsStringList( input.alignFrom, "alignFrom", ref messages, false );
			output.alignTo = AssignValidUrlListAsStringList( input.alignTo, "alignTo", ref messages, false );

			output.altCodedNotation = AssignListToList( input.altCodedNotation);
			output.author = AssignListToList(input.author);
			output.codedNotation = input.codedNotation;
			output.complexityLevel = AssignListToList( input.complexityLevel);
			output.comprisedOf = AssignValidUrlListAsStringList( input.comprisedOf, "comprisedOf", ref messages, false );
			output.conceptKeyword = AssignLanguageMapList( input.conceptKeyword, input.conceptKeyword_maplist, "conceptKeyword", DefaultLanguageForMaps, ref messages );

			output.conceptTerm = AssignValidUrlListAsStringList( input.conceptTerm, "conceptTerm", ref messages, false );
			output.creator = input.creator;

			output.dateCreated = MapDate( input.dateCreated, "dateCreated", ref messages );
			output.dateModified = MapDate( input.dateModified, "dateModified", ref messages );
			output.derivedFrom = input.derivedFrom;
			output.educationLevelType = input.educationLevelType;

			output.hasChild = AssignValidUrlListAsStringList( input.hasChild, "hasChild", ref messages, false );

			output.identifier = input.identifier;
			output.broadAlignment = AssignValidUrlListAsStringList( input.broadAlignment, "broadAlignment", ref messages, false );

			output.isChildOf = AssignValidUrlListAsStringList( input.isChildOf, "isChildOf", ref messages, false );
			output.isPartOf = input.isPartOf;
			output.isTopChildOf = input.isTopChildOf;

			//
			output.isVersionOf = input.isVersionOf;
			output.listID = input.listID;
			output.localSubject = AssignLanguageMapList( input.localSubject, input.localSubject_maplist, "conceptKeyword", DefaultLanguageForMaps, ref messages );

			output.broadAlignment = AssignValidUrlListAsStringList( input.broadAlignment, "broadAlignment", ref messages, false );
			output.majorAlignment = AssignValidUrlListAsStringList( input.majorAlignment, "majorAlignment", ref messages, false );
			output.minorAlignment = AssignValidUrlListAsStringList( input.minorAlignment, "minorAlignment", ref messages, false );
			output.narrowAlignment = AssignValidUrlListAsStringList( input.narrowAlignment, "narrowAlignment", ref messages, false );
			output.prerequisiteAlignment = AssignValidUrlListAsStringList( input.prerequisiteAlignment, "prerequisiteAlignment", ref messages, false );
			output.skillEmbodied = AssignValidUrlListAsStringList( input.skillEmbodied, "skillEmbodied", ref messages, false );
		
			output.weight = input.weight;


			return isValid;
		}
	}
}
