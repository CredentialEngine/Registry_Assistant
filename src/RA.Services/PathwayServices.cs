using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using RA.Models.Input;
using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.PathwayRequest;
using InputEntity = RA.Models.Input.Pathway;
using OutputEntity = RA.Models.JsonV2.Pathway;
using OutputGraph = RA.Models.JsonV2.GraphContainer;
using OutputComponent = RA.Models.JsonV2.PathwayComponent;
using OutputCondition = RA.Models.JsonV2.ComponentCondition;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class PathwayServices : ServiceHelperV2
	{
		static string status = "";
		List<OutputComponent> outputComponents = new List<OutputComponent>();
		List<OutputCondition> outputConditions = new List<OutputCondition>();
		/// <summary>
		/// Publish an Pathway to the Credential Registry
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
			OutputGraph og = new OutputGraph();

			if ( ToMap( request, output, ref messages ) )
			{
				og.Graph.Add( output );
				if ( outputComponents != null && outputComponents.Count > 0 )
				{
					foreach ( var item in outputComponents )
					{
						og.Graph.Add( item );
					}
				}
				if ( outputConditions != null && outputConditions.Count > 0 )
				{
					foreach ( var item in outputConditions )
					{
						og.Graph.Add( item );
					}
				}

				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}
				//this must be: /graph/
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				og.Type = output.Type;
				og.Context = ctdlContext;

				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

				CER cer = new CER( "Pathway", output.Type, output.CTID, helper.SerializedInput )
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
						cer.PublishingByOrgCtid = publisherCTID;  //this could be set in method if whole object sent
					}
					else
					{
						//should be an error message returned

						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "Pathway.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				/* check if previously published
				 * - if found, use the same publishing method
				 * 
				 * 
				 */
				if ( !SupportServices.ValidateAgainstPastRequest( "Pathway", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
				}
				else
				{
					string identifier = "Pathway_" + request.Pathway.Ctid;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;

						string msg = string.Format( "<p>Published Pathway: {0}</p><p>CTID: {1}</p> <p>EnvelopeId: {2}</p> ", output.Name, output.CTID, crEnvelopeId );
						NotifyOnPublish( "Pathway", msg );
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
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
				og.Type = output.Type;

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
				if ( outputComponents != null && outputComponents.Count > 0 )
				{
					foreach ( var item in outputComponents )
					{
						og.Graph.Add( item );
					}
				}
				if ( outputConditions != null && outputConditions.Count > 0 )
				{
					foreach ( var item in outputConditions )
					{
						og.Graph.Add( item );
					}
				}
				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}

				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
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

				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
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
			CurrentEntityType = "Pathway";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.Pathway;
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
				output.InLanguage = PopulateInLanguage( input.InLanguage, "Pathway", input.Name, hasDefaultLanguage, ref messages );

				//required

				CurrentCtid = output.CTID = FormatCtid( input.Ctid, "Pathway Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);

				//required
				if ( string.IsNullOrWhiteSpace( input.Name ) )
				{
					if ( input.Name_Map == null || input.Name_Map.Count == 0 )
					{
						messages.Add( FormatMessage( "Error - A name or Name_Map must be entered for Pathway with CTID: '{0}'.", input.Ctid ) );
					}
					else
					{
						output.Name = AssignLanguageMap( input.Name_Map, "Pathway Name", ref messages );
						CurrentEntityName = GetFirstItemValue( output.Name );
					}
				}
				else
				{
					output.Name = Assign( input.Name, DefaultLanguageForMaps );
					CurrentEntityName = input.Name;
				}
				output.Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Pathway Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

				output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, false );

				output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages );
				//output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", false, ref messages );
				output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

				if ( output.OwnedBy == null && output.OfferedBy == null )
				{
					messages.Add( string.Format( "Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Pathway: '{0}'", input.Name ) );
				}

				//
				if ( input.HasChild == null || input.HasChild.Count == 0 )
				{
					messages.Add( "At least one PathwayComponent must be included with in the Pathway HasChild property." );
				}
				else
				{
					//it would be a burden to have a user provide a blank node Id.
					output.HasChildUri = AssignRegistryResourceURIsListAsStringList( input.HasChild, "Pathway HasChild", ref messages, false, true );
				}
				output.HasDestinationComponent = AssignRegistryResourceURIsListAsStringList( input.HasDestinationComponent, "Pathway HasDestinationComponent", ref messages, false, true );
				//
				#region  Populate the components
				if ( request.PathwayComponents == null || request.PathwayComponents.Count == 0 )
				{
					messages.Add( "At least one PathwayComponent must be included with a Pathway request." );
				}
				else
				{
					//
					int childCntr = 0;
					foreach (var item in input.HasChild )
					{
						childCntr++;
						if ( !string.IsNullOrWhiteSpace( item ) && item.StartsWith( "ce-" ) && item.Length == 39 )
						{
							//then must exist in components
							var exists = request.PathwayComponents.FirstOrDefault( a => a.CTID.ToLower() == item.ToLower() );
							if ( exists == null && string.IsNullOrWhiteSpace(exists.CTID))
							{
								messages.Add( string.Format("In Pathway.HasChild #{0}, the CTID: '{1}' was not found in the PathwayComponents. ", childCntr, item) );
							}
						}
					}
					//
					OutputComponent component = new OutputComponent();
					int compCntr = 0;
					//add each top component
					foreach ( var item in request.PathwayComponents )
					{
						component = new OutputComponent();
						compCntr++;
						if ( ToMapComponent( item, component, output, hasDefaultLanguage, compCntr, ref messages ) )
						{
							outputComponents.Add( component );
						}
					}
				}
				#endregion
				//
				#region  Populate the conditions
				//if ( request.ComponentConditions == null || request.ComponentConditions.Count == 0 )
				//{
				//	messages.Add( "At least one ComponentConditions must be included with a Pathway request." );
				//}
				//else
				{
					OutputCondition condition = new OutputCondition();
					int compCntr = 0;
					//add each top component
					foreach ( var item in request.ComponentConditions )
					{
						condition = new OutputCondition();
						compCntr++;
						if ( ToMapCondition( item, condition, output, hasDefaultLanguage, compCntr, ref messages ) )
						{
							outputConditions.Add( condition );
						}
					}
				}
				#endregion

				//xref checks that elements of HasChild exist in pathway components

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "PathwayServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}


		public bool ToMapComponent( RA.Models.Input.PathwayComponent input, OutputComponent output, OutputEntity outputEntity, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			//
			output.CTID = FormatCtid( input.CTID, string.Format( "PathwayComponent (#{0})", compCntr ), ref messages );
			output.CtdlId = GenerateBNodeId();// SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);

			string outputType = "";
			//validate component type:
			if ( ValidatePathwayComponentType( input.PathwayComponentType, ref outputType, ref messages ) )
				output.PathwayComponentType = outputType;


			//establish language. make a common method
			//output.inLanguage = PopulateInLanguage( input.inLanguage, "Competency", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );
			//
			//required???

			output.Name = AssignLanguageMap( ConvertSpecialCharacters( input.Name ), input.Name_Map, string.Format( "Component Name (#{0})", compCntr ), DefaultLanguageForMaps, ref messages, false, 2 );
			//required???
			output.Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, string.Format("Component Description (#{0})", compCntr), DefaultLanguageForMaps, ref messages, false, MinimumDescriptionLength );

			output.CodedNotation = input.CodedNotation;

			output.HasChild = AssignRegistryResourceURIsListAsStringList( input.HasChild, "Pathway Component HasChild", ref messages, false );
			output.IsChildOf = AssignRegistryResourceURIsListAsStringList( input.IsChildOf, "Pathway Component IsChildOf", ref messages, false );

			output.IsDestinationComponentOf = AssignRegistryResourceURIsListAsStringList( input.IsDestinationComponentOf, "Pathway Component IsDestinationComponentOf", ref messages, false );

			output.Points = input.Points;

			output.Preceeds = AssignRegistryResourceURIsListAsStringList( input.Preceeds, "Pathway Component Preceeds", ref messages, false );
			output.Prerequisite = AssignRegistryResourceURIsListAsStringList( input.Prerequisite, "Pathway Component Prerequisite", ref messages, false );
			output.SourceData = AssignValidUrlAsString( input.SourceData, string.Format( "SourceData (#{0})", compCntr ), ref messages, false );
			output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, string.Format( "Subject Webpage (#{0})", compCntr ), ref messages, false );
			return isValid;
		}


		public bool ToMapCondition( RA.Models.Input.ComponentCondition input, OutputCondition output, OutputEntity outputEntity, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			//output.CTID = FormatCtid( input.CTID, string.Format( "ComponentCondition (#{0})", compCntr ), ref messages );
			//change to use a blank node
			//NOTE: must match the id that the parent references
			output.CtdlId = GenerateBNodeId();// SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);

			//output.inLanguage = PopulateInLanguage( input.inLanguage, "Competency", string.Format( "#", compCntr ), hasDefaultLanguage, ref messages );

			if ( string.IsNullOrWhiteSpace( input.Name ) )
			{
				if ( input.Name_Map == null || input.Name_Map.Count == 0 )
				{
					messages.Add( FormatMessage( "Error - A name or Name_Map must be entered for Component Condition '{0}'.", compCntr.ToString() ) );
				}
				else
				{
					output.Name = AssignLanguageMap( input.Name_Map, "Pathway Component Name", ref messages );
				}
			}
			else
			{
				output.Name = Assign( input.Name, DefaultLanguageForMaps );
			}
			output.Description = AssignLanguageMap( ConvertSpecialCharacters( input.Description ), input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

			//is this required?
			output.RequiredNumber = input.RequiredNumber;

			output.TargetComponent = AssignRegistryResourceURIsListAsStringList( input.TargetComponent, "Pathway Component TargetComponent", ref messages, false );

			return isValid;
		}

		/// <summary>
		/// Type of PathwayComponent. 
		/// Valid values (with or without ceterms:) :
		/// ceterms:AssessmentComponent	
		/// ceterms:BasicComponent	
		/// ceterms:CocurricularComponent	
		/// ceterms:CompetencyComponent	
		/// ceterms:CourseComponent 	
		/// ceterms:CredentialComponent 	
		/// ceterms:ExtracurricularComponent 	
		/// ceterms:JobComponent 	
		/// ceterms:WorkExperienceComponent
		/// </summary>
		bool ValidatePathwayComponentType( string type, ref string outputType, ref List<string> messages )
		{
			bool isValid = true;
			switch ( type.ToLower() )
			{
				case "assessmentcomponent":
				case "ceterms:assessmentcomponent":
					outputType = "ceterms:AssessmentComponent";
					break;
				case "basiccomponent":
				case "ceterms:basiccomponent":
					outputType = "ceterms:BasicComponent";
					break;
				case "cocurricularcomponent":
				case "ceterms:cocurricularcomponent":
					outputType = "ceterms:CocurricularComponent";
					break;
				case "competencycomponent":
				case "ceterms:competencycomponent":
					outputType = "ceterms:CompetencyComponent";
					break;
				case "coursecomponent":
				case "ceterms:coursecomponent":
					outputType = "ceterms:CourseComponent";
					break;
				case "credentialcomponent":
				case "ceterms:credentialcomponent":
					outputType = "ceterms:CredentialComponent";
					break;
				case "extracurricularcomponent":
				case "ceterms:extracurricularcomponent":
					outputType = "ceterms:ExtracurricularComponent";
					break;
				case "jobcomponent":
				case "ceterms:jobcomponent":
					outputType = "ceterms:JobComponent";
					break;
				case "workexperiencecomponent":
				case "ceterms:workexperiencecomponent":
					outputType = "ceterms:WorkExperienceComponent";
					break;
				default:
				{
					isValid = false;
					messages.Add( string.Format("Error: {0} is not a valid PathwayComponent Type ", type ));
					break;
				}
			}

			return isValid;
		}

	}
}
