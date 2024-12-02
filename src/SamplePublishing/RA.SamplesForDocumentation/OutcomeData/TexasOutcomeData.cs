using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using Microsoft.VisualBasic.FileIO;
using LumenWorks.Framework.IO.Csv;

using RA.SamplesForDocumentation;

using RA.Models.Input;
using RMI = RA.Models.Input;
using RA.Models.Input.profiles.QData;
using Newtonsoft.Json;

namespace RA.SamplesForDocumentation.OutcomeData
{
    public class TexasOutcomeData : OutcomeDataServices
    {
        /// <summary>
        /// Sample code for reading outcome data from a comma separated file
        /// </summary>
        public void Prototype( string apiAction = "publish" )
        {
            //path to the input file
            string inputFile = "D:\\testing\\THECB-Outcomes-sample-test_23-06-07.csv";

            var sandboxApiKey = "PROVIDE YOUR API KEY";
            var sandboxCTID = "PROVIDE THE DATA PROFILE CTID";


            var file = "";
            //1-sandbox test 2-
            int dspType = 1;
            if ( dspType == 1 )
            {
                file = inputFile;
                PublishTHECBOutcomeData( file, sandboxCTID, sandboxApiKey, apiAction );
            }


        }

        public void PublishTHECBOutcomeData( string file, string publishedByCTID, string orgAPI, string apiAction = "format" )
        {
            //where the name, etc. are fixed, can just use constants
            var dataSetProfileName = "Texas Annual Outcome Data";
            var dataSetProfileDescription = "The outcome data includes statistics for graduate's loans, earnings, and average time to complete credentials.";
            var dataSetTimePeriodDescription = "Outcome data for 2019";
            var passRateDescription = "";

            var outcomesLabel = "THECB outcomes";
            //================================================

            var dataSetTimePeriod = new List<DataSetTimeFrameDTO>();
            var period1 = new DataSetTimeFrameDTO()
            {
                Description = dataSetTimePeriodDescription,
                PassRateDescription = passRateDescription
            };
            var period2 = new DataSetTimeFrameDTO()
            {
                Description = dataSetTimePeriodDescription,
                PassRateDescription = passRateDescription
            };
            var period3 = new DataSetTimeFrameDTO()
            {
                Description = dataSetTimePeriodDescription,
                PassRateDescription = passRateDescription
            };
            //data profile, one per row
            var dataProfile = new DataProfileDTO();
            //one earning method
            var earningsAmount = new RMI.MonetaryAmount();
            //
            List<string> messages = new List<string>();
            int rowNbr = 0;

            try
            {
                using ( CsvReader csv = new CsvReader( new StreamReader( file, System.Text.Encoding.UTF7 ), true ) )
                {
                    //TODO - may want to skip the second line - if it still contains the ctdl
                    int fieldCount = csv.FieldCount;
                    string[] headers = csv.GetFieldHeaders();
                    //validate headers
                    while ( csv.ReadNextRecord() )
                    {
                        rowNbr++;
                        var entity = new OutcomeDataDTO();

                        var valueInput = "";
                        var percentInput = "";
                        //cannot guarantee the order of the columns
                        var subjectValue1 = new QuantitativeValue();
                        var subjectValue2 = new QuantitativeValue();
                        var subjectValue3 = new QuantitativeValue();
                        var subjectValue4 = new QuantitativeValue();
                        var subjectValue5 = new QuantitativeValue();
                        //use header columns rather than hard-code index numbers to enable flexibility
                        //this approach currently only handles one row per dataSetProfile
                        for ( int i = 0; i < fieldCount; i++ )
                        {
                            Debug.Write( string.Format( "{0} = {1};", headers[i], csv[i] ) );
                            Debug.WriteLine( "" );


                            //may want to make case insensitive!
                            var header = headers[i];//.ToLowerInvariant();
                            var header2 = header.Replace( " ", "" ).Replace( "_", "" ).ToLowerInvariant();
                            switch ( header2 )
                            {
                                case "publishedby": //would already have the api key for this custom process
                                    //don't actually need org name
                                    //entity.DataProviderName = csv[i];
                                    break;
                                case "publishedbyctid": //CTID
                                    //don't actually need org name
                                    //entity.DataProviderName = csv[i];
                                    break;
                                //***dataSet ***
                                case "dataprovider": //CTID. may not be same as publisher
                                    entity.DataProvider = new OrganizationReference()
                                    {
                                        Type = "Organization",
                                        CTID = csv[i]
                                    };
                                    publishedByCTID = csv[i];
                                    break;
                                //
                                case "credentialorganization":
                                case "organization":
                                    //don't need directly
                                    entity.OrganizationName = csv[i];
                                    break;
                                case "schoolctid":
                                case "credentialorganizationctid":
                                    //
                                    entity.Organization = new OrganizationReference()
                                    {
                                        Type = "Organization",
                                        CTID = csv[i]
                                    };
                                    entity.CredentialOrganizationCTID = csv[i];
                                    break;
                                //
                                case "nameofcredential":
                                case "credentialname":
                                case "credential":
                                    entity.CredentialName = csv[i];
                                    break;
                                //for simplicity add to single credential, then copy to About
                                case "credentialctid":
                                    entity.Credential = new EntityReference()
                                    {
                                        Type = "Credential",
                                        CTID = csv[i]
                                    };
                                    entity.CredentialCTID = csv[i];
                                    break;

                                //Data Set Profile 
                                case "datasetprofilectid":
                                    entity.CTID = csv[i];
                                    break;
                                case "datasetprofilename":
                                    entity.Name = csv[i];
                                    break;
                                case "source":
                                case "datasetprofilewebpage":
                                    entity.Source = csv[i];
                                    break;
                                case "datasetprofiledescription":
                                    entity.Description = csv[i];
                                    break;
                                case "datasuppressionpolicy":
                                    entity.DataSuppressionPolicy = csv[i];
                                    break;
                                //=============="Data Set Time Period 3:  Description"
                                case "datasettimeframedescription":
                                    var desc = csv[i];
                                    //don't wipe out any defaults
                                    if ( !string.IsNullOrWhiteSpace( desc ) )
                                    {
                                        period1.Description = desc;
                                    }
                                    break;
                                case "datasettimeperiod1:startdate":
                                case "datasettimeframestartdate":
                                    period1.StartDate = csv[i];
                                    break;
                                case "datasettimeperiod1:enddate":
                                case "datasettimeframeenddate":
                                    period1.EndDate = csv[i];
                                    break;
                                //=============="Data Profile" 
                                case "dataprofiledescription":
                                    dataProfile.Description = csv[i];
                                    break;
                                case "dataprofileholdersinset":
                                    //expect int
                                    var amt = csv[i];
                                    if ( !string.IsNullOrWhiteSpace( amt ) )
                                    {
                                        if ( int.TryParse( amt, out var amt2 ) )
                                        {
                                            dataProfile.HoldersInSet.Value = amt2;
                                        }
                                        else
                                        {
                                            //error
                                            messages.Add( $"Row: {rowNbr}. A non-integer value was found for holders in set: {amt}." );
                                        }
                                    }
                                    break;
                                //=====earnings amt =========
                                case "dataprofileearningsamountdescription":
                                case "earningsamountdescription":
                                    earningsAmount.Description = csv[i];
                                    break;
                                case "dataprofileearningsamountvalue":
                                case "earningsamount_value":
                                    //expect decimal?
                                    var earningsAmtValue = csv[i];
                                    if ( !string.IsNullOrWhiteSpace( earningsAmtValue ) )
                                    {
                                        if ( decimal.TryParse( earningsAmtValue, out var amt2 ) )
                                        {
                                            earningsAmount.Value = amt2;
                                        }
                                        else
                                        {
                                            //error
                                            messages.Add( $"Row: {rowNbr}. A decimal value was found for Earnings Amount value: {earningsAmtValue}." );
                                        }
                                    }

                                    break;
                                case "dataprofileearningsamountmonetaryamountcurrency":
                                case "earningsamountcurrency":
                                    earningsAmount.Currency = csv[i];
                                    break;
                                //==    subjectvalue1   ============
                                case "dataprofilesubjectvalue1quantitativevaluedescription":
                                case "subjectvalue1description":
                                    subjectValue1.Description = csv[i];
                                    break;
                                case "dataprofilesubjectvalue1quantitativevaluepercentage":
                                case "subjectvalue1percentage":
                                    AssignPercentage( csv[i], subjectValue1, rowNbr, 1, ref messages );
                                    break;
                                case "dataprofilesubjectvalue1quantitativevaluevalue":
                                case "subjectvalue1value":
                                    AssignValue( csv[i], subjectValue1, rowNbr, 1, ref messages );
                                    break;
                                case "dataprofilesubjectvalue1quantitativevalueunittext":
                                case "subjectvalue1unittext":
                                    subjectValue1.UnitText = csv[i];
                                    break;

                                //==    subjectvalue2   ============
                                case "dataprofilesubjectvalue2quantitativevaluedescription":
                                case "subjectvalue2description":
                                    subjectValue2.Description = csv[i];
                                    break;
                                case "dataprofilesubjectvalue2quantitativevaluepercentage":
                                case "subjectvalue2percentage":
                                    AssignPercentage( csv[i], subjectValue2, rowNbr, 2, ref messages );
                                    break;
                                case "dataprofilesubjectvalue2quantitativevaluevalue":
                                case "subjectvalue2value":
                                    AssignValue( csv[i], subjectValue2, rowNbr, 2, ref messages );
                                    break;
                                case "dataprofilesubjectvalue2quantitativevalueunittext":
                                case "subjectvalue2unittext":
                                    subjectValue2.UnitText = csv[i];
                                    break;
                                //==    subjectValue3   ============
                                //DataProfile_subjectValue3_QuantitativeValue_Description
                                //    dataprofilesubjectvalue3quantitativevaluedescription
                                case "dataprofilesubjectvalue3quantitativevaluedescription":
                                case "subjectValue3description":
                                    subjectValue3.Description = csv[i];
                                    break;
                                case "dataprofilesubjectvalue3quantitativevaluepercentage":
                                case "subjectValue3percentage":
                                    AssignPercentage( csv[i], subjectValue3, rowNbr, 3, ref messages );
                                    break;
                                case "dataprofilesubjectvalue3quantitativevaluevalue":
                                case "subjectValue3value":
                                    AssignValue( csv[i], subjectValue3, rowNbr, 3, ref messages );
                                    break;
                                case "dataprofilesubjectvalue3quantitativevalueunittext":
                                case "subjectValue3unittext":
                                    subjectValue3.UnitText = csv[i];
                                    break;
                                //==    subjectValue4   ============
                                case "dataprofilesubjectvalue4quantitativevaluedescription":
                                case "subjectValue4description":
                                    subjectValue4.Description = csv[i];
                                    break;
                                case "dataprofilesubjectvalue4quantitativevaluepercentage":
                                case "subjectValue4percentage":
                                    AssignPercentage( csv[i], subjectValue4, rowNbr, 4, ref messages );
                                    break;
                                case "dataprofilesubjectvalue4quantitativevaluevalue":
                                case "subjectValue4value":
                                    AssignValue( csv[i], subjectValue4, rowNbr, 4, ref messages );
                                    break;
                                case "dataprofilesubjectvalue4quantitativevalueunittext":
                                case "subjectValue4unittext":
                                    subjectValue4.UnitText = csv[i];
                                    break;

                                //==    subjectValue5   ============
                                case "dataprofilesubjectvalue5quantitativevaluedescription":
                                case "subjectValue5description":
                                    subjectValue5.Description = csv[i];
                                    break;
                                case "dataprofilesubjectvalue5quantitativevaluepercentage":
                                case "subjectValue5percentage":
                                    AssignPercentage( csv[i], subjectValue5, rowNbr, 5, ref messages );
                                    break;
                                case "dataprofilesubjectvalue5quantitativevaluevalue":
                                case "subjectValue5value":
                                    AssignValue( csv[i], subjectValue5, rowNbr, 5, ref messages );
                                    break;
                                case "dataprofilesubjectvalue5quantitativevalueunittext":
                                case "subjectValue5unittext":
                                    subjectValue5.UnitText = csv[i];
                                    break;
                                //ignore
                                case "ctdlproperty":

                                    break;
                                //
                                default:
                                    //action?
                                    if ( headers[i].IndexOf( "Column" ) == 0 )
                                    {
                                        //OK
                                    }
                                    else
                                    {
                                        LoggingHelper.DoTrace( 1, string.Format( "RA_UnitTestProject.PublishTHECBOutcomeData. Unknown header {0}/{1}", header, header2 ) );
                                    }
                                    break;
                            }
                        }   //loop next column
                        if ( messages.Count > 0 )
                        {
                            LoggingHelper.DoTrace( 1, $"Errors were encountered for row: {rowNbr}. Skipping:" );
                            foreach ( var message in messages )
                            {
                                LoggingHelper.DoTrace( 1, message );
                            }
                            continue;
                        }
                        //validate? and assemble
                        var output = new DataSetProfile()
                        {
                            Name = entity.Name,
                            Description = entity.Description,   // dataSetProfileDescription,
                            CTID = entity.CTID,
                            Source = entity.Source,
                            DataSuppressionPolicy = entity.DataSuppressionPolicy,
                        };
                        if ( string.IsNullOrWhiteSpace( entity.CTID ) )
                        {
                            //can't handle this unless more info is provided
                            //log
                            LoggingHelper.DoTrace( 1, $"**** {outcomesLabel} Record #{rowNbr} is missing a DataSet Profile CTID" );

                            continue;
                        }
                        if ( string.IsNullOrWhiteSpace( entity.CredentialCTID ) )
                        {
                            //can't handle this unless more info is provided
                            LoggingHelper.DoTrace( 1, $"**** {outcomesLabel} Record #{rowNbr} is missing a Credential CTID" );

                            //log
                            continue;
                        }
                        if ( string.IsNullOrWhiteSpace( entity.CredentialOrganizationCTID ) )
                        {
                            //can't handle this unless more info is provided
                            //log
                            continue;
                        }
                        output.About.Add( entity.Credential );
                        output.DataProvider = entity.DataProvider;
                        output.Source = entity.Source;
                        output.DataSuppressionPolicy = entity.DataSuppressionPolicy;

                        int value = 0;
                        decimal percent = 0M;
                        var dataSetTimeFrame = new DataSetTimeFrame()
                        {
                            Description = period1.Description,
                            StartDate = period1.StartDate,
                            EndDate = period1.EndDate,
                        };

                        var outputDataProfile = new DataProfile()
                        {
                            Description = dataProfile.Description,
                        };
                        outputDataProfile.HoldersInSet.Add( dataProfile.HoldersInSet );
                        if ( earningsAmount != null && earningsAmount.Value > 0 )
                        {
                            outputDataProfile.EarningsAmount.Add( earningsAmount );
                        }
                        outputDataProfile.SubjectsInSet.Add( subjectValue1 );
                        outputDataProfile.SubjectsInSet.Add( subjectValue2 );
                        outputDataProfile.SubjectsInSet.Add( subjectValue3 );
                        outputDataProfile.SubjectsInSet.Add( subjectValue4 );
                        outputDataProfile.SubjectsInSet.Add( subjectValue5 );

                        dataSetTimeFrame.DataAttributes.Add( outputDataProfile );

                        if ( dataSetTimeFrame != null )
                            output.DataSetTimePeriod.Add( dataSetTimeFrame );

                        if ( output.DataSetTimePeriod.Count == 0 )
                        {
                            //must be at least one timeframe
                            //log
                            continue;
                        }
                        //publish
                        var myRequest = new DataSetProfileRequest()
                        {
                            DataSetProfile = output,
                            DefaultLanguage = "en-US",
                            PublishForOrganizationIdentifier = publishedByCTID
                        };

                        //Serialize the credential request object
                        var payload = JsonConvert.SerializeObject( myRequest );

                        //otherwise use a method where return status can be inspected
                        var identifier = "DSP_" + myRequest.DataSetProfile.CTID.ToLowerInvariant();
                        //OR. All credential names will be similar
                        identifier = "DSP_" + entity.OrganizationName.Replace( " ", "" ) + "--" + entity.CredentialName;
                        SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
                        {
                            EndpointType = "datasetprofile",
                            RequestType = apiAction,
                            OrganizationApiKey = orgAPI,
                            CTID = myRequest.DataSetProfile.CTID.ToLower(),   //added here for logging
                            Identifier = identifier,     //useful for logging, might use the ctid
                            InputPayload = payload
                        };

                        bool isValid = new SampleServices().PublishRequest( req );
                    }
                } //loop

            }
            catch ( Exception ex )
            {
                LoggingHelper.DoTrace( 1, string.Format( "TexasOutcomeData.PublishTHECB_OutcomeData. {0}", ex.Message ) );
            }

        }
        public void AssignDescription( string input, QuantitativeValue subjectValue )
        {
            subjectValue.Description = input;
        }
        public void AssignValue( string input, QuantitativeValue subjectValue, int rowNbr, int svNumber, ref List<string> messages )
        {
            if ( !string.IsNullOrWhiteSpace( input ) )
            {
                if ( decimal.TryParse( input, out var amt2 ) )
                {
                    subjectValue.Value = amt2;
                }
                else
                {
                    //error
                    messages.Add( $"Row: {rowNbr}. An invalid decimal value was found for subject value (#{svNumber}): {input}." );
                }
            }
        }
        public void AssignPercentage( string input, QuantitativeValue subjectValue, int rowNbr, int svNumber, ref List<string> messages )
        {
            if ( !string.IsNullOrWhiteSpace( input ) )
            {
                if ( decimal.TryParse( input, out var amt2 ) )
                {
                    subjectValue.Percentage = amt2;
                }
                else
                {
                    //error
                    messages.Add( $"Row: {rowNbr}. A invalid percentage was found for subject value: {input}." );
                }
            }
        }
        public void AssignUnitText( string input, QuantitativeValue subjectValue, int rowNbr, int svNumber, ref List<string> messages )
        {

        }
    }
}
