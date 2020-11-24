using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using RA.Models.Input;
using RJ = RA.Models.JsonV2;
using PathwaySetRequest = RA.Models.Input.PathwaySetRequest;
using PathwaySetOutputEntity = RA.Models.JsonV2.PathwaySet;
//
using EntityRequest = RA.Models.Input.PathwayRequest;
using InputEntity = RA.Models.Input.Pathway;
using OutputEntity = RA.Models.JsonV2.Pathway;
using OutputGraph = RA.Models.JsonV2.GraphContainer;
using OutputComponent = RA.Models.JsonV2.PathwayComponent;
using OutputCondition = RA.Models.JsonV2.ComponentCondition;

using CER = RA.Services.RegistryServices;
using Utilities;
using System.Security.Policy;

namespace RA.Services
{
	public class PathwayServices : ServiceHelperV2
	{
		static string status = "";
		List<OutputComponent> outputComponents = new List<OutputComponent>();
		List<OutputCondition> outputConditions = new List<OutputCondition>();

		public string AssessmentComponent = "ceterms:AssessmentComponent";
		public string BasicComponent = "ceterms:BasicComponent";
		public string CocurricularComponent = "ceterms:CocurricularComponent";
		public string CompetencyComponent = "ceterms:CompetencyComponent";
		public string CourseComponent = "ceterms:CourseComponent";
		public string CredentialComponent = "ceterms:CredentialComponent";
		public string ExtracurricularComponent = "ceterms:ExtracurricularComponent";
		public string JobComponent = "ceterms:JobComponent";
		public string SelectionComponent = "ceterms:SelectionComponent";
		public string WorkExperienceComponent = "ceterms:WorkExperienceComponent";

		#region Pathway set
		public void PublishPathwaySet(PathwaySetRequest request, ref bool isValid, RA.Models.RequestHelper helper)
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			List<string> messages = new List<string>();
			var output = new PathwaySetOutputEntity();
			OutputGraph og = new OutputGraph();
			/*
			 * By this point all referenced pathways will have been published, or were already published.
			 */
			if ( ToMap( request, output, ref messages ) )
			{
				og.Graph.Add( output );
				//not likely
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

				CER cer = new CER( "PathwaySet", output.Type, output.CTID, helper.SerializedInput )
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
						LoggingHelper.DoTrace( 4, string.Format( "PathwaySet.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				if ( !SupportServices.ValidateAgainstPastRequest( "PathwaySet_", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
				}
				else
				{
					string identifier = "PathwaySet_" + request.PathwaySet.Ctid;

					if ( cer.Publish( helper, "", identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published PathwaySet: {0}</p><p>CTID: {1}</p> <p>EnvelopeId: {2}</p> ", output.Name, output.CTID, crEnvelopeId );
						NotifyOnPublish( "PathwaySet", msg );
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

				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
			}


		
			helper.SetWarningMessages( warningMessages );
			helper.SetMessages( messages );
		}
		public bool ToMap( PathwaySetRequest request, PathwaySetOutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "PathwaySetSet";
			bool isValid = true;
			Community = request.Community ?? "";

			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			PathwaySet input = request.PathwaySet;
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
				//required
				CurrentCtid = output.CTID = FormatCtid( input.Ctid, "PathwaySet Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

				//required
				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "PathwaySet.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				CurrentEntityName = GetFirstItemValue( output.Name );

				output.Description = AssignLanguageMap( input.Description, input.Description_Map, "PathwaySet Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

				output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "PathwaySet.Subject Webpage", ref messages, false );

				output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "PathwaySet.Owning Organization", false, ref messages, false, true );
				//output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", false, ref messages );
				output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "PathwaySet.Offered By", false, ref messages, false, true );

				if ( output.OwnedBy == null && output.OfferedBy == null )
				{
					messages.Add( string.Format( "Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for PathwaySet: '{0}'", input.Name ) );
				}

				//

				//TBD - if no pathway are included, then there must be a list of ctids/uris in HasPathway
				//these must exist. Could update assign to optionally check for existance, or do after
				output.HasPathway = AssignRegistryResourceURIsListAsStringList( input.HasPathway, "PathwaySet.HasPathway", ref messages, false, true );
				//validate
				var ctdlType = "";
				var statusMessage = "";
				int cntr = 0;
				//at this point all pathways must exist
				//could have an optimization to skip this step if all pathways were part of the upload.
				//	- however if one or more failed, we would to stop, although we should not get this far.
				foreach (var item in output.HasPathway)
				{
					cntr++;
					var payload = RegistryServices.GetResourceByUrl( item, ref ctdlType, ref statusMessage );
					if (string.IsNullOrWhiteSpace( payload))
					{
						messages.Add( string.Format( "Error - pathway: #{0} '{1}' of PathwaySet.HasPathway was not found in the registry. All referenced pathways must exist in the registry before publishing a PathwaySet.", cntr, item ) );
					} else if (ctdlType != "Pathway" )
					{
						messages.Add( string.Format( "Error - pathway entry: #{0} '{1}' of PathwaySet.HasPathway does not reference a pathway. It references a '{2}'", cntr, item, ctdlType ) );
					}
				}
				
				
				//

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "PathwaySetServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}

		#endregion

		#region Pathway
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

					if ( cer.Publish( helper, "", identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
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
				//required

				CurrentCtid = output.CTID = FormatCtid( input.Ctid, "Pathway Profile", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);

				//required
				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Pathway.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				//CurrentEntityName = GetFirstItemValue( output.Name );
				CurrentEntityName = !string.IsNullOrWhiteSpace( input.Name ) ? input.Name : GetFirstItemValue( output.Name );
				output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Pathway.Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

				output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Pathway.Subject Webpage", ref messages, false );

				output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Pathway.Owning Organization", false, ref messages, false, true );
				//output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", false, ref messages );
				output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Pathway.Offered By", false, ref messages, false, true );

				if ( output.OwnedBy == null && output.OfferedBy == null )
				{
					messages.Add( string.Format( "Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Pathway: '{0}'", input.Name ) );
				}

				//
				if ( input.HasChild == null || input.HasChild.Count == 0 )
				{
					//this is not strictly required, unless there are any pathway components
					if ( request.PathwayComponents != null && request.PathwayComponents.Count > 0 )
					{
						//messages.Add( "Where PathwayComponents are provided, there must be at least one included in the Pathway HasChild property." );
					}
				}
				else
				{
					//it would be a burden to have a user provide a blank node Id.
					output.HasChild = AssignRegistryResourceURIsListAsStringList( input.HasChild, "Pathway.HasChild", ref messages, false, true );
					//validated below with HasPart
				}
				//TBD - should this be required if there are any components?
				output.HasDestinationComponent = AssignRegistryResourceURIsListAsStringList( input.HasDestinationComponent, "Pathway.HasDestinationComponent", ref messages, false, true);
				if ( output.HasDestinationComponent ==null && request.PathwayComponents != null && request.PathwayComponents.Count > 0 )
				{
					messages.Add( "Where PathwayComponents are provided, there must be at least one included in the Pathway HasDestinationComponent property." );
				} else
				{
					//validate
					int partCntr = 0;
					foreach ( var item in input.HasDestinationComponent )
					{
						partCntr++;
						if ( !string.IsNullOrWhiteSpace( item ) && item.StartsWith( "ce-" ) && item.Length == 39 )
						{
							//then must exist in components
							var exists = request.PathwayComponents.FirstOrDefault( a => a.Ctid.ToLower() == item.ToLower() );
							if ( exists == null || string.IsNullOrWhiteSpace( exists.Ctid ) )
							{
								messages.Add( string.Format( "In Pathway.HasDestinationComponent #{0}, the CTID: '{1}' was not found in the PathwayComponents. ", partCntr, item ) );
							}
						}
					}
				}
				//is this always a url, or can it just be a namespace concept
				output.HasProgressionModel = AssignRegistryResourceURIAsString( input.HasProgressionModel, "Pathway.HasProgressionModel", ref messages);				
				//

				output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Pathway.Keywords", ref messages );
				output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject, input.Subject_Map, "Pathway Subjects" );
				//frameworks
				//=== occupations ===============================================================
				//can't depend on the codes being SOC
				output.OccupationType = FormatCredentialAlignmentListFromFrameworkItemList( input.OccupationType, true, "Pathway.OccupationType", ref messages );
				//just used for simple list of strings. append to OccupationType
				output.OccupationType = AppendCredentialAlignmentListFromList( input.AlternativeOccupationType, null, "", "", "Pathway.AlternativeOccupationType", output.OccupationType, ref messages );

				//NEW - allow a list of Onet codes, and resolve
				output.OccupationType = HandleListOfONET_Codes( input.ONET_Codes, output.OccupationType, ref messages );


				//=== industries ===============================================================
				//can't depend on the codes being NAICS??
				output.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( input.IndustryType, true, "Pathway.IndustryType", ref messages );

				//append to IndustryType
				output.IndustryType = AppendCredentialAlignmentListFromList( input.AlternativeIndustryType, null, "", "", "Pathway.AlternativeIndustryType", output.IndustryType, ref messages );
				//NEW - allow a list of Naics, and resolve
				output.IndustryType = HandleListOfNAICS_Codes( input.NaicsList, output.IndustryType, ref messages );
				//
				//
				#region  Populate the components
				//may need to make this conditional
				if ( request.PathwayComponents == null || request.PathwayComponents.Count == 0 )
				{
					//not required
					//messages.Add( "At least one PathwayComponent must be included with a Pathway request." );
				}
				else
				{
					bool generatingHasPart = false;
					if ( input.HasPart != null && input.HasPart.Count > 0 )
					{
						//actually just assign all and then validate
						output.HasPart = AssignRegistryResourceURIsListAsStringList( input.HasPart, "Pathway HasPart", ref messages, false, true );
						//or should validate
						int partCntr = 0;
						foreach ( var item in input.HasPart )
						{
							partCntr++;
							if ( !string.IsNullOrWhiteSpace( item ) && item.StartsWith( "ce-" ) && item.Length == 39 )
							{
								//then must exist in components
								var exists = request.PathwayComponents.FirstOrDefault( a => a.Ctid.ToLower() == item.ToLower() );
								if ( exists == null && string.IsNullOrWhiteSpace( exists.Ctid ) )
								{
									messages.Add( string.Format( "In Pathway.HasPart #{0}, the CTID: '{1}' was not found in the PathwayComponents. ", partCntr, item ) );
								}
							}
						}
					}
					else
					{
						//just generate while handling components
						generatingHasPart = true;
					}

					//assigned previously, now validate
					int childCntr = 0;
					foreach (var item in input.HasChild )
					{
						childCntr++;
						if ( !string.IsNullOrWhiteSpace( item ) && item.StartsWith( "ce-" ) && item.Length == 39 )
						{
							//then must exist in components
							var exists = request.PathwayComponents.FirstOrDefault( a => a.Ctid.ToLower() == item.ToLower() );
							if ( exists == null && string.IsNullOrWhiteSpace(exists.Ctid))
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
						if ( ToMapComponent( request, item, component, output, hasDefaultLanguage, compCntr, ref messages ) )
						{
							//additional validations for 
							//hasChild
							//isChildOf
							//prerequisite
							//preceeds
							outputComponents.Add( component );
							if ( generatingHasPart )
							{
								//don't have ctdlid ??
								output.HasPart.Add( component.CtdlId );
							}
						}
					}
				}
				#endregion
				//


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


		public bool ToMapComponent( EntityRequest request, RA.Models.Input.PathwayComponent input, OutputComponent output, OutputEntity outputEntity, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			//
			output.CTID = FormatCtid( input.Ctid, string.Format( "PathwayComponent (#{0})", compCntr ), ref messages );
			output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.CTID, Community );

			string outputType = "";

			string displayName = "missing";
			output.Name = AssignLanguageMap( input.Name, input.Name_Map, string.Format( "{0} Name (#{1})", outputType, compCntr ), DefaultLanguageForMaps, ref messages, true, 2 );
			if ( output.Name != null )
				displayName = output.Name.ToString();
			//
			//validate component type. Need to add name to identify errors
			if ( ValidatePathwayComponentType( input.PathwayComponentType, "PathwayComponent: " + displayName, ref outputType, ref messages ) )
				output.PathwayComponentType = outputType;

			bool isComponentDescriptionRequired = UtilityManager.GetAppKeyValue( "isComponentDescriptionRequired", true );
			bool isComponentSubjectWebpageOrSourceRequired = UtilityManager.GetAppKeyValue( "isComponentSubjectWebpageOrSourceRequired", true );
			bool isSelectionComponent = false;
			if ( output.PathwayComponentType == SelectionComponent )
			{
				isComponentDescriptionRequired = false;
				isComponentSubjectWebpageOrSourceRequired = false;
				isSelectionComponent = true;
			}

			//required???
			
			output.Description = AssignLanguageMap( input.Description, input.Description_Map, string.Format( "Component Description (#{0})", compCntr ), DefaultLanguageForMaps, ref messages, isComponentDescriptionRequired, MinimumDescriptionLength );

			if ( input.IsPartOf != null && input.IsPartOf.Count > 0 )
			{
				//should validate? May be case where can reference an external pathway!
				output.IsPartOf = AssignRegistryResourceURIsListAsStringList( input.IsPartOf, "Pathway Component IsPartOf", ref messages );
			} //otherwise generate
			else
			{
				output.IsPartOf.Add( outputEntity.CtdlId );
			}

			//TBD only coded notation or identifier value
			output.CodedNotation = input.CodedNotation;
			//TBD
			output.Identifier = AssignIdentifierListToList( input.Identifier, ref messages );
			//
			output.ComponentDesignation = FormatCredentialAlignmentVocabs( "componentDesignation", input.ComponentDesignation, ref messages );
			//
			output.HasChild = AssignRegistryResourceURIsListAsStringList( input.HasChild, "Pathway Component HasChild", ref messages, false );
			//validate
			if ( output.HasChild != null && output.HasChild.Count > 0 )
			{
				ValidateAPathwayComponent( request, input.HasChild, string.Format( "{0}-{1}.HasChild", outputType, displayName ), ref messages );
				//int childCntr = 0;
				//foreach ( var item in input.HasChild )
				//{
				//	childCntr++;
				//	if ( !string.IsNullOrWhiteSpace( item ) && item.StartsWith( "ce-" ) && item.Length == 39 )
				//	{
				//		//then must exist in components
				//		var exists = request.PathwayComponents.FirstOrDefault( a => a.Ctid.ToLower() == item.ToLower() );
				//		if ( exists == null && string.IsNullOrWhiteSpace( exists.Ctid ) )
				//		{
				//			messages.Add( string.Format( "In {0}-{1}.HasChild #{2}, the CTID: '{3}' was not found in the PathwayComponents. ", outputType, displayName, childCntr, item ) );
				//		}
				//	}
				//}
			}
			//
			output.IsChildOf = AssignRegistryResourceURIsListAsStringList( input.IsChildOf, "Pathway Component IsChildOf", ref messages, false );
			//validate
			if ( output.IsChildOf != null && output.IsChildOf.Count > 0 )
			{
				//this can also be a child of the pathway - check it first
				int partCntr = 0;
				bool childOfPathway = false;
				foreach ( var c in input.IsChildOf )
				{
					partCntr++;
					if ( !string.IsNullOrWhiteSpace( c ) && c.StartsWith( "ce-" ) && c.Length == 39 )
					{
						//if child of pathway, should have any oher child of!
						if ( c == outputEntity.CTID )
						{
							childOfPathway = true;
							break;
						}
					}
				}
				if (!childOfPathway )
					ValidateAPathwayComponent( request, input.IsChildOf, string.Format( "{0}-{1}.IsChildOf", outputType, displayName ), ref messages );

			}
			//
			//must be a CTID or URI
			output.HasProgressionLevel = AssignRegistryResourceURIsListAsStringList( input.HasProgressionLevel, "Pathway Component HasProgressionLevel", ref messages, false );
			//
			output.IsDestinationComponentOf = AssignRegistryResourceURIsListAsStringList( input.IsDestinationComponentOf, "Pathway Component IsDestinationComponentOf", ref messages, false );
			//validate - this should be a method!!!!!
			//this should check pathway only
			if ( output.IsDestinationComponentOf != null && output.IsDestinationComponentOf.Count > 0 )
			{
				//ValidateAPathwayComponent( request, input.IsDestinationComponentOf, string.Format( "{0}-{1}.IsDestinationComponentOf", outputType, displayName ), ref messages );

				int partCntr = 0;
				foreach ( var c in input.IsDestinationComponentOf )
				{
					partCntr++;
					if ( !string.IsNullOrWhiteSpace( c ) && c.StartsWith( "ce-" ) && c.Length == 39 )
					{
						if ( c != outputEntity.CTID )
						{
							messages.Add( string.Format( "In {0} #{1}, the CTID: '{2}' does not match the Pathway. ", "IsDestinationComponentOf", partCntr, c ) );
						}
					}
				}
			}
			output.PointValue = AssignQuantitiveValue( input.PointValue, "PointValue", "PathwayComponent", ref messages );

			output.Preceeds = AssignRegistryResourceURIsListAsStringList( input.Preceeds, "Pathway Component Preceeds", ref messages, false );
			ValidateAPathwayComponent( request, input.Preceeds, string.Format( "{0}-{1}.Preceeds", outputType, displayName ), ref messages );
			//
			output.Prerequisite = AssignRegistryResourceURIsListAsStringList( input.Prerequisite, "Pathway Component Prerequisite", ref messages, false );
			ValidateAPathwayComponent( request, input.Prerequisite, string.Format( "{0}-{1}.Prerequisite", outputType, displayName ), ref messages );
			//===============
			//TBD - some doc suggests this is always a registry source, so be sure to handle just a ctid
			output.SourceData = AssignRegistryResourceURIAsString( input.SourceData, string.Format( "SourceData (#{0})", compCntr ), ref messages, false, false, true );
			output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, string.Format( "Subject Webpage (#{0})", compCntr ), ref messages, false );
			//
			
			if ( isComponentSubjectWebpageOrSourceRequired && string.IsNullOrWhiteSpace(output.SubjectWebpage) && string.IsNullOrWhiteSpace(output.SourceData))
			{
				//
				messages.Add( string.Format( "A Pathway Component must have either a SubjectWebpge or SourceData URL and none were provided for component: '{0}'. ", displayName ) );
			}
			//component specific properties
			//				CourseComponent
			//just ignore inappropriate or flag as error?
			//list
			output.CreditValue = AssignValueProfileToList( input.CreditValue, "CreditValue", "PathwayComponent", ref messages );
			if ( !string.IsNullOrWhiteSpace( input.ProgramTerm)  )
			{
				output.ProgramTerm = AssignLanguageMap( input.ProgramTerm, input.ProgramTerm_Map, "ProgramTerm", DefaultLanguageForMaps, ref messages, false, 2 );
				if ( output.ProgramTerm != null && output.ProgramTerm.ToString().Length > 1 && outputType != CourseComponent )
				{
					messages.Add( string.Format( "A ProgramTerm ( '{0}') may only be included for a 'CourseComponent' not a '{1}'.", output.ProgramTerm.ToString(), outputType ) );
				}					
			}
			//
			if ( output.CreditValue != null && output.CreditValue.Count > 0 && outputType != CourseComponent )
			{
				messages.Add( string.Format("A 'CreditValue' property may only be included for a 'CourseComponent' not a '{0}'. ", outputType) );
			}
			//				CredentialComponent
			if ( !string.IsNullOrWhiteSpace( input.CredentialType ) )
			{
				string validSchema = "";
				if ( !ValidationServices.IsValidCredentialType( input.CredentialType, ref validSchema, true ) )
				{
					messages.Add( FormatMessage( "Error - The credential type: ({0}) is invalid for PathwayComponent '{1}'.", input.CredentialType, input.Name ) );
				}
				else if ( outputType != CredentialComponent )
				{
					messages.Add( string.Format( "A CredentialType ( '{0}') may only be included for a 'CredentialComponent' not a '{1}'. ", input.CredentialType.ToString(), outputType ) );
				}
				else
					output.CredentialType = validSchema;
			}
			if ( outputType == CredentialComponent  && string.IsNullOrWhiteSpace(output.CredentialType) )
			{
				messages.Add( string.Format( "Error: A CredentialType must be included for the 'CredentialComponent': '{0}'. ", displayName) );
			}
			//				BasicComponent,	ceterms:CocurricularComponent, ceterms:ExtracurricularComponent 

			output.ComponentCategory = AssignLanguageMap( input.ComponentCategory, input.ComponentCategory_Map, "ComponentCategory", DefaultLanguageForMaps, ref messages, false, 2 );
			if ( output.ComponentCategory != null && output.ComponentCategory.ToString().Length > 1 )
			{
				if ( outputType != BasicComponent && outputType != CocurricularComponent && outputType != ExtracurricularComponent )
					messages.Add( string.Format( "A ComponentCategory ('{0}') may only be included for a BasicComponent, CocurricularComponent, or ExtracurricularComponent not a '{1}'.", output.ComponentCategory.ToString(), outputType ) );
			}

			#region  Populate the conditions
			if ( isSelectionComponent && ( input.HasCondition == null || input.HasCondition.Count() == 0 ))
			{
				messages.Add( string.Format( "Component Conditions (Has Condition) are required for a SelectionComponent: '{0}'.", displayName) );
			}

			
			OutputCondition condition = new OutputCondition();
			int conditionCntr = 0;
			//add each top component
			foreach ( var item in input.HasCondition )
			{
				conditionCntr++;
				if ( conditionCntr  == 1)
					output.HasCondition = new List<OutputCondition>();

				condition = new OutputCondition();

				if ( ToMapCondition( item, condition, output, hasDefaultLanguage, compCntr, ref messages ) )
				{
					output.HasCondition.Add( condition );

					if (item.TargetComponent != null && item.TargetComponent.Count() > 1)
					{
						ValidateAPathwayComponent( request, item.TargetComponent, string.Format( "{0}-{1} ComponentCondition.TargetComponent", outputType, displayName ), ref messages );
					}
				}
			}
			
			#endregion

			return isValid;
		}


		public bool ToMapCondition( RA.Models.Input.ComponentCondition input, OutputCondition output, OutputComponent outputEntity, bool hasDefaultLanguage, int compCntr, ref List<string> messages )
		{
			bool isValid = true;
			//change to use a blank node??
			//NOTE: must match the id that the parent references
			//output.CtdlId = GenerateBNodeId();// SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);
			string displayName = "missing";
			output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Condition.Name", DefaultLanguageForMaps, ref messages, true, 3 );
			if ( output.Name != null )
				displayName = output.Name.ToString();
			//
			output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Condition Description for Condition: " + displayName, DefaultLanguageForMaps, ref messages, false,  MinimumDescriptionLength );

			//is this required Yes, and must be > 1?
			int componentConditionMinRequiredNumber = UtilityManager.GetAppKeyValue( "componentConditionMinRequiredNumber", 1 );
			output.RequiredNumber = input.RequiredNumber;
			if ( output.RequiredNumber < componentConditionMinRequiredNumber )
			{
				messages.Add( string.Format( " As there must be two or more TargetComponents, the RequiredNumber ({0}) for the '{1}' of '{2}' must be {3} or more.", input.RequiredNumber, outputEntity.PathwayComponentType, outputEntity.Name.ToString(), componentConditionMinRequiredNumber ) );
			}
			//required and greater than 1
			output.TargetComponent = AssignRegistryResourceURIsListAsStringList( input.TargetComponent, "Pathway Component TargetComponent", ref messages, false );
			if ( output.TargetComponent == null || output.TargetComponent.Count < componentConditionMinRequiredNumber )
			{
				messages.Add( string.Format( "There must be {2} or more TargetComponents for the '{0}' of '{1}'.", outputEntity.PathwayComponentType, outputEntity.Name.ToString(), componentConditionMinRequiredNumber ) );
			}
			
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
		bool ValidatePathwayComponentType( string type, string property, ref string outputType, ref List<string> messages )
		{
			bool isValid = true;
			if (string.IsNullOrWhiteSpace(type))
			{
				messages.Add( string.Format( "Error: A valid PathwayComponent Type was not supplied for {0}.", property ) );
				return false;
			}
			switch ( type.ToLower().Replace(" ", "") )
			{
				case "assessmentcomponent":
				case "ceterms:assessmentcomponent":
					outputType = AssessmentComponent;
					break;
				case "basiccomponent":
				case "ceterms:basiccomponent":
					outputType = BasicComponent;
					break;
				case "cocurricularcomponent":
				case "ceterms:cocurricularcomponent":
					outputType = CocurricularComponent;
					break;
				case "competencycomponent":
				case "ceterms:competencycomponent":
					outputType = CompetencyComponent;
					break;
				case "coursecomponent":
				case "ceterms:coursecomponent":
					outputType = CourseComponent;
					break;
				case "credentialcomponent":
				case "ceterms:credentialcomponent":
					outputType = CredentialComponent;
					break;
				case "extracurricularcomponent":
				case "ceterms:extracurricularcomponent":
					outputType = ExtracurricularComponent;
					break;
				case "jobcomponent":
				case "ceterms:jobcomponent":
					outputType = JobComponent;
					break;
				case "selectioncomponent":
				case "ceterms:selectioncomponent":
					outputType = SelectionComponent;
					break;
				case "workexperiencecomponent":
				case "ceterms:workexperiencecomponent":
					outputType = WorkExperienceComponent;
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


		public void ValidateAPathwayComponent( EntityRequest request, List<string> list, string property, ref List<string> messages )
		{
			if ( list == null || list.Count == 0 )
				return;

			int partCntr = 0;
			foreach ( var item in list )
			{
				partCntr++;
				if ( !string.IsNullOrWhiteSpace( item ) && item.StartsWith( "ce-" ) && item.Length == 39 )
				{
					//then must exist in components
					var exists = request.PathwayComponents.FirstOrDefault( a => a.Ctid.ToLower() == item.ToLower() );
					if ( exists == null || string.IsNullOrWhiteSpace( exists.Ctid ) )
					{
						messages.Add( string.Format( "In {0} #{1}, the CTID: '{2}' was not found in the PathwayComponents. ", property, partCntr, item ) );
					}
				}
			}
		}

		#endregion

	}
}
