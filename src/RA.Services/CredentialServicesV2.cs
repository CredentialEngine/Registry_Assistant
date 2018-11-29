using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;

using RJ = RA.Models.JsonV2;
using EntityRequest = RA.Models.Input.CredentialRequest;
using InputEntity = RA.Models.Input.Credential;
using OutputEntity = RA.Models.JsonV2.Credential;
//using OutputGraph = RA.Models.JsonV2.CredentialGraph;
using OutputGraph = RA.Models.JsonV2.GraphContainer;
using Utilities;

namespace RA.Services
{
    /// <summary>
    /// TODO - identify concrete methods here that could become common methods
    /// </summary>
    public class CredentialServicesV2 : ServiceHelperV2
    {
        static string status = "";
        static List<string> warnings = new List<string>();

        /// <summary>
        /// Publish a Credential to the Credential Registry as an @graph
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isValid"></param>
        /// <param name="helper"></param>
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

            if ( ToMap(request, output, ref messages) )
            {
                if ( warnings.Count > 0 )
                    messages.AddRange(warnings);

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
                og.Type = output.CredentialType;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject(og, GetJsonSettings());
                CER cer = new CER( "Credential", output.CredentialType, output.Ctid, helper.SerializedInput )
                {
                    PublisherAuthorizationToken = helper.ApiKey,
                    PublishingForOrgCtid = helper.OwnerCtid
                };
                if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
                    cer.IsManagedRequest = true;

                string identifier = "Credential_" + request.Credential.Ctid;
                if ( cer.Publish(helper.Payload, submitter, identifier, ref status, ref crEnvelopeId) )
                {
                    //for now need to ensure envelopid is returned
                    helper.RegistryEnvelopeId = crEnvelopeId;
                    string msg = string.Format("<p>Published credential: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId);
                    NotifyOnPublish("Credential", msg);
                }
                else
                {
                    if ( !string.IsNullOrWhiteSpace(( status ?? "Unknown Error" )) )
                        messages.Add(status);
                    isValid = false;

                }
            }
            else
            {
                isValid = false;
                if ( !string.IsNullOrWhiteSpace(status) )
                    messages.Add(status);
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
                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.CredentialType;
                og.Context = output.Context;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }

            helper.SetMessages(messages);
        }

        public string FormatAsJson(EntityRequest request, ref bool isValid, ref List<string> messages)
        {
            OutputGraph og = new OutputGraph();

            var output = new OutputEntity();
            string payload = "";
            isValid = true;
            IsAPublishRequest = false;

            if ( ToMap(request, output, ref messages) )
            {

                og.Graph.Add( output );
                if (BlankNodes != null && BlankNodes.Count > 0)
                {
                    foreach (var item in BlankNodes)
                    {
                        og.Graph.Add( item );
                    }
                }
                

                og.CtdlId = credRegistryGraphUrl + output.Ctid;
                og.CTID = output.Ctid;
                og.Type = output.CredentialType;
                og.Context = output.Context;

                payload = JsonConvert.SerializeObject(og, GetJsonSettings());
                if ( warnings.Count > 0 )
                    messages.AddRange(warnings);
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
                og.Type = output.CredentialType;
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
			CurrentEntityType = "Credential";
			bool isValid = true;
            string property = "";
            RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
            InputEntity input = request.Credential;
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

            if ( request.NotConvertingFromResourceLinkToGraphLink )
                ConvertingFromResourceLinkToGraphLink = false;

            try
			{
                //HandleRequiredFields
                //HandleRequiredFields( input, output, ref messages );
                output.Ctid = FormatCtid( input.Ctid, ref messages );
                output.CtdlId = idBaseUrl + output.Ctid;
                //establish language. make a common method
                output.InLanguage = PopulateInLanguage( input.InLanguage, "Credential", input.Name, hasDefaultLanguage, ref messages );

                //required
                if ( string.IsNullOrWhiteSpace( input.Name ) )
                {
                    if ( input.Name_Map == null || input.Name_Map.Count == 0 )
                    {
                        messages.Add( FormatMessage( "Error - A name or Name_Map must be entered for Credential with CTID: '{0}'.", input.Ctid ) );
                    } else
                    {
                        output.Name = AssignLanguageMap( input.Name_Map, "Credential Name", ref messages);
                        CurrentEntityName = GetFirstItemValue( output.Name );
                    }
                }
                else
                {
                    output.Name = Assign( input.Name, DefaultLanguageForMaps );
                    CurrentEntityName = input.Name;
                }
                output.Description = AssignLanguageMap( ConvertSpecialInput( input.Description ), input.Description_Map, "Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

                
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

                if ( output.OwnedBy == null && output.OfferedBy == null )
                    messages.Add( string.Format( "At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Credential: '{0}'", input.Name ) );
                //
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

                output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Credential Keywords", ref messages );

                //if ( input.Keyword != null && input.Keyword.Count > 0 )
                //{
                //    output.Keyword = FormatLanguageMapList( input.Keyword, "Keywords", ref messages );
                //}
                //else if ( input.Keyword_Map != null && input.Keyword_Map.Count > 0 )
                //{
                //    output.Keyword = FormatLanguageMapList( input.Keyword_Map, "Keywords", ref messages );
                //}
                //else
                //    output.Keyword = null;

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "ProcessStandards", ref messages );
                output.ProcessStandardsDescription = AssignLanguageMap( ConvertSpecialInput( input.ProcessStandardsDescription ), input.ProcessStandardsDescription_Map,"ProcessStandardsDescription",  DefaultLanguageForMaps, ref messages );

                output.CommonConditions = AssignValidUrlListAsStringList( input.CommonConditions, "CommonConditions", ref messages );
				output.CommonCosts = AssignValidUrlListAsStringList( input.CommonCosts, "CommonCosts", ref messages );

				if ( input.HasPart.Count > 0 )
				{
					//RJ.Credential.classType
					output.HasPart = FormatEntityReferencesList( input.HasPart, "", false, ref messages );
				}
				if ( input.IsPartOf.Count > 0 )
				{
					//RJ.Credential.classType
					output.IsPartOf = FormatEntityReferencesList( input.IsPartOf, "", false, ref messages );
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
					var list = new List<RJ.RevocationProfile>();
					foreach ( var r in input.Revocation )
					{
						var rp = new RJ.RevocationProfile();
						rp.Description = Assign(r.Description, DefaultLanguageForMaps);
						rp.DateEffective = MapDate( r.DateEffective, "DateEffective", ref messages );
						rp.RevocationCriteria = AssignValidUrlAsString( r.RevocationCriteria, "RevocationCriteria", ref messages );
						rp.RevocationCriteriaDescription = Assign(r.RevocationCriteriaDescription, DefaultLanguageForMaps);
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

        public void HandleOrgProperties( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			
			output.CopyrightHolder = FormatOrganizationReferenceToList( input.CopyrightHolder, "Copyright Holder", false, ref messages );

            //other roles
            output.AccreditedBy = FormatOrganizationReferences( input.AccreditedBy, "Accredited By", false, ref messages, true);
            output.ApprovedBy = FormatOrganizationReferences( input.ApprovedBy, "Approved By", false, ref messages );
			//moved offered by to the required section
            //output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );
            output.RecognizedBy = FormatOrganizationReferences( input.RecognizedBy, "Recognized By", false, ref messages );
            output.RevokedBy = FormatOrganizationReferences( input.RevokedBy, "Revoked By", false, ref messages );
            output.RenewedBy = FormatOrganizationReferences( input.RenewedBy, "Renewed By", false, ref messages );
            output.RegulatedBy = FormatOrganizationReferences( input.RegulatedBy, "Regulated By", false, ref messages, true);

        }


   //     public bool HandleRequiredFields( InputEntity input, OutputEntity output, ref List<string> messages )
   //     {
   //         bool isValid = true;
   //         string property = "";

   //         output.Ctid = FormatCtid(input.Ctid, ref messages);
   //         output.CtdlId = idBaseUrl + output.Ctid;

   //         //todo determine if will generate where not found
   //         //         if ( string.IsNullOrWhiteSpace( input.Ctid ) && GeneratingCtidIfNotFound() )
   //         //             input.Ctid = GenerateCtid();

   //         //         if ( IsCtidValid( input.Ctid, ref messages ) )
   //         //         {
   //         //             //can't do this yet, as the registry may treat an existing records as new!!!
   //         //             //input.Ctid = input.Ctid.ToLower();
   //         //             output.Ctid = input.Ctid;
   //         //             output.CtdlId = idBaseUrl + output.Ctid;
   //         //	CurrentCtid = input.Ctid;
   //         //}

   //         //establish language
   //         if ( input.InLanguage != null && input.InLanguage.Count > 0 )
   //         {
   //             output.InLanguage = input.InLanguage;
   //         }
   //         else
   //         {
   //             //make configurable. Default to english if not found
   //             if ( UtilityManager.GetAppKeyValue( "ra.RequiringLanguage", false ) )
   //             {
   //                 messages.Add( string.Format( "At least one language (InLanguage) must be provided for Credential: '{0}'", input.Name ) );
   //             } else
   //             {

   //             }
   //         }

   //         //required
   //         if ( string.IsNullOrWhiteSpace( input.Name ) )
			//{
			//	messages.Add( FormatMessage( "Error - A name must be entered for Credential with CTID: '{0}'.", input.Ctid ) );
			//}
			//else
			//{
			//	output.Name = Assign(input.Name);
			//	CurrentEntityName = input.Name;
			//}
   //         if ( string.IsNullOrWhiteSpace( input.Description ) )
   //         {
   //             messages.Add( FormatMessage( "Error - A  description must be entered for Credential '{0}'.", input.Name ) );
   //         }
   //         else
   //         {
   //             output.Description = Assign(ConvertSpecialInput( input.Description ));
   //         }

   //         if ( string.IsNullOrWhiteSpace( input.CredentialType ) )
   //         {
			//	messages.Add( FormatMessage( "Error - A credential type must be entered for Credential '{0}'.", input.Name ) );
			//}
   //         else
   //         {
			//	string validSchema = "";

			//	if ( input.CredentialType.IndexOf( "ceterms:" ) == -1 )
   //                 input.CredentialType = "ceterms:" + input.CredentialType.Trim();
   //             property = input.CredentialType;
			//	//if ( ValidationServices.IsCredentialTypeValid( "credentialType", ref property ) )
			//	//{
			//	//    output.CredentialType = property;
			//	//}
			//	if ( ValidationServices.IsValidCredentialType( property, ref validSchema ) )
			//	{
			//		output.CredentialType = validSchema;
			//	}
			//	else
   //             {
			//		messages.Add( FormatMessage( "Error - The credential type: ({0}) is invalid for Credential '{1}'.", input.CredentialType, input.Name ) );
			//	}
   //         }

			////now literal
			//output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Subject Webpage", ref messages, true );

			////need either ownedBy OR offeredBy
			//output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages );
			//output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages );

			//if (output.OwnedBy == null && output.OfferedBy == null)
			//	messages.Add( string.Format( "At least one of an 'Offered By' organization, or an 'Owned By' organization must be provided for Credential: '{0}'", input.Name ) );


   //         return isValid;
   //     }
	
		public void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            output.AlternateName = AssignLanguageMapList( input.AlternateName, input.AlternateName_Map, "AlternateName", ref messages );

            //now literal
            //output.CodedNotation = AssignListToString( input.CodedNotation );
            output.CodedNotation = input.CodedNotation;

            output.CredentialId = ( input.CredentialId ?? "" ).Length > 0 ? input.CredentialId : null;
            output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );


			//placeholder: VersionIdentifier (already a list, the input needs output change
			output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier, ref messages );
		}

        public void HandleUrlFields( InputEntity from, OutputEntity to, ref List<string> messages )
        {


            to.AvailableOnlineAt = AssignValidUrlAsStringList( from.AvailableOnlineAt, "AvailableOnlineAt", ref messages );
            to.AvailabilityListing = AssignValidUrlAsStringList( from.AvailabilityListing, "AvailabilityListing", ref messages );

            to.Image = AssignValidUrlAsString( from.Image, "Image", ref messages );
            to.PreviousVersion = AssignValidUrlAsString( from.PreviousVersion, "PreviousVersion", ref messages );
            to.LatestVersion = AssignValidUrlAsString( from.LatestVersion, "LatestVersion", ref messages );


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
		public void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject );

			//can't depend on the codes being SOC
			output.OccupationType = FormatCredentialAlignmentListFromFrameworkItemList( input.OccupationType, true, ref messages );

			//can't depend on the codes being NAICS??
			output.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( input.IndustryType, true, ref messages );
			if ( input.Naics != null && input.Naics.Count > 0 )
				output.Naics = input.Naics;
			else
				output.Naics = null;

			if ( !string.IsNullOrWhiteSpace( input.CredentialStatusType ) )
            {
                output.CredentialStatusType = FormatCredentialAlignment( "credentialStatusType", input.CredentialStatusType, ref messages ) ;
            }
            else
                output.CredentialStatusType = null;

            //
            output.AudienceLevel = FormatCredentialAlignmentVocabs( "audienceLevelType", input.AudienceLevelType, ref messages );
            output.AudienceType = FormatCredentialAlignmentVocabs( "audienceType", input.AudienceType, ref messages );
            

            if ( IsValidDegreeType( output.CredentialType ) )
            {
                output.DegreeConcentration = FormatCredentialAlignmentListFromStrings( input.DegreeConcentration, input.DegreeConcentration_Map );

                output.DegreeMajor = FormatCredentialAlignmentListFromStrings( input.DegreeMajor, input.DegreeMajor_Map );

                output.DegreeMinor = FormatCredentialAlignmentListFromStrings( input.DegreeMinor, input.DegreeMinor_Map );
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

        public InputEntity DeSerializedFormat( string serialized )
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
