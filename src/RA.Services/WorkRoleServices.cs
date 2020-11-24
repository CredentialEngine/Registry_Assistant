using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using RA.Models.Input;
using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.WorkRoleRequest;
using InputEntity = RA.Models.Input.WorkRole;
using OutputEntity = RA.Models.JsonV2.WorkRole;
using OutputGraph = RA.Models.JsonV2.GraphContainer;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class WorkRoleServices : ServiceHelperV2
	{
		string className = "WorkRoleServices";
		/// <summary>
		/// Publish an WorkRole to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="messages"></param>
		public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			LoggingHelper.DoTrace( 6, string.Format( "WorkRoleServices.Publish Request for: {0} Started.", request.WorkRole.Name ) );
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

			if ( ToMap( request, output, ref messages ) )
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

				CER cer = new CER( "WorkRole", output.Type, output.CTID, helper.SerializedInput )
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
						LoggingHelper.DoTrace( 4, string.Format( "WorkRole.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				/* check if previously published
				 * - if found, use the same publishing method
				 */
				if ( !SupportServices.ValidateAgainstPastRequest( "WorkRole", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
				}
				else
				{
					string identifier = "WorkRole_" + request.WorkRole.CTID;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published WorkRole: {0}</p><p>CTID: {1}</p> <p>EnvelopeId: {2}</p> ", output.Name, output.CTID, crEnvelopeId );
						NotifyOnPublish( "WorkRole", msg );
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
			CurrentEntityType = "WorkRole";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.WorkRole;
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
				CurrentCtid = output.CTID = FormatCtid( input.CTID, "WorkRole Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

				//required
				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "WorkRole.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				CurrentEntityName = GetFirstItemValue( output.Name );
				output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

				//classification? Range of Concept, but no target concept scheme.
				//
				output.Comment = AssignLanguageMapList( input.Comment, input.Comment_map, "comment", DefaultLanguageForMaps, ref messages );
				output.HasTask = AssignRegistryResourceURIsListAsStringList( input.HasTask, "HasTask", ref messages, false );

				output.Identifier = AssignIdentifierListToList( input.Identifier, ref messages );

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
	}
}
