using System;
using System.Collections.Generic;
using System.Linq;
using RA.Models.Input;
using RAJ = RA.Models.Json;
using RA.Models.Json;
using Newtonsoft.Json;
using EntityRequest = RA.Models.Input.LearningOpportunityRequest;
using InputEntity = RA.Models.Input.LearningOpportunity;
using OutputEntity = RA.Models.Json.LearningOpportunityProfile;
using CER = RA.Services.RegistryServices;
//using Models.Common;
using Utilities;

namespace RA.Services
{
    public class LearningOpportunityServices : ServiceHelper
    {

        static string status = "";
		static bool isUrlPresent = true;
        /// <summary>
        /// Publish a Credential to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        /// <param name="payload"></param>
        public static void Publish( EntityRequest request, string apiKey, ref bool isValid, ref List<string> messages, ref string payload, ref string registryEnvelopeId )
        {
            RA.Models.RequestHelper helper = new Models.RequestHelper();
            helper.RegistryEnvelopeId = registryEnvelopeId;
            helper.ApiKey = apiKey;
            helper.OwnerCtid = request.PublishForOrganizationIdentifier;

            Publish( request, ref isValid, helper );

            payload = helper.Payload;
            messages = helper.GetAllMessages();
            registryEnvelopeId = helper.RegistryEnvelopeId;
        }
        /// <summary>
        /// Publish a Learning Opportunity to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        public static void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
            isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;
            string submitter = "";
            List<string> messages = new List<string>();
            var output = new OutputEntity();
            if ( ToMap( request.LearningOpportunity, output, ref messages ) )
            {
                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

                CER cer = new CER( "LearningOpportunity", output.Type, output.Ctid, helper.SerializedInput);
                cer.PublisherAuthorizationToken = helper.ApiKey;
                cer.PublishingForOrgCtid = helper.OwnerCtid;

                if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;

				string identifier = "LearningOpportunity_" + request.LearningOpportunity.Ctid;
				if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
                    //for now need to ensure envelopid is returned
                    helper.RegistryEnvelopeId = crEnvelopeId;

                    string msg = string.Format( "<p>Published LearningOpportunity: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId );
					NotifyOnPublish( "LearningOpportunity", msg );
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
                if ( !string.IsNullOrWhiteSpace( status ) )
                    messages.Add( status );
                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }

            helper.SetMessages( messages );

        }

		//Used for demo page - NA 6/5/2017
		//public static string DemoPublish( Models.Json.LearningOpportunityProfile ctdlFormattedLearningOpportunity, ref bool isValid, ref List<string> messages, ref string rawResponse, bool forceSkipValidation = false )
		//{
		//	isValid = true;
		//	var crEnvelopeId = "";
		//	var payload = JsonConvert.SerializeObject( ctdlFormattedLearningOpportunity, ServiceHelper.GetJsonSettings() );
		//	var identifier = "learningopportunity_" + ctdlFormattedLearningOpportunity.Ctid;
		//	rawResponse = new CER().Publish( payload, "", identifier, ref isValid, ref status, ref crEnvelopeId, forceSkipValidation );

		//	return crEnvelopeId;
		//}
		//

		public static string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
        {
            var output = new OutputEntity();
            string payload = "";
            isValid = true;
            IsAPublishRequest = false;

            if ( ToMap( request.LearningOpportunity, output, ref messages ) )
            {
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }
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
			CurrentEntityType = "LearningOpportunity";
			bool isValid = true;
           
            EntityReferenceHelper helper = new EntityReferenceHelper();
			try
			{


				HandleRequiredFields( input, output, ref messages );

				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );

				if ( input.Keyword != null && input.Keyword.Count > 0 )
					output.Keyword = input.Keyword;
				else
					output.Keyword = null;

				output.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );

				output.AvailableAt = FormatAvailableAtList( input.AvailableAt, ref messages );

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );
				output.EstimatedDuration = FormatDuration( input.EstimatedDuration, ref messages );

				output.Recommends = FormatConditionProfile( input.Recommends, ref messages );
				output.Requires = FormatConditionProfile( input.Requires, ref messages );
				output.Corequisite = FormatConditionProfile( input.Corequisite, ref messages );
				output.EntryCondition = FormatConditionProfile( input.EntryCondition, ref messages );

				output.AdvancedStandingFrom = FormatConnections( input.AdvancedStandingFrom, ref messages );
				output.IsAdvancedStandingFor = FormatConnections( input.IsAdvancedStandingFor, ref messages );

				output.PreparationFrom = FormatConnections( input.PreparationFrom, ref messages );
				output.IsPreparationFor = FormatConnections( input.IsPreparationFor, ref messages );
				output.IsRequiredFor = FormatConnections( input.IsRequiredFor, ref messages );
				output.IsRecommendedFor = FormatConnections( input.IsRecommendedFor, ref messages );

				output.Teaches = FormatCompetencies( input.Teaches, ref messages );

				HandleAssertedINsProperties( input, output, helper, ref messages );
			
				//the hasPart are limit to lopps
				output.HasPart = FormatEntityReferences( input.HasPart, OutputEntity.classType, false, ref messages );
				//isPartOf could be tougher! may want separate properties, dependent upon the parent!
				//for example, for a credential, it would be via a condition profile, and we have no facility for the latter, only part of a credential
				output.IsPartOf = FormatEntityReferences( input.IsPartOfLearningOpportunity, OutputEntity.classType, false, ref messages );

				output.CommonConditions = AssignValidUrlListAsStringList( input.CommonConditions, "CommonConditions", ref messages );
				output.CommonCosts = AssignValidUrlListAsStringList( input.CommonCosts, "CommonCosts", ref messages );

				output.FinancialAssistance = MapFinancialAssitance( input.FinancialAssistance, ref messages );

				output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier );


				HandleOrgProperties( input, output, ref messages );
			}
			catch ( Exception ex )
			{
				LogError( ex, "LearningOpportunityServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

            return isValid;
        }

        public static void HandleOrgProperties( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			//see HandleRequiredFields
			//output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", true, ref messages );
			//output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

			output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages, true);
			output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
			
			output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
            output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages, true);


        }


        public static bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
            string property = "";

            output.Ctid = FormatCtid(input.Ctid, ref messages);
            output.CtdlId = idBaseUrl + output.Ctid;

            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
				messages.Add( FormatMessage( "Error - A name must be entered for Learning Opportunity with CTID: '{0}'.", input.Ctid ) );
			}
			else
			{
				output.Name = input.Name;
				CurrentEntityName = input.Name;
			}
			if ( string.IsNullOrWhiteSpace( input.Description ) )
            {
                messages.Add( "Error - A Learning Opportunity description must be entered." );
            }
            else
                output.Description = ConvertWordFluff( input.Description );

            //now literal
            output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );


            output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages );
            output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

			if ( output.OwnedBy == null && output.OfferedBy == null )
			{
				messages.Add( "Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for a Learning Opportunity" );
			}


			if ( ( input.AvailableOnlineAt == null || input.AvailableOnlineAt.Count == 0 ) &&
				 ( input.AvailabilityListing == null || input.AvailabilityListing.Count == 0 ) &&
				 ( input.AvailableAt == null || input.AvailableAt.Count == 0 ) )
				messages.Add( string.Format( "Error - At least one of: 'Available Online At', 'Availability Listing', or 'Available At' (address) must be provided for Assessment: '{0}'", input.Name ) );

			return isValid;
        }
	
		public static void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			//now literal
			//output.CodedNotation = AssignListToString( input.CodedNotation );
            output.CodedNotation = input.CodedNotation;
            output.VerificationMethodDescription = input.VerificationMethodDescription;
            output.DeliveryTypeDescription = input.DeliveryTypeDescription;
            output.DateEffective = MapDate( input.DateEffective, "Learning Opportunity Date Effective", ref messages);
			//should have validation for languages?
			if ( input.InLanguage != null && input.InLanguage.Count > 0 )
				output.InLanguage = input.InLanguage;

			if ( ValidateCreditUnitOrHoursProperties( input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref messages ) )
			{
				output.CreditUnitTypeDescription = string.IsNullOrEmpty( input.CreditUnitTypeDescription ) ? null : input.CreditUnitTypeDescription;
				//credential alignment object
				if ( !string.IsNullOrWhiteSpace( input.CreditUnitType ) )
				{
					output.CreditUnitType =  FormatCredentialAlignment( "creditUnitType", input.CreditUnitType, ref messages ) ;
				}
				else
					output.CreditUnitType = null;

				output.CreditUnitValue = input.CreditUnitValue;
				output.CreditHourType = input.CreditHourType;
				output.CreditHourValue = input.CreditHourValue;
			}
			else
			{
				output.CreditUnitType = null;
			}
		}

        public static void HandleUrlFields( InputEntity from, OutputEntity to, ref List<string> messages )
        {
			//17-11-27 Added a requirement check for these in the required section
			to.AvailableOnlineAt = AssignValidUrlListAsStringList( from.AvailableOnlineAt, "Available Online At", ref messages );
			to.AvailabilityListing = AssignValidUrlListAsStringList( from.AvailabilityListing, "Availability Listing", ref messages );


        }


		public static void HandleAssertedINsProperties( InputEntity input, OutputEntity output, EntityReferenceHelper helper, ref List<string> messages )
		{
			RAJ.JurisdictionProfile jp = new RAJ.JurisdictionProfile();
			if ( input.JurisdictionAssertions != null && input.JurisdictionAssertions.Count > 0 )
			{
				foreach ( var item in input.JurisdictionAssertions )
				{
					if ( item.AssertsAccreditedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.AccreditedIn = JurisdictionProfileAdd( jp, output.AccreditedIn );
					}
					if ( item.AssertsApprovedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.ApprovedIn = JurisdictionProfileAdd( jp, output.ApprovedIn );
					}
					if ( item.AssertsOfferedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.OfferedIn = JurisdictionProfileAdd( jp, output.OfferedIn );
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

					if ( item.AssertsRevokedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.RevokedIn = JurisdictionProfileAdd( jp, output.RevokedIn );
					}
				}
			}

		} //

		#region === CredentialAlignmentObject ===
		public static void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			if ( input.LearningMethodType.Any() )
				foreach ( string item in input.LearningMethodType )
					output.LearningMethodType.Add( FormatCredentialAlignment( "learningMethodType", item, ref messages ) );
			else
				output.LearningMethodType = null;

			if ( input.DeliveryType.Any() )
                foreach ( string item in input.DeliveryType )
                    output.DeliveryType.Add( FormatCredentialAlignment( "deliveryType", item, ref messages ) );
            else output.DeliveryType = null;

            //if ( UtilityManager.GetAppKeyValue("usingCredentialAudienceType", false) )
                output.AudienceType = FormatCredentialAlignmentVocabs("audienceType", input.AudienceType, ref messages);
            //else
             //   output.AudienceType = null;

            output.InstructionalProgramType = FormatCredentialAlignmentListFromFrameworkItemList( input.InstructionalProgramType, true, ref messages, "Classification of Instructional Programs", "https://nces.ed.gov/ipeds/cipcode/Default.aspx?y=55" );
			//if ( input.InstructionalProgramType != null && input.InstructionalProgramType.Count > 0 )
			//{
			//	foreach ( FrameworkItem item in input.InstructionalProgramType )
			//	{
			//		output.InstructionalProgramType.Add( FormatCredentialAlignment( item, true ) );
			//	}
			//}
			//else
			//	output.InstructionalProgramType = null;

			output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject );

			//if ( input.Subject.Any() )
   //             foreach ( var item in input.Subject )
   //                 output.Subject.Add( FormatCredentialAlignment( item ) );
   //         else output.Subject = null;
        }
        //see common methods in ServiceHelper
        #endregion  
    }
}
