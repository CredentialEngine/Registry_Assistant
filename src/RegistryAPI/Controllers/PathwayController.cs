using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using RA.Models;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.PathwayRequest;
using PathwaySetRequest = RA.Models.Input.PathwaySetRequest;
using RA.Services;
using Mgr = RA.Services.PathwayServices;
using ServiceHelper = RA.Services.ServiceHelperV2;
using Utilities;

namespace RegistryAPI.Controllers
{
    public class PathwayController : BaseController
	{
		string statusMessage = "";
		readonly string thisClassName = "PathwayController";
		readonly string controllerEntity = "pathway";
		RA.Models.RequestHelper helper = ServiceHelper.InitializeRequestHelper();
		Mgr mgr = new Mgr();
		ServiceHelper serviceHelper = new ServiceHelper();

		/// <summary>
		/// Request to format a Pathway and all related components.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[ HttpPost, Route( "Pathway/format" )]
		public RegistryAssistantFormatResponse Format( EntityRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				if ( request == null || request.Pathway == null )
				{
					response.Messages.Add( "Error - please provide a valid Pathway request." );
					return response;
				}

				ServiceHelper.LogInputFile( request, request.Pathway.CTID, "Pathway", "Format" );

				response.Payload = new PathwayServices().FormatAsJson( request, ref isValid, ref messages );
				new SupportServices().AddActivityForFormat( helper, "Pathway", mgr.CurrentEntityName, request.Pathway.CTID, ref statusMessage );

				response.Successful = isValid;

				if ( !isValid )
				{
					response.Messages = messages;
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		} //

		/// <summary>
		/// Request to publish a Pathway and all related components.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "Pathway/publish" )]
		public RegistryAssistantResponse Publish( EntityRequest request )
		{
			bool isValid = true;
			var response = new RegistryAssistantResponse();
			List<string> messages = new List<string>();
			try
			{
				if ( request == null || request.Pathway == null  )
				{
					response.Messages.Add( "Error - please provide a valid Pathway request with a Pathway." );
					return response;
				}
				if ( string.IsNullOrWhiteSpace( request.PublishForOrganizationIdentifier ) )
				{
					response.Messages.Add( "Error - please provide a valid CTID for PublishForOrganizationIdentifier." );
					return response;
				}
				if ( !ServiceHelper.IsCtidValid( request.Pathway.CTID, "Pathway CTID", ref messages ) )
				{
					//response.Messages.Add( "Error - please provide a valid CTID for the Pathway." );
					//return response;
				}
				if ( !ServiceHelper.IsCtidValid( request.PublishForOrganizationIdentifier, "Pathway PublishForOrganizationIdentifier", ref messages ) )
				{
					//response.Messages.Add( "Error - please provide a valid CTID for the PublishForOrganizationIdentifier." );
					//return response;
				}
				if ( messages.Count > 0 )
				{
					response.Messages.AddRange( messages );
					return response;
				}

				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish. IPaddress: {1}, ctid: {2}, envelopeId: {3}", thisClassName, ServiceHelper.GetCurrentIP(), request.Pathway.CTID, request.RegistryEnvelopeId ) );
				helper = new RequestHelper
				{
					OwnerCtid = request.PublishForOrganizationIdentifier
				};
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
				}
				else
				{
					helper.SerializedInput = ServiceHelper.LogInputFile( request, request.Pathway.CTID, "Pathway", "Publish", 5 );
					string originalCTID = request.Pathway.CTID ?? "";

					new PathwayServices().Publish( request, ref isValid, helper );

					response.CTID = request.Pathway.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse(helper, response);
					}
					else
					{
						response.Messages = helper.GetAllMessages();
					}
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		} //

		/// <summary>
		/// Delete request of an Pathway by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "pathway/delete" )]
		public RegistryAssistantDeleteResponse Delete( DeleteRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			try
			{
				if ( request == null || request.CTID == null )
				{
					response.Messages.Add( "Error - please provide a valid delete request." );
					return response;
				}
				RegistryServices cer2 = new RegistryServices( controllerEntity, "", request.CTID );
				isValid = cer2.DeleteRequest( request, controllerEntity, ref messages );
				if ( isValid )
				{
					response.Successful = true;
				}
				else
				{
					response.Messages.AddRange( messages );
					response.Successful = false;
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		} //

		/// <summary>
		/// Format is not available for a PathwaySet
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "PathwaySet/format" )]
		public List<RegistryAssistantFormatResponse> PathwaySetFormat( PathwaySetRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var responses = new List<RegistryAssistantFormatResponse>();
			var response = new RegistryAssistantFormatResponse();

			try
			{
				messages.Add( "NOTE: the format endpoint for a Pathway Set is not available at this time." );
				if ( messages.Count > 0 )
				{
					response.Messages.AddRange( messages );
					responses.Add( response );
					return responses;
				}
				//-----------------------------
				if ( request == null || request.PathwaySet == null
					|| request.PathwaySet.HasPathway == null || request.PathwaySet.HasPathway.Count < 2 )
				{
					response.Messages.Add( "Error - please provide a valid PathwaySet request with a PathwaySet and HasPathway that references at least two pathways." );
					responses.Add( response );
					return responses;
				}
				if ( !ServiceHelper.IsCtidValid( request.PathwaySet.CTID, "PathwaySet CTID", ref messages ) )
				{
					messages.Add( "Error - please provide a valid CTID for the PathwaySet." );
					//return response;
				}
				if ( !ServiceHelper.IsCtidValid( request.PublishForOrganizationIdentifier, "PathwaySet PublishForOrganizationIdentifier", ref messages ) )
				{
					messages.Add( "Error - please provide a valid CTID for the PublishForOrganizationIdentifier." );
					//return response;
				}
				if ( messages.Count > 0 )
				{
					response.Messages.AddRange( messages );
					responses.Add( response );
					return responses;
				}
				//---------
				//ServiceHelper.LogInputFile( request, request.Pathway.Ctid, "Pathway", "Format" );

				//response.Payload = new PathwayServices().FormatAsJson( request, ref isValid, ref messages );
				response.Successful = isValid;

				if ( !isValid )
				{
					response.Messages = messages;
				}
				responses.Add( response );
				
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
				responses.Add( response );
			}
			return responses;
		} //

		/// <summary>
		/// Request to publish a Pathway Set
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost, Route( "PathwaySet/publish" )]
		public List<RegistryAssistantResponse> PublishSet(PathwaySetRequest request)
		{
			bool isValid = true;
			var responses = new List<RegistryAssistantResponse>();
			var response = new RegistryAssistantResponse();
			List<string> messages = new List<string>();
			try
			{
				//could publish a pathway set that just reference existing pathways
				//PathwaySet.HasPathway must always exist - not doing helper to generate
				//|| request.Pathways == null || request.Pathways.Count < 2
				if ( request == null || request.PathwaySet == null 
					|| request.PathwaySet.HasPathway == null || request.PathwaySet.HasPathway.Count < 2 )
				{
					//response.Messages.Add( "Error - please provide a valid PathwaySet request with a PathwaySet and HasPathway that references at least two pathways." );
					responses.Add( new RegistryAssistantResponse()
					{
						Messages = new List<string>()
						{ "Error - please provide a valid PathwaySet request with a PathwaySet and HasPathway that references at least two pathways." }
						} 
					);
					return responses;
				}
				if ( !ServiceHelper.IsCtidValid( request.PathwaySet.CTID, "PathwaySet CTID", ref messages ) )
				{
					messages.Add( "Error - please provide a valid CTID for the PathwaySet." );
					//return response;
				}
				if ( !ServiceHelper.IsCtidValid( request.PublishForOrganizationIdentifier, "PathwaySet PublishForOrganizationIdentifier", ref messages ) )
				{
					messages.Add( "Error - please provide a valid CTID for the PublishForOrganizationIdentifier." );
					//return response;
				}
				if (messages.Count > 0)
				{
					response.Messages.AddRange( messages );
					responses.Add( response );
					return responses;
				}

				//
				LoggingHelper.DoTrace( 2, string.Format( "RegistryAssistant.{0}.Publish PathwaySet. IPaddress: {1}, ctid: {2}.", thisClassName, ServiceHelper.GetCurrentIP(), request.PathwaySet.CTID ) );
				statusMessage = "";
				helper = new RequestHelper
				{
					OwnerCtid = request.PublishForOrganizationIdentifier
				};
				if ( !new AuthorizationServices().ValidateRequest( helper, ref statusMessage ) )
				{
					response.Messages.Add( statusMessage );
					return responses;
				}

				//=============================================================
				//need to format the pathway set. Only publish if at least one pathway publishes
				//or could use the format method and get list of payloads ready to publish
				//	All or none??
				bool hasPathwaysExists = true;
				var allowingHybridWithPathwaySet = UtilityManager.GetAppKeyValue( "allowingHybridWithPathwaySet", false );
				//PathwaySet.HasPathway must always exist - not doing helper to generate
				//Considered only allowing HasPathway withhout any pathways included (references must have aleady been published) or all references must be included. OR should we allow a hybrid/mixture
				//	YES - allow hybrid
				var pathwayList = new List<string>();
				int cntr = 0;
				int pathwayErrors = 0;
				mgr = new Mgr();
				//there could just be CTIDs in HasPathway - actually will always be
				if ( request.Pathways == null || request.Pathways.Count == 0 )
				{
					//if no pathways then all members of HasPathway must exist
					//the validation is done in PathwayServices.PublishPathwaySet.ToMap
					foreach ( var pw in request.PathwaySet.HasPathway )
					{

					}
				}
				else
				{
					//each member of Pathway must exist in HasPathway. Will need to handle URIs.
					//It is possible to have an entry from HasPathway that is not in the Pathways list.
					if ( !allowingHybridWithPathwaySet )
					{
						int partCntr = 0;
						//check that each ctid in HasPathway exists in Pathways
						foreach ( var item in request.PathwaySet.HasPathway )
						{
							partCntr++;
							var ctid = item;
							//need to check if a ctid. if not, then extract
							if ( !ServiceHelper.IsCtidValid( ctid, "HasPathways.CTID", ref messages, false ) )
							{
								//must be a url that contains a ctid
								int pos = ctid.IndexOf( "/ce-" );
								if (pos == -1)
								{
									messages.Add( string.Format("Error: Entry #{0} of HasPathway: '{1}' must be a CTID, or contain a valid CTID. ", partCntr, ctid ));
									continue;
								}
								ctid = ctid.Substring( ctid.IndexOf( "ce-" ) );
							}
							var exists = request.Pathways.Where( a => a.Pathway.CTID.ToLower() == ctid.ToLower() ).ToList();
							if ( exists == null || exists.Count == 0 )
							{
								messages.Add( string.Format( "In PathwaySet.HasPathway #{0}, the CTID: '{1}' was not found in the Pathways list. ", partCntr, ctid ) );
							}
						}//
						// check
						if ( request.PathwaySet.HasPathway.Count != request.Pathways.Count )
						{
							messages.Add( string.Format( "The number ({0}) of items in PathwaySet.HasPathway is different from the number ({1}) of items in request.Pathways. ", request.PathwaySet.HasPathway.Count, request.Pathways.Count ) );
						}//
					}
					//
					//regardless all pathway elements must exist in HasPathway
					cntr = 0;
					foreach ( var item in request.Pathways )
					{
						cntr++;
						if (item.Pathway == null || string.IsNullOrWhiteSpace( item.Pathway.CTID ) )
						{
							messages.Add( string.Format( "Error: Entry #{0} of Request.Pathway: '{1}' does not have a valid CTID. ", cntr, item.Pathway.Name ?? "PATHWAY NAME IS MISSING." ) );
							continue;
						}
						int index = request.PathwaySet.HasPathway.FindIndex( a => a.ToLower() == item.Pathway.CTID.ToLower() );
						if ( index < 0 )
						{
							messages.Add( string.Format( "In PathwaySet.Pathway #{0}, the CTID: '{1}' was not found in the HasPathways list", cntr, item.Pathway.CTID ));
						}
					}//

					if ( messages.Count > 0 )
					{
						response.Messages.AddRange( messages );
						responses.Add( response );
						return responses;
					}
					//=========================================================
					cntr = 0;
					foreach ( var pwr in request.Pathways )
					{
						cntr++;
						response = new RegistryAssistantResponse();
						//may not want to do a new - in ValidateRequest sets whether request is from the publisher
						helper.ResetHelper();
						if ( pwr == null || pwr.Pathway == null || string.IsNullOrWhiteSpace( pwr.Pathway.CTID ) )
						{
							response.Messages.Add( string.Format( "Pathway #{0}. Error - please provide a valid Pathway request with a Pathway and pathway CTID.", cntr ));
							responses.Add( response );
							continue;
						}
						if ( !ServiceHelper.IsCtidValid( pwr.Pathway.CTID, "Pathway CTID", ref messages ) )
						{
							response.Messages.Add( string.Format( "Pathway #{0}. Error - please provide a valid CTID (not: '{1}') for the Pathway.", cntr, pwr.Pathway.CTID ) );
							responses.Add( response );
							continue;
						}
						//check for just ctid??

						helper.SerializedInput = ServiceHelper.LogInputFile( pwr, pwr.Pathway.CTID, "Pathway", "PathwaySet_Publish", 5 );
						//
						new PathwayServices().Publish( pwr, ref isValid, helper );

						response.Payload = helper.Payload;
						response.Successful = isValid;
						if ( isValid )
						{
							if ( helper.Messages.Count > 0 )
								response.Messages = helper.GetAllMessages();
							if ( !hasPathwaysExists )
								request.PathwaySet.HasPathway.Add( pwr.Pathway.CTID );
							//maybe use separate list to xedit
							pathwayList.Add( pwr.Pathway.CTID );

							UpdateResponse( helper, response );
						}
						else
						{
							pathwayErrors++;
							response.Messages = helper.GetAllMessages();
						}
						responses.Add( response );
					} //
				}
				//TBD
				response = new RegistryAssistantResponse();
				if ( pathwayErrors == 0 )
				{
					//could just have a list of CTIDs in HasPathways referencing previously published pathways or a list of PathwayRequests
					new PathwayServices().PublishPathwaySet( request, ref isValid, helper );

					response.CTID = request.PathwaySet.CTID.ToLower();
					response.Payload = helper.Payload;
					response.Successful = isValid;
					if ( isValid )
					{
						if ( helper.Messages.Count > 0 )
							response.Messages = helper.GetAllMessages();
						UpdateResponse( helper, response );
					}
					else
					{
						response.Messages = helper.GetAllMessages();
					}

				} else
				{
					//note that set was not published due to at least one pathway failing
					response.Messages.Add( string.Format( "PathwaySet was not published because of issues with {0} pathway(s). See previous list of errors.", pathwayErrors ) );
				}

				responses.Add( response );
				
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
				responses.Add( response );
			}
			return responses;
		} //

		/// <summary>
		/// Delete request of an Pathway by CTID and owning organization
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpDelete, Route( "pathwayset/delete" )]
		public RegistryAssistantDeleteResponse PathwaySetDelete( DeleteRequest request )
		{
			bool isValid = true;
			List<string> messages = new List<string>();
			var response = new RegistryAssistantDeleteResponse();
			try
			{
				if ( request == null || request.CTID == null )
				{
					response.Messages.Add( "Error - please provide a valid delete request." );
					return response;
				}
				RegistryServices cer2 = new RegistryServices( controllerEntity, "", request.CTID );
				isValid = cer2.DeleteRequest( request, controllerEntity, ref messages );
				if ( isValid )
				{
					response.Successful = true;
				}
				else
				{
					response.Messages.AddRange( messages );
					response.Successful = false;
				}
			}
			catch ( Exception ex )
			{
				response.Messages.Add( ex.Message );
				response.Successful = false;
			}
			return response;
		} //


	}
}
