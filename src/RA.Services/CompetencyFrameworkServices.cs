using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using RJ = RA.Models.JsonV2;
using RA.Models.Input;
using GraphRequest = RA.Models.Input.CompetencyFrameworkGraphRequest;
using EntityRequest = RA.Models.Input.CompetencyFrameworkRequest;
using InputEntity = RA.Models.Input.CompetencyFramework;

using OutputEntity = RA.Models.JsonV2.CompetencyFramework;

using OutputGraph = RA.Models.JsonV2.CompetencyFrameworksGraph;
using OutputCompetency = RA.Models.JsonV2.Competency;
using ServiceHelper = RA.Services.ServiceHelperV2;
using GraphContainer = RA.Models.JsonV2.GraphContainer;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class CompetencyFrameworkServices : ServiceHelperV2
	{
		static string status = "";

		List<OutputCompetency> outputCompetencies = new List<RJ.Competency>();
		public static bool errorIfDescriptionEqualsName = UtilityManager.GetAppKeyValue( "errorIfDescriptionEqualsName", false );
		#region graph publish
		/// <summary>
		/// A request from CASS will come already formatted
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="helper"></param>
		public void PublishGraph( GraphRequest request, ref bool isValid, RA.Models.RequestHelper helper, ref string outputCTID )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;
			LoggingHelper.DoTrace( 5, "CompetencyFrameworkServices.PublishGraph Entered." );
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
				int competenciesCount = outputCompetencies.Count;
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				//og.CTID = output.CTID;
				outputCTID = output.CTID;
				//og.Type = "ceasn:CompetencyFramework"; //ignored anyway
				og.Context = ceasnContext;
				//format payload
				helper.Payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );
				//helper.Payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );

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

				// if first time, and large framework, force use of CE keys
				bool recordWasFound = false;
				if ( !SupportServices.ValidateAgainstPastRequest( "CompetencyFramework_", output.CTID, ref cer, ref messages, ref recordWasFound ) )
				{
					isValid = false;
					//return; //===================
				} else
				{
					//
					if ( helper.Payload.Length > 1000000 )
					{
						LoggingHelper.DoTrace( 1, string.Format( "CompetencyServices.PublishGraph. *******Note. Larger payload: {0} bytes , competencies: {1}.", helper.Payload.Length, competenciesCount ) );
						//try force use of CE keys
						//can only do this if first time, otherwise will have a key mismatch
						//just non-prod for now
						if ( cer.IsManagedRequest && !recordWasFound )
						{
							LoggingHelper.DoTrace( 1, "CompetencyServices.PublishGraph. Forcing use of SelfPublish for larger framework" );
							//OK if will be done direct now
							//cer.IsManagedRequest = false;
						} else
						{
							//not managed, or was managed and record not found-need to clarify 
							LoggingHelper.DoTrace( 1, "CompetencyServices.PublishGraph. WARNING. Encountered large framework that was previously published, so CANNOT force use of SelfPublish. See what happens!" );
						}
					}

					string identifier = "CompetencyFramework_" + output.CTID;
					//do publish
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Competency Framework</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
						//NotifyOnPublish( "CompetencyFramework", msg );
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
				//helper.Payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );
				helper.Payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );
			}
			helper.SetMessages( messages );

			LoggingHelper.DoTrace( 6, "CompetencyFrameworkServices.PublishGraph Exited." );
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
		public bool ToMapFromGraph(GraphRequest request, ref OutputEntity output, ref List<string> messages )
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
						CurrentEntityName = GetFirstItemValue( output.name );
					}
					if ( !HasData( output.description ) )
						messages.Add( "A description must be provided for the competency framework." );
					else
					{
						if ( CurrentEntityName == output.description.ToString() )
						{
							if ( errorIfDescriptionEqualsName )
								messages.Add( "A framework description is required and must be a description, not just be a repeat of the competency framework name." );
						}
					}

					if ( output.inLanguage == null || output.inLanguage.Count() == 0 )
						messages.Add( "At least one entry must be provided for the inLanguage of a competency framework." );

					if ( output.creator == null || output.creator.Count() == 0 )
					{
						if ( isFrameworkCreatorRequired )
							messages.Add( "At least one entry must be provided for the creator of a competency framework." );
					}
					output.creator = SetUrlsToLowerCase( output.creator );
					if ( output.publisher == null || output.publisher.Count() == 0 )
					{
						if ( usingFrameworkCreatorIfPublisherMissing && ( output.creator != null && output.creator.Count() > 0 ) )
						{
							output.publisher = output.creator;
						}
					}
					
					if ( output.publisher == null || output.publisher.Count() == 0 )
					{
						if ( isFrameworkPublisherRequired )
							messages.Add( "At least one entry must be provided for the publisher of a competency framework." );
					} else
					{
						output.publisher = SetUrlsToLowerCase( output.publisher );
					}
					//
					if ( string.IsNullOrWhiteSpace( output.dateCreated ) )
					{
						if ( isFrameworkDateCreatedRequired )
							messages.Add( "A dateCreated must be provided for the competency framework." );
					}
					else if ( !IsDate( output.dateCreated ) )
					{
						messages.Add( string.Format( "DateCreated: '{0}' is invalid.", output.dateCreated ) );
					}
					else
					{
						//ensure dateCreated is just a date, no time
						if ( !string.IsNullOrWhiteSpace( output.dateCreated ) && output.dateCreated.Length > 10 )
							output.dateCreated = MapDate( output.dateCreated, "", ref messages, true );
					}
					if ( string.IsNullOrWhiteSpace( output.dateModified ) )
					{
						
					}
					//if (DateTime.Now.Month == 1 && DateTime.Now.Day == 16
					//	&& request.RegistryEnvelopeId == "1923-01-17"
					//	&& !string.IsNullOrWhiteSpace(output.dateModified) 
					//	&& output.dateModified.Length == 24)
					//{
					//	//some other check
					//	//2019-10-09T15:19:49.328Z
					//	output.dateModified = output.dateModified.Substring(0, 20) + "342Z";
					//}

					output.alignFrom = SetUrlsToLowerCase( output.alignFrom );
					output.alignTo = SetUrlsToLowerCase( output.alignTo );

					//temp = 2020-06 - still needed for older frameworks ====================
					output.publicationStatusType = ( output.publicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );

					//TBD - competencies should be required, but are not yet so in the policy
					if ( output.hasTopChild == null || output.hasTopChild.Count() == 0 )
					{
						if ( competenciesCount == 0 )
						{
							//this error/condition will be addresse below
							//messages.Add( "Error: hasTopChild has to have at least one entry." );
						}
						else
							messages.Add( "Error: Competencies exist for this framework and hasTopChild is empty. The property hasTopChild must have at least one entry." );
					}

					// =========================
					if ( competenciesCount == 0 )
						messages.Add( "Error: No Competencies were found. A Competency Framework must have at least one Competency (and realistically multiple)." );

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

		public OutputEntity GetFramework(object graph, ref int competenciesCount, ref List<string> messages)
		{
			//string ctid = "";
			competenciesCount = 0;
			if ( graph == null )
			{
				return null;
			}
			//the big cheat
			//OutputEntity2 entity2 = GetFramework2( graph, ref competenciesCount, ref messages, true );

			var entity = new OutputEntity();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;

			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "ceasn:CompetencyFramework" ) > -1 )
					{
						//entity = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<OutputEntity>( GetJsonSerializerSettings() );
						//works
						entity = Newtonsoft.Json.JsonConvert.DeserializeObject<OutputEntity>( token.ToString() );

						//RJ.CompetencyFrameworkInput cf = ( RJ.CompetencyFrameworkInput ) JsonConvert.DeserializeObject( token.ToString() );
						if ( competenciesCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the competency framework is now first in the export document
							//competenciesCount = jarray.Count - 1;
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
						if (competenciesCount > 2000)
						{
							//return entity;
						}
						//var competency2 = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.Competency>();
						var competency = Newtonsoft.Json.JsonConvert.DeserializeObject<RJ.Competency>( token.ToString() );
						if ( !HasData( competency.competencyText ) )
							messages.Add( string.Format( "The property competencyText must be provided for the compentency (#{0}), CTID: {1}.", competenciesCount, competency.Ctid ) );
						//=======================================
						//handle property that should be a list
						if ( competency.exactAlignment != null ) 
						{
							var list = new List<string>();
							if ( competency.exactAlignment.GetType() == typeof( List<string> ) )
							{
								//OK
							}
							else if ( competency.exactAlignment.GetType() == typeof( string ) )
							{
								list.Add( competency.exactAlignment.ToString() );
								competency.exactAlignment = list;
							}
						}

						//===============================
						if ( string.IsNullOrWhiteSpace( competency.isTopChildOf ) )
						{
							//if no top scheme, then must have
							if ( competency.isChildOf == null || competency.isChildOf.Count() == 0 )
								messages.Add( string.Format( "Either the isTopChildOf or the isChildOf property must be provided for the compentency (#{0}).", competenciesCount ) );
						}
						//ensure dateCreated is just a date, no time
						if ( !string.IsNullOrWhiteSpace( competency.dateCreated ) && competency.dateCreated.Length > 10 )
							competency.dateCreated = MapDate( competency.dateCreated, "", ref messages, true );

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
				//og.CTID = output.CTID;
				//og.Type = output.Type; // "ceasn:CompetencyFramework";
				og.Context = ceasnContext;
				//
				//helper.Payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );
				helper.Payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );

				CER cer = new CER( "CompetencyFramework", output.Type, output.CTID, helper.SerializedInput )
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
				//helper.Payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );
				helper.Payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );
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
			//og.CTID = output.CTID;
			//og.Type = output.Type; // "ceasn:CompetencyFramework";
			og.Context = ceasnContext;
			//
			//payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );
			payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );
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
				CurrentEntityName = !string.IsNullOrWhiteSpace( input.name ) ? input.name : GetFirstItemValue( output.name );

				output.description = AssignLanguageMap( input.description, input.description_map, "Competency Framework Description", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

				output.alignFrom = AssignRegistryResourceURIsListAsStringList( input.alignFrom, "Framework alignFrom", ref messages );
				output.alignTo = AssignRegistryResourceURIsListAsStringList( input.alignTo, "Framework alignTo", ref messages );
				output.alignFrom = SetUrlsToLowerCase( output.alignFrom );
				output.alignTo = SetUrlsToLowerCase( output.alignTo );

				output.author = input.author;
				//language map  TBD
				output.conceptKeyword = AssignLanguageMapList( input.conceptKeyword, input.conceptKeyword_map, "Framework conceptKeyword", ref messages );

				output.conceptTerm = AssignValidUrlListAsStringList( input.conceptTerm, "Framework conceptTerm", ref messages );

				output.creator = AssignRegistryResourceURIsListAsStringList( input.creator, "Framework creator", ref messages );
				output.creator = SetUrlsToLowerCase( output.creator );
				// =========================
				output.publisher = AssignRegistryResourceURIsListAsStringList( input.publisher, "Framework publisher", ref messages );
				output.publisher = SetUrlsToLowerCase( output.publisher );
				output.publisherName = AssignLanguageMapList( input.publisherName_map, "Framework publisherName", ref messages );

				if ( output.creator == null || output.creator.Count() == 0 )
				{
					//if ( isFrameworkCreatorRequired )
					//	messages.Add( "At least one entry must be provided for the creator of a competency framework." );
				}
				if ( output.publisher == null || output.publisher.Count() == 0 )
				{
					if ( usingFrameworkCreatorIfPublisherMissing && ( output.creator != null && output.creator.Count() > 0 ) )
					{
						output.publisher = output.creator;
					}
				}
				if ( output.publisher == null || output.publisher.Count() == 0 )
				{
					if ( isFrameworkPublisherRequired )
						messages.Add( "At least one entry must be provided for the publisher of a competency framework." );
				}

				output.dateCopyrighted = MapDate(input.dateCopyrighted, "dateCopyrighted", ref messages);
				//only date, no time
				output.dateCreated = MapDate( input.dateCreated, "dateCreated", ref messages );

				if ( string.IsNullOrWhiteSpace( output.dateCreated ) )
				{
					//always require this for direct calls
					if ( isFrameworkDateCreatedRequired )
						messages.Add( "A dateCreated must be provided for the competency framework." );
				}

				output.dateModified = MapDateTime( input.dateModified, "dateModified", ref messages );
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
				//temp = 2020-06 - still needed for older frameworks ====================
				output.publicationStatusType = ( output.publicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );

				output.repositoryDate = MapDate( input.repositoryDate, "repositoryDate", ref messages );
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
			output.competencyLabel = AssignLanguageMap( input.competencyLabel, input.competencyLabel_map, "competencyLabel", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
			//
			output.comment = AssignLanguageMapList( input.comment, input.comment_map, "comment", DefaultLanguageForMaps, ref messages );

			output.complexityLevel = AssignValidUrlListAsStringList( input.complexityLevel, "complexityLevel", ref messages, false );

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
			//hasMaintenanceTask
			output.hasMaintenanceTask = AssignRegistryResourceURIsListAsStringList( input.hasMaintenanceTask, "hasMaintenanceTask", ref messages, false );
			//hasTrainingTask
			output.hasTrainingTask = AssignRegistryResourceURIsListAsStringList( input.hasTrainingTask, "hasTrainingTask", ref messages, false );

			return isValid;
		}
	}
}
