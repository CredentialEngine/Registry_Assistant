using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using CER = RA.Services.RegistryServices;
using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.OrganizationRequest;
using InputEntity = RA.Models.Input.Organization;
using OutputEntity = RA.Models.JsonV2.Agent;
using OutputGraph = RA.Models.JsonV2.GraphContainer;

using Utilities;

namespace RA.Services
{
	public class OrganizationServicesV2 : ServiceHelperV2
	{
		static string status = "";
		static bool isUrlPresent = true;
		string thisClassName = "OrganizationServicesV2";
		public static string CredentialOrganization = "ceterms:CredentialOrganization";
		public static string QACredentialOrganization = "ceterms:QACredentialOrganization";
        /// <summary>
        /// Publish a Learning Opportunity to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        public void Publish(EntityRequest request, string apiKey, ref bool isValid, ref List<string> messages, ref string payload, ref string registryEnvelopeId)
        {
            RA.Models.RequestHelper helper = new Models.RequestHelper();
            helper.RegistryEnvelopeId = registryEnvelopeId;
            helper.ApiKey = apiKey;
            helper.OwnerCtid = request.PublishForOrganizationIdentifier;

            Publish(request, ref isValid, helper);

            payload = helper.Payload;
            messages = helper.GetAllMessages();
            registryEnvelopeId = helper.RegistryEnvelopeId;
        }
        /// <summary>
        ///Publish an organization to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="helper"></param>
        public void Publish(EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper)
        {
            isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;
            //submitter is not a person for this api, rather the organization
            //may want to do a lookup via the api key?
            string submitter = "";

            var output = new OutputEntity();
			//if ( environment != "production" )
				//output.LastUpdated = DateTime.Now.ToUniversalTime().ToString( "yyyy-MM-dd HH:mm:ss UTC" );
			OutputGraph og = new OutputGraph();
            List<string> messages = new List<string>();
            if (ToMap(request, output, helper, ref messages ))
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
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = ctdlContext;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
				bool isTrustedPartner = false;

				CER cer = new CER( "Organization", output.Type, output.Ctid, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					PublishingForOrgCtid = helper.OwnerCtid
                };
				//
				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
				{
					cer.IsManagedRequest = true;
					//get publisher org
					string publisherCTID = "";
					
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages, ref isTrustedPartner, false ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;
					}
					else
					{
						//should be an error message returned
						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "OrganizationServices.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.Ctid, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				} else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				//also return if previously published
				//if first time for an organization, and publisher is a trusted partner, check if org is in accounts
				bool recordWasFound = false;
				if ( !SupportServices.ValidateAgainstPastRequest( "Organization", output.Ctid, ref cer, ref messages, ref recordWasFound ) )
				{
					isValid = false;
					//helper.SetMessages( messages );
					//return; //===================
				}
				else
				{
					if ( !recordWasFound && isTrustedPartner )
					{
						//check if org exists in accounts, and create if not
						if (!HandleFirstPublishForTrustedPartner( request, cer.PublisherAuthorizationToken, ref messages ))
						{
							//probably should return 
							helper.AddError( "This organization doesn't exist in the CE account system, and cannot be added due to the following error(s)." );
							helper.SetWarningMessages( warningMessages );
							helper.SetMessages( messages );
							isValid = false;
							return;
						}
					}
					string identifier = "Organization_" + request.Organization.Ctid;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published organization: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", request.Organization.Name, output.SubjectWebpage.ToString(), output.Ctid, crEnvelopeId );
						NotifyOnPublish( "Organization", msg );
					}
					else
					{
						messages.Add( status );
						isValid = false;
					}
				}
            }
            else //errors found
            {
                isValid = false;
                og.Graph.Add( output );
                //TODO - is there other info needed, like in context?
                if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = ctdlContext;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }

			helper.SetWarningMessages( warningMessages );
			helper.SetMessages(messages);
		}
		//

		/// <summary>
		/// Prototyping Just-In-Time registration of an organization in the account system
		/// </summary>
		/// <param name="request"></param>
		/// <param name="apiKey"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool HandleFirstPublishForTrustedPartner(EntityRequest request, string apiKey, ref List<string> messages)
		{
			bool isValid = true;
			bool recordExists = false;
			int msgCnt = messages.Count();
			//check if exists
			//hidden
			return isValid;
		}
        //

        public string FormatAsJson( EntityRequest request, RA.Models.RequestHelper helper, ref bool isValid, ref List<string> messages )
		{
            OutputGraph og = new OutputGraph();
            IsAPublishRequest = false;
            var output = new OutputEntity();
            string payload = "";
            isValid = true;
			helper.IsPublishRequestType = false;

            if ( ToMap( request, output, helper, ref messages, false ) )
            {
                og.Graph.Add( output );
                if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }

                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
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


                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = ctdlContext;

                payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }
			if ( warningMessages.Count > 0 )
				messages.AddRange( warningMessages );
			return payload;
		}

		#region Mapping
		/// <summary>
		/// Map and validate:
		/// - trim all strings
		/// - if empty/default values, set to null
		/// - use codeManager to look up codes
		/// - each code should be checked for proper prefix - supply if missing
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool ToMap( EntityRequest request, OutputEntity output, RA.Models.RequestHelper helper, ref List<string> messages, bool isAPublishRequest = true )
		{
			CurrentEntityType = "Organization";
			bool isValid = true;
			Community = request.Community ?? "";
			RJ.EntityReferenceHelper refHelper = new RJ.EntityReferenceHelper();

			InputEntity input = request.Organization;
            if ( !string.IsNullOrWhiteSpace( request.DefaultLanguage ) )
            {
                //validate
                if ( ValidateLanguageCode( request.DefaultLanguage, "request.DefaultLanguage", ref messages ) )
                {
                    DefaultLanguageForMaps = request.DefaultLanguage;
                }
            } else
                DefaultLanguageForMaps = SystemDefaultLanguageForMaps;

            try
			{

				CurrentCtid= output.Ctid = FormatCtid( input.Ctid, "Organization", ref messages );
				output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.Ctid, Community);

				HandleRequiredFields( input, output, ref messages );
				output.LifecycleStatusType = AssignStatusType( "Organization LifecycleStatusType", input.LifecycleStatusType, ref messages );
				output.ParentOrganization = FormatOrganizationReferences( input.ParentOrganization, "ParentOrganization", false, ref messages, false, true );
				//TODO - we need to have an edit that a top level org cannot be created unless matches the CTID of the publishing org
				//otherwise must indicate a parent org
				if ( isAPublishRequest && !string.IsNullOrWhiteSpace(output.Ctid) && output.Ctid.ToLower() != helper.OwnerCtid.ToLower() )
				{
					//must be a child org, so  
					if ( input.ParentOrganization == null || input.ParentOrganization.Count == 0 )
						messages.Add( "Error: Organization is not a registered organization. An organization must have the same CTID as the owing organization or a ParentOrganization must be provided (with a CTID that  the owning organization)." );
					else
					{
						//e
						var exists = input.ParentOrganization.FirstOrDefault( a => a.CTID == helper.OwnerCtid );
						if (exists == null || string.IsNullOrWhiteSpace(exists.CTID))
							messages.Add( "Error: Organization is not a registered organization. The organization has a different CTID than the owing organization and a parent organization was not found that matches the CTID of the owning organization." );
					}
				}
				//Following can be a variety of entities, so valid type must be provided
				output.Accredits = FormatEntityReferencesList( input.Accredits, "AgentAccredits", false, ref messages );
				output.Approves = FormatEntityReferencesList( input.Approves, "AgentApproves", false, ref messages );
				output.Owns = FormatEntityReferencesList( input.Owns, "AgentOwns", false, ref messages ); //Owns artifacts
				output.Offers = FormatEntityReferencesList( input.Offers, "AgentOffers", false, ref messages );//Offers artifacts

				output.Renews = FormatEntityReferencesList( input.Renews, "AgentRenews", false, ref messages );//Renews artifacts
				output.Revokes = FormatEntityReferencesList( input.Revokes, "AgentRevokes", false, ref messages ); //Revokes artifacts
				output.Recognizes = FormatEntityReferencesList( input.Recognizes, "AgentRecognizes", false, ref messages ); //Recognizes artifacts
				output.Regulates = FormatEntityReferencesList( input.Regulates, "AgentRegulates", false, ref messages ); //regulates artifacts

				if ( input.IsQAOrganization )
				{
					//note some orgs can be both
				} else
				{
					//definitely need owns/offers

					//or could do a quick lookup
				}

				//
				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );
				HandleLocations( input, output, ref messages );

                output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Organization Keywords", ref messages );
                
                output.AlternateName = AssignLanguageMapList( input.AlternateName, input.AlternateName_Map, "Organization Alternate Names", ref messages );

    //            if ( input.AlternateName != null && input.AlternateName.Count > 0 )
				//	output.AlternateName = FormatLanguageMapList(input.AlternateName, "Organization Alternate Names", ref messages);
				//else
				//	output.AlternateName = null;

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );

				output.AdministrationProcess = FormatProcessProfile( input.AdministrationProcess, ref messages );
				output.MaintenanceProcess = FormatProcessProfile( input.MaintenanceProcess, ref messages );
				output.DevelopmentProcess = FormatProcessProfile( input.DevelopmentProcess, ref messages );
				output.ComplaintProcess = FormatProcessProfile( input.ComplaintProcess, ref messages );
				output.AppealProcess = FormatProcessProfile( input.AppealProcess, ref messages );
				output.ReviewProcess = FormatProcessProfile( input.ReviewProcess, ref messages );
				output.RevocationProcess = FormatProcessProfile( input.RevocationProcess, ref messages );



                output.HasConditionManifest = AssignRegistryResourceURIsListAsStringList( input.HasConditionManifest, "HasConditionManifest", ref messages,false );
				output.HasCostManifest = AssignRegistryResourceURIsListAsStringList( input.HasCostManifest, "HasCostManifest", ref messages, false );

				HandleVerificationProfiles( input, output, ref messages );
				HandleJurisdictionAssertions( input, output, refHelper, ref messages );

				output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages, true );
				output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
				output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
				output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages, true  );

				output.Department = FormatOrganizationReferences( input.Department, "Department", false, ref messages );
				output.SubOrganization = FormatOrganizationReferences( input.SubOrganization, "Sub Organization", false, ref messages );

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "AgentServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}

		/// <summary>
		/// Validate and map required properties
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			bool isValid = true;

			//required
			output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Organization.Name", DefaultLanguageForMaps, ref messages, true, 3 );
			CurrentEntityName = GetFirstItemValue( output.Name );
            output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Organization Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );


            //TODO - need output handle credentialOrgnization vs QAcredentialOrgnization
            if ( string.IsNullOrWhiteSpace( input.Type ) )
			{
				messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
			}
			else if ( "credentialorganization" == input.Type.ToLower() || "ceterms:credentialorganization" == input.Type.ToLower() )
			{
				output.Type = "ceterms:CredentialOrganization";
			}
			else if ( "qacredentialorganization" == input.Type.ToLower() 
				|| "ceterms:qacredentialorganization" == input.Type.ToLower())
			{
				output.Type = "ceterms:QACredentialOrganization";
			}
			else
			{
				messages.Add( "Error - An valid organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
			}

			output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

			//additional required properties include:
			/*
			 * Owns or offers something - unless QA, in which case must QA something
			 *	Organization Type – enumerations of organization types. Examples include, but are not limited output, terms representing educational institutions, governmental bodies, credentialing and assurance bodies, and labor unions.
			 *	SEE: HandleCredentialAlignmentFields
			 *	Organization Sector Type – sectors include public, private for profit, public for profit, and business industry association.
			 *	SEE: HandleCredentialAlignmentFields
				● Contact Information – a means of contacting a resource or its representative(s). May include physical address, email address, and phone number.
			 */

			if ( input.AgentType != null && input.AgentType.Count > 0 )
			{
				foreach ( var item in input.AgentType )
				{
					//output.AgentType.Add( FormatCredentialAlignment( item ) );
					//should be concept scheme of organizationType
					
					output.AgentType.Add( FormatCredentialAlignment( "agentType", item, ref messages ) );
				}
			}
			else
			{
				messages.Add( "Error = Agent type is a required property. At least one type must be provided." );
			}


			//17-10-02 mp - can only have one sector type
			if ( !string.IsNullOrWhiteSpace( input.AgentSectorType ) )
			{
				//foreach ( string item in input.AgentSectorType )
				//{
				output.AgentSectorType.Add( FormatCredentialAlignment( "agentSectorType", input.AgentSectorType, ref messages ) );
				//}
			}
			else
			{
				messages.Add( "Error = Agent Sector Type is a required property. " );
				output.AgentSectorType = null;
			}

            //no longer included: 
            //				( input.ContactPoint == null || input.ContactPoint.Count == 0 )
            if ( (input.Address == null || input.Address.Count == 0) &&
				( input.Email == null   || input.Email.Count == 0 ) 
				)
			{
				messages.Add( "Error: At least one type of contact property, like  physical address, or email address is required." );
			}
			return isValid;

		}

		public void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			//output.AlternativeIdentifier = AssignIdentifierValueToList( input.AlternativeIdentifier );
			output.AlternativeIdentifier = AssignIdentifierListToList( input.AlternativeIdentifier, ref messages );
			//
			output.MissionAndGoalsStatementDescription = AssignLanguageMap( ConvertSpecialCharacters( input.MissionAndGoalsStatementDescription ), input.MissionAndGoalsStatementDescription_Map, "MissionAndGoalsStatementDescription", DefaultLanguageForMaps, ref messages );
			//
			output.AgentPurposeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.AgentPurposeDescription ), input.AgentPurposeDescription_Map, "AgentPurposeDescription",  DefaultLanguageForMaps, ref messages );

            output.DUNS = string.IsNullOrWhiteSpace((input.Duns ?? "").Trim()) ? "" : ( input.Duns ?? "" ).Trim() ;
			output.FEIN = string.IsNullOrWhiteSpace( ( input.Fein ?? "" ).Trim() ) ? "" : ( input.Fein ?? "" ).Trim();
			output.IpedsID = string.IsNullOrWhiteSpace( ( input.IpedsId ?? "" ).Trim() ) ? "" : ( input.IpedsId ?? "" ).Trim();
			output.OPEID = string.IsNullOrWhiteSpace( ( input.OpeId ?? "" ).Trim() ) ? "" : ( input.OpeId ?? "" ).Trim();
			output.LEICode = string.IsNullOrWhiteSpace( ( input.LEICode ?? "" ).Trim() ) ? "" : ( input.LEICode ?? "" ).Trim();
			output.ISICV4 = MapIsicV4( input.ISICV4 );

			if ( !string.IsNullOrWhiteSpace( ( input.NcesID ?? "" ) ))
			{
				if ( ( input.NcesID.Trim()).Length != 11)
				{
					//not sure on this 1-222-333-4444. Could be 11 without dashes
					messages.Add( "Error: The organization NcesID must be a 11 digit code." );
				} else
					output.NcesID = string.IsNullOrWhiteSpace( ( input.NcesID ?? "" ).Trim() ) ? "" : ( input.NcesID ?? "" ).Trim();
			}
			
			//
			//founded date will require special handling
			//19-12-03 - add edit on maximum length
			if ( !string.IsNullOrWhiteSpace( input.FoundingDate ) )
			{
				if ( input.FoundingDate.Length > 20 )
				{
					messages.Add( "The Founding Date is invalid, must be less than 20 characters.  Sample formats include: Just a 4 digit year (ex: 1999); or format like yyyy-mm (ex: 2001-03) or January 1999, or format like yyyy-mm-dd (ex: 1997-12-23)" );
				}
				else if ( input.FoundingDate.Length == 4 && IsInteger( input.FoundingDate ) )
					output.FoundingDate = input.FoundingDate;
				else if ( input.FoundingDate.IndexOf( "-" ) > 1 )
					output.FoundingDate = input.FoundingDate;
				else if ( input.FoundingDate.Length == 10 && IsDate( input.FoundingDate ) )
					output.FoundingDate = MapDate( input.FoundingDate, "FoundingDate", ref messages );
				else
				{
					//not sure we can enforce the above formats, so just let it go
					output.FoundingDate = input.FoundingDate;
					//messages.Add( "The Founding Date is invalid. It must be one of: Just a 4 digit year (ex: 1999); or format like yyyy-mm (ex: 2001-03) or format like yyyy-mm-dd (ex: 1997-12-23)" );
				}

			}
		}
		public void HandleLocations( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			//individual phone numbers are not allowed, only contact points
			//if ( input.PhoneNumbers != null && input.PhoneNumbers.Count() > 0 )
			//{
			//	output.PhoneNumber = input.PhoneNumbers;
			//}

			//17-06-14 MP - decided to not make email required
			if ( input.Email != null && input.Email.Count() > 0 )
			{
				output.Email = input.Email;
			}
			//else
			//{
			//	messages.Add( "Error - A valid email must be entered." );
			//}
			output.Address = FormatPlacesList( input.Address, ref messages );
			
			//top level contact points will be added under a default place/address
            //no longer allowed at org level
			//if ( input.ContactPoint != null && input.ContactPoint.Count > 0 )
			//{
			//	Models.Input.Place contactPlaces = new Models.Input.Place();
			//	contactPlaces.ContactPoint.AddRange( input.ContactPoint );
			//	AppendPlaceContactPoints( contactPlaces, output.Address, ref messages, false );
			//}
			//else
			//	output.ContactPoint = null;

		}

	
		public void HandleVerificationProfiles( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			foreach ( var vsp in input.VerificationServiceProfiles )
			{
				var vs = new RJ.VerificationServiceProfile
				{
					DateEffective = MapDate( vsp.DateEffective, "", ref messages ),
                    Description = AssignLanguageMap( ConvertSpecialCharacters( vsp.Description ), vsp.Description_Map,"VerificationServiceProfile Description",  DefaultLanguageForMaps, ref messages, false ),
                    EstimatedCost = FormatCosts( vsp.EstimatedCost, ref messages ),
					HolderMustAuthorize = vsp.HolderMustAuthorize,
                    VerificationMethodDescription = AssignLanguageMap( ConvertSpecialCharacters( vsp.VerificationMethodDescription ), vsp.VerificationMethodDescription_Map, "VerificationMethodDescription", DefaultLanguageForMaps, ref messages, false )

                };
				vs.Jurisdiction = MapJurisdictions( vsp.Jurisdiction, ref messages );
				vs.OfferedBy = FormatOrganizationReferences( vsp.OfferedBy, "Offered By", true, ref messages );
				vs.OfferedIn = MapJurisdictionAssertionsList( vsp.OfferedIn, ref helper, ref messages );
				//vs.Region = MapRegions( vsp.Region, ref messages );

				vs.VerifiedClaimType = FormatCredentialAlignmentVocabs( "verifiedClaimType", vsp.VerifiedClaimType, ref messages );

				vs.SubjectWebpage = AssignValidUrlAsString( vsp.SubjectWebpage, "Subject Webpage", ref messages, false );

				if ( !IsUrlValid( vsp.VerificationDirectory, ref status, ref isUrlPresent ) )
					messages.Add( "The Verification Directory is invalid" + status );
				else
				{
					if ( isUrlPresent )
						vs.VerificationDirectory.Add( vsp.VerificationDirectory );
				}

				if ( !IsUrlValid( vsp.VerificationService, ref status, ref isUrlPresent ) )
					messages.Add( "The Verification Service is invalid" + status );
				else
				{
					if ( isUrlPresent )
						vs.VerificationService.Add( vsp.VerificationService );
				}

				vs.TargetCredential = FormatEntityReferencesList( vsp.TargetCredential, RJ.Credential.classType, false, ref messages );

				output.VerificationServiceProfiles.Add( vs );
			}
		}

		public void HandleUrlFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			output.Image = AssignValidUrlAsString( input.Image, "Image", ref messages, false );

			output.AvailabilityListing = AssignValidUrlListAsStringList( input.AvailabilityListing, "Availability Listing", ref messages );

			output.MissionAndGoalsStatement = AssignValidUrlAsString( input.MissionAndGoalsStatement, "Mission and Goals Statement", ref messages, false );
			//output.MissionAndGoalsStatement = AssignValidUrlListAsStringList( input.MissionAndGoalsStatement, "Mission and Goals Statement", ref messages );

			output.AgentPurpose = AssignValidUrlAsString( input.AgentPurpose, "Agent Purpose", ref messages, false );
			//output.AgentPurpose = AssignValidUrlListAsStringList( input.AgentPurpose, "Agent Purpose", ref messages );

			output.SocialMedia = AssignValidUrlListAsStringList( input.SocialMedia, "Social Media", ref messages );

			output.SameAs = AssignValidUrlListAsStringList( input.SameAs, "Same As", ref messages );

		}


		public void HandleCredentialAlignmentFields( InputEntity from, OutputEntity to, ref List<string> messages )
		{
			//can't depend on the codes being NAICS
			to.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( from.IndustryType, true, ref messages);
			if ( from.Naics != null && from.Naics.Count > 0 )
				to.Naics = from.Naics;
			else
				to.Naics = null;

			//to.AlternativeIndustryType = AssignLanguageMapList( from.AlternativeIndustryType, from.AlternativeIndustryType_Map, "Organization AlternativeIndustryType", ref messages );

			if ( from.ServiceType != null && from.ServiceType.Count > 0 )
			{
				to.ServiceType = FormatCredentialAlignmentVocabs( "serviceType", from.ServiceType, ref messages );
			}
			else
				to.ServiceType = null;

		}


		public void HandleJurisdictionAssertions( InputEntity input, OutputEntity output, RJ.EntityReferenceHelper helper, ref List<string> messages )
		{
			RJ.JurisdictionProfile jp = new RJ.JurisdictionProfile();
			//need to check with partners, and set date for sunsetting this approach
			//if ( input.JurisdictionAssertions != null && input.JurisdictionAssertions.Count > 0 )
			//{
			//	if( !UtilityManager.GetAppKeyValue( "allowingJurisdictionAssertions", false ) )
			//	{
			//		messages.Add( "Error: As of 2020, the property JurisdictionAssertions is now obsolete. Instead the individual properties like AssertedIn, ApprovedIn should be used." );
			//		//return;
			//	}
			//	else
			//	{
			//		foreach( var item in input.JurisdictionAssertions )
			//		{
			//			if( item.AssertsAccreditedIn )
			//			{
			//				jp = MapJurisdictionAssertions( item, ref helper, ref messages );
			//				output.AccreditedIn = JurisdictionProfileAdd( jp, output.AccreditedIn );
			//			}
			//			if( item.AssertsApprovedIn )
			//			{
			//				jp = MapJurisdictionAssertions( item, ref helper, ref messages );
			//				output.ApprovedIn = JurisdictionProfileAdd( jp, output.ApprovedIn );
			//			}
			//			if( item.AssertsRecognizedIn )
			//			{
			//				jp = MapJurisdictionAssertions( item, ref helper, ref messages );
			//				output.RecognizedIn = JurisdictionProfileAdd( jp, output.RecognizedIn );
			//			}
			//			if( item.AssertsRegulatedIn )
			//			{
			//				jp = MapJurisdictionAssertions( item, ref helper, ref messages );
			//				output.RegulatedIn = JurisdictionProfileAdd( jp, output.RegulatedIn );
			//			}
			//		}

			//		warningMessages.Add( "Warning: the property JurisdictionAssertions will be removed by March 2020. The individual properties like AssertedIn should be used instead." );
			//	}
			//} 
			//else check regardless
			
			output.AccreditedIn = MapJurisdictionAssertionsList( input.AccreditedIn, ref helper, ref messages );
			output.ApprovedIn = MapJurisdictionAssertionsList( input.ApprovedIn, ref helper, ref messages );
			output.RecognizedIn = MapJurisdictionAssertionsList( input.RecognizedIn, ref helper, ref messages );
			output.RegulatedIn = MapJurisdictionAssertionsList( input.RegulatedIn, ref helper, ref messages );
			
		}

		#endregion

	}
}
