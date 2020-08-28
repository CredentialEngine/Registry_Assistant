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
		string className = "AssessmentServicesV2";
        /// <summary>
        /// Publish an Assessment to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
        {
			LoggingHelper.DoTrace( 6, string.Format("AssessmentServicesV2.Publish Request for: {0} Started.", request.Assessment.Name ));
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
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
				og.CTID = output.CTID;
                og.Type = output.Type;
                og.Context = ctdlContext;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );

				CER cer = new CER( "Assessment", output.Type, output.CTID, helper.SerializedInput )
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
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ))
					{
						cer.PublishingByOrgCtid = publisherCTID;  //this could be set in method if whole object sent
					} else
					{
						//should be an error message returned

						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "Assessment.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.CTID, cer.PublisherAuthorizationToken ) );
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
				if ( !SupportServices.ValidateAgainstPastRequest( "Assessment", output.CTID, ref cer, ref messages ) )
				{
					isValid = false;
					//helper.SetWarningMessages( warningMessages );
					//helper.SetMessages( messages );
					//return; //===================
				}
				else
				{
					string identifier = "Assessment_" + request.Assessment.Ctid;

					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Assessment: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.CTID, crEnvelopeId );
						NotifyOnPublish( "Assessment", msg );
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
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.CTID, Community);
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
			DateTime mappingStarted = DateTime.Now;
			CurrentEntityType = "Assessment";
			bool isValid = true;
			Community = request.Community ?? "";

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

				//
				output.LifecycleStatusType = AssignStatusType( "Assessment LifecycleStatusType", input.LifecycleStatusType, ref messages );
				//
				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );

                output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Assessment Keywords", ref messages );

				output.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );

				output.AvailableAt = FormatAvailableAtList( input.AvailableAt, ref messages );

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );

				output.EstimatedDuration = FormatDuration( input.EstimatedDuration, "Assessment.EstimatedDuration", ref messages );

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
				output.CommonConditions = AssignRegistryResourceURIsListAsStringList( input.CommonConditions, "CommonConditions", ref messages, false );
				output.CommonCosts = AssignRegistryResourceURIsListAsStringList( input.CommonCosts, "CommonCosts", ref messages, false );

				//output.FinancialAssistanceOLD = MapFinancialAssistance( input.FinancialAssistanceOLD, ref messages );
				output.FinancialAssistance = MapFinancialAssistance( input.FinancialAssistance, ref messages, "Assessment" );

				output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier, ref messages );
                HandleOrgProperties( input, output, ref messages );

            }
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "AssessmentServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			TimeSpan duration = DateTime.Now.Subtract( mappingStarted );
			if ( duration.TotalSeconds > 2 )
				LoggingHelper.DoTrace( 1, string.Format( "{0}.ToMap *******Mapping took a little longer: elapsed: {1:N2} seconds. Competencies: {2}", className, duration.TotalSeconds, ( input.Assesses != null && input.Assesses.Count() > 0) ? input.Assesses.Count() : 0 ) ); ;

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

			CurrentCtid = output.CTID = FormatCtid(input.Ctid, "Assessment Profile", ref messages);
            //output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.Ctid, Community);
			output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.CTID, Community);

			//required
			output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Assessment.Name", DefaultLanguageForMaps, ref messages, true, 3 );
			CurrentEntityName = GetFirstItemValue( output.Name );
            output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

            //now literal
            output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

            output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages, false, true );
            //output.OwnedBy = FormatOrganizationReferenceToList( input.OwnedBy, "Owning Organization", false, ref messages );
			output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages, false, true );

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
		

        public void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            //now literal, should only be one
            //output.CodedNotation = AssignListToString( input.CodedNotation );
            output.CodedNotation = input.CodedNotation;

            output.AssessmentExample = AssignValidUrlAsString( input.AssessmentExample, "AssessmentExample", ref messages, false );
            output.AssessmentExampleDescription = AssignLanguageMap( ConvertSpecialCharacters( input.AssessmentExampleDescription ), input.AssessmentExampleDescription_Map, "AssessmentExampleDescription", DefaultLanguageForMaps,  ref messages );

            output.AssessmentOutput = AssignLanguageMap( ConvertSpecialCharacters( input.AssessmentOutput ), input.AssessmentOutput_Map, "AssessmentOutput", DefaultLanguageForMaps, ref messages );


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

            output.DeliveryTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.DeliveryTypeDescription ), input.DeliveryTypeDescription_Map, "DeliveryTypeDescription", DefaultLanguageForMaps,  ref messages );
            output.ProcessStandardsDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ProcessStandardsDescription ), input.ProcessStandardsDescription_Map, "ProcessStandardsDescription",  DefaultLanguageForMaps, ref messages );

            output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );
            output.ScoringMethodDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ScoringMethodDescription ), input.ScoringMethodDescription_Map, "ScoringMethodDescription", DefaultLanguageForMaps, ref messages );
            output.ScoringMethodExampleDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ScoringMethodExampleDescription ), input.ScoringMethodExampleDescription_Map, "ScoringMethodExampleDescription", DefaultLanguageForMaps, ref messages );

			//
			//output.CreditUnitType = null;
			output.CreditValue = AssignQuantitiveValueToList( input.CreditValue, "CreditValue", "Assessment", ref messages );
			//at this point could have had no data, or bad data
			if ( output.CreditValue == null )
			{
				//check legacy
				//output.CreditValue = AssignQuantitiveValue( "Assessment", input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref messages );

				
			}
			//apparantly will still allow just a description. TBD: is it allowed if creditValue is provided?
			output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map, "CreditUnitTypeDescription", DefaultLanguageForMaps, ref messages );
			#region old credit code
			//
			//bool hasData = false;
			//RJ.QuantitativeValue qv = new RJ.QuantitativeValue();
			//if ( AssignQuantitiveValue( input.CreditValue, "CreditValue", "Assessment", ref qv, ref messages ) )
			//{
			//	output.CreditValue = new List<RJ.QuantitativeValue> { qv };
			//}
			////at this point could have had no data, or bad data
			//else if ( !usingQuantitiveValue )
			//{
			//	if ( ValidateCreditUnitOrHoursProperties( input.CreditHourValue, input.CreditHourType, input.CreditUnitType, input.CreditUnitValue, input.CreditUnitTypeDescription, ref hasData, ref messages ) )
			//	{
			//		output.CreditUnitTypeDescription = AssignLanguageMap( ConvertSpecialCharacters( input.CreditUnitTypeDescription ), input.CreditUnitTypeDescription_Map, "CreditUnitTypeDescription", DefaultLanguageForMaps, ref messages );
			//		//credential alignment object
			//		if ( !string.IsNullOrWhiteSpace( input.CreditUnitType ) )
			//		{
			//			output.CreditUnitType = FormatCredentialAlignment( "creditUnitType", input.CreditUnitType, ref messages );
			//		}
			//		output.CreditUnitValue = input.CreditUnitValue;
			//		output.CreditHourType = AssignLanguageMap( ConvertSpecialCharacters( input.CreditHourType ), input.CreditHourType_Map, "CreditHourType", DefaultLanguageForMaps, ref messages );
			//		output.CreditHourValue = input.CreditHourValue;
			//	}
			//}
			#endregion

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

			output.TargetLearningResource = AssignValidUrlListAsStringList( input.TargetLearningResource, "TargetLearningResource", ref messages );
		}

		public void HandleAssertedINsProperties( InputEntity input, OutputEntity output, RJ.EntityReferenceHelper helper, ref List<string> messages )
		{
			RJ.JurisdictionProfile jp = new RJ.JurisdictionProfile();
			//need to check with partners, and set date for sunsetting this approach
			//if( input.JurisdictionAssertions != null && input.JurisdictionAssertions.Count > 0 )
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
			//{
				output.AccreditedIn = MapJurisdictionAssertionsList( input.AccreditedIn, ref helper, ref messages );
				output.ApprovedIn = MapJurisdictionAssertionsList( input.ApprovedIn, ref helper, ref messages );
				output.OfferedIn = MapJurisdictionAssertionsList( input.OfferedIn, ref helper, ref messages );
				output.RecognizedIn = MapJurisdictionAssertionsList( input.RecognizedIn, ref helper, ref messages );
				output.RegulatedIn = MapJurisdictionAssertionsList( input.RegulatedIn, ref helper, ref messages );
			//}

		} //
		#region === CredentialAlignmentObject ===
		public void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject, input.Subject_Map );

			output.DeliveryType = FormatCredentialAlignmentVocabs( "deliveryType", input.DeliveryType, ref messages );
			output.AssessmentMethodType = FormatCredentialAlignmentVocabs( "assessmentMethodType", input.AssessmentMethodType, ref messages );
			//
			output.AssessmentMethodDescription = AssignLanguageMap( input.AssessmentMethodDescription, input.AssessmentMethodDescription_Map, "AssessmentMethodDescription", DefaultLanguageForMaps, ref messages, false, MinimumDescriptionLength );
			//
			output.LearningMethodDescription = AssignLanguageMap( input.LearningMethodDescription, input.LearningMethodDescription_Map, "LearningMethodDescription", DefaultLanguageForMaps, ref messages, false, MinimumDescriptionLength );
			//
			output.AssessmentUseType = FormatCredentialAlignmentVocabs( "assessmentUseType", input.AssessmentUseType, ref messages );
			output.ScoringMethodType = FormatCredentialAlignmentVocabs( "scoringMethodType", input.ScoringMethodType, ref messages );

            output.AudienceType = FormatCredentialAlignmentVocabs("audienceType", input.AudienceType, ref messages);
			output.AudienceLevelType = FormatCredentialAlignmentVocabs( "audienceLevelType", input.AudienceLevelType, ref messages );

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
			//=== instructional programs ===============================================================
			output.InstructionalProgramType = FormatCredentialAlignmentListFromFrameworkItemList( input.InstructionalProgramType, true, ref messages, "Classification of Instructional Programs", "https://nces.ed.gov/ipeds/cipcode/Default.aspx?y=55" );
			//append to InstructionalProgramType
			output.InstructionalProgramType = AppendCredentialAlignmentListFromList( input.AlternativeInstructionalProgramType, null, "", "", "AlternativeInstructionalProgramType", output.InstructionalProgramType, ref messages );

			//NEW - allow a list of CIP codes, and resolve
			output.InstructionalProgramType = HandleListOfCip_Codes( input.CIP_Codes, output.InstructionalProgramType, ref messages );
			//
			//output.AlternativeInstructionalProgramType = AssignLanguageMapList( input.AlternativeInstructionalProgramType, input.AlternativeInstructionalProgramType_Map, "Credential AlternativeInstructionalProgramType", ref messages );
		}

		//see common methods in ServiceHelper
		#endregion


	}
}
