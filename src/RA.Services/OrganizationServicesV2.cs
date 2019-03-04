using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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
            OutputGraph og = new OutputGraph();
            List<string> messages = new List<string>();
            if (ToMap(request, output, ref messages))
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
                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

                CER cer = new CER( "Organization", output.Type, output.Ctid, helper.SerializedInput )
                {
                    PublisherAuthorizationToken = helper.ApiKey,
                    PublishingForOrgCtid = helper.OwnerCtid
                };
                if (cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32)
                    cer.IsManagedRequest = true;
				//
				bool recordWasFound = false;
				bool usedCEKeys = false;
				string message = "";
				var result = HistoryServices.GetMostRecentHistory( "Organization", output.Ctid, ref recordWasFound, ref usedCEKeys, ref message );
				if ( recordWasFound ) //found previous
				{
					if ( usedCEKeys && cer.IsManagedRequest )
					{
						LoggingHelper.DoTrace( 5, "Organization publish. Was managed request. Overriding to CE publish." );
						cer.IsManagedRequest = false;   //should record override
						cer.OverrodeOriginalRequest = true;
					}
					else if ( !usedCEKeys && !cer.IsManagedRequest )
					{
						//this should not happen. Means used publisher
						cer.IsManagedRequest = true;   //should record override
						cer.OverrodeOriginalRequest = true;
					}
				}
				else
				{
					//eventually will always do managed
				}
				//
				string identifier = "Organization_" + request.Organization.Ctid;

                if (cer.Publish(helper.Payload, submitter, identifier, ref status, ref crEnvelopeId))
                {
                    helper.RegistryEnvelopeId = crEnvelopeId;
                    string msg = string.Format("<p>Published organization: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", request.Organization.Name, output.SubjectWebpage.ToString(), output.Ctid, crEnvelopeId);
                    NotifyOnPublish("Organization", msg);
                }
                else
                {
                    messages.Add(status);
                    isValid = false;
                }
            }
            else
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
                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }
            helper.SetMessages(messages);
        }
        //

        public string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
		{
            OutputGraph og = new OutputGraph();
            IsAPublishRequest = false;
            var output = new OutputEntity();
            string payload = "";
            isValid = true;

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

                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

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


                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }

			return payload;
		}


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
		public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "Organization";
			bool isValid = true;
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
				HandleRequiredFields( input, output, ref messages );

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

                //Following can be a variety of entities, so valid type must be provided
                output.Accredits = FormatEntityReferencesList( input.Accredits, "AgentAccredits", false, ref messages );
                output.Approves = FormatEntityReferencesList( input.Approves, "AgentApproves", false, ref messages );
				output.Owns = FormatEntityReferencesList( input.Owns, "AgentOwns", false, ref messages ); //Owns artifacts
				output.Offers = FormatEntityReferencesList( input.Offers, "AgentOffers", false, ref messages );//Offers artifacts

                output.Renews = FormatEntityReferencesList( input.Renews, "AgentRenews", false, ref messages );//Renews artifacts
                output.Revokes = FormatEntityReferencesList( input.Revokes, "AgentRevokes", false, ref messages ); //Revokes artifacts
                output.Recognizes = FormatEntityReferencesList( input.Recognizes, "AgentRecognizes", false, ref messages ); //Recognizes artifacts
                output.Regulates = FormatEntityReferencesList( input.Regulates, "AgentRegulates", false, ref messages ); //regulates artifacts

                output.HasConditionManifest = AssignValidUrlListAsStringList( input.HasConditionManifest, "HasConditionManifest", ref messages,false );
				output.HasCostManifest = AssignValidUrlListAsStringList( input.HasCostManifest, "HasCostManifest", ref messages, false );

				HandleVerificationProfiles( input, output, ref messages );
				HandleJurisdictionAssertions( input, output, ref messages );

				output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages, true );
				output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
				output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
				output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages, true  );

				output.ParentOrganization = FormatOrganizationReferences( input.ParentOrganization, "ParentOrganization", false, ref messages );

				output.Department = FormatOrganizationReferences( input.Department, "Department", false, ref messages );
				output.SubOrganization = FormatOrganizationReferences( input.SubOrganization, "Sub Organization", false, ref messages );

			}
			catch ( Exception ex )
			{
				LogError( ex, "AgentServices.ToMap" );
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

            output.Ctid = FormatCtid(input.Ctid, ref messages);
            output.CtdlId = idBaseUrl + output.Ctid;


			if ( string.IsNullOrWhiteSpace( output.CtdlId ) )
			{
				messages.Add( "Error - An @Id must be provided/generated input CTID." );
			}

            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                if ( input.Name_Map == null || input.Name_Map.Count == 0 )
                {
                    messages.Add( FormatMessage( "Error - A Name or Name_Map must be entered for Organization with CTID: '{0}'.", input.Ctid ) );
                }
                else
                {
                    output.Name = AssignLanguageMap( input.Name_Map, "Organization Name", ref messages );
                    CurrentEntityName = GetFirstItemValue( output.Name );
                }
            }
            else
            {
                output.Name = Assign( input.Name, DefaultLanguageForMaps );
                CurrentEntityName = input.Name;
            }
            output.Description = AssignLanguageMap( ConvertSpecialInput( input.Description ), input.Description_Map, "Organization Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );


            //TODO - need output handle credentialOrgnization vs QAcredentialOrgnization
            if ( string.IsNullOrWhiteSpace( input.Type ) )
			{
				messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
			}
			else if ( "credentialorganization" == input.Type.ToLower() )
			{
				output.Type = "ceterms:CredentialOrganization";
			}
			else if ( "qacredentialorganization" == input.Type.ToLower() )
			{
				output.Type = "ceterms:QACredentialOrganization";
			}
			else
			{
				messages.Add( "Error - An organization type must be entered: one of  CredentialOrganization or QACredentialOrganization." );
			}

			output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

			//additional required properties include:
			/*
			 *	● Organization Type – enumerations of organization types. Examples include, but are not limited output, terms representing educational institutions, governmental bodies, credentialing and assurance bodies, and labor unions.
			 *	SEE: HandleCredentialAlignmentFields
			 *	● Organization Sector Type – sectors include public, private for profit, public for profit, and business industry association.
			 *	SEE: HandleCredentialAlignmentFields
				● Contact Information – a means of contacting a resource or its representative(s). May include physical address, email address, and phone number.
			 */

			if ( input.AgentType != null && input.AgentType.Count > 0 )
			{
				foreach ( var item in input.AgentType )
				{
					//output.AgentType.Add( FormatCredentialAlignment( item ) );
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
			output.MissionAndGoalsStatementDescription = input.MissionAndGoalsStatementDescription;
			//output.AgentPurposeDescription = Assign(input.AgentPurposeDescription, DefaultLanguageForMaps );
            output.AgentPurposeDescription = AssignLanguageMap( ConvertSpecialInput( input.AgentPurposeDescription ), input.AgentPurposeDescription_Map, "AgentPurposeDescription",  DefaultLanguageForMaps, ref messages );

            output.DUNS = input.Duns;
			output.FEIN = input.Fein;
			output.IpedsID = input.IpedsId;
			output.OPEID = input.OpeId;
            output.LEICode = input.LEICode;
            
            //founded date will require special handling
            if ( !string.IsNullOrWhiteSpace( input.FoundingDate ) )
			{
				if ( input.FoundingDate.Length == 4 && IsInteger( input.FoundingDate ) )
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
                    Description = AssignLanguageMap( ConvertSpecialInput( vsp.Description ), vsp.Description_Map,"VerificationServiceProfile Description",  DefaultLanguageForMaps, ref messages, false ),
                    EstimatedCost = FormatCosts( vsp.EstimatedCost, ref messages ),
					HolderMustAuthorize = vsp.HolderMustAuthorize,
                    VerificationMethodDescription = AssignLanguageMap( ConvertSpecialInput( vsp.VerificationMethodDescription ), vsp.VerificationMethodDescription_Map, "VerificationMethodDescription", DefaultLanguageForMaps, ref messages, false )

                };
				//foreach ( var vc in vsp.VerifiedClaimType )
				//	vs.VerifiedClaimType.Add( FormatCredentialAlignmentVocabToList( vc ) );

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


				vs.Jurisdiction = MapJurisdictions( vsp.Jurisdiction, ref messages );
				//vs.Region = MapRegions( vsp.Region, ref messages );
				vs.OfferedBy = FormatOrganizationReferences( vsp.OfferedBy, "Offered By", true, ref messages );

				vs.TargetCredential = FormatEntityReferencesList( vsp.TargetCredential, RJ.Credential.classType, false, ref messages );

				//foreach ( var target in vsp.TargetCredential )
				//	if ( FormatEntityReference( target, "Target Credential", ref helper, true, ref messages ) )
				//		if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
				//			vs.TargetCredential.Add( helper.EntityBaseList[ 0 ] );

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


		public void HandleJurisdictionAssertions( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			RJ.JurisdictionProfile jp = new RJ.JurisdictionProfile();
			if ( input.JurisdictionAssertions != null && input.JurisdictionAssertions.Count > 0 )
			{
				foreach ( var item in input.JurisdictionAssertions )
				{
					if ( item.AssertsAccreditedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.AccreditedIn = JurisdictionProfileAdd( jp, output.AccreditedIn );
						//if ( jp != null )
						//	output.AccreditedIn.Add( jp );
					}
					if ( item.AssertsApprovedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.ApprovedIn = JurisdictionProfileAdd( jp, output.ApprovedIn );
					}
					if ( item.AssertsRecognizedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.RecognizedIn = JurisdictionProfileAdd( jp, output.RecognizedIn );
					}
					if ( item.AssertsRegulatedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.RegulatedIn = JurisdictionProfileAdd( jp, output.RegulatedIn );
					}
				}
			}
		}
	}
}
