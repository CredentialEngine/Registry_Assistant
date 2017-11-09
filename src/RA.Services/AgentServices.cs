using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
//using CER = RA.Services.RegistryServices;
using CER = RA.Services.RegistryServices;
using EntityRequest = RA.Models.Input.OrganizationRequest;
using InputEntity = RA.Models.Input.Organization;
using OutputEntity = RA.Models.Json.Agent;
using RA.Models.Json;
using RJ = RA.Models.Json;
using RA.Models.Input;
using Utilities;

namespace RA.Services
{
	public class AgentServices : ServiceHelper
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
		public static void Publish( EntityRequest request, ref bool isValid, ref List<string> messages, ref string payload, ref string registryEnvelopeId )
		{
			isValid = true;
			registryEnvelopeId = "";
			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";

			var output = new Agent();
			if ( ToMap( request.Organization, output, ref messages ) )
			{
				payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

				CER cer = new CER();
				//crEnvelopeId = registryEnvelopeId;

				string identifier = "agent_" + request.Organization.Ctid;
				if ( cer.Publish( payload, submitter, identifier, ref status, ref registryEnvelopeId ) )
				{
					string msg = string.Format( "<p>Published organization: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage.ToString(), output.Ctid, registryEnvelopeId );
					NotifyOnPublish( "Organization", msg );
				}
				else
				{
					messages.Add( status );
					isValid = false;
				}
			}
			else
			{
				isValid = false;
				payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}

		}
		//

		//Used for demo page - NA 6/5/2017
		public static string DemoPublish( OutputEntity ctdlFormattedAgent, ref bool isValid, ref List<string> messages, ref string rawResponse, bool forceSkipValidation = false )
		{
			isValid = true;
			var crEnvelopeId = "";
			var payload = JsonConvert.SerializeObject( ctdlFormattedAgent, ServiceHelper.GetJsonSettings() );
			var identifier = "agent_" + ctdlFormattedAgent.Ctid;
			rawResponse = new CER().Publish( payload, "", identifier, ref isValid, ref status, ref crEnvelopeId, forceSkipValidation );

			return crEnvelopeId;
		}

		public static string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
		{
			return FormatAsJson( request.Organization, ref isValid, ref messages );
		}
		public static string FormatAsJson( InputEntity input, ref bool isValid, ref List<string> messages )
		{
			var output = new OutputEntity();
			string payload = "";
			isValid = true;

			if ( ToMap( input, output, ref messages ) )
				payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			else
			{
				isValid = false;
				//do payload anyway
				payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
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
		public static bool ToMap( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			bool isValid = true;
			try
			{
				HandleRequiredFields( input, output, ref messages );

				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );
				HandleLocations( input, output, ref messages );

				if ( input.Keyword != null && input.Keyword.Count > 0 )
					output.Keyword = input.Keyword;
				else
					output.Keyword = null;

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
				output.Approves = FormatEntityReferences( input.Approves, "", false, ref messages );
				output.Owns = FormatEntityReferences( input.Owns, "", false, ref messages ); //Owns Credentials
				output.Offers = FormatEntityReferences( input.Offers, "", false, ref messages );//Offers Credentials

				output.Renews = FormatEntityReferences( input.Renews, "", false, ref messages );//Renews Credentials
				output.Revokes = FormatEntityReferences( input.Revokes, "", false, ref messages ); //Revokes Credentials
				output.Recognizes = FormatEntityReferences( input.Recognizes, "", false, ref messages ); //Recognizes Credentials

				output.HasConditionManifest = AssignRegistryURIsListAsStringList( input.HasConditionManifest, "HasConditionManifest", ref messages );
				output.HasCostManifest = AssignRegistryURIsListAsStringList( input.HasCostManifest, "HasCostManifest", ref messages );

				HandleVerificationProfiles( input, output, ref messages );
				HandleJurisdictionAssertions( input, output, ref messages );

				output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages );
				output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
				output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
				output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages );

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
		public static bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			bool isValid = true;

			//todo determine if will generate where not found
			if ( string.IsNullOrWhiteSpace( input.Ctid ) && GeneratingCtidIfNotFound() )
				input.Ctid = GenerateCtid();

			if ( IsCtidValid( input.Ctid, ref messages ) )
			{
				output.Ctid = input.Ctid;
				output.CtdlId = idUrl + output.Ctid;
			}
			if ( string.IsNullOrWhiteSpace( output.CtdlId ) )
			{
				messages.Add( "Error - An @Id must be provided/generated input CTID." );
			}
			//required
			if ( string.IsNullOrWhiteSpace( input.Name ) )
			{
				messages.Add( "Error - An organization name must be entered." );
			}
			else
				output.Name = input.Name;
			if ( string.IsNullOrWhiteSpace( input.Description ) )
			{
				messages.Add( "Error - An organization description must be entered." );
			}
			else
				output.Description = input.Description;


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

			if ( input.Address == null && input.Address.Count == 0 &&
				( input.Email == null || input.Email.Count == 0 ) &&
				( input.Address == null || input.Address.Count == 0 ) &&
				( input.ContactPoint == null || input.ContactPoint.Count == 0 )
				)
			{
				messages.Add( "Error: At least one type of contact property, like  physical address, contact point, email address, or phone number. is required." );
			}
			return isValid;

		}

		public static void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			output.AlternativeIdentifier = AssignIdentifierValueToList( input.AlternativeIdentifier );
			output.MissionAndGoalsStatementDescription = input.MissionAndGoalsStatementDescription;
			output.AgentPurposeDescription = input.AgentPurposeDescription;
			output.DUNS = input.Duns;
			output.FEIN = input.Fein;
			output.IpedsID = input.IpedsId;
			output.OPEID = input.OpeId;

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
		public static void HandleLocations( InputEntity input, OutputEntity output, ref List<string> messages )
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
			if ( input.ContactPoint != null && input.ContactPoint.Count > 0 )
			{
				Models.Input.Place contactPlaces = new Models.Input.Place();
				contactPlaces.ContactPoint.AddRange( input.ContactPoint );
				AppendPlaceContactPoints( contactPlaces, output.Address, ref messages, false );
				//foreach ( var cp in input.ContactPoint )
				//{
				//	var ccp = new Models.Json.ContactPoint
				//	{
				//		Name = cp.Name,
				//		ContactType = cp.ContactType,
				//		Emails = cp.Emails,
				//		PhoneNumbers = cp.PhoneNumbers,
				//		ContactOption = cp.ContactOption
				//	};
				//	foreach ( var smp in cp.SocialMediaPages )
				//		ccp.SocialMediaPages.Add( smp );
				//	output.ContactPoint.Add( ccp );
				//}
			}
			//else
			//	output.ContactPoint = null;

		}

	
		public static void HandleVerificationProfiles( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			EntityReferenceHelper helper = new EntityReferenceHelper();
			foreach ( var vsp in input.VerificationServiceProfiles )
			{
				var vs = new RA.Models.Json.VerificationServiceProfile
				{
					DateEffective = MapDate( vsp.DateEffective, "", ref messages ),
					Description = vsp.Description,
					EstimatedCost = FormatCosts( vsp.EstimatedCost, ref messages ),
					HolderMustAuthorize = vsp.HolderMustAuthorize,
					VerificationMethodDescription = vsp.VerificationMethodDescription

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
				vs.TargetCredential = FormatEntityReferences( vsp.TargetCredential, RJ.Credential.classType, false, ref messages );

				foreach ( var target in vsp.TargetCredential )
					if ( FormatEntityReference( target, "Target Credential", ref helper, true, ref messages ) )
						if ( helper.ReturnedDataType == 1 || helper.ReturnedDataType == 2 )
							vs.TargetCredential.Add( helper.EntityBaseList[ 0 ] );

				output.VerificationServiceProfiles.Add( vs );
			}
		}

		public static void HandleUrlFields( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			output.Image = AssignValidUrlAsString( input.Image, "Image", ref messages );

			output.AvailabilityListing = AssignValidUrlListAsStringList( input.AvailabilityListing, "Availability Listing", ref messages );

			output.MissionAndGoalsStatement = AssignValidUrlAsString( input.MissionAndGoalsStatement, "Mission and Goals Statement", ref messages );
			//output.MissionAndGoalsStatement = AssignValidUrlListAsStringList( input.MissionAndGoalsStatement, "Mission and Goals Statement", ref messages );

			output.AgentPurpose = AssignValidUrlAsString( input.AgentPurpose, "Agent Purpose", ref messages );
			//output.AgentPurpose = AssignValidUrlListAsStringList( input.AgentPurpose, "Agent Purpose", ref messages );

			output.SocialMedia = AssignValidUrlListAsStringList( input.SocialMedia, "Social Media", ref messages );

			output.SameAs = AssignValidUrlListAsStringList( input.SameAs, "Same As", ref messages );

		}


		public static void HandleCredentialAlignmentFields( InputEntity from, OutputEntity to, ref List<string> messages )
		{
			//can't depend on the codes being NAICS
			to.IndustryType = FormatCredentialAlignmentListFromList( from.IndustryType, true, "" );
			if ( from.Naics != null && from.Naics.Count > 0 )
				to.Naics = from.Naics;
			else
				to.Naics = null;

			//if ( from.IndustryType != null && from.IndustryType.Count > 0 )
			//{
			//	foreach ( FrameworkItem item in from.IndustryType )
			//	{
			//		to.IndustryType.Add( FormatCredentialAlignment( item, true ) );
			//	}
			//}
			//else
			//	to.IndustryType = null;

			if ( from.ServiceType != null && from.ServiceType.Count > 0 )
			{
				to.ServiceType = FormatCredentialAlignmentVocabs( "serviceType", from.ServiceType, ref messages );
			}
			else
				to.ServiceType = null;



		}


		public static void HandleJurisdictionAssertions( InputEntity input, OutputEntity output, ref List<string> messages )
		{
			EntityReferenceHelper helper = new EntityReferenceHelper();
			JurisdictionProfile jp = new JurisdictionProfile();
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
