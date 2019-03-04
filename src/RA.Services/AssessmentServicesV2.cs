using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using RA.Models.Input;
using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.AssessmentRequest;
using InputEntity = RA.Models.Input.Assessment;
using OutputEntity = RA.Models.JsonV2.AssessmentProfile;
using OutputGraph = RA.Models.JsonV2.GraphContainer;

using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class AssessmentServicesV2 : ServiceHelperV2
    {
        static string status = "";
        static List<string> warnings = new List<string>();
        
        /// <summary>
        /// Publish an Assessment to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        /// <param name="payload"></param>
        public void Publish( EntityRequest request, string apiKey, ref bool isValid, ref List<string> messages, ref string payload, ref string registryEnvelopeId )
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
                if ( warnings.Count > 0 )
                    messages.AddRange( warnings );

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
                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

				CER cer = new CER( "Assessment", output.Type, output.Ctid, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					PublishingForOrgCtid = helper.OwnerCtid
				};

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;
				bool recordWasFound = false;
				bool usedCEKeys = false;
				string message = "";
				var result = HistoryServices.GetMostRecentHistory( "Assessment", output.Ctid, ref recordWasFound, ref usedCEKeys, ref message );
				if ( recordWasFound ) //found previous
				{
					if ( usedCEKeys && cer.IsManagedRequest )
					{
						LoggingHelper.DoTrace( 5, "Assessment publish. Was managed request. Overriding to CE publish." );
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
                //helper.Payload = JsonConvert.SerializeObject(output, GetJsonSettings());
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
                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.Type;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }

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
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="messages"></param>
        /// <returns></returns>        
        public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
        {
			CurrentEntityType = "Assessment";
			bool isValid = true;
            RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
            InputEntity input = request.Assessment;
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
                output.InLanguage = PopulateInLanguage( input.InLanguage, "Assessment", input.Name, hasDefaultLanguage, ref messages );

                HandleRequiredFields( input, output, ref messages );

				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );

                output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Assessment Keywords", ref messages );

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
				output.CommonConditions = AssignValidUrlListAsStringList( input.CommonConditions, "CommonConditions", ref messages, false );
				output.CommonCosts = AssignValidUrlListAsStringList( input.CommonCosts, "CommonCosts", ref messages, false );

				output.FinancialAssistance = MapFinancialAssitance( input.FinancialAssistance, ref messages );

				output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier, ref messages );
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
        public void HandleOrgProperties( InputEntity input, OutputEntity output, ref List<string> messages )
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
        public bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
            ///string property = "";

            output.Ctid = FormatCtid(input.Ctid, ref messages);
            output.CtdlId = idBaseUrl + output.Ctid;

            //required
            if ( string.IsNullOrWhiteSpace( input.Name ) )
            {
                if ( input.Name_Map == null || input.Name_Map.Count == 0 )
                {
                    messages.Add( FormatMessage( "Error - A name or Name_Map must be entered for Assessment with CTID: '{0}'.", input.Ctid ) );
                }
                else
                {
                    output.Name = AssignLanguageMap( input.Name_Map, "Assessment Name", ref messages );
                    CurrentEntityName = GetFirstItemValue( output.Name );
                }
            }
            else
            {
                output.Name = Assign( input.Name, DefaultLanguageForMaps );
                CurrentEntityName = input.Name;
            }
            output.Description = AssignLanguageMap( ConvertSpecialInput( input.Description ), input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

            //now literal
            output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

            output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages );
            //output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", false, ref messages );
			output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

			if (output.OwnedBy == null && output.OfferedBy == null )
			{
				messages.Add( string.Format("Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Assessment: '{0}'", input.Name ));
			}
            //if ( input.InLanguage != null && input.InLanguage.Count > 0 )
            //{
            //    output.InLanguage = input.InLanguage;
            //    if ( input.InLanguage.Count == 1 )
            //    {
            //        //need to check if already set from the request.DefaultLanguage
            //        DefaultLanguageForMaps = input.InLanguage[ 0 ];
            //    }
            //    else
            //    {
            //        //if (input.lan)
            //    }
            //}
            //else
            //{
            //    messages.Add( string.Format( "At least one language (InLanguage) must be provided for Assessment: '{0}'", input.Name ) );
            //}
            if ( (input.AvailableOnlineAt == null		|| input.AvailableOnlineAt.Count ==0 ) &&
				 ( input.AvailabilityListing == null	|| input.AvailabilityListing.Count == 0 ) &&
				 ( input.AvailableAt == null			|| input.AvailableAt.Count == 0 )	 )
				messages.Add( string.Format( "Error - At least one of: 'Available Online At', 'Availability Listing', or 'Available At' (address) must be provided for Assessment: '{0}'", input.Name ) );

			return isValid;
		}
		

        public void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            //now literal, should only be one
            //output.CodedNotation = AssignListToString( input.CodedNotation );
            output.CodedNotation = input.CodedNotation;

            output.AssessmentExample = AssignValidUrlAsString( input.AssessmentExample, "AssessmentExample", ref messages, false );
            output.AssessmentExampleDescription = AssignLanguageMap( ConvertSpecialInput( input.AssessmentExampleDescription ), input.AssessmentExampleDescription_Map, "AssessmentExampleDescription", DefaultLanguageForMaps,  ref messages );

            output.AssessmentOutput = AssignLanguageMap( ConvertSpecialInput( input.AssessmentOutput ), input.AssessmentOutput_Map, "AssessmentOutput", DefaultLanguageForMaps, ref messages );


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

            output.DeliveryTypeDescription = AssignLanguageMap( ConvertSpecialInput( input.DeliveryTypeDescription ), input.DeliveryTypeDescription_Map, "DeliveryTypeDescription", DefaultLanguageForMaps,  ref messages );
            output.ProcessStandardsDescription = AssignLanguageMap( ConvertSpecialInput( input.ProcessStandardsDescription ), input.ProcessStandardsDescription_Map, "ProcessStandardsDescription",  DefaultLanguageForMaps, ref messages );

            output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );
            output.ScoringMethodDescription = AssignLanguageMap( ConvertSpecialInput( input.ScoringMethodDescription ), input.ScoringMethodDescription_Map, "ScoringMethodDescription", DefaultLanguageForMaps, ref messages );
            output.ScoringMethodExampleDescription = AssignLanguageMap( ConvertSpecialInput( input.ScoringMethodExampleDescription ), input.ScoringMethodExampleDescription_Map, "ScoringMethodExampleDescription", DefaultLanguageForMaps, ref messages );

            if ( ValidateCreditUnitOrHoursProperties( input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref messages ) )
			{
                output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialInput( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map,"CreditUnitTypeDescription", DefaultLanguageForMaps,  ref messages, false );
                //credential alignment object
                if ( !string.IsNullOrWhiteSpace( input.CreditUnitType ) )
				{
					output.CreditUnitType =  FormatCredentialAlignment( "creditUnitType", input.CreditUnitType, ref messages ) ;
				}
				else
					output.CreditUnitType = null;

				output.CreditUnitValue = input.CreditUnitValue;
                output.CreditHourType = AssignLanguageMap( ConvertSpecialInput( input.CreditHourType ), input.CreditHourType_Map, "CreditHourType", DefaultLanguageForMaps, ref messages, false );

                output.CreditHourValue = input.CreditHourValue;
			}
			else
			{
				output.CreditUnitType = null;
			}

		}

        public void HandleUrlFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			//17-11-27 Added a requirement check for these in the required section
			output.AvailableOnlineAt = AssignValidUrlListAsStringList( input.AvailableOnlineAt, "Available Online At", ref messages );
			output.AvailabilityListing = AssignValidUrlListAsStringList( input.AvailabilityListing, "Availability Listing", ref messages );


			output.ExternalResearch = AssignValidUrlListAsStringList( input.ExternalResearch, "External Research", ref messages );
			//foreach ( var field in input.ExternalResearch )
   //             output.ExternalResearch = HandleSingleUrlField( field, "ExternalResearch", ref messages );

			output.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "Process Standards", ref messages, false );
			output.ScoringMethodExample = AssignValidUrlAsString( input.ScoringMethodExample, "Scoring Method Example", ref messages, false );

		}

		public void HandleAssertedINsProperties( InputEntity input, OutputEntity output, RJ.EntityReferenceHelper helper, ref List<string> messages )
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
		public void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject );

			output.DeliveryType = FormatCredentialAlignmentVocabs( "deliveryType", input.DeliveryType, ref messages );
			output.AssessmentMethodType = FormatCredentialAlignmentVocabs( "assessmentMethodType", input.AssessmentMethodType, ref messages );
			output.AssessmentUseType = FormatCredentialAlignmentVocabs( "assessmentUseType", input.AssessmentUseType, ref messages );
			output.ScoringMethodType = FormatCredentialAlignmentVocabs( "scoringMethodType", input.ScoringMethodType, ref messages );

            output.AudienceType = FormatCredentialAlignmentVocabs("audienceType", input.AudienceType, ref messages);

			//frameworks
			//can't depend on the codes being SOC
			output.OccupationType = FormatCredentialAlignmentListFromFrameworkItemList( input.OccupationType, true, ref messages );

			//output.AlternativeOccupationType = AssignLanguageMapList( input.AlternativeOccupationType, input.AlternativeOccupationType_Map, "Credential AlternativeOccupationType", ref messages );

			//can't depend on the codes being NAICS??
			output.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( input.IndustryType, true, ref messages );

			//output.AlternativeIndustryType = AssignLanguageMapList( input.AlternativeIndustryType, input.AlternativeIndustryType_Map, "Credential AlternativeIndustryType", ref messages );
			//
			output.InstructionalProgramType = FormatCredentialAlignmentListFromFrameworkItemList( input.InstructionalProgramType, true, ref messages, "Classification of Instructional Programs", "https://nces.ed.gov/ipeds/cipcode/Default.aspx?y=55" );
			//
			//output.AlternativeInstructionalProgramType = AssignLanguageMapList( input.AlternativeInstructionalProgramType, input.AlternativeInstructionalProgramType_Map, "Credential AlternativeInstructionalProgramType", ref messages );
			//
		}

		//see common methods in ServiceHelper
		#endregion


	}
}
