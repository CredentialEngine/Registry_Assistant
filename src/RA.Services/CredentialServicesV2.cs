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
using GraphContentRequest = RA.Models.Input.GraphContentRequest;
using GraphContainer = RA.Models.JsonV2.GraphContainer;
using OutputGraph = RA.Models.JsonV2.GraphContainer;
using Utilities;
using System.Web.Caching;

namespace RA.Services
{
	/// <summary>
	/// TODO - identify concrete methods here that could become common methods
	/// </summary>
	public class CredentialServicesV2 : ServiceHelperV2
	{
		static string status = "";

		#region future option to publish a graph input
		/// <summary>
		/// A request will come already formatted as JSON-LD
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="helper"></param>
		public void PublishGraph( GraphContentRequest request, ref bool isValid, RA.Models.RequestHelper helper, ref string outputCTID)
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			//consider making configurable?

			GraphContainer og = new GraphContainer();
			var output = new OutputEntity();

			if( ToMapFromGraph( request, ref output, ref messages ) )
			{
				og.Graph.Add( output );
				//TODO - is there other info needed, like in context?
				//
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community );
				//og.Ctid = output.Ctid;
				outputCTID = output.Ctid;
				//og.Type = "ceasn:Credential"; //ignored anyway
				og.Context = ceasnContext;
				//format payload
				helper.Payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );
				//helper.Payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );

				//will need to extract a ctid?
				CER cer = new CER( "Credential", output.CredentialType, output.Ctid, helper.SerializedInput )
				{
					PublisherAuthorizationToken = helper.ApiKey,
					IsPublisherRequest = helper.IsPublisherRequest,
					EntityName = CurrentEntityName,
					Community = request.Community ?? "",
					PublishingForOrgCtid = helper.OwnerCtid
				};

				if( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
				{
					cer.IsManagedRequest = true;
					//get publisher org
					string publisherCTID = "";
					if( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;
					}
					else
					{
						//should be an error message returned

						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "CredentialServices.PublishGraph. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.Ctid, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				// if first time, and large framework, force use of CE keys
				bool recordWasFound = false;
				if( !SupportServices.ValidateAgainstPastRequest( "Credential_", output.Ctid, ref cer, ref messages, ref recordWasFound ) )
				{
					isValid = false;
					//return; //===================
				}
				else
				{
					//
					if( helper.Payload.Length > 1000000 )
					{
						//LoggingHelper.DoTrace( 1, string.Format( "CredentialServices.PublishGraph. *******Note. Larger payload: {0} bytes , competencies: {1}.", helper.Payload.Length, competenciesCount ) );
						//try force use of CE keys
						//can only do this if first time, other will have a key mismatch
						//just non-prod for now
						if( cer.IsManagedRequest && !recordWasFound )
						{
							LoggingHelper.DoTrace( 1, "CredentialServices.PublishGraph. Forcing use of SelfPublish" );
							cer.IsManagedRequest = false;
						}
						else
						{
							LoggingHelper.DoTrace( 1, "CredentialServices.PublishGraph. WARNING. Encountered large framework that was previously published, so CANNOT force use of SelfPublish. See what happens!" );
						}
					}

					string identifier = "Credential_" + output.Ctid;
					//do publish
					if( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published Competency Framework</p><p>Ctid: {0}</p> <p>EnvelopeId: {1}</p> ", output.Ctid, crEnvelopeId );
						NotifyOnPublish( "Credential", msg );
					}
					else
					{
						messages.Add( status );
						isValid = false;
					}
				}
				//}
			}
			else
			{
				helper.HasErrors = true;
				isValid = false;
				//helper.Payload = JsonSerializer.Serialize( og, JsonSerializerOptions() );
				helper.Payload = Newtonsoft.Json.JsonConvert.SerializeObject( og, GetJsonSettings() );
			}
			helper.SetMessages( messages );

		}

		public bool ToMapFromGraph(GraphContentRequest request, ref OutputEntity output, ref List<string> messages)
		{
			CurrentEntityType = "Credential";
			bool isValid = true;
			var input = request.GraphInput;// 
			Community = request.Community ?? "";

			//TODO - if from CASS, just pass thru, with minimum validation
			//output.Graph = input.Graph;
			int blankNodesCount = 0;
			try
			{
				//output = GetCredential( input.Graph, ref blankNodesCount, ref messages );
				if( output == null || string.IsNullOrWhiteSpace( output.Ctid ) )
				{
					messages.Add( "A ceterms:Credential document was not found." );
				}
				else
				{
					//CHECK for required fields
					CurrentCtid = output.Ctid = FormatCtid( output.Ctid, "Credential", ref messages );

					if( !HasData( output.Name ) )
						messages.Add( "A Name must be provided for the Credential." );
					else
					{
						CurrentEntityName = output.Name.ToString();
					}
					if( !HasData( output.Description ) )
						messages.Add( "A Description must be provided for the Credential." );
					
					if( output.InLanguage == null || output.InLanguage.Count() == 0 )
						messages.Add( "At least one entry must be provided for the InLanguage of a Credential." );
					//.............

					output.CtdlId = SupportServices.FormatRegistryUrl( ResourceTypeUrl, output.Ctid, Community );
				}

			}
			catch( Exception ex )
			{
				LoggingHelper.LogError( ex, "CredentialServices.ToMapFromGraph" );
				messages.Add( ex.Message );
			}
			if( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
		#endregion
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
			//if ( environment != "production" )
			//output.LastUpdated = DateTime.Now.ToUniversalTime().ToString( "yyyy-MM-dd HH:mm:ss UTC" );
			if ( environment != "production" )
			{	}
				//populate the output. The profiles will be published only if the credential was successful. 

			//datasetProfiles is populated by the latter
			//var datasetProfiles = FormatHolderProfiles( request.HoldersProfile, ref messages );

			if ( ToMap(request, output, helper.IsPublisherRequest, ref messages) )
            {
				var earningsProfile = FormatEarningsProfile( request.Earnings, ref messages );
				var employmentOutcomeProfile = FormatEmploymentOutcomeProfile( request.EmploymentOutcome, ref messages );
				var holdersProfile = FormatHolderProfiles( request.HoldersProfile, ref messages );

				og.Graph.Add( output );
                //TODO - is there other info needed, like in context?
                if ( holdersProfile != null && holdersProfile.Count > 0 )
                {
					output.Holders = new List<string>();
                    foreach ( var item in holdersProfile )
                    {
                        og.Graph.Add( item );
						output.Holders.Add( item.CtdlId );

					}
                }
				if ( earningsProfile != null && earningsProfile.Count > 0 )
				{
					output.Earnings = new List<string>();

					foreach ( var item in earningsProfile )
					{
						og.Graph.Add( item );
						output.Earnings.Add( item.CtdlId );

					}
				}
				if ( employmentOutcomeProfile != null && employmentOutcomeProfile.Count > 0 )
				{
					output.EmploymentOutcome = new List<string>();

					foreach ( var item in employmentOutcomeProfile )
					{
						og.Graph.Add( item );
						output.EmploymentOutcome.Add( item.CtdlId );

					}
				}
				//
				if ( DataSetProfiles != null && DataSetProfiles.Count > 0 )
				{
					foreach ( var item in DataSetProfiles )
					{
						og.Graph.Add( item );
					}

					//can't have DataSetTimeFrames without DataSetProfiles
					if ( DataSetTimeFrames != null && DataSetTimeFrames.Count > 0 )
					{
						foreach ( var item in DataSetTimeFrames )
						{
							og.Graph.Add( item );
						}

						//can't have DataProfiles without DataSetTimeFrames 
						if ( DataProfiles != null && DataProfiles.Count > 0 )
						{
							foreach ( var item in DataProfiles )
							{
								og.Graph.Add( item );
							}
						}
					}
					
				}

				//
				if ( BlankNodes != null && BlankNodes.Count > 0 )
				{
					foreach ( var item in BlankNodes )
					{
						og.Graph.Add( item );
					}
				}
				og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.CredentialType;
                og.Context = ctdlContext;

                helper.Payload = JsonConvert.SerializeObject(og, GetJsonSettings() );
                CER cer = new CER( "Credential", output.CredentialType, output.Ctid, helper.SerializedInput )
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
					//TODO - should return publisher and owner names for activity logging, etc.
					if ( SupportServices.GetPublishingOrgByApiKey( cer.PublisherAuthorizationToken, ref publisherCTID, ref messages ) )
					{
						cer.PublishingByOrgCtid = publisherCTID;
					}
					else
					{
						//should be an error message returned
						isValid = false;
						helper.SetMessages( messages );
						LoggingHelper.DoTrace( 4, string.Format( "Credential Services.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.Ctid, cer.PublisherAuthorizationToken ) );
						return; //===================
					}
				}
				else
					cer.PublishingByOrgCtid = cer.PublishingForOrgCtid;

				// need to generalize this
				/* check if previously published
				 * - if found, use the same publishing method
				 * 
				 * 
				 */
				if ( !SupportServices.ValidateAgainstPastRequest( "Credential", output.Ctid, ref cer, ref messages ) )
				{
					isValid = false;
					//helper.SetMessages( messages );
					LoggingHelper.DoTrace( 4, string.Format( "Credential Services.Publish. Validate ApiKey failed. Org Ctid: {0}, Document Ctid: {1}, apiKey: {2}", helper.OwnerCtid, output.Ctid, cer.PublisherAuthorizationToken ) );
					//return; //===================
				}
				else
				{
					string identifier = "Credential_" + request.Credential.Ctid;
					if ( cer.Publish( helper, submitter, identifier, ref status, ref crEnvelopeId ) )
					{
						//for now need to ensure envelopid is returned
						helper.RegistryEnvelopeId = crEnvelopeId;
					
						CheckIfChanged( helper, cer.WasChanged );
						string msg = string.Format( "<p>Published credential: {0}</p><p>Subject webpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.Name, output.SubjectWebpage, output.Ctid, crEnvelopeId );
						NotifyOnPublish( "Credential", msg );
					}
					else
					{
						if ( !string.IsNullOrWhiteSpace( ( status ?? "Unknown Error" ) ) )
							messages.Add( status );
						isValid = false;

					}
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
                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.CredentialType;
                og.Context = ctdlContext;

                helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }
			helper.SetWarningMessages( warningMessages );
			helper.SetMessages(messages);
        }

        public void FormatAsJson(EntityRequest request, RA.Models.RequestHelper helper, ref bool isValid )
        {
            OutputGraph og = new OutputGraph();

            var output = new OutputEntity();
            isValid = true;
            IsAPublishRequest = false;
			List<string> messages = new List<string>();
			//not sure if this should be before or after the mapping
			//doing before here so available where error occured. 
			var earningsProfile = FormatEarningsProfile( request.Earnings, ref messages );
			var employmentOutcomeProfile = FormatEmploymentOutcomeProfile( request.EmploymentOutcome, ref messages );
			var holdersProfile = FormatHolderProfiles( request.HoldersProfile, ref messages );

			if ( ToMap(request, output, helper.IsPublisherRequest, ref messages) )
            {


				og.Graph.Add( output );
				if ( holdersProfile != null && holdersProfile.Count > 0 )
				{
					output.Holders = new List<string>();
					foreach ( var item in holdersProfile )
					{
						og.Graph.Add( item );
						output.Holders.Add( item.CtdlId );

					}
				}
				if ( earningsProfile != null && earningsProfile.Count > 0 )
				{
					output.Earnings = new List<string>();

					foreach ( var item in earningsProfile )
					{
						og.Graph.Add( item );
						output.Earnings.Add( item.CtdlId );

					}
				}
				if ( employmentOutcomeProfile != null && employmentOutcomeProfile.Count > 0 )
				{
					output.EmploymentOutcome = new List<string>();

					foreach ( var item in employmentOutcomeProfile )
					{
						og.Graph.Add( item );
						output.EmploymentOutcome.Add( item.CtdlId );

					}
				}
				//
				if ( DataSetProfiles != null && DataSetProfiles.Count > 0 )
				{
					foreach ( var item in DataSetProfiles )
					{
						og.Graph.Add( item );
					}

					//can't have DataSetTimeFrames without DataSetProfiles
					if ( DataSetTimeFrames != null && DataSetTimeFrames.Count > 0 )
					{
						foreach ( var item in DataSetTimeFrames )
						{
							og.Graph.Add( item );
						}

						//can't have DataProfiles without DataSetTimeFrames 
						if ( DataProfiles != null && DataProfiles.Count > 0 )
						{
							foreach ( var item in DataProfiles )
							{
								og.Graph.Add( item );
							}
						}
					}

				}

				if ( BlankNodes != null && BlankNodes.Count > 0)
                {
                    foreach (var item in BlankNodes)
                    {
                        og.Graph.Add( item );
                    }
                }                

                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.CredentialType;
                og.Context = ctdlContext;

				helper.Payload= JsonConvert.SerializeObject(og, GetJsonSettings());

            }
            else
            {
                isValid = false;
                //do payload anyway
                og.Graph.Add( output );
				if ( holdersProfile != null && holdersProfile.Count > 0 )
				{
					output.Holders = new List<string>();
					foreach ( var item in holdersProfile )
					{
						og.Graph.Add( item );
						output.Holders.Add( item.CtdlId );

					}
				}
				if ( earningsProfile != null && earningsProfile.Count > 0 )
				{
					output.Earnings= new List<string>();

					foreach ( var item in earningsProfile )
					{
						og.Graph.Add( item );
						output.Earnings.Add( item.CtdlId );

					}
				}
				if ( employmentOutcomeProfile != null && employmentOutcomeProfile.Count > 0 )
				{
					output.EmploymentOutcome= new List<string>();

					foreach ( var item in employmentOutcomeProfile )
					{
						og.Graph.Add( item );
						output.EmploymentOutcome.Add( item.CtdlId );

					}
				}
				//
				if ( DataSetProfiles != null && DataSetProfiles.Count > 0 )
				{
					foreach ( var item in DataSetProfiles )
					{
						og.Graph.Add( item );
					}

					//can't have DataSetTimeFrames without DataSetProfiles
					if ( DataSetTimeFrames != null && DataSetTimeFrames.Count > 0 )
					{
						foreach ( var item in DataSetTimeFrames )
						{
							og.Graph.Add( item );
						}

						//can't have DataProfiles without DataSetTimeFrames 
						if ( DataProfiles != null && DataProfiles.Count > 0 )
						{
							foreach ( var item in DataProfiles )
							{
								og.Graph.Add( item );
							}
						}
					}

				}
				if ( BlankNodes != null && BlankNodes.Count > 0 )
                {
                    foreach ( var item in BlankNodes )
                    {
                        og.Graph.Add( item );
                    }
                }

                og.CtdlId = SupportServices.FormatRegistryUrl( GraphTypeUrl, output.Ctid, Community);
                og.CTID = output.Ctid;
                og.Type = output.CredentialType;
                og.Context = ctdlContext;

				helper.Payload = JsonConvert.SerializeObject( og, GetJsonSettings() );
            }

			helper.SetWarningMessages( warningMessages );
			helper.SetMessages( messages );
			//return payload;
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
        public bool ToMap( EntityRequest request, OutputEntity output, bool isRequestFromPublisher, ref List<string> messages )
        {
			CurrentEntityType = "Credential";
			bool isValid = true;
			Community = request.Community ?? "";

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

            //if ( request.NotConvertingFromResourceLinkToGraphLink )
            //    ConvertingFromResourceLinkToGraphLink = false;

            try
			{
				//HandleRequiredFields
				CurrentCtid = output.Ctid = FormatCtid( input.Ctid, "Credential", ref messages );
                output.CtdlId = SupportServices.FormatRegistryUrl(ResourceTypeUrl, output.Ctid, Community);
                //establish language. make a common method
                output.InLanguage = PopulateInLanguage( input.InLanguage, "Credential", input.Name, hasDefaultLanguage, ref messages );

				//required
				output.Name = AssignLanguageMap( input.Name, input.Name_Map, "Credential.Name", DefaultLanguageForMaps, ref messages, true, 3 );
				CurrentEntityName = GetFirstItemValue( output.Name );

                output.Description = AssignLanguageMap( input.Description, input.Description_Map, "Credential.Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );

                
                if ( string.IsNullOrWhiteSpace( input.CredentialType ) )
                {
                    messages.Add( FormatMessage( "Error - A credential type must be entered for Credential '{0}'.", input.Name ) );
                }
                else
                {
                    string validSchema = "";

					//if ( ValidationServices.IsCredentialTypeValid( "credentialType", ref property ) )
					//{
					//    output.CredentialType = property;
					//}
					if ( ValidationServices.IsValidCredentialType( input.CredentialType, ref validSchema ) )
                    {
                        output.CredentialType = validSchema;
                    }
                    else
                    {
                        messages.Add( FormatMessage( "Error - The credential type: ({0}) is invalid for Credential '{1}'.", input.CredentialType, input.Name ) );
                    }
                }
				//
				
				if ( !string.IsNullOrWhiteSpace( input.CredentialStatusType ) )
				{
					output.CredentialStatusType = FormatCredentialAlignment( "credentialStatusType", input.CredentialStatusType, ref messages );
					if ( input.CredentialStatusType.ToLower().IndexOf( "active" ) == -1 
						//|| input.CredentialStatusType.ToLower().IndexOf( "eliminated" ) > -1
						//|| input.CredentialStatusType.ToLower().IndexOf( "suspended" ) > -1 
						//|| input.CredentialStatusType.ToLower().IndexOf( "superseded" ) > -1 //this is now invalid?
						)
					{
						//only warn on invalid urls, don't reject
						WarnOnInvalidUrls = true;
					}
					
				}
				else
				{
					//or just make a warning, and default to active
					output.CredentialStatusType = FormatCredentialAlignment( "credentialStatusType", "Active", ref messages );
					//messages.Add( "Error - The credential status type is required for Credentials" );
				}
				//
				//must be valid, unless a deprecicated or other status?
				output.SubjectWebpage = AssignValidUrlAsString( input.SubjectWebpage, "Credential.Subject Webpage", ref messages, true );

                //need either ownedBy OR offeredBy
                output.OwnedBy = FormatOrganizationReferences( input.OwnedBy, "Owning Organization", false, ref messages, false, true );
                output.OfferedBy = FormatOrganizationReferences( input.OfferedBy, "Offered By", false, ref messages, false, true );

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

				output.EstimatedDuration = FormatDuration( input.EstimatedDuration, "Credential.EstimatedDuration", ref messages );
				output.RenewalFrequency = FormatDurationItem( input.RenewalFrequency, "RenewalFrequency", ref messages );

                output.Keyword = AssignLanguageMapList( input.Keyword, input.Keyword_Map, "Credential.Keywords", ref messages );

				HandleCredentialAlignmentFields( input, output, ref messages );

				output.ProcessStandards = AssignValidUrlAsString( input.ProcessStandards, "Credential.ProcessStandards", ref messages, false );
                output.ProcessStandardsDescription = AssignLanguageMap( ConvertSpecialCharacters( input.ProcessStandardsDescription ), input.ProcessStandardsDescription_Map,"ProcessStandardsDescription",  DefaultLanguageForMaps, ref messages );

				//allows just ctid or full URI
                output.CommonConditions = AssignRegistryResourceURIsListAsStringList( input.CommonConditions, "CommonConditions", ref messages, false );
				output.CommonCosts = AssignRegistryResourceURIsListAsStringList( input.CommonCosts, "Credential.CommonCosts", ref messages, false );

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

				//
				if ( allowingBaseSalary )
				{
					//TODO - should this be limited to only certain credential types?
					output.BaseSalary = AssignMonetaryAmount( input.BaseSalary, "Credential.BaseSalary", "Credential", ref messages );
				}
				//
				if ( allowingETPLData )
				{
					if ( input.HasETPLResource != null && input.HasETPLResource.Any() )
					{
						//must be QA credential
						if ( !string.IsNullOrWhiteSpace( output.CredentialType ) )
						{
							if ( output.CredentialType != "ceterms:QualityAssuranceCredential" )
							{
								messages.Add( string.Format( "The property: HasETPLResource is only valid for a Quality Assurance Credential not for a: '{0}'", output.CredentialType ) );
							}
							else
							{
								//should be just CTIDs, but will handle URIs
								//or could these be references to something not in the registry? There could be hundreds.  May want to leave to the link checker
								output.HasETPLResource = AssignRegistryResourceURIsListAsStringList( input.HasETPLResource, "QACredential.HasETPLResource", ref messages, false );
								//at this time, the ETPL member must exist - this will slow down the process 
								//if (ETPLmemberMustExist)
								if ( !isRequestFromPublisher )
								{
									string status = "";
									string ctdlType = "";
									foreach ( var item in output.HasETPLResource )
									{
										var payload = RegistryServices.GetResourceByUrl( item, ref ctdlType, ref status );
										if ( !string.IsNullOrWhiteSpace( status ) )
										{
											messages.Add( status );
										}
									}
								}
							}
						}
					}
				}

				//
				output.AvailableAt = FormatAvailableAtList( input.AvailableAt, ref messages );
               
				output.Recommends = FormatConditionProfile( input.Recommends, ref messages, "RecommendsCondition" );
				output.Requires = FormatConditionProfile( input.Requires, ref messages, "RequiresCondition" );
				output.Corequisite = FormatConditionProfile( input.Corequisite, ref messages, "CorequisiteCondition" );
                output.Renewal = FormatConditionProfile( input.Renewal, ref messages, "RenewalCondition" );

                output.AdministrationProcess = FormatProcessProfile( input.AdministrationProcess, ref messages );
				output.MaintenanceProcess = FormatProcessProfile( input.MaintenanceProcess, ref messages );
				output.DevelopmentProcess = FormatProcessProfile( input.DevelopmentProcess, ref messages );
				output.ComplaintProcess = FormatProcessProfile( input.ComplaintProcess, ref messages );
				output.AppealProcess = FormatProcessProfile( input.AppealProcess, ref messages );
				output.ReviewProcess = FormatProcessProfile( input.ReviewProcess, ref messages );
				output.RevocationProcess = FormatProcessProfile( input.RevocationProcess, ref messages );

				//output.FinancialAssistanceOLD = MapFinancialAssistance( input.FinancialAssistanceOLD, ref messages );
				output.FinancialAssistance = MapFinancialAssistance( input.FinancialAssistance, ref messages, "Credential" );
				//
				//output.EarningsProfile = FormatEarningsProfile( request.Earnings, ref messages );
				//output.EmploymentOutcomeProfile = FormatEmploymentOutcomeProfile( request.EmploymentOutcome, ref messages );

				//
				//
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
						//rp.Description = Assign(r.Description, DefaultLanguageForMaps);
						rp.Description = AssignLanguageMap( r.Description, r.Description_Map, "Revocation.Description", DefaultLanguageForMaps, ref messages, true, MinimumDescriptionLength );
						
						rp.DateEffective = MapDate( r.DateEffective, "DateEffective", ref messages );
						rp.RevocationCriteria = AssignValidUrlAsString( r.RevocationCriteria, "RevocationCriteria", ref messages, false );
						//rp.RevocationCriteriaDescription = Assign(r.RevocationCriteriaDescription, DefaultLanguageForMaps);
						rp.RevocationCriteriaDescription = AssignLanguageMap( r.RevocationCriteriaDescription, r.RevocationCriteriaDescription_Map, "RevocationCriteriaDescription", DefaultLanguageForMaps, ref messages, false, MinimumDescriptionLength );
						rp.Jurisdiction = MapJurisdictions( r.Jurisdiction, ref messages );
						list.Add( rp );
					}
					
					output.Revocation = list;
				}

				//Navy specific
				output.HasRating = AssignRegistryResourceURIsListAsStringList( input.HasRating, "HasRating", ref messages );
				//or
				//output.HasRatingType = FormatCredentialAlignmentListFromFrameworkItemList( input.NavyRating, true, ref messages );

			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "CredentialServices.ToMap" );
				messages.Add( ex.Message );
			}
            //how output handle warning messages?
            if ( messages.Count > 0 )
            {
                isValid = false;
            }

            return isValid;
        }

        public void HandleOrgProperties( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			
			output.CopyrightHolder = FormatOrganizationReferenceToList( input.CopyrightHolder, "Copyright Holder", false, ref messages, false, true );

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


   
	
		public void HandleLiteralFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
            output.AlternateName = AssignLanguageMapList( input.AlternateName, input.AlternateName_Map, "AlternateName", ref messages );

            //now literal
            //output.CodedNotation = AssignListToString( input.CodedNotation );
            output.CodedNotation = input.CodedNotation;
			output.Identifier = AssignIdentifierListToList( input.Identifier, ref messages );

			output.CredentialId = ( input.CredentialId ?? "" ).Length > 0 ? input.CredentialId : null;
            output.DateEffective = MapDate( input.DateEffective, "DateEffective", ref messages );
			//should have validation
			output.ISICV4 = MapIsicV4(input.ISICV4);

			//placeholder: VersionIdentifier (already a list, the input needs output change
			output.VersionIdentifier = AssignIdentifierListToList( input.VersionIdentifier, ref messages );
		}

        public void HandleUrlFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {


            output.AvailableOnlineAt = AssignValidUrlAsStringList( input.AvailableOnlineAt, "AvailableOnlineAt", ref messages, false );
            output.AvailabilityListing = AssignValidUrlAsStringList( input.AvailabilityListing, "AvailabilityListing", ref messages, false );

            output.Image = AssignValidUrlAsString( input.Image, "Image", ref messages, false );

			output.PreviousVersion = AssignRegistryResourceURIAsString( input.PreviousVersion, "PreviousVersion", ref messages, false );
            output.LatestVersion = AssignRegistryResourceURIAsString( input.LatestVersion, "LatestVersion", ref messages, false );

			output.NextVersion = AssignRegistryResourceURIAsString( input.NextVersion, "NextVersion", ref messages, false );
			output.SupersededBy = AssignRegistryResourceURIAsString( input.SupersededBy, "SupersededBy", ref messages, false );
			output.Supersedes = AssignRegistryResourceURIAsString( input.Supersedes, "Supersedes", ref messages, false );
			

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
				output.RenewedIn = MapJurisdictionAssertionsList( input.RenewedIn, ref helper, ref messages );
				output.RevokedIn = MapJurisdictionAssertionsList( input.RevokedIn, ref helper, ref messages );
			//}

		} //
		#region === CredentialAlignmentObject ===
		public void HandleCredentialAlignmentFields( InputEntity input, OutputEntity output, ref List<string> messages )
        {
			LoggingHelper.DoTrace( 7, "CredentialServicesV2.HandleCredentialAlignmentFields - enter. " + CurrentCtid );
            output.Subject = FormatCredentialAlignmentListFromStrings( input.Subject, input.Subject_Map );

			//frameworks
			//=== occupations ===============================================================
			//can't depend on the codes being SOC
			output.OccupationType = FormatCredentialAlignmentListFromFrameworkItemList( input.OccupationType, true, ref messages );
			//no longer using as concrete property, just used for simple list of strings
			//append to OccupationType
			output.OccupationType = AppendCredentialAlignmentListFromList( input.AlternativeOccupationType, null, "","", "AlternativeOccupationType", output.OccupationType, ref messages );

			//NEW - allow a list of Onet codes, and resolve
			output.OccupationType = HandleListOfONET_Codes( input.ONET_Codes, output.OccupationType, ref messages );

			//output.AlternativeOccupationType = AssignLanguageMapList( input.AlternativeOccupationType, input.AlternativeOccupationType_Map, "Credential AlternativeOccupationType", ref output.OccupationType, ref messages );

			//=== industries ===============================================================
			//can't depend on the codes being NAICS??
			output.IndustryType = FormatCredentialAlignmentListFromFrameworkItemList( input.IndustryType, true, ref messages );
			if ( input.Naics != null && input.Naics.Count > 0 )
			{
				//should this be standardized?
				//we want a method that just returns valid/normalized naics
				output.Naics = ValidateListOfNAICS_Codes( input.Naics, ref messages );
				//output.Naics = input.Naics;
			}
			else
				output.Naics = null;
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


			//
			LoggingHelper.DoTrace( 7, "CredentialServicesV2.HandleCredentialAlignmentFields - AudienceLevelType" );
			output.AudienceLevelType = FormatCredentialAlignmentVocabs( "audienceLevelType", input.AudienceLevelType, ref messages );
            output.AudienceType = FormatCredentialAlignmentVocabs( "audienceType", input.AudienceType, ref messages );

			LoggingHelper.DoTrace( 7, "CredentialServicesV2.HandleCredentialAlignmentFields - deliveryTypes" );
			//Delivery
			//
			//var types = SchemaServices.GetConceptScheme( "http://credreg.net/ctdl/terms/Delivery/json" );

			if ( input.AssessmentDeliveryType != null && input.AssessmentDeliveryType.Count() > 0)
			{
				output.AssessmentDeliveryType = FormatCredentialAlignmentVocabs( "AssessmentDeliveryType", input.AssessmentDeliveryType, ref messages, "assessmentDeliveryType" );
			}
			if( input.LearningDeliveryType != null && input.LearningDeliveryType.Count() > 0 )
			{
				output.LearningDeliveryType = FormatCredentialAlignmentVocabs( "LearningDeliveryType", input.LearningDeliveryType, ref messages, "learningDeliveryType" );
			}
			//

			LoggingHelper.DoTrace( 7, "CredentialServicesV2.HandleCredentialAlignmentFields - checking for a degree type" );
			if ( !string.IsNullOrWhiteSpace( output.CredentialType ) && IsValidDegreeType( output.CredentialType ) )
            {
				LoggingHelper.DoTrace( 7, "CredentialServicesV2.HandleCredentialAlignmentFields - degrees" );
				output.DegreeConcentration = FormatCredentialAlignmentListFromStrings( input.DegreeConcentration, input.DegreeConcentration_Map );

                output.DegreeMajor = FormatCredentialAlignmentListFromStrings( input.DegreeMajor, input.DegreeMajor_Map );

                output.DegreeMinor = FormatCredentialAlignmentListFromStrings( input.DegreeMinor, input.DegreeMinor_Map );
            }

			LoggingHelper.DoTrace( 7, "CredentialServicesV2.HandleCredentialAlignmentFields - exit. " + CurrentCtid );
		}


        private static bool IsValidDegreeType( string credentialType )
        {
			if ( string.IsNullOrWhiteSpace( credentialType ) )
				return false;
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
