using System;
using System.Collections.Generic;
using System.Linq;

using RJ = RA.Models.Json;
using RA.Models.Input;
using EntityRequest = RA.Models.Input.AssessmentRequest;
using InputEntity = RA.Models.Input.Assessment;
using OutputEntity = RA.Models.Json.AssessmentProfile;
using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class AssessmentServices : ServiceHelper
    {
        static string status = "";

        /// <summary>
        /// Publish an Assessment to the Credential Registry
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
        /// Publish an Assessment to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        public static void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
			isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;

            //submitter is not a person for this api, rather the organization
            //may want to do a lookup via the api key?
            string submitter = "";
            List<string> messages = new List<string>();
            var output = new OutputEntity();
			if ( ToMap( request.Assessment, output, ref messages ) )
			{

                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

				CER cer = new CER( "Assessment", output.Type, output.Ctid, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					PublishingForOrgCtid = helper.OwnerCtid
				};

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;

				string identifier = "Assessment_" + request.Assessment.Ctid;

				if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
                    //for now need to ensure envelopid is returned
                    helper.RegistryEnvelopeId = crEnvelopeId;

                    string msg = string.Format( "<p>Published Assessment: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId );
					NotifyOnPublish( "Assessment", msg );
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
		//
		//Used for demo page - NA 6/5/2017
		//public static string DemoPublish( OutputEntity ctdlFormattedAssessment, ref bool isValid, ref List<string> messages, ref string rawResponse, bool forceSkipValidation = false )
		//{
		//	isValid = true;
		//	var crEnvelopeId = "";
		//	var payload = JsonConvert.SerializeObject( ctdlFormattedAssessment, ServiceHelper.GetJsonSettings() );
		//	var identifier = "assessment_" + ctdlFormattedAssessment.Ctid;
		//	rawResponse = new CER().Publish( payload, "", identifier, ref isValid, ref status, ref crEnvelopeId, forceSkipValidation );

		//	return crEnvelopeId;
		//}
		//

		public static string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
		{
            return FormatAsJson( request.Assessment, ref isValid, ref messages );
		}
		//
		public static string FormatAsJson( InputEntity input, ref bool isValid, ref List<string> messages )
        {
            var output = new OutputEntity();
			string payload = "";
            isValid = true;
            IsAPublishRequest = false;

            if ( ToMap( input, output, ref messages ) )
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
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="messages"></param>
        /// <returns></returns>        
        public static bool ToMap( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			CurrentEntityType = "Assessment";
			bool isValid = true;
            RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();

			try
			{
				HandleRequiredFields( input, output, ref messages );

				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );

				if ( input.Keyword != null && input.Keyword.Count > 0 )
					output.Keyword = input.Keyword;
				else
					output.Keyword = null;

				output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject );

				output.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );

				output.AvailableAt = FormatAvailableAtList( input.AvailableAt, ref messages );

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );

				output.EstimatedDuration = FormatDuration( input.EstimatedDuration, ref messages );

				output.Requires = FormatConditionProfile( input.Requires, ref messages );
				output.Recommends = FormatConditionProfile( input.Recommends, ref messages );

				output.Corequisite = FormatConditionProfile( input.Corequisite, ref messages );
				output.EntryCondition = FormatConditionProfile( input.EntryCondition, ref messages );

				output.AdvancedStandingFrom = FormatConnections( input.AdvancedStandingFrom, ref messages );
				output.IsAdvancedStandingFor = FormatConnections( input.IsAdvancedStandingFor, ref messages );

				output.PreparationFrom = FormatConnections( input.PreparationFrom, ref messages );
				output.IsPreparationFor = FormatConnections( input.IsPreparationFor, ref messages );

				output.IsRequiredFor = FormatConnections( input.IsRequiredFor, ref messages );
				output.IsRecommendedFor = FormatConnections( input.IsRecommendedFor, ref messages );

				//TargetNodeName is required, so what to use for competencies
				output.Assesses = FormatCompetencies( input.Assesses, ref messages );

				output.AdministrationProcess = FormatProcessProfile( input.AdministrationProcess, ref messages );
				output.MaintenanceProcess = FormatProcessProfile( input.MaintenanceProcess, ref messages );
				output.DevelopmentProcess = FormatProcessProfile( input.DevelopmentProcess, ref messages );


					HandleAssertedINsProperties( input, output, helper, ref messages );
		
				//17-10-19 changing these to strings, as these should never(?) be 3rd party
				output.CommonConditions = AssignValidUrlListAsStringList( input.CommonConditions, "CommonConditions", ref messages );
				output.CommonCosts = AssignValidUrlListAsStringList( input.CommonCosts, "CommonCosts", ref messages );

				output.FinancialAssistance = MapFinancialAssitance( input.FinancialAssistance, ref messages );

				output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier );
                HandleOrgProperties( input, output, ref messages );

            }
			catch ( Exception ex )
			{
				LogError( ex, "AssessmentServices.ToMap" );
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

			//other roles
			output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages, true );
			output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
			
			output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
            output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages, true);

        }
        public static bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
            ///string property = "";

            output.Ctid = FormatCtid(input.Ctid, ref messages);
            output.CtdlId = credRegistryResourceUrl + output.Ctid;
            //todo determine if will generate where not found
   //         if ( string.IsNullOrWhiteSpace( input.Ctid ) && GeneratingCtidIfNotFound() )
   //             input.Ctid = GenerateCtid();

   //         if ( IsCtidValid( input.Ctid, ref messages ) )
   //         {
   //             //input.Ctid = input.Ctid.ToLower();
   //             output.Ctid = input.Ctid;
   //             output.CtdlId = credRegistryResourceUrl + output.Ctid;
			//	CurrentCtid = input.Ctid;
			//}
            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                messages.Add( "Error - An Assessment name must be entered." );
            }
			else
			{
				output.Name = input.Name;
				CurrentEntityName = input.Name;
			}
			if ( string.IsNullOrWhiteSpace( input.Description ) )
            {
                messages.Add( string.Format( "Error - An Assessment description must be provided for Assessment: '{0}'", input.Name ) );
			}
            else
                output.Description = ConvertWordFluff( input.Description );

            //now literal
            output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

            output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages );
            //output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", false, ref messages );
			output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

			if (output.OwnedBy == null && output.OfferedBy == null )
			{
				messages.Add( string.Format("Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Assessment: '{0}'", input.Name ));
			}

			if ( (input.AvailableOnlineAt == null		|| input.AvailableOnlineAt.Count ==0 ) &&
				 ( input.AvailabilityListing == null	|| input.AvailabilityListing.Count == 0 ) &&
				 ( input.AvailableAt == null			|| input.AvailableAt.Count == 0 )	 )
				messages.Add( string.Format( "Error - At least one of: 'Available Online At', 'Availability Listing', or 'Available At' (address) must be provided for Assessment: '{0}'", input.Name ) );

			return isValid;
		}
		

        public static void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            //now literal, should only be one
            //output.CodedNotation = AssignListToString( input.CodedNotation );
            output.CodedNotation = input.CodedNotation;

            output.AssessmentExample = AssignValidUrlAsString( input.AssessmentExample, "AssessmentExample", ref messages );
			output.AssessmentExampleDescription = input.AssessmentExampleDescription;

            output.AssessmentOutput = input.AssessmentOutput;
			if ( input.InLanguage != null && input.InLanguage .Count > 0)
				output.InLanguage = input.InLanguage;

			if ( input.HasGroupEvaluation != null )
				output.HasGroupEvaluation = ( bool ) input.HasGroupEvaluation;
			else
				output.HasGroupEvaluation = null;

			if ( input.HasGroupParticipation != null )
				output.HasGroupParticipation = ( bool ) input.HasGroupParticipation;
			else
				output.HasGroupParticipation = null;

			if ( input.IsProctored != null )
				output.IsProctored = ( bool )	input.IsProctored;
			else
				output.IsProctored = null;

            output.DeliveryTypeDescription = input.DeliveryTypeDescription;
            output.ProcessStandardsDescription = input.ProcessStandardsDescription;
            output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );
            output.ScoringMethodDescription = input.ScoringMethodDescription;
            output.ScoringMethodExampleDescription = input.ScoringMethodExampleDescription;

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

        public static void HandleUrlFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			//17-11-27 Added a requirement check for these in the required section
			output.AvailableOnlineAt = AssignValidUrlListAsStringList( input.AvailableOnlineAt, "Available Online At", ref messages );
			output.AvailabilityListing = AssignValidUrlListAsStringList( input.AvailabilityListing, "Availability Listing", ref messages );


			output.ExternalResearch = AssignValidUrlListAsStringList( input.ExternalResearch, "External Research", ref messages );
			//foreach ( var field in input.ExternalResearch )
   //             output.ExternalResearch = HandleSingleUrlField( field, "ExternalResearch", ref messages );

			output.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "Process Standards", ref messages );
			output.ScoringMethodExample = AssignValidUrlAsString( input.ScoringMethodExample, "Scoring Method Example", ref messages );

		}

		public static void HandleAssertedINsProperties( InputEntity input, OutputEntity output, RJ.EntityReferenceHelper helper, ref List<string> messages )
		{
			RJ.JurisdictionProfile jp = new RJ.JurisdictionProfile();
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
				}
			}

		} //
		#region === CredentialAlignmentObject ===
		public static void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			output.DeliveryType = FormatCredentialAlignmentVocabs( "deliveryType", input.DeliveryType, ref messages );
			output.AssessmentMethodType = FormatCredentialAlignmentVocabs( "assessmentMethodType", input.AssessmentMethodType, ref messages );
			output.AssessmentUseType = FormatCredentialAlignmentVocabs( "assessmentUseType", input.AssessmentUseType, ref messages );
			output.ScoringMethodType = FormatCredentialAlignmentVocabs( "scoringMethodType", input.ScoringMethodType, ref messages );

            output.AudienceType = FormatCredentialAlignmentVocabs("audienceType", input.AudienceType, ref messages);
            
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

		}

        //see common methods in ServiceHelper
        #endregion


    }
}
