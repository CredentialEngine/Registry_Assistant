using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RJ = RA.Models.JsonV2;
using RA.Models.Input;
using CER = RA.Services.RegistryServices;
using SkosEntityRequest = RA.Models.Input.SkosConceptSchemeRequest;
using EntityRequest = RA.Models.Input.ConceptSchemeRequest;
using InputEntity = RA.Models.Input.ConceptScheme;
using SkosInputEntity = RA.Models.Input.SkosConceptScheme;
using OutputEntity = RA.Models.JsonV2.ConceptScheme;
using OutputConcept = RA.Models.JsonV2.Concept;

using OutputGraph = RA.Models.JsonV2.ConceptSchemeGraph;
using OutputGraphEntity = RA.Models.JsonV2.ConceptScheme;
using GraphContainer = RA.Models.JsonV2.GraphContainer;


using Utilities;

namespace RA.Services
{
	public class ConceptSchemeServices : ServiceHelperV2
	{
		static string status = "";

		List<OutputConcept> outputConcepts = new List<OutputConcept>();
		public bool isConceptschemeDateCreatedRequired = GetAppKeyValue( "conceptSchemeDateCreatedIsRequired", true );

		#region publish from graph
		public void PublishGraph( GraphRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			//var output = new OutputGraph();
			GraphContainer og = new GraphContainer();
			var output = new OutputEntity();

			if ( ToMapFromGraph( request, ref output, ref messages ) )
			{

				if ( output.InLanguage.Count() > 0 )
				{

				}
				og.Graph.Add( output );
				//TODO - is there other info needed, like in context?
				if ( outputConcepts != null && outputConcepts.Count > 0 )
				{
					foreach ( var item in outputConcepts )
					{
						og.Graph.Add( item );
					}
				}
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = request.CTID = output.CTID;
				og.Type = "skos:ConceptScheme"; //ignored anyway
				og.Context = ceasnContext;
				//
				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
				//helper.Payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );

				//will need to extract a ctid?
				CER cer = new CER( "ConceptScheme", output.Type, output.CTID, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					PublishingForOrgCtid = helper.OwnerCtid,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					IsPublisherRequest = helper.IsPublisherRequest,
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
						LoggingHelper.DoTrace( 4, string.Format( "ConceptScheme.PublishGraph. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "ConceptScheme", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					//return; //===================
				}
				else
				{
					//
					string identifier = "ConceptScheme_" + output.CTID;
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						//if (!string.IsNullOrWhiteSpace(status))
						//	messages.Add( status );
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Concept Scheme</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
						NotifyOnPublish( "ConceptScheme", msg );
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
				helper.HasErrors = true;
				isValid = false;
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );
			}
			helper.SetMessages( messages );
			return;
		}
		public bool ToMapFromGraph(GraphRequest request, ref OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "ConceptScheme";
			bool isValid = true;
			var input = request.GraphInput;// 
			Community = request.Community ?? "";
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
			//TODO - if from CASS, just pass thru, with minimum validation
			//output.Graph = input.Graph;
			int conceptsCount = 0;
			try
			{
				if ( request.HasLanguageMaps )
				{
					output = GetConceptScheme( input.Graph, ref conceptsCount, ref messages );
				} else
				{
					output = GetConceptSchemeFromPlainGraph( request, hasDefaultLanguage, ref conceptsCount, ref messages );
				}//

				if ( output == null || string.IsNullOrWhiteSpace( output.CTID ) )
				{
					messages.Add( "A skos:ConceptScheme document was not found." );
				}
				else
				{
					//CHECK for required fields
					output.CTID = FormatCtid( output.CTID, "ConceptScheme", ref messages );
					if ( !HasData( output.Name ) )
						messages.Add( "A name must be provided for the concept scheme." );
					else
					{
						CurrentEntityName = GetFirstItemValue( output.Name );
					}
					if ( !HasData( output.Description ) )
						messages.Add( "A description must be provided for the concept scheme." );

					if ( output.HasTopConcept == null || output.HasTopConcept.Count() == 0 )
						messages.Add( "HasTopConcept is a required property." );
					else
					{
						//check hasTopConcepts for purls? why??
						//foreach (var item in output.HasTopConcept)
						//{

						//}
					}
					if ( string.IsNullOrWhiteSpace( output.DateCreated ) )
					{
						if ( isConceptschemeDateCreatedRequired )
							messages.Add( "A dateCreated must be provided for the concept scheme." );
						//else
						//	output.DateCreated = DateTime.Now.ToString("yyyy-MM-dd");
					} else
					{
						//ensure dateCreated is just a date, no time
						if ( !string.IsNullOrWhiteSpace( output.DateCreated ) && output.DateCreated.Length > 10 )
							output.DateCreated = MapDate( output.DateCreated, "", ref messages, true );
					}
					if ( output.InLanguage == null || output.InLanguage.Count() == 0 )
						messages.Add( "A ConceptScheme.InLanguage must be provided for the concept scheme." );

					if ( output.Source == null || output.Source.ToString() == "") 
						messages.Add( "A Source must be provided for the concept scheme." );

					output.Creator = SetUrlsToLowerCase( output.Creator );
					output.Publisher = SetUrlsToLowerCase( output.Publisher );

					if ( string.IsNullOrWhiteSpace( output.PublicationStatusType ) )
						messages.Add( "A PublicationStatusType must be provided for the concept scheme." );
					else
					{
						//temp =====================
						//output.PublicationStatusType = ( output.PublicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );
						// =========================
					}
					if ( conceptsCount == 0 )
						messages.Add( "No documents of type skos:Concept were found for this concept scheme." );
				}
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "ConceptSchemeServices.ToMapFromGraph" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}

		#endregion

		#region direct publish
		public void Publish( EntityRequest request, ref bool isValid, Models.RequestHelper helper )
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
				if ( outputConcepts != null && outputConcepts.Count > 0 )
				{
					foreach ( var item in outputConcepts )
					{
						og.Graph.Add( item );
					}
				}
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				og.Type = "skos:ConceptScheme"; //ignored anyway
				og.Context = ceasnContext;
				//
				helper.Payload = JsonConvert.SerializeObject( og, ServiceHelperV2.GetJsonSettings() );
				CER cer = new CER( "ConceptScheme", output.Type, output.CTID, helper.SerializedInput )
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
						LoggingHelper.DoTrace( 4, string.Format( "ConceptScheme.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "ConceptScheme", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					//helper.SetMessages( messages );
					//return; //===================
				}
				else
				{
					//
					string identifier = "ConceptScheme_" + request.ConceptScheme.Ctid;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						//string msg = string.Format( "<p>Published ConceptScheme: {0}</p><p>sourcewebpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.name, output.source, output.CTID, crEnvelopeId );
						//NotifyOnPublish( "ConceptScheme", msg );
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
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );
			}

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

				//payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );
			}
			else
			{
				isValid = false;
				//do payload anyway
				//payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );
			}
			og.Graph.Add( output );
			//add competencies
			if ( outputConcepts != null && outputConcepts.Count > 0 )
			{
				foreach ( var item in outputConcepts )
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
			payload = JsonConvert.SerializeObject( og, ServiceHelperV2.GetJsonSettings() );
			return payload;
		}
		public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "ConceptScheme";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.ConceptScheme;
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
			//
			output.InLanguage = PopulateInLanguage( input.InLanguage, "concept scheme", "concept scheme", hasDefaultLanguage, ref messages );
			var idUrl = "";
			try
			{
				#region  Populate the concept scheme 
				//id - assume the schema name
				//actually id may just be the schema name for this concept scheme ex: ceterms:Audience
				//19-03-12 make required input
				CurrentCtid = output.CTID = FormatCtid( input.Ctid, "ConceptScheme", ref messages );
				//output.CTID = ExtractCtid( input.Id, "Skos Concept Scheme", ref messages ).ToLower();
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);

				//output.AltIdentifier = AssignListToList( input.AltIdentifier );
				//output.ChangeNote = AssignLanguageMapList( input.ChangeNote, input.ChangeNote_Map, "ChangeNote", DefaultLanguageForMaps, ref messages );
				output.ConceptKeyword = AssignLanguageMapList( input.ConceptKeyword, input.ConceptKeyword_Map, "ConceptKeywork", DefaultLanguageForMaps, ref messages );
				output.ConceptTerm = AssignRegistryResourceURIsListAsStringList( input.ConceptTerm, "ConceptTerm", ref messages );
				//should we used org reference here as well?
				//output.Creator = AssignValidUrlAsStringList( input.Creator, "Concept scheme creator", ref messages, false, false );
				output.Creator = FormatOrganizationReferences( input.Creator, "Concept scheme creator", false, ref messages, false, true );

				//this actually is a year, not datetime!
				output.DateCopyrighted = MapYear( input.DateCopyrighted, "DateCopyrighted", ref messages );
				output.DateCreated = MapDate( input.DateCreated, "DateCreated", ref messages );
				if ( string.IsNullOrWhiteSpace( output.DateCreated ) )
				{
					//always require this for direct calls
					if ( isConceptschemeDateCreatedRequired )
						messages.Add( "A DateCreated must be provided for the concept scheme." );
				}
				output.DateModified = MapDateTime( input.DateModified, "DateModified", ref messages );

				//allow CTID or full URI
				//temp hack for navy
				// output.CTID != "ce-8387d1d5-1992-4c3f-a76d-6b09fd928dd9" && output.CTID != "ce-8387d1d5-2019-4c3f-a76d-6b09fd928dd9"
				if ( !request.GenerateHasTopChild && !request.GenerateHasTopChildFromIsTopChild )
				{
					output.HasTopConcept = AssignRegistryResourceURIsListAsStringList( input.HasTopConcept, "Concept scheme HasTopConcept", ref messages, false, true );
				}
				//history note is not compatible from skos format
				//20-08-05 no longer on credreg.neet
				//output.HistoryNote = AssignLanguageMap( input.HistoryNote, input.HistoryNote_Map, "HistoryNote", DefaultLanguageForMaps, CurrentCtid, false, ref messages );

				output.License = AssignValidUrlAsString( input.License, "License", ref messages, false, false );

				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Concept Scheme Title", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

				CurrentEntityName = !string.IsNullOrWhiteSpace( input.Name ) ? input.Name : GetFirstItemValue( output.Name );

				output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Concept Scheme Description", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

				//this url will likely be a purl to the particular status
				//	http://purl.org/ctdlasn/vocabs/publicationStatus/Draft
				output.PublicationStatusType = AssignValidUrlAsString( input.PublicationStatusType, "PublicationStatusType", ref messages, true, false );
									
					
				//temp =====================
				output.PublicationStatusType = ( output.PublicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );
				// =========================
				
				//output.Publisher = AssignRegistryResourceURIAsString( input.Publisher, "Publisher", ref messages, false, false );
				if ( FormatOrganizationReference( input.Publisher, "ConceptScheme.Publisher", ref idUrl, false, ref messages, false, true ) )
				{
					if ( !string.IsNullOrWhiteSpace( idUrl ) )
					{
						output.Publisher = idUrl;
					}
				}

				output.PublisherName = AssignLanguageMap( input.PublisherName, input.PublisherName_Map, "PublisherName", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
				output.Rights = AssignLanguageMap( input.Rights, input.Rights_Map, "Rights", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
				//from CaSS we have url, but for here, allow flexibility using org reference.
				//output.RightsHolder = AssignValidUrlAsString( input.RightsHolder, "RightsHolder", ref messages, false, false );
				
				if ( FormatOrganizationReference( input.RightsHolder, "ConceptScheme.RightsHolder", ref idUrl, false, ref messages, false, true ) )
				{
					if ( !string.IsNullOrWhiteSpace( idUrl ) )
					{
						output.RightsHolder = idUrl;
					}
				}

				//
				output.Source = AssignValidUrlAsString( input.Source, "ConceptScheme.Source", ref messages, false, true );
				#endregion


				#region  Populate the concepts
				if ( request.Concepts == null || request.Concepts.Count == 0 )
				{
					messages.Add( "At least one Concepts must be included with a concept scheme." );
				}
				else
				{
					OutputConcept concept = new OutputConcept();
					int compCntr = 0;
					//add each top competency
					foreach ( var item in request.Concepts )
					{
						concept = new OutputConcept();
						compCntr++;
						
						if ( ToMapConcept( item, concept, hasDefaultLanguage, compCntr, ref messages ) )
						{
							//for Navy
							//if ( output.CTID  == "ce-8387d1d5-1992-4c3f-a76d-6b09fd928dd9" || output.CTID == "ce-8387d1d5-2019-4c3f-a76d-6b09fd928dd9" )
							if ( request.GenerateHasTopChild )
							{
								if ( output.HasTopConcept == null )
									output.HasTopConcept = new List<string>();

								output.HasTopConcept.Add( concept.CtdlId );
							}
							else if ( request.GenerateHasTopChildFromIsTopChild
							  && !string.IsNullOrWhiteSpace( concept.TopConceptOf ) )
							{
								if ( output.HasTopConcept == null )
									output.HasTopConcept = new List<string>();
								output.HasTopConcept.Add( concept.CtdlId );
							}
							//or set to InScheme
							//only do this all concepts are top
							if ( request.GenerateIsTopChild )
							{
								concept.TopConceptOf = output.CtdlId;
								//will inScheme have to be set as well? Or should this be an arbitrary check?
								if ( concept.InScheme == null )
									concept.InScheme = output.CtdlId;
							}

							outputConcepts.Add( concept );
						}
					}
				}
				#endregion
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "ConceptSchemeServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
		public bool ToMapConcept( RA.Models.Input.Concept input, OutputConcept output, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			output.CTID = FormatCtid( input.Ctid, string.Format( "Concept (#{0})", compCntr), ref messages );

			output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
			output.AltLabel = AssignLanguageMapList( input.AltLabel, input.AltLabel_Map, "AltLabel", DefaultLanguageForMaps, ref messages );
			output.Broader = AssignRegistryResourceURIAsString( input.Broader, string.Format( "Broader - Concept (#{0})", compCntr ), ref messages, false, false );
			output.BroadMatch = AssignRegistryResourceURIsListAsStringList( input.BroadMatch, "BroadMatch", ref messages );

			output.ChangeNote = AssignLanguageMapList( input.ChangeNote, input.ChangeNote_Map, "ChangeNote", DefaultLanguageForMaps, ref messages );
			output.CloseMatch = AssignRegistryResourceURIsListAsStringList( input.CloseMatch, "CloseMatch", ref messages );

			output.PrefLabel = AssignLanguageMap( input.PrefLabel, input.PrefLabel_Map, "PrefLabel", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
			output.Definition = AssignLanguageMap( input.Definition, input.Definition_Map, "Definition", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
			output.ExactMatch = AssignRegistryResourceURIsListAsStringList( input.ExactMatch, "ExactMatch", ref messages );
			output.Narrower = AssignRegistryResourceURIsListAsStringList( input.Narrower, "Narrower", ref messages );
			output.NarrowMatch = AssignRegistryResourceURIsListAsStringList( input.NarrowMatch, "NarrowerMatch", ref messages );
			output.Related= AssignRegistryResourceURIsListAsStringList( input.Related, "Related", ref messages );
			//output.RelatedMatch = AssignRegistryResourceURIsListAsStringList( input.RelatedMatch, "RelatedMatch", ref messages );
			//SupersededBy
			output.SupersededBy = AssignRegistryResourceURIAsString( input.SupersededBy, "SupersededBy", ref messages, false, false );
			output.HiddenLabel = AssignLanguageMapList( input.HiddenLabel, input.HiddenLabel_Map, "HiddenLabel", DefaultLanguageForMaps, ref messages );

			//TODO - handle where the ConceptScheme is supplied and need to convert to URI
			output.InScheme = AssignRegistryResourceURIAsString( input.InScheme, "InScheme", ref messages, false, false );

			output.InLanguage = PopulateInLanguage( input.Language, "Language", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );
			if ( !string.IsNullOrWhiteSpace( input.Notation ) )
				output.Notation = input.Notation;

			output.Note = AssignLanguageMapList( input.Note, input.Note_Map, "Note", DefaultLanguageForMaps, ref messages );

			//TODO - need to add helpers to allow ctids and convert to registry url
			output.TopConceptOf = AssignRegistryResourceURIAsString( input.TopConceptOf, "TopConceptOf", ref messages, false, false );

			//TEMP navy
			if ( CurrentCtid == "ce-8387d1d5-1992-4c3f-a76d-6b09fd928dd9" || CurrentCtid == "ce-8387d1d5-2019-4c3f-a76d-6b09fd928dd9" )
			{
				output.CodeNEC = input.CodeNEC;
				output.LegacyCodeNEC = input.LegacyCodeNEC;
				output.SourceCareerFieldCode = AssignListToList( input.SourceCareerFieldCode );
			}

			return isValid;
		}

		public bool ToMapConcept(RA.Models.JsonV2.ConceptPlain input, OutputConcept output, bool hasDefaultLanguage, int compCntr, ref List<string> messages)
		{
			bool isValid = true;
			try
			{
				output.CTID = FormatCtid( input.CTID, string.Format( "Concept (#{0})", compCntr ), ref messages );

				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );
				//handle inconsistant input of a string or a list
				output.AltLabel = AssignLanguageMapList( AssignObjectToList(input.AltLabel, "AltLabel", ref messages), null, "AltLabel", DefaultLanguageForMaps, ref messages );
				//
				output.Broader = AssignRegistryResourceURIAsString( input.Broader, string.Format( "Broader - Concept (#{0})", compCntr ), ref messages, false, false );
				output.BroadMatch = AssignRegistryResourceURIsListAsStringList( input.BroadMatch, "BroadMatch", ref messages );

				output.ChangeNote = AssignLanguageMapList( input.ChangeNote, null, "ChangeNote", DefaultLanguageForMaps, ref messages );
				output.CloseMatch = AssignRegistryResourceURIsListAsStringList( input.CloseMatch, "CloseMatch", ref messages );

				output.PrefLabel = AssignLanguageMap( input.PrefLabel, null, "PrefLabel", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				output.Definition = AssignLanguageMap( input.Definition, null, "Definition", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
				output.ExactMatch = AssignRegistryResourceURIsListAsStringList( input.ExactMatch, "ExactMatch", ref messages );
				output.Narrower = AssignRegistryResourceURIsListAsStringList( input.Narrower, "Narrower", ref messages );
				output.NarrowMatch = AssignRegistryResourceURIsListAsStringList( input.NarrowMatch, "NarrowerMatch", ref messages );
				output.Related = AssignRegistryResourceURIsListAsStringList( input.Related, "Related", ref messages );
				//output.RelatedMatch = AssignRegistryResourceURIsListAsStringList( input.RelatedMatch, "RelatedMatch", ref messages );
				//SupersededBy
				output.SupersededBy = AssignRegistryResourceURIAsString( input.SupersededBy, "SupersededBy", ref messages, false, false );
				output.HiddenLabel = AssignLanguageMapList( input.HiddenLabel, null, "HiddenLabel", DefaultLanguageForMaps, ref messages );


				//output.InLanguage = PopulateInLanguage( input.Language, "Language", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );
				if ( !string.IsNullOrWhiteSpace( input.Notation ) )
					output.Notation = input.Notation;

				output.Note = AssignLanguageMapList( input.Note, null, "Note", DefaultLanguageForMaps, ref messages );

				//TODO - handle where could be list or string
				var inScheme = "";
				//stop using InSchemeList
				//input.InSchemeList = null;
				if ( input.InScheme != null )
				{
					if ( input.InScheme.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
					{
						Newtonsoft.Json.Linq.JArray stringArray = ( Newtonsoft.Json.Linq.JArray )input.InScheme;
						//input.InScheme = stringArray[ 0 ].ToString();
						inScheme = stringArray[ 0 ].ToString();
					}
					else
						inScheme = input.InScheme.ToString();
				}
				

				//if ( input.InScheme == null || input.InScheme.ToString() == "" )
				//{
				//	if ( input.InSchemeList != null && input.InSchemeList.Count() > 0 )
				//	{
				//		//temp workaround - a
				//		//input.InScheme = input.InSchemeList[ 0 ];
				//		inScheme = input.InSchemeList[ 0 ];
				//		input.InSchemeList = null;
				//	}
				//}
				//else
				//{
				//	input.InSchemeList = null;
				//	if ( input.InScheme.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
				//	{
				//		Newtonsoft.Json.Linq.JArray stringArray = ( Newtonsoft.Json.Linq.JArray )input.InScheme;
				//		//input.InScheme = stringArray[ 0 ].ToString();
				//		inScheme = stringArray[ 0 ].ToString();
				//	}
				//}

				//TODO - handle where the InScheme is supplied and need to convert to URI
				if ( !string.IsNullOrWhiteSpace( inScheme ))
				{
					output.InScheme = AssignRegistryResourceURIAsString( inScheme, "InScheme", ref messages, false, false );
				}
				//==== handle multiple formats for topConceptOf
				var topConceptOf = "";
				if ( input.TopConceptOf != null )
				{
					if ( input.TopConceptOf.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
					{
						Newtonsoft.Json.Linq.JArray stringArray = ( Newtonsoft.Json.Linq.JArray )input.TopConceptOf;
						//input.InScheme = stringArray[ 0 ].ToString();
						topConceptOf = stringArray[ 0 ].ToString();
					}
					else
						topConceptOf = input.TopConceptOf.ToString();
				}
					

				if ( !string.IsNullOrWhiteSpace( topConceptOf ) )
				{
					output.TopConceptOf = AssignRegistryResourceURIAsString( topConceptOf, "TopConceptOf", ref messages, false, false );
				}
			}
			catch(Exception ex)
			{
				LoggingHelper.LogError( ex, string.Format("Exception on ConceptScheme: {0}, Concept#{1}: {2}", CurrentCtid,  compCntr, ex.Message) );
				messages.Add( string.Format( "Exception on Concept#{0}: {1}", compCntr, ex.Message ) );
				isValid = false;
			}
			return isValid;
		}
		private OutputEntity GetConceptScheme( object graph, ref int conceptsCount, ref List<string> messages )
		{
			//string ctid = "";
			conceptsCount = 0;
			//int conceptsCntr = 0;
			if ( graph == null )
			{
				return null;
			}
			var mainEntity = new Dictionary<string, object>();
			var payload = graph.ToString();


			//
			var entity = new OutputEntity();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "skos:ConceptScheme" ) > -1 || token.ToString().IndexOf( "ConceptScheme" ) > -1 )
					{
						//entity = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.ConceptScheme>( GetJsonSerializerSettings() );
						entity = Newtonsoft.Json.JsonConvert.DeserializeObject<OutputEntity>( token.ToString() );

						if ( entity.Source.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
						{
							//we could just force the object back to a string?
							Newtonsoft.Json.Linq.JArray stringArray = ( Newtonsoft.Json.Linq.JArray )entity.Source;
							entity.Source = stringArray[ 0 ];
							
						}
					}
					else if ( token.ToString().IndexOf( "skos:Concept" ) > -1 || token.ToString().IndexOf( "Concept" ) > -1 )
					{
						conceptsCount++;
						//var concept2 = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.Concept>();
						var concept = Newtonsoft.Json.JsonConvert.DeserializeObject<OutputConcept>( token.ToString() );

						if ( !HasData( concept.PrefLabel ) )
							messages.Add( string.Format( "A pref label must be provided for the concept (#{0}).", conceptsCount ) );
						//
						
						if ( concept.InScheme.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
						{
							Newtonsoft.Json.Linq.JArray stringArray = ( Newtonsoft.Json.Linq.JArray )concept.InScheme;
							concept.InScheme = stringArray[ 0 ].ToString();
						}
						//stop using InSchemeList
						//concept.InSchemeList = null;
						//if ( concept.InScheme == null || concept.InScheme.ToString() == "" )
						//{
						//	if( concept.InSchemeList != null && concept.InSchemeList.Count() > 0 )
						//	{
						//		//temp workaround - a
						//		concept.InScheme = concept.InSchemeList[ 0 ];
						//		concept.InSchemeList = null;
						//	}
						//}
						//else
						//	concept.InSchemeList = null;
						//
						if ( concept.InScheme == null || concept.InScheme.ToString() == "")
							messages.Add( string.Format( "The InScheme property must be provided for the concept (#{0}).", conceptsCount ) );

						if ( string.IsNullOrWhiteSpace( concept.TopConceptOf ) )
						{
							//if no top scheme, then must have. 20-01-22 not sure if this is accurate, even though a warning!
							if ( string.IsNullOrWhiteSpace( concept.Broader ) )
								warningMessages.Add( string.Format( "Either the TopConceptOf or the Broader property must be provided for the concept '{0}'(#{1}).", concept.PrefLabel, conceptsCount ) );
						}
						

						outputConcepts.Add( concept );

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


		private OutputEntity GetConceptSchemeFromPlainGraph(GraphRequest request, bool hasDefaultLanguage, ref int conceptsCount, ref List<string> messages)
		{
			//string ctid = "";
			conceptsCount = 0;
			object graph = request.GraphInput.Graph;// 
			if ( graph == null )
			{
				return null;
			}
			var mainEntity = new Dictionary<string, object>();
			var payload = graph.ToString();


			//
			var output = new OutputEntity();
			OutputConcept concept = new OutputConcept();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "skos:ConceptScheme" ) > -1 || token.ToString().IndexOf( "ConceptScheme" ) > -1 )
					{
						//var input2 = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.ConceptSchemePlain>();
						var input = Newtonsoft.Json.JsonConvert.DeserializeObject<RJ.ConceptSchemePlain>( token.ToString() );
						#region  Populate the concept scheme 

						CurrentCtid = output.CTID = FormatCtid( input.CTID, "ConceptScheme", ref messages );
						output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );
						output.InLanguage = PopulateInLanguage( input.InLanguage, "concept scheme", "concept scheme", hasDefaultLanguage, ref messages );

						//output.altIdentifier = AssignListToList( input.altIdentifier );
						//output.ChangeNote = AssignLanguageMapList( input.ChangeNote, null, "ChangeNote", DefaultLanguageForMaps, ref messages );
						output.ConceptKeyword = AssignLanguageMapList( input.ConceptKeyword, null, "ConceptKeywork", DefaultLanguageForMaps, ref messages );
						output.ConceptTerm = AssignRegistryResourceURIsListAsStringList( input.ConceptTerm, "ConceptTerm", ref messages );

						output.Creator = AssignValidUrlListAsStringList( input.Creator, "Concept scheme creator", ref messages, false );
						output.Creator = SetUrlsToLowerCase( output.Creator );
						//this actually is a year, not datetime!
						output.DateCopyrighted = MapYear( input.DateCopyrighted, "DateCopyrighted", ref messages );
						output.DateCreated = MapDate( input.DateCreated, "DateCreated", ref messages );
						if ( string.IsNullOrWhiteSpace( output.DateCreated ) )
						{
							//always require this for direct calls
							//if ( isConceptschemeDateCreatedRequired )
							//messages.Add( "A DateCreated must be provided for the concept scheme." );
						}
						output.DateModified = MapDateTime( input.DateModified, "DateModified", ref messages );

						//allow CTID or full URI
						if ( !request.GenerateHasTopChild && !request.GenerateHasTopChildFromIsTopChild )
						{
							output.HasTopConcept = AssignRegistryResourceURIsListAsStringList( input.HasTopConcept, "Concept scheme HasTopConcept", ref messages, false, true );
						}
						//history note is not compatible from skos format
						//output.HistoryNote = AssignRegistryURIsListAsStringList( input.HistoryNote, "Concept scheme HistoryNote", ref messages );
						//output.HistoryNote = AssignLanguageMap( input.HistoryNote, null, "HistoryNote", DefaultLanguageForMaps, CurrentCtid, false, ref messages );

						output.License = AssignValidUrlAsString( input.License, "License", ref messages, false, false );

						output.Name = AssignLanguageMap( input.Name, null, "Concept Scheme Title", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

						CurrentEntityName = GetFirstItemValue( output.Name );

						output.Description = AssignLanguageMap( input.Description, null, "Concept Scheme Description", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

						//this url will likely be a purl to the particular status
						//	http://purl.org/ctdlasn/vocabs/publicationStatus/Draft
						output.PublicationStatusType = AssignValidUrlAsString( input.PublicationStatusType, "PublicationStatusType", ref messages, true, false );


						//temp =====================
						output.PublicationStatusType = ( output.PublicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );
						// =========================

						output.Publisher = AssignRegistryResourceURIAsString( input.Publisher, "Publisher", ref messages, false, false );
						output.PublisherName = AssignLanguageMap( input.PublisherName, null, "PublisherName", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
						output.Rights = AssignLanguageMap( input.Rights, null, "Rights", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
						output.RightsHolder = AssignValidUrlAsString( input.RightsHolder, "RightsHolder", ref messages, false, false );
						output.Source = AssignValidUrlAsString( input.Source, "ConceptScheme.Source", ref messages, true, false );
						#endregion

						if ( conceptsCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the conceptScheme is now first in the export document
							//conceptsCount = jarray.Count - 1;
						}
						
						//return entity;
					}
					else if ( token.ToString().IndexOf( "skos:Concept" ) > -1 || token.ToString().IndexOf( "Concept" ) > -1 )
					{
						conceptsCount++;
						//var conceptPlain2 = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.ConceptPlain>();
						var conceptPlain = Newtonsoft.Json.JsonConvert.DeserializeObject<RJ.ConceptPlain>( token.ToString() );
						concept = new OutputConcept();

						if ( ToMapConcept( conceptPlain, concept, hasDefaultLanguage, conceptsCount, ref messages ) )
						{
							if ( request.GenerateHasTopChild )
							{
								if ( output.HasTopConcept == null )
									output.HasTopConcept = new List<string>();

								output.HasTopConcept.Add( concept.CtdlId );
								//if ( compCntr > 900 )
								//	break;
							} else if ( request.GenerateHasTopChildFromIsTopChild 
								&& !string.IsNullOrWhiteSpace( concept.TopConceptOf ) )
							{
								if ( output.HasTopConcept == null )
									output.HasTopConcept = new List<string>();

								output.HasTopConcept.Add( concept.CtdlId );
							}

							//only do this all concepts are top
							if ( request.GenerateIsTopChild )
							{
								concept.TopConceptOf = output.CtdlId;
								//will inScheme have to be set as well?
								if ( concept.InScheme == null || concept.InScheme.ToString() == "" )
									concept.InScheme = output.CtdlId;
							}

							//if ( string.IsNullOrWhiteSpace( concept.PrefLabel ) )
							//	messages.Add( string.Format( "A pref label must be provided for the concept (#{0}).", conceptsCount ) );
							if ( concept.InScheme == null || concept.InScheme.ToString() == "" )
							{
								if (request.GenerateInScheme)
								{
									concept.InScheme = output.CtdlId;
								} else 
									messages.Add( string.Format( "The InScheme property must be provided for the concept (#{0}).", conceptsCount ) );
							}

							if ( string.IsNullOrWhiteSpace( concept.TopConceptOf ) )
							{
								//if no top scheme, then must have
								if ( string.IsNullOrWhiteSpace( concept.Broader ) )
									warningMessages.Add( string.Format( "Either the TopConceptOf or the Broader property must be provided for the concept '{0}'(#{1}).", concept.PrefLabel, conceptsCount ) );
							}
							outputConcepts.Add( concept );
						}
					}
				}
				else
				{
					//error
				}
			}

			return output;
		}
		#endregion


		#region from a skos input request
		/*
		public void PublishFromSkosGraph( SkosGraphConceptSchemeRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			var output = new OutputEntity();
			GraphContainer og = new GraphContainer();
			SkosConceptSchemeGraph input = request.ConceptSchemeGraph;// 

			if ( ToMapFromSkosGraph( input, ref output, ref messages ) )
			{
				if ( string.IsNullOrWhiteSpace( output.CTID ) && output.CtdlId.IndexOf( "/ce-" ) > -1 )
				{
					output.CTID = output.CtdlId.Substring( output.CtdlId.IndexOf( "/ce-" ) + 1 );
					request.CTID = output.CTID;
				}
				if (string.IsNullOrWhiteSpace(request.CTID))
					request.CTID = output.CTID;

				og.Graph.Add( output );
				//add competencies
				if ( outputConcepts != null && outputConcepts.Count > 0 )
				{
					foreach ( var item in outputConcepts )
					{
						og.Graph.Add( item );
					}
				}
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				og.Type = "skos:ConceptScheme"; //ignored anyway
				og.Context = ceasnContext;
				//
				helper.Payload = JsonConvert.SerializeObject( og, ServiceHelperV2.GetJsonSettings() );
				

				//will need to extract a ctid?
				CER cer = new CER( "ConceptScheme", output.Type, output.CTID, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					PublishingForOrgCtid = helper.OwnerCtid,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					IsPublisherRequest = helper.IsPublisherRequest,
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
						LoggingHelper.DoTrace( 4, string.Format( "ConceptScheme.PublishFromSkosGraph. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "ConceptScheme", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					return; //===================
				}
				//
				string identifier = "ConceptScheme_" + output.CTID;
				if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
					//for now need to ensure envelopid is returned
					helper.RegistryEnvelopeId = crEnvelopeId;

					string msg = string.Format( "<p>Published Concept Scheme</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
					NotifyOnPublish( "ConceptScheme", msg );
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
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );
			}
			helper.SetMessages( messages );
			return;
		}


		public bool ToMapFromSkosGraph( SkosConceptSchemeGraph input, ref OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "ConceptScheme";
			bool isValid = true;
			int conceptsCount = 0;
			try
			{
				output = GetSkosConceptScheme( input.Graph, ref conceptsCount, ref messages );
				if ( output == null || string.IsNullOrWhiteSpace( output.CTID ) )
				{
					messages.Add( "A skos:ConceptScheme document was not found." );
				}
				else
				{
					//CHECK for required fields
					CurrentCtid = output.CTID = FormatCtid( output.CTID, "ConceptScheme", ref messages );
					if ( !HasData( output.Name ) )
						messages.Add( "A name must be provided for the concept scheme." );
					if ( !HasData( output.Description ) )
						messages.Add( "A description must be provided for the concept scheme." );

					if ( output.HasTopConcept == null || output.HasTopConcept.Count() == 0 )
						messages.Add( "HasTopConcept is a required property." );
					if ( string.IsNullOrWhiteSpace( output.DateCreated ) )
						if ( isConceptschemeDateCreatedRequired )
							messages.Add( "A dateCreated must be provided for the concept scheme." );
					if ( output.InLanguage == null || output.InLanguage.Count() == 0 )
						messages.Add( "A ConceptScheme.InLanguage must be provided for the concept scheme." );

					if ( string.IsNullOrWhiteSpace( output.Source ) )
						messages.Add( "A Source must be provided for the concept scheme." );

					if ( string.IsNullOrWhiteSpace( output.PublicationStatusType ) )
						messages.Add( "A PublicationStatusType must be provided for the concept scheme." );
					else
					{
						//temp =====================
						output.PublicationStatusType = ( output.PublicationStatusType ?? "" ).Replace( "/vocab/publicationStatus", "/vocabs/publicationStatus" );
						// =========================
					}
					if ( conceptsCount == 0 )
						messages.Add( "No documents of type skos:Concept were found for this concept scheme." );
				}
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "ConceptSchemeServices.ToMapFromCASS" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		} //
		private OutputEntity GetSkosConceptScheme( object graph, ref int conceptsCount, ref List<string> messages )
		{
			//string ctid = "";
			conceptsCount = 0;
			if ( graph == null )
			{
				return null;
			}
			bool hasDefaultLanguage = false;
			var entity = new OutputEntity();
			var concept = new RJ.Concept();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "skos:ConceptScheme" ) > -1 || token.ToString().IndexOf( "ConceptScheme" ) > -1 )
					{
						//map first to skos, and then map to concept scheme
						var sentity = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<SkosConceptScheme>();
						if ( sentity != null )
						{
							
							entity.CTID = sentity.CTID;
							entity.Name = AssignLanguageMap(sentity.Label, "skos Label", ref messages);
							entity.Description = AssignLanguageMap(sentity.Description, "skos Description", ref messages);
							entity.Source = sentity.Source;
							entity.DateCreated = MapDateTime( sentity.DateCreated, "", ref messages );
							entity.DateModified = MapDateTime( sentity.DateModified, "DateModified", ref messages );
							entity.PublicationStatusType = sentity.PublicationStatusType;

							foreach( var item in sentity.HasTopConcept)
							{
								if ( entity.HasTopConcept == null )
									entity.HasTopConcept = new List<string>();
								entity.HasTopConcept.Add( item );
							}

							entity.InLanguage = PopulateInLanguage( sentity.Language, "concept scheme", "concept scheme", hasDefaultLanguage, ref messages );
						}
						if ( conceptsCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the conceptScheme is now first in the export document
							conceptsCount = jarray.Count - 1;
						}
						//return entity;
					}
					else if ( token.ToString().IndexOf( "skos:Concept" ) > -1 || token.ToString().IndexOf( "Concept" ) > -1 )
					{
						conceptsCount++;
						concept = new RJ.Concept();
						var skosConcept = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<SkosConcept>();
						if (skosConcept != null)
						{
							concept.CTID = skosConcept.CTID;
							concept.PrefLabel = AssignLanguageMap(skosConcept.PrefLabel, "skosConcept.PrefLabel", ref messages);
							concept.Definition = AssignLanguageMap( skosConcept.Definition, "skosConcept.Definition", ref messages );
							concept.InScheme = skosConcept.InScheme;
							concept.TopConceptOf = skosConcept.TopConceptOf;
							if ( !string.IsNullOrEmpty( skosConcept.Id ) )
							{
								if ( concept.AltLabel == null )
									concept.AltLabel = new RJ.LanguageMapList();

								concept.AltLabel.Add( DefaultLanguageForMaps, new List<string>() { skosConcept.Id } );
							}
						}

						//var concept = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.Concept>();

						if ( !HasData( concept.PrefLabel ) )
							messages.Add( string.Format( "A pref label must be provided for the concept (#{0}).", conceptsCount ) );
						if ( string.IsNullOrWhiteSpace( concept.InScheme ) )
							messages.Add( string.Format( "The InScheme property must be provided for the concept (#{0}).", conceptsCount ) );
						if ( string.IsNullOrWhiteSpace( concept.TopConceptOf ) )
						{
							//if no top scheme, then must have
							if ( string.IsNullOrWhiteSpace( concept.Broader ) )
								warningMessages.Add( string.Format( "Either the TopConceptOf or the Broader property must be provided for the concept '{0}'(#{1}).", concept.PrefLabel, conceptsCount ) );
						}
						outputConcepts.Add( concept );
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

		/// <summary>
		/// A request from CASS will come already formatted
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="helper"></param>
		public void PublishFromSkos( SkosEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			var output = new OutputEntity();
			GraphContainer og = new GraphContainer();


			if ( ToMapFromSkos( request, output, ref messages ) )
			{
				if ( string.IsNullOrWhiteSpace( output.CTID ) && output.CtdlId.IndexOf( "/ce-" ) > -1 )
				{
					output.CTID = output.CtdlId.Substring( output.CtdlId.IndexOf( "/ce-" ) + 1 );
					request.CTID = output.CTID;
				}

				og.Graph.Add( output );
				//add competencies
				if ( outputConcepts != null && outputConcepts.Count > 0 )
				{
					foreach ( var item in outputConcepts )
					{
						og.Graph.Add( item );
					}
				}
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				og.Type = "skos:ConceptScheme"; //ignored anyway
				og.Context = ceasnContext;

				//
				helper.Payload = JsonConvert.SerializeObject( og, ServiceHelperV2.GetJsonSettings() );
				//helper.Payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );

				//will need to extract a ctid?
				CER cer = new CER( "ConceptScheme", output.Type, output.CTID, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					PublishingForOrgCtid = helper.OwnerCtid,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					IsPublisherRequest = helper.IsPublisherRequest,
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
						LoggingHelper.DoTrace( 4, string.Format( "ConceptScheme.PublishFromSkos. Validate ApiKey failed. Org CTID: {0},  CTID: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "ConceptScheme", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					return; //===================
				}
				//
				string identifier = "ConceptScheme_" + output.CTID;
				if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
					//for now need to ensure envelopid is returned
					helper.RegistryEnvelopeId = crEnvelopeId;

					string msg = string.Format( "<p>Published Concept Scheme</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
					NotifyOnPublish( "ConceptScheme", msg );
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
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelperV2.GetJsonSettings() );
			}
			helper.SetMessages( messages );
			return;
		}


		/// <summary>
		/// Input from CASS should already be properly formatted.
		/// Ensure a conceptScheme exists
		/// </summary>
		/// <param name="input"></param>
		/// <param name="requestCTID"></param>
		/// <param name="output"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool ToMapFromSkos( SkosEntityRequest request, OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "SkosConceptScheme";
			bool isValid = true;
			//
			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			SkosInputEntity input = request.ConceptScheme;
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
			//

			output.InLanguage = PopulateInLanguage( input.Language, "concept scheme", "concept scheme", hasDefaultLanguage, ref messages );
			try
			{
				#region  Populate the concept scheme 
				//id - assume the schema name
				//actually id make just be the schema name for this concept scheme ex: ceterms:Audience
				//19-03-12 make required input
				CurrentCtid = output.CTID = FormatCtid( input.CTID, "SkosConceptScheme", ref messages );
				//output.CTID = ExtractCtid( input.Id, "Skos Concept Scheme", ref messages ).ToLower();
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);;

				//output.ChangeNote = AssignLanguageMapList( input.ChangeNote, "ChangeNote", DefaultLanguageForMaps, ref messages );
				output.ConceptKeyword = AssignLanguageMapList( input.ConceptKeyword, "ConceptKeywork", DefaultLanguageForMaps, ref messages );
				//output.ConceptTerm = AssignRegistryURIsListAsStringList( input.ConceptTerm, "", ref messages );

				output.Creator = AssignValidUrlAsStringList( input.Creator, "Concept scheme creator", ref messages, false, false );

				//this actually date, not datetime!
				output.DateCopyrighted = MapDate( input.DateCopyrighted, "DateCopyrighted", ref messages );
				output.DateCreated = MapDateTime( input.DateCreated, "DateCreated", ref messages );
				output.DateModified = MapDateTime( input.DateModified, "DateModified", ref messages );

				output.HasTopConcept = AssignRegistryResourceURIsListAsStringList( input.HasTopConcept, "Concept scheme HasTopConcept", ref messages, false, true );
				//history note is not compatible from skos format
				//output.HistoryNote = AssignRegistryURIsListAsStringList( input.HistoryNote, "Concept scheme HistoryNote", ref messages );



				output.License = AssignValidUrlAsString( input.License, "License", ref messages, false, false );

				//output.Name = AssignLanguageMap( input.Title, "Concept Title", DefaultLanguageForMaps, CurrentCtid, ref messages, true );
				output.Name = AssignLanguageMap( "", input.Label, "Concept Scheme Title", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
				output.Description = AssignLanguageMap( "", input.Description, "Concept Scheme Description", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

				output.PublicationStatusType = AssignValidUrlAsString( input.PublicationStatusType, "PublicationStatusType", ref messages, false, false );
				output.Publisher = AssignValidUrlAsString( input.Publisher, "Publisher", ref messages, false, false );
				output.PublisherName = AssignLanguageMap( input.PublisherName, null, "PublisherName", DefaultLanguageForMaps, CurrentCtid, false, ref messages );
				output.Rights = AssignLanguageMap( input.Rights, "Rights", DefaultLanguageForMaps, CurrentCtid, ref messages, false );
				output.RightsHolder = AssignValidUrlAsString( input.RightsHolder, "RightsHolder", ref messages, false, false );
				//output.Source = AssignValidUrlAsString( input.Source, "Source", ref messages, false, false );
				#endregion


				#region  Populate the concepts
				if ( request.Concepts == null || request.Concepts.Count == 0 )
				{
					messages.Add( "At least one Concept must be included with a concept scheme." );
				}
				else
				{
					OutputConcept concept = new OutputConcept();
					int compCntr = 0;
					//add each top competency
					foreach ( var item in request.Concepts )
					{
						concept = new OutputConcept();
						compCntr++;
						if ( ToMapSkosConcept( item, concept, hasDefaultLanguage, compCntr, ref messages ) )
						{
							outputConcepts.Add( concept );
						}
					}
				}
				#endregion
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "ConceptSchemeServices.ToMapFromSkos" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}

		public bool ToMapSkosConcept( RA.Models.Input.SkosConcept input, OutputConcept output, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			output.CTID = FormatCtid( input.CTID, string.Format( "Concept (#{0})", compCntr ), ref messages );
			
			output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
			output.AltLabel = AssignLanguageMapList( input.AltLabel, "AltLabel", DefaultLanguageForMaps, ref messages );
			//output.Broader = AssignRegistryResourceURIAsString( input.Broader, "Broader", ref messages, false, false );
			output.BroadMatch = AssignRegistryResourceURIsListAsStringList( input.BroadMatch, "BroadMatch", ref messages );

			output.ChangeNote = AssignLanguageMapList( input.ChangeNote, "ChangeNote", DefaultLanguageForMaps, ref messages );
			output.CloseMatch = AssignRegistryResourceURIsListAsStringList( input.CloseMatch, "CloseMatch", ref messages );

			output.PrefLabel = AssignLanguageMap( "", input.PrefLabel, "PrefLabel", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
			output.Definition = AssignLanguageMap( "", input.Definition, "Definition", DefaultLanguageForMaps,  CurrentCtid, true ,ref messages);

			output.ExactMatch = AssignRegistryResourceURIsListAsStringList( input.ExactMatch, "ExactMatch", ref messages );
			output.Narrower = AssignRegistryResourceURIsListAsStringList( input.Narrower, "Narrower", ref messages );
			output.NarrowMatch = AssignRegistryResourceURIsListAsStringList( input.NarrowMatch, "NarrowerMatch", ref messages );
			output.RelatedMatch = AssignRegistryResourceURIsListAsStringList( input.RelatedMatch, "RelatedMatch", ref messages );

			output.HiddenLabel = AssignLanguageMapList( input.HiddenLabel, "HiddenLabel", DefaultLanguageForMaps, ref messages );

			//TODO - handle where the ConceptScheme is supplied and need to convert to URI
			output.InScheme = AssignRegistryResourceURIAsString( input.InScheme, "InScheme", ref messages, false, false );

			output.InLanguage = PopulateInLanguage( input.InLanguage, "Language", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );
			//if ( !string.IsNullOrWhiteSpace( input.Notation ) )
			//	output.Notation = input.Notation;

			output.Note = AssignLanguageMapList( input.Note, "Note", DefaultLanguageForMaps, ref messages );
			//TODO - need to add helpers to allow ctids and convert to registry url
			output.TopConceptOf = AssignRegistryResourceURIAsString( input.TopConceptOf, "TopConceptOf", ref messages, false, false );

			return isValid;
		}

		*/
		#endregion
	}
}
