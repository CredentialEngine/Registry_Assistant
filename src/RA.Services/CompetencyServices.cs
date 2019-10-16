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

		List<OutputCompetency> outputCompetencies = new List<RJ.Competency>();

		#region graph publish
		/// <summary>
		/// A request from CASS will come already formatted
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="helper"></param>
		public void PublishGraph( CASSEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper, ref string outputCTID )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			//consider making configurable?
			
			GraphContainer og = new GraphContainer();
			var output = new OutputEntity();

			if ( ToMapFromGraph( request, ref output, ref messages ) )
			{
				og.Graph.Add( output );
				//TODO - is there other info needed, like in context?
				if ( outputCompetencies != null && outputCompetencies.Count > 0 )
				{
					foreach ( var item in outputCompetencies )
					{
						og.Graph.Add( item );
					}
				}
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				outputCTID = output.CTID;
				og.Type = "ceasn:CompetencyFramework"; //ignored anyway
				og.Context = ceasnContext;

				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

				//will need to extract a ctid?
				CER cer = new CER( "CompetencyFramework", output.Type, output.CTID, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					PublishingForOrgCtid = helper.OwnerCtid,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					SkippingValidation = true
				};

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
				{
					cer.IsManagedRequest = true;
					//get publisher org
					string publisherCTID = "";
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;
					}
					else
					{
						//should be an error message returned

						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "CompetencyServices.PublishGraph. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "CompetencyFramework_", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					//return; //===================
				} else
				{
					string identifier = "CompetencyFramework_" + output.CTID;
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Competency Framework</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
						NotifyOnPublish( "CompetencyFramework", msg );
					}
					else
					{
						messages.Add( status );
						isValid = false;
					}
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
		public bool ToMapFromGraph(CASSEntityRequest request, ref OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "CASSCompetencyFramework";
			bool isValid = true;
			CompetencyFrameworkGraph input = request.CompetencyFrameworkGraph;// 
			Community = request.Community ?? "";

			//TODO - if from CASS, just pass thru, with minimum validation
			//output.Graph = input.Graph;
			int competenciesCount = 0;
			try
			{
				output = GetFramework( input.Graph, ref competenciesCount, ref messages );
				if ( output == null || string.IsNullOrWhiteSpace( output.CTID ) )
				{
					messages.Add( "A ceasn:CompetencyFramework document was not found." );
				}
				else
				{
					//CHECK for required fields
					CurrentCtid = output.CTID = FormatCtid( output.CTID, "CompetencyFramework", ref messages );

					if ( !HasData( output.name ) )
						messages.Add( "A name must be provided for the competency framework." );
					else
					{
						CurrentEntityName = output.name.ToString();
					}
					if ( !HasData( output.description ) )
						messages.Add( "A description must be provided for the competency framework." );
					if ( output.hasTopChild == null || output.hasTopChild.Count() == 0 )
					{
						//messages.Add( "Error: hasTopChild has to have at least one entry." );
					}
					if ( output.inLanguage == null || output.inLanguage.Count() == 0 )
						messages.Add( "At least one entry must be provided for the inLanguage of a competency framework." );

					//if ( output.creator == null || output.creator.Count() == 0 )
					//{
					//	if ( isFrameworkCreatorRequired )
					//		messages.Add( "At least one entry must be provided for the creator of a competency framework." );
					//}
					if ( output.publisher == null || output.publisher.Count() == 0 )
					{
						if ( isFrameworkPublisherRequired )
							messages.Add( "At least one entry must be provided for the publisher of a competency framework." );
					}

					if ( string.IsNullOrWhiteSpace( output.dateCreated ) )
					{
						if ( isFrameworkDateCreatedRequired )
							messages.Add( "A dateCreated must be provided for the competency framework." );
					}
					else if ( !IsDate( output.dateCreated ) )
						messages.Add( "DateCreated is invalid." );
					//temp =====================
					output.publicationStatusType = ( output.publicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );
					// =========================
					if ( competenciesCount == 0 )
						messages.Add( "No documents of type ceasn:Competency were found." );

					output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
				}

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "CompetencyServices.ToMapFromGraph" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
		private OutputEntity GetFramework( object graph, ref int competenciesCount, ref List<string> messages )
		{
			//string ctid = "";
			competenciesCount = 0;
			if ( graph == null )
			{
				return null;
			}
			var entity = new OutputEntity();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "ceasn:CompetencyFramework" ) > -1 )
					{
						entity = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<OutputEntity>();

						//RJ.CompetencyFrameworkInput cf = ( RJ.CompetencyFrameworkInput ) JsonConvert.DeserializeObject( token.ToString() );
						if ( competenciesCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the competency framework is now first in the export document
							competenciesCount = jarray.Count - 1;
						}
						//handle map 
						//if (entity.exactAlignment != null && entity.exactAlignment.Count() > 0)
						//{
						//	if ( entity.source != null && entity.source.Count() > 0 )
						//	{
						//		entity.exactAlignment = null;
						//	} else
						//	{
						//		entity.source = AssignListToList( entity.exactAlignment );
						//		entity.exactAlignment = null;
						//	}
						//}
						//return entity;
					}
					else if ( token.ToString().IndexOf( "ceasn:Competency" ) > -1 )
					{
						competenciesCount++;
						var competency = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.Competency>();
						if ( !HasData( competency.competencyText ) )
							messages.Add( string.Format( "The property competencyText must be provided for the compentency (#{0}), CTID: {1}.", competenciesCount, competency.Ctid ) );

						if ( string.IsNullOrWhiteSpace( competency.isTopChildOf ) )
						{
							//if no top scheme, then must have
							if ( competency.isChildOf == null || competency.isChildOf.Count() == 0 )
								messages.Add( string.Format( "Either the isTopChildOf or the isChildOf property must be provided for the compentency (#{0}).", competenciesCount ) );
						}
						outputCompetencies.Add( competency );
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

			if ( ToMap( request, output, ref messages ) )
			{

				og.Graph.Add( output );
				//add competencies
				if ( outputCompetencies != null && outputCompetencies.Count > 0 )
				{
					foreach ( var item in outputCompetencies )
					{
						og.Graph.Add( item );
					}
				}
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				og.Type = output.Type; // "ceasn:CompetencyFramework";
				og.Context = ceasnContext;
				//
				helper.Payload = JsonConvert.SerializeObject( og, ServiceHelper.GetJsonSettings() );

				CER cer = new CER( "CompetencyFramework", og.Type, output.CTID, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					PublishingForOrgCtid = helper.OwnerCtid
				};

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
				{
					cer.IsManagedRequest = true;
					//get publisher org
					string publisherCTID = "";
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;
					}
					else
					{
						//should be an error message returned

						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "CompetencyServices.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				//
				if ( !SupportServices.ValidateAgainstPastRequest( "CompetencyFramework_", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					//helper.SetMessages( messages );
					//return; //===================
				}
				else
				{

					string identifier = "CompetencyFramework_" + request.CompetencyFramework.Ctid;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published CompetencyFramework: {0}</p><p>CTID: {1}</p> <p>EnvelopeId: {2}</p> ", output.name, output.CTID, crEnvelopeId );
						//NotifyOnPublish( "CompetencyFramework", msg );
					}
					else
					{
						messages.Add( status );
						isValid = false;
					}
				}
			}
			else
			{
				isValid = false;
				if ( !string.IsNullOrWhiteSpace( status ) )
					messages.Add( status );
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}
			helper.SetWarningMessages( warningMessages );
			helper.SetMessages( messages );
		}

		//
		public string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
		{
			GraphContainer og = new GraphContainer();
			var output = new OutputEntity();
			string payload = "";
			isValid = true;
			IsAPublishRequest = false;

			if ( ToMap( request, output, ref messages ) )
			{
				
				//payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}
			else
			{
				isValid = false;
				//do payload anyway
				//payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}
			og.Graph.Add( output );
			//add competencies
			if ( outputCompetencies != null && outputCompetencies.Count > 0 )
			{
				foreach ( var item in outputCompetencies )
				{
					og.Graph.Add( item );
				}
			}
			//
			og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
			og.CTID = output.CTID;
			og.Type = output.Type; // "ceasn:CompetencyFramework";
			og.Context = ceasnContext;
			//
			payload = JsonConvert.SerializeObject( og, ServiceHelper.GetJsonSettings() );
			if ( warningMessages.Count > 0 )
				messages.AddRange( warningMessages );
			return payload;
		}
		public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "CompetencyFramework";
			bool isValid = true;
			Community = request.Community ?? "";

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

			#region  Populate the competency framework

			//output.inLanguage = input.inLanguage;
			output.inLanguage = PopulateInLanguage( input.inLanguage, "Competency Framework", "Competency Framework", hasDefaultLanguage, ref messages );
			try
			{
				//??????????????????
				//output.CtdlId = AssignValidUrlAsPropertyIdList( input.creator, "Framework creator", ref messages );
				if ( IsCtidValid( input.Ctid, "Competency Framework CTID", ref messages ) )
				{
					//input.Ctid = input.Ctid.ToLower();
					output.CTID = input.Ctid;
					output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
					CurrentCtid = input.Ctid;
				}

				output.name = AssignLanguageMap( input.name, input.name_map, "Competency Framework", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				CurrentEntityName = output.name.ToString();
				
				output.description = AssignLanguageMap( input.description, input.description_map, "Competency Framework Description", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

				output.alignFrom = AssignRegistryResourceURIsListAsStringList( input.alignFrom, "Framework alignFrom", ref messages );
				output.alignTo = AssignRegistryResourceURIsListAsStringList( input.alignTo, "Framework alignTo", ref messages );

				output.author = input.author;
				//language map  TBD
				output.conceptKeyword = AssignLanguageMapList( input.conceptKeyword, input.conceptKeyword_map, "Framework conceptKeyword", ref messages );

				output.conceptTerm = AssignValidUrlListAsStringList( input.conceptTerm, "Framework conceptTerm", ref messages );

				output.creator = AssignRegistryResourceURIsListAsStringList( input.creator, "Framework creator", ref messages );
				// =========================
				output.publisher = AssignRegistryResourceURIsListAsStringList( input.publisher, "Framework publisher", ref messages );
				output.publisherName = AssignLanguageMapList( input.publisherName_map, "Framework publisherName", ref messages );

				if ( output.creator == null || output.creator.Count() == 0 )
				{
					//if ( isFrameworkCreatorRequired )
					//	messages.Add( "At least one entry must be provided for the creator of a competency framework." );
				}
				if ( output.publisher == null || output.publisher.Count() == 0 )
				{
					if ( isFrameworkPublisherRequired )
						messages.Add( "At least one entry must be provided for the publisher of a competency framework." );
				}

				output.dateCopyrighted = MapDate(input.dateCopyrighted, "dateCopyrighted", ref messages);
				output.dateCreated = MapDate( input.dateCreated, "dateCreated", ref messages );

				if ( string.IsNullOrWhiteSpace( output.dateCreated ) )
				{
					//always require this for direct calls
					//if ( isFrameworkDateCreatedRequired )
						messages.Add( "A dateCreated must be provided for the competency framework." );
				}

				output.dateModified = MapDate( input.dateModified, "dateModified", ref messages );
				output.dateValidFrom = MapDate( input.dateValidFrom, "dateValidFrom", ref messages );
				output.dateValidUntil = MapDate( input.dateValidUntil, "dateValidUntil", ref messages );

				output.derivedFrom = AssignValidUrlAsString( input.derivedFrom, "Framework derivedFrom", ref messages, false );


				output.educationLevelType = AssignRegistryResourceURIsListAsStringList( input.educationLevelType, "Framework educationLevelType", ref messages );
				output.hasTopChild = AssignRegistryResourceURIsListAsStringList( input.hasTopChild, "Framework hasTopChild", ref messages );
				if ( output.hasTopChild == null || output.hasTopChild.Count == 0 )
				{
					//messages.Add( "At least one competency must be referenced in the hasTopChild property of a competency framework." );
				}
				output.identifier = AssignValidUrlListAsStringList( input.identifier, "Framework identifier", ref messages );
				output.altIdentifier = AssignListToList( input.altIdentifier);

				output.license = AssignValidUrlAsString( input.license, "Framework license", ref messages, false );
				//output.localSubject = FormatLanguageMapList( input.localSubject, "Framework localSubject", ref messages );

				output.publicationStatusType = AssignValidUrlAsString( input.publicationStatusType, "Framework publicationStatusType", ref messages, false );
				//temp =====================
				output.publicationStatusType = ( output.publicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );
				
				output.repositoryDate = input.repositoryDate;
				//
				//output.rights = AssignValidUrlAsString( input.rights, "Framework rights", ref messages );
				output.rights = AssignLanguageMap( input.rights, input.rights_map, "Competency Framework Rights", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
				//output.rights = AssignLanguageMap( input.rights_map, "Framework rights", ref messages );

				output.rightsHolder = AssignValidUrlAsString( input.rightsHolder, "Framework rightsHolder", ref messages, false );
				output.source = AssignValidUrlListAsStringList( input.source, "Framework source", ref messages );
				
				output.tableOfContents = AssignLanguageMap( input.tableOfContents_map, "Framework tableOfContents", ref messages );

				output.OccupationType = FormatCredentialAlignmentListFromFrameworkItemList( input.OccupationType, true, ref messages );

				output.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( input.IndustryType, true, ref messages );



				#endregion


				#region  Populate the competencies
				if ( request.Competencies == null || request.Competencies.Count == 0 )
				{
					messages.Add( "At least one competency must be included with a competency framework." );
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
						if ( ToMapCompetency( item, competency, output, hasDefaultLanguage, compCntr, ref messages ) )
						{
							outputCompetencies.Add( competency );
						}
					}
				}
				#endregion
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "CompetencyServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}

		public bool ToMapCompetency( RA.Models.Input.Competency input, OutputCompetency output, OutputEntity framework, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			//
			CurrentCtid = output.Ctid = FormatCtid( input.Ctid, string.Format("Competency (#{0})", compCntr), ref messages );
			output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.Ctid, Community);
			//establish language. make a common method
			//output.inLanguage = PopulateInLanguage( input.inLanguage, "Competency", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );
			//
			output.competencyText = AssignLanguageMap( input.competencyText, input.competencyText_map, "competencyText", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
			//
			output.comment = AssignLanguageMapList( input.comment, input.comment_map, "comment", DefaultLanguageForMaps, ref messages );

			output.complexityLevel = AssignValidUrlListAsStringList( input.complexityLevel, "complexityLevel", ref messages, false );
			output.competencyText = AssignLanguageMap( input.competencyText, input.competencyText_map, "competencyText", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

			output.alignFrom = AssignRegistryResourceURIsListAsStringList( input.alignFrom, "alignFrom", ref messages, false );
			output.alignTo = AssignRegistryResourceURIsListAsStringList( input.alignTo, "alignTo", ref messages, false );

			output.altCodedNotation = AssignListToList( input.altCodedNotation);
			output.author = AssignListToList(input.author);
			output.codedNotation = input.codedNotation;
			output.complexityLevel = AssignListToList( input.complexityLevel);
			output.comprisedOf = AssignRegistryResourceURIsListAsStringList( input.comprisedOf, "comprisedOf", ref messages, false );
			output.conceptKeyword = AssignLanguageMapList( input.conceptKeyword, input.conceptKeyword_maplist, "conceptKeyword", DefaultLanguageForMaps, ref messages );

			output.conceptTerm = AssignRegistryResourceURIsListAsStringList( input.conceptTerm, "conceptTerm", ref messages, false );
			output.creator = input.creator;

			output.dateCreated = MapDate( input.dateCreated, "dateCreated", ref messages );
			if ( string.IsNullOrWhiteSpace( output.dateCreated ) )
			{
				if ( isFrameworkDateCreatedRequired )
				{
					if ( string.IsNullOrWhiteSpace( framework.dateCreated ) )
						messages.Add( "A dateCreated must be provided for the competency." );
					else
						output.dateCreated = framework.dateCreated;
				}
			}

			output.dateModified = MapDate( input.dateModified, "dateModified", ref messages );
			output.derivedFrom = AssignValidUrlAsString( input.derivedFrom, "Competency derivedFrom", ref messages, false );
			output.educationLevelType = input.educationLevelType;
			output.encompasses = AssignRegistryResourceURIsListAsStringList( input.encompasses, "encompasses", ref messages, false );
			output.hasChild = AssignRegistryResourceURIsListAsStringList( input.hasChild, "hasChild", ref messages, false );

			output.identifier = input.identifier;

			output.isChildOf = AssignRegistryResourceURIsListAsStringList( input.isChildOf, "isChildOf", ref messages, false );
			if ( string.IsNullOrWhiteSpace( input.isPartOf ) )
			{
				messages.Add( "A value must be provided for isPartOf for the competency: '" + output.competencyText.ToString() + "' " );
			}
			else
			{
				output.isPartOf = AssignRegistryResourceURIAsString( input.isPartOf, "Competency isPartOf", ref messages, false);
			}
			output.isTopChildOf = AssignRegistryResourceURIAsString( input.isTopChildOf, "Compentency isTopChildOf", ref messages, false );

			//
			output.isVersionOf = input.isVersionOf;
			output.listID = input.listID;
			output.competencyCategory = AssignLanguageMap( input.competencyCategory, input.competencyCategory_map, "competencyCategory", DefaultLanguageForMaps, ref messages );

			output.broadAlignment = AssignRegistryResourceURIsListAsStringList( input.broadAlignment, "broadAlignment", ref messages, false );
			output.exactAlignment = AssignRegistryResourceURIsListAsStringList( input.exactAlignment, "exactAlignment", ref messages, false );
			output.majorAlignment = AssignRegistryResourceURIsListAsStringList( input.majorAlignment, "majorAlignment", ref messages, false );
			output.minorAlignment = AssignRegistryResourceURIsListAsStringList( input.minorAlignment, "minorAlignment", ref messages, false );
			output.narrowAlignment = AssignRegistryResourceURIsListAsStringList( input.narrowAlignment, "narrowAlignment", ref messages, false );
			output.prerequisiteAlignment = AssignRegistryResourceURIsListAsStringList( input.prerequisiteAlignment, "prerequisiteAlignment", ref messages, false );
			output.skillEmbodied = AssignRegistryResourceURIsListAsStringList( input.skillEmbodied, "skillEmbodied", ref messages, false );
			//
			output.knowledgeEmbodied = AssignRegistryResourceURIsListAsStringList( input.knowledgeEmbodied, "knowledgeEmbodied", ref messages, false );
			//
			output.taskEmbodied = AssignRegistryResourceURIsListAsStringList( input.taskEmbodied, "taskEmbodied", ref messages, false );
			//
			output.weight = input.weight;

			//navy extension
			output.hasSourceIdentifier = AssignRegistryResourceURIsListAsStringList( input.hasSourceIdentifier, "hasSourceIdentifier", ref messages, false );



			return isValid;
		}
	}
}
