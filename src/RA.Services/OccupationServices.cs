using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using RA.Models.Input;
using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.OccupationRequest;
using InputEntity = RA.Models.Input.Occupation;
using OutputEntity = RA.Models.JsonV2.Occupation;
using OutputGraph = RA.Models.JsonV2.GraphContainer;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class OccupationServices : ServiceHelperV2
	{
		string className = "OccupationServices";
		/// <summary>
		/// Publish an Occupation to the Credential Registry
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="messages"></param>
		public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			LoggingHelper.DoTrace( 6, string.Format( "OccupationServices.Publish Request for: {0} Started.", request.Occupation.Name ) );
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

				CER cer = new CER( "Occupation", output.Type, output.CTID, helper.SerializedInput )
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
						LoggingHelper.DoTrace( 4, string.Format( "Occupation.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				/* check if previously published
				 * - if found, use the same publishing method
				 */
				if ( !SupportServices.ValidateAgainstPastRequest( "Occupation", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
				}
				else
				{
					string identifier = "Occupation_" + request.Occupation.CTID;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Occupation: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.CTID, crEnvelopeId );
						NotifyOnPublish( "Occupation", msg );
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
			CurrentEntityType = "Occupation";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.Occupation;
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
				CurrentCtid = output.CTID = FormatCtid( input.CTID, "Occupation Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

				//required
				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Occupation.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				CurrentEntityName = GetFirstItemValue( output.Name );
				output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

				//classification? Range of Concept, but no target concept scheme.
				//
				output.Comment = AssignLanguageMapList( input.Comment, input.Comment_map, "comment", DefaultLanguageForMaps, ref messages );
				output.HasJob = AssignRegistryResourceURIsListAsStringList( input.HasJob, "HasJob", ref messages, false );
				output.HasSpecialization = AssignRegistryResourceURIsListAsStringList( input.HasSpecialization, "HasSpecialization", ref messages, false );
				output.HasWorkRole = AssignRegistryResourceURIsListAsStringList( input.HasWorkRole, "HasWorkRole", ref messages, false );

				output.Identifier = AssignIdentifierListToList( input.Identifier, ref messages );
				output.IsSpecializationOf = AssignRegistryResourceURIsListAsStringList( input.IsSpecializationOf, "IsSpecializationOf", ref messages, false );
				output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Occupation Keywords", ref messages );
				//sameAs - should this be an entity reference for flexibility?. If so what type(s)
				//or list of URIs
				//output.SameAs = AssignValidUrlListAsStringList( input.SameAs, "Same As", ref messages );
				output.SameAs = FormatEntityReferencesList( input.SameAs, "Occupation.Same As", false, ref messages );
				//
				output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

				output.AbilityEmbodied = AssignRegistryResourceURIsListAsStringList( input.AbilityEmbodied, "AbilityEmbodied", ref messages, false );
				output.KnowledgeEmbodied = AssignRegistryResourceURIsListAsStringList( input.KnowledgeEmbodied, "knowledgeEmbodied", ref messages, false );
				output.SkillEmbodied = AssignRegistryResourceURIsListAsStringList( input.SkillEmbodied, "skillEmbodied", ref messages, false );
			

				//handle industry and occupation
				HandleCredentialAlignmentFields( input, output, ref messages );

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
				LoggingHelper.DoTrace( 1, string.Format( "{0}.ToMap *******Mapping took a little longer: elapsed: {1:N2} seconds.", className, duration.TotalSeconds )); ;

			return isValid;
		}



		#region === CredentialAlignmentObject ===
		public void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			//frameworks
			//=== occupations ===============================================================
			//can't depend on the codes being SOC
			output.OccupationType = FormatCredentialAlignmentListFromFrameworkItemList( input.OccupationType, true, ref messages );
			//no longer using as concrete property, just used for simple list of strings
			//append to OccupationType
			output.OccupationType = AppendCredentialAlignmentListFromList( input.AlternativeOccupationType, null, "", "", "AlternativeOccupationType", output.OccupationType, ref messages );

			//NEW - allow a list of Onet codes, and resolve
			output.OccupationType = HandleListOfONET_Codes( input.ONET_Codes, output.OccupationType, ref messages );

			//output.AlternativeOccupationType = AssignLanguageMapList( input.AlternativeOccupationType, input.AlternativeOccupationType_Map, "Credential AlternativeOccupationType", ref output.OccupationType, ref messages );

			//=== industries ===============================================================
			//can't depend on the codes being NAICS??
			output.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( input.IndustryType, true, ref messages );
			//NEW - allow a list of Naics, and resolve
			output.IndustryType = HandleListOfNAICS_Codes( input.NaicsList, output.IndustryType, ref messages );

			//append to IndustryType
			output.IndustryType = AppendCredentialAlignmentListFromList( input.AlternativeIndustryType, null, "", "", "AlternativeIndustryType", output.IndustryType, ref messages );
			//output.AlternativeIndustryType = AssignLanguageMapList( input.AlternativeIndustryType, input.AlternativeIndustryType_Map, "Credential AlternativeIndustryType", ref messages );
			//

		}

		//see common methods in ServiceHelper
		#endregion


	}
}
