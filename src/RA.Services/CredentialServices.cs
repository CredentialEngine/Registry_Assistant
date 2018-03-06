using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
using RA.Models.Input;
using RJ = RA.Models.Json;
using EntityRequest = RA.Models.Input.CredentialRequest;
using InputEntity = RA.Models.Input.Credential;
using OutputEntity = RA.Models.Json.Credential;
using Utilities;

namespace RA.Services
{
    /// <summary>
    /// TODO - identify concrete methods here that could become common methods
    /// </summary>
    public class CredentialServices : ServiceHelper
    {
        static string status = "";
        static List<string> warnings = new List<string>();
        /// <summary>
        /// Publish a Credential to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        /// <param name="payload"></param>
        public static void Publish(EntityRequest request, string apiKey, ref bool isValid, ref List<string> messages, ref string payload, ref string registryEnvelopeId)
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
        ///Publish a Credential to the Credential Registry
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="helper"></param>
        public static void Publish(EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper)
        {
            isValid = true;
            string crEnvelopeId = request.RegistryEnvelopeId;
            //submitter is not a person for this api, rather the organization
            //may want to do a lookup via the api key?
            string submitter = "";
            List<string> messages = new List<string>();
            var output = new OutputEntity();
            if ( ToMap( request.Credential, output, ref messages ) )
            {
                if ( warnings.Count > 0 )
                    messages.AddRange( warnings );

                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
                CER cer = new CER("Credential", output.CredentialType, output.Ctid, helper.SerializedInput); 
                cer.PublisherAuthorizationToken = helper.ApiKey;
                cer.PublishingForOrgCtid = helper.OwnerCtid;
                if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32)
					cer.IsManagedRequest = true;

                string identifier = "Credential_" + request.Credential.Ctid;
                if ( cer.Publish(helper.Payload, submitter, identifier, ref status, ref crEnvelopeId) )
                {
                    //for now need to ensure envelopid is returned
                    helper.RegistryEnvelopeId = crEnvelopeId;
                    string msg = string.Format( "<p>Published credential: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId);
					NotifyOnPublish( "Credential", msg );
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
                messages.Add( status );
                helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
            }

            helper.SetMessages(messages);
        }

        /// <summary>
        /// Used for demo page - NA 6/5/2017
        /// ACTUALLY SHOULD NOT ALLOW PUBLISH FROM DEMO - at least until can publish without envelope id.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isValid"></param>
        /// <param name="messages"></param>
        /// <param name="rawResponse"></param>
        /// <param name="forceSkipValidation"></param>
        /// <returns></returns>
        //public static string DemoPublish( OutputEntity input, ref bool isValid,
        //    ref List<string> messages,
        //    ref string rawResponse,
        //    bool forceSkipValidation = false )
        //{
        //    isValid = true;
        //    var crEnvelopeId = "";
        //    var payload = JsonConvert.SerializeObject( input, ServiceHelper.GetJsonSettings() );
        //    var identifier = "credential_" + input.Ctid;
        //    //crEnvelopeId = input.RegistryEnvelopeId;

        //    rawResponse = new CER().Publish( payload, "", identifier, ref isValid, ref status, ref crEnvelopeId, forceSkipValidation );

        //    return crEnvelopeId;
        //}

        public static string FormatAsJson( EntityRequest request, ref bool isValid, ref List<string> messages )
        {
            return FormatAsJson( request.Credential, ref isValid, ref messages );
        }
        //
        public static string FormatAsJson( InputEntity input, ref bool isValid, ref List<string> messages )
        {
            var output = new OutputEntity();
            string payload = "";
            isValid = true;

            if ( ToMap( input, output, ref messages ) )
            {
                payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
                if ( warnings.Count > 0 )
                    messages.AddRange( warnings );
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
			CurrentEntityType = "Credential";
			bool isValid = true;
			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			try
			{
				HandleRequiredFields( input, output, ref messages );

				HandleLiteralFields( input, output, ref messages );

				HandleUrlFields( input, output, ref messages );
				//org
				HandleOrgProperties( input, output, ref messages );

				output.Jurisdiction = MapJurisdictions( input.Jurisdiction, ref messages );

				HandleAssertedINsProperties( input, output, helper, ref messages );

				//costs
				output.EstimatedCost = FormatCosts( input.EstimatedCost, ref messages );

				output.EstimatedDuration = FormatDuration( input.EstimatedDuration, ref messages );
				output.RenewalFrequency = FormatDurationItem( input.RenewalFrequency, "RenewalFrequency", ref messages );

				if ( input.Keyword != null && input.Keyword.Count > 0 )
					output.Keyword = input.Keyword;
				else
					output.Keyword = null;

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "ProcessStandards", ref messages );
				output.ProcessStandardsDescription = ( input.ProcessStandardsDescription ?? "" ).Length > 0 ? input.ProcessStandardsDescription : null;

				//output.CommonConditions = FormatEntityReferences( input.CommonConditions, RJ.ConditionManifest.classType, false, ref messages );
				//output.CommonCosts = FormatEntityReferences( input.CommonCosts, RJ.CostManifest.classType, false, ref messages );
				output.CommonConditions = AssignValidUrlListAsStringList( input.CommonConditions, "CommonConditions", ref messages );
				output.CommonCosts = AssignValidUrlListAsStringList( input.CommonCosts, "CommonCosts", ref messages );

				if ( input.HasPart.Count > 0 )
				{
					//RJ.Credential.classType
					output.HasPart = FormatEntityReferences( input.HasPart, "", false, ref messages );
				}
				if ( input.IsPartOf.Count > 0 )
				{
					//RJ.Credential.classType
					output.IsPartOf = FormatEntityReferences( input.IsPartOf, "", false, ref messages );
				}
				output.AvailableAt = FormatAvailableAtList( input.AvailableAt, ref messages );
               
				output.Recommends = FormatConditionProfile( input.Recommends, ref messages );
				output.Requires = FormatConditionProfile( input.Requires, ref messages );
				output.Corequisite = FormatConditionProfile( input.Corequisite, ref messages );
                output.Renewal = FormatConditionProfile( input.Renewal, ref messages );

                output.AdministrationProcess = FormatProcessProfile( input.AdministrationProcess, ref messages );
				output.MaintenanceProcess = FormatProcessProfile( input.MaintenanceProcess, ref messages );
				output.DevelopmentProcess = FormatProcessProfile( input.DevelopmentProcess, ref messages );
				output.ComplaintProcess = FormatProcessProfile( input.ComplaintProcess, ref messages );
				output.AppealProcess = FormatProcessProfile( input.AppealProcess, ref messages );
				output.ReviewProcess = FormatProcessProfile( input.ReviewProcess, ref messages );
				output.RevocationProcess = FormatProcessProfile( input.RevocationProcess, ref messages );

				output.FinancialAssistance = MapFinancialAssitance( input.FinancialAssistance, ref messages );

				output.AdvancedStandingFrom = FormatConnections( input.AdvancedStandingFrom, ref messages );
				output.IsAdvancedStandingFor = FormatConnections( input.IsAdvancedStandingFor, ref messages );

				output.PreparationFrom = FormatConnections( input.PreparationFrom, ref messages );
				output.IsPreparationFor = FormatConnections( input.IsPreparationFor, ref messages );

				output.IsRequiredFor = FormatConnections( input.IsRequiredFor, ref messages );
				output.IsRecommendedFor = FormatConnections( input.IsRecommendedFor, ref messages );

				if ( input.Revocation.Count > 0 )
				{
					var list = new List<Models.Json.RevocationProfile>();
					foreach ( var r in input.Revocation )
					{
						var rp = new Models.Json.RevocationProfile();
						rp.Description = r.Description;
						rp.DateEffective = MapDate( r.DateEffective, "DateEffective", ref messages );
						rp.RevocationCriteria = AssignValidUrlAsString( r.RevocationCriteria, "RevocationCriteria", ref messages );
						rp.RevocationCriteriaDescription = r.RevocationCriteriaDescription;
						rp.Jurisdiction = MapJurisdictions( r.Jurisdiction, ref messages );
						list.Add( rp );
					}
					output.Revocation = list;
				}
				

			}
			catch ( Exception ex )
			{
				LogError( ex, "CredentialServices.ToMap" );
				messages.Add( ex.Message );
			}
            //how output handle warning messages?
            if ( messages.Count > 0 )
            {
                isValid = false;
                if ( warnings.Count > 0 )
                    messages.AddRange( warnings );
            }

            return isValid;
        }

        public static void HandleOrgProperties( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			
			output.CopyrightHolder = FormatOrganizationReferenceToList( input.CopyrightHolder, "Copyright Holder", false, ref messages );

            //other roles
            output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages );
            output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
			//moved offered by to the required section
            //output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );
            output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
            output.RevokedBy = FormatOrganizationReferences( input.RevokedBy, "Revoked By", false, ref messages );
            output.RenewedBy = FormatOrganizationReferences( input.RenewedBy, "Renewed By", false, ref messages );
            output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages );

        }


        public static bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            bool isValid = true;
            string property = "";

            //todo determine if will generate where not found
            if ( string.IsNullOrWhiteSpace( input.Ctid ) && GeneratingCtidIfNotFound() )
                input.Ctid = GenerateCtid();

            if ( IsCtidValid( input.Ctid, ref messages ) )
            {
                output.Ctid = input.Ctid;
                output.CtdlId = idUrl + output.Ctid;
				CurrentCtid = input.Ctid;
			}
			//required
			if ( string.IsNullOrWhiteSpace( input.Name ) )
			{
				messages.Add( FormatMessage( "Error - A name must be entered for Credential with CTID: '{0}'.", input.Ctid ) );
			}
			else
			{
				output.Name = input.Name;
				CurrentEntityName = input.Name;
			}
            if ( string.IsNullOrWhiteSpace( input.Description ) )
            {
                messages.Add( FormatMessage("Error - A  description must be entered for Credential '{0}'.", input.Name) );
            }
            else
                output.Description = input.Description;

            if ( string.IsNullOrWhiteSpace( input.CredentialType ) )
            {
				messages.Add( FormatMessage( "Error - A credential type must be entered for Credential '{0}'.", input.Name ) );
			}
            else
            {
				string validSchema = "";

				if ( input.CredentialType.IndexOf( "ceterms:" ) == -1 )
                    input.CredentialType = "ceterms:" + input.CredentialType.Trim();
                property = input.CredentialType;
				//if ( ValidationServices.IsCredentialTypeValid( "credentialType", ref property ) )
				//{
				//    output.CredentialType = property;
				//}
				if ( ValidationServices.IsValidCredentialType( property, ref validSchema ) )
				{
					output.CredentialType = validSchema;
				}
				else
                {
					messages.Add( FormatMessage( "Error - The credential type: ({0}) is invalid for Credential '{1}'.", input.CredentialType, input.Name ) );
				}
            }

			//now literal
			output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

			//need either ownedBy OR offeredBy
			output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages );
			output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

			if (output.OwnedBy == null && output.OfferedBy == null)
				messages.Add( string.Format( "Error - At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Credential: '{0}'", input.Name ) );

			return isValid;
        }
	
		public static void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            //18-02-20 - changed definition of AlternateNameto a list 
            if ( input.AlternateName != null && input.AlternateName.Count > 0)
                output.AlternateName = input.AlternateName;
            //contniue to handle single value
            //if ( !string.IsNullOrWhiteSpace( input.AlternateName ) )
            //{
            //    output.AlternateName.Add( input.AlternateName );
            //    //would like to return a warning
            //    warnings.Add( "WARNING - Credential AlternateName string is obsolete. Please use the AlternateNames List" );
            //}
            //now literal
            output.CodedNotation = AssignListToString( input.CodedNotation );

			output.CredentialId = ( input.CredentialId ?? "" ).Length > 0 ? input.CredentialId : null;
            output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );
			if ( input.InLanguage != null && input.InLanguage.Count > 0 )
				output.InLanguage = input.InLanguage;

			//placeholder: VersionIdentifier (already a list, the input needs output change
			output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier );
		}

        public static void HandleUrlFields( InputEntity from, OutputEntity to, ref List<string> messages )
        {


            to.AvailableOnlineAt = AssignValidUrlAsStringList( from.AvailableOnlineAt, "AvailableOnlineAt", ref messages );
            to.AvailabilityListing = AssignValidUrlAsStringList( from.AvailabilityListing, "AvailabilityListing", ref messages );

            to.Image = AssignValidUrlAsString( from.Image, "Image", ref messages );
            to.PreviousVersion = AssignValidUrlAsString( from.PreviousVersion, "PreviousVersion", ref messages );
            to.LatestVersion = AssignValidUrlAsString( from.LatestVersion, "LatestVersion", ref messages );


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
					if ( item.AssertsRenewedIn )
					{
						jp = MapJurisdictionAssertions( item, ref helper, ref messages );
						output.RenewedIn = JurisdictionProfileAdd( jp, output.RenewedIn );
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
		public static void HandleCredentialAlignmentFields( InputEntity from, OutputEntity to, ref List<string> messages )
        {
            to.Subject = FormatCredentialAlignmentListFromStrings( from.Subject );

			//can't depend on the codes being SOC
			to.OccupationType = FormatCredentialAlignmentListFromList( from.OccupationType, true, ref messages );
			//if ( from.OccupationType != null && from.OccupationType.Count > 0 )
   //         {
   //             //need to add a framework
   //             foreach ( FrameworkItem item in from.OccupationType )
   //             {
   //                 to.OccupationType.Add( FormatCredentialAlignment( item, true ) );
   //             }
   //         }
   //         else
   //             to.OccupationType = null;

			//can't depend on the codes being NAICS??
			to.IndustryType = FormatCredentialAlignmentListFromList( from.IndustryType, true, ref messages );
			if ( from.Naics != null && from.Naics.Count > 0 )
				to.Naics = from.Naics;
			else
				to.Naics = null;

			//if ( from.IndustryType != null && from.IndustryType.Count > 0 )
			//         {
			//             foreach ( FrameworkItem item in from.IndustryType )
			//             {
			//                 to.IndustryType.Add( FormatCredentialAlignment( item, true ) );
			//             }
			//         }
			//         else
			//             to.IndustryType = null;

			if ( !string.IsNullOrWhiteSpace( from.CredentialStatusType ) )
            {
                to.CredentialStatusType = FormatCredentialAlignment( "credentialStatusType", from.CredentialStatusType, ref messages ) ;
            }
            else
                to.CredentialStatusType = null;

            //
            to.AudienceLevel = FormatCredentialAlignmentVocabs( "audienceLevelType", from.AudienceLevelType, ref messages );

            if ( IsValidDegreeType( to.CredentialType ) )
            {
                to.DegreeConcentration = FormatCredentialAlignmentListFromStrings( from.DegreeConcentration );

                to.DegreeMajor = FormatCredentialAlignmentListFromStrings( from.DegreeMajor );

                to.DegreeMinor = FormatCredentialAlignmentListFromStrings( from.DegreeMinor );
            }


        }


        private static bool IsValidDegreeType( string credentialType )
        {
            try
            {
                var validDegreeTypes = new List<string>() { "ceterms:AssociateDegree", "ceterms:BachelorDegree", "ceterms:Degree", "ceterms:DoctoralDegree", "ceterms:MasterDegree" };
                if ( validDegreeTypes.Contains( credentialType ) )
                {
                    return true;
                }

            }
            catch
            {

            }
            return false;
        }


        //see common methods in ServiceHelper
        #endregion

        public static InputEntity DeSerializedFormat( string serialized )
        {
            InputEntity credential = null;
            try
            {
                credential = JsonConvert.DeserializeObject<InputEntity>( serialized );
            }
            catch ( Exception ex )
            {

            }

            return credential;
        } //
    }
}
