using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using RA.Models.Input;
using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.TaskRequest;
using GraphEntityRequest = RA.Models.Input.GraphContentRequest;
using InputEntity = RA.Models.Input.Task;
using OutputEntity = RA.Models.JsonV2.Task;
using OutputGraph = RA.Models.JsonV2.GraphContainer;
using GraphContainer = RA.Models.JsonV2.GraphContainer;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class TaskServices : ServiceHelperV2
	{
		string className = "TaskServices";

		#region publish from graph
		public void PublishGraph( GraphEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;
			string status = "";

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			GraphContainer og = new GraphContainer();
			var output = new OutputEntity();

			if ( ToMapFromGraph( request, ref output, ref messages ) )
			{
				og.Graph.Add( output );
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community );
				og.CTID = request.CTID = output.CTID;
				og.Type = "ceterms:Task"; //ignored anyway
				og.Context = ceasnContext;
				//
				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

				//will need to extract a ctid?
				CER cer = new CER( "Task", output.Type, output.CTID, helper.SerializedInput )
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
						LoggingHelper.DoTrace( 4, string.Format( "Task.PublishGraph. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;
				//
				if ( !SupportServices.ValidateAgainstPastRequest( "Task", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					//return; //===================
				}
				else
				{
					//
					string identifier = "Task_" + output.CTID;
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						//if (!string.IsNullOrWhiteSpace(status))
						//	messages.Add( status );
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Concept Scheme</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
						NotifyOnPublish( "Task", msg );
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
		public bool ToMapFromGraph( GraphEntityRequest request, ref OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "Task";
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

			int conceptsCount = 0;
			try
			{
				if ( request.HasLanguageMaps )
				{
					output = GetTask( input.Graph, ref conceptsCount, ref messages );
				}
				else
				{
					output = GetTaskFromPlainGraph( request, hasDefaultLanguage, ref conceptsCount, ref messages );
				}//

				if ( output == null || string.IsNullOrWhiteSpace( output.CTID ) )
				{
					messages.Add( "A ceterms:Task document was not found." );
				}
				else
				{
					//CHECK for required fields
					output.CTID = FormatCtid( output.CTID, "Task", ref messages );
					if ( !HasData( output.Name ) )
						messages.Add( "A name must be provided for the concept scheme." );
					else
					{
						CurrentEntityName = GetFirstItemValue( output.Name );
					}
					if ( !HasData( output.Description ) )
						messages.Add( "A description must be provided for the concept scheme." );
				}
				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "TaskServices.ToMapFromGraph" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
		private OutputEntity GetTask( object graph, ref int conceptsCount, ref List<string> messages )
		{
			conceptsCount = 0;
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
					if ( token.ToString().IndexOf( "ceterms:Task" ) > -1 || token.ToString().IndexOf( "Task" ) > -1 )
					{
						entity = Newtonsoft.Json.JsonConvert.DeserializeObject<OutputEntity>( token.ToString() );

						//if ( entity.Source.GetType() == typeof( Newtonsoft.Json.Linq.JArray ) )
						//{
						//	//we could just force the object back to a string?
						//	Newtonsoft.Json.Linq.JArray stringArray = ( Newtonsoft.Json.Linq.JArray )entity.Source;
						//	entity.Source = stringArray[ 0 ];

						//}
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
		private OutputEntity GetTaskFromPlainGraph( GraphContentRequest request, bool hasDefaultLanguage, ref int conceptsCount, ref List<string> messages )
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
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "ceterms:Task" ) > -1 || token.ToString().IndexOf( "Task" ) > -1 )
					{
						//var input2 = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.TaskPlain>();
						var input = Newtonsoft.Json.JsonConvert.DeserializeObject<RJ.TaskPlain>( token.ToString() );
						#region  Populate the concept scheme 

						CurrentCtid = output.CTID = FormatCtid( input.CTID, "Task", ref messages );
						output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

						output.Name = AssignLanguageMap( input.Name, null, "Concept Scheme Title", DefaultLanguageForMaps, CurrentCtid, true, ref messages );

						CurrentEntityName = GetFirstItemValue( output.Name );

						output.Description = AssignLanguageMap( input.Description, null, "Concept Scheme Description", DefaultLanguageForMaps, CurrentCtid, true, ref messages );
						#endregion

						if ( conceptsCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the Task is now first in the export document
							//conceptsCount = jarray.Count - 1;
						}

						//return entity;
					}
					//else if ( token.ToString().IndexOf( "ceterms:Concept" ) > -1 || token.ToString().IndexOf( "Concept" ) > -1 )
					//{
					//	conceptsCount++;
					//	//var conceptPlain2 = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.ConceptPlain>();
						
					//}
				}
				else
				{
					//error
				}
			}

			return output;
		}
		#endregion
		/// <summary>
		/// Publish an Task to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="messages"></param>
		public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			LoggingHelper.DoTrace( 6, string.Format( "TaskServices.Publish Request for: Publisher: {0} Started.", request.PublishForOrganizationIdentifier) );
			DateTime started = DateTime.Now;
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;
			string status = "";
			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			var output = new OutputEntity();
			OutputGraph og = new OutputGraph();
			//if ( environment != "production" )
			//output.LastUpdated = DateTime.Now.ToUniversalTime().ToString( "yyyy-MM-dd HH:mm:ss UTC" );
			//handle plain or JSON-LD
			if ( request.Task == null )
			{
				//output = request.FormattedTask;
				isValid = ToMapFormatted( request, ref output, ref messages );
			}
			else
			{
				isValid = ToMap( request, output, ref messages );
			}

			if ( isValid )
			{
				og.Graph.Add( output );
				//TODO - is there other info needed, like in context?
				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}
				//this must be: /graph/
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community );
				og.CTID = output.CTID;
				og.Type = output.Type;
				og.Context = ctdlContext;

				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

				CER cer = new CER( "Task", output.Type, output.CTID, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					PublishingForOrgCtid = helper.OwnerCtid
				};

				if ( cer.HasValidPublisherToken() )
				{
					cer.IsManagedRequest = true;
					//get publisher org
					string publisherCTID = "";
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;  //this could be set in method if whole object sent
					}
					else
					{
						//should be an error message returned

						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "Task.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				/* check if previously published
				 * - if found, use the same publishing method
				 */
				if ( !SupportServices.ValidateAgainstPastRequest( "Task", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
				}
				else
				{
					string identifier = "Task_" + output.CTID;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Task: {0}</p><p>CTID: {1}</p> <p>EnvelopeId: {2}</p> ", output.Name, output.CTID, crEnvelopeId );
						NotifyOnPublish( "Task", msg );
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
				og.Graph.Add( output );
				//TODO - is there other info needed, like in context?
				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}
				//this must be: /graph/
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community );
				og.CTID = output.CTID;
				og.Type = output.Type;
				og.Context = ctdlContext;

				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
			}
			helper.SetWarningMessages( warningMessages );
			helper.SetMessages( messages );
		}

		public string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
		{
			OutputGraph og = new OutputGraph();
			var output = new OutputEntity();
			string payload = "";
			isValid = true;
			IsAPublishRequest = false;

			if ( ToMap( request, output, ref messages ) )
			{
				og.Graph.Add( output );
				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}

				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community );
				og.CTID = output.CTID;
				og.Type = output.Type;
				og.Context = ctdlContext;
				payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
			}
			else
			{
				isValid = false;
				//do payload anyway
				og.Graph.Add( output );
				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}

				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community );
				og.CTID = output.CTID;
				og.Type = output.Type;
				og.Context = ctdlContext;

				payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
			}
			if ( warningMessages.Count > 0 )
				messages.AddRange( warningMessages );
			return payload;
		}

		/// <summary>
		/// Map and validate:
		/// - trim all strings
		/// - if empty/default values, set to null
		/// - use codeManager to look up codes
		/// - each code should be checked for proper prefix - supply if missing
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <param name="messages"></param>
		/// <returns></returns>        
		public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
		{
			DateTime mappingStarted = DateTime.Now;
			CurrentEntityType = "Task";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.Task;
			//if request.DefaultLanguage exists use it. 
			//otherwise will come from inLanguage which is required.
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
			try
			{
				CurrentCtid = output.CTID = FormatCtid( input.CTID, "Task Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

				//required
				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Task.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				CurrentEntityName = GetFirstItemValue( output.Name );
				output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

				//classification? Range of Concept, but no target concept scheme.
				if ( input.Classification != null && input.Classification.Count > 0 )
				{
					foreach ( var item in input.Classification )
					{
						if ( string.IsNullOrWhiteSpace( item ) )
							continue;
						bool isUrlPresent = false;
						var statusMessage = "";
						var idUrl = "";
						if ( IsCtidValid( item, "Task.Classification", ref messages ) )
						{
							if ( !RegistryServices.DoesResourceExistByCTID( item, ref idUrl, ref statusMessage, Community ) )
							{
								if ( environment == "development" || environment == "sandbox" )
								{
									warningMessages.Add( string.Format( "WARNING: When a CTID ({0}) is provided for the property: '{1}', the related document must exist in the registry. NOTE: outside the sandbox, this would be an error! {2}", item, "Task.Classification", statusMessage ) );
								}
								else
								{
									messages.Add( string.Format( "Error: When a CTID ({0}) is provided for the property: '{1}', the related document must exist in the registry. {2}", item, "Task.Classification", statusMessage ) );
									return false;
								}
							}
							else
								output.Classification.Add( idUrl );
						} else
						{
							if ( IsUrlValid( item, ref statusMessage, ref isUrlPresent, true ) )
							{
								if ( isUrlPresent )
									output.Classification.Add( item );
							}
							else
								messages.Add( string.Format( "The Task.Classification '(0)' is invalid {1}", item, statusMessage ) );
						}
							
					}
				}
				//
				output.Comment = AssignLanguageMapList( input.Comment, input.Comment_map, "comment", DefaultLanguageForMaps, ref messages );
				output.HasChild = AssignRegistryResourceURIsListAsStringList( input.HasChild, "HasChild", ref messages, false );
				output.IsChildOf = AssignRegistryResourceURIsListAsStringList( input.IsChildOf, "HasTask", ref messages, false );

				output.Identifier = AssignIdentifierListToList( input.Identifier, ref messages );
				output.ListID = string.IsNullOrWhiteSpace(input.ListID) ? null : input.ListID;
				//
				output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages, false, true );

				output.AbilityEmbodied = AssignRegistryResourceURIsListAsStringList( input.AbilityEmbodied, "AbilityEmbodied", ref messages, false );
				output.KnowledgeEmbodied = AssignRegistryResourceURIsListAsStringList( input.KnowledgeEmbodied, "knowledgeEmbodied", ref messages, false );
				output.SkillEmbodied = AssignRegistryResourceURIsListAsStringList( input.SkillEmbodied, "skillEmbodied", ref messages, false );

				output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier, ref messages );

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, className + ".ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			TimeSpan duration = DateTime.Now.Subtract( mappingStarted );
			if ( duration.TotalSeconds > 2 )
				LoggingHelper.DoTrace( 1, string.Format( "{0}.ToMap *******Mapping took a little longer: elapsed: {1:N2} seconds.", className, duration.TotalSeconds ) ); ;

			return isValid;
		}

		public bool ToMapFormatted( EntityRequest request, ref OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "Task";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.Task;
			//if request.DefaultLanguage exists use it. 
			//otherwise will come from inLanguage which is required.
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
			try
			{
				//NO - this approach could result in bad properties
				//or can it
				output = request.FormattedTask;
				if ( string.IsNullOrWhiteSpace( output.CTID ) )
					messages.Add( "Error: Task is missing a CTID." );
				CurrentCtid = output.CTID;// = FormatCtid( input.CTID, "Task Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

				//required
				if ( output.Name == null || string.IsNullOrWhiteSpace( output.Name.ToString() ) )
				{
					//may not be required
					//messages.Add( string.Format("Error: Task ({0}) is missing a Name.", output.CTID) );
				}
				//output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Task.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				else
				{
					CurrentEntityName = GetFirstItemValue( output.Name );
				}
				//output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );
				if ( output.Description == null || string.IsNullOrWhiteSpace( output.Description.ToString() ) )
					messages.Add( string.Format("Error: Task ({0}) is missing a Description.", output.CTID) );

				//classification? Range of Concept, but no target concept scheme.
				//this should exist 
				if (output.Classification != null && output.Classification.Count > 0)
				{
					foreach(var item in output.Classification)
					{
						var statusMessage = "";
						bool isUrlPresent = false;
						if ( IsUrlValid( item, ref statusMessage, ref isUrlPresent, true ) )
						{
							//already present in output
							//if ( isUrlPresent )
							//	output.Classification.Add( item );
						}
						else
							messages.Add( string.Format("The Task.Classification '(0)' is invalid {1}", item, statusMessage ));
					}
				}


				//output.Comment = AssignLanguageMapList( input.Comment, input.Comment_map, "comment", DefaultLanguageForMaps, ref messages );
				//output.HasChild = AssignRegistryResourceURIsListAsStringList( input.HasChild, "HasChild", ref messages, false );
				//output.IsChildOf = AssignRegistryResourceURIsListAsStringList( input.IsChildOf, "HasTask", ref messages, false );

				//output.Identifier = AssignIdentifierListToList( input.Identifier, ref messages );
				//output.ListID = string.IsNullOrWhiteSpace( input.ListID ) ? null : input.ListID;

				//TODO - add means to inject an org
				//output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages, false, true );

				//output.AbilityEmbodied = AssignRegistryResourceURIsListAsStringList( input.AbilityEmbodied, "AbilityEmbodied", ref messages, false );
				//output.KnowledgeEmbodied = AssignRegistryResourceURIsListAsStringList( input.KnowledgeEmbodied, "knowledgeEmbodied", ref messages, false );
				//output.SkillEmbodied = AssignRegistryResourceURIsListAsStringList( input.SkillEmbodied, "skillEmbodied", ref messages, false );
				//output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier, ref messages );

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, className + ".ToMapFormatted" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
	}
}
