using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models;
using RA.Models.Input;
using RA.Models.Input.profiles.QData;
using YourCredential = RA.SamplesForDocumentation.SampleModels.Credential;
using APIRequestCredential = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
	public class KansasExamples
	{
		/// <summary>
		/// Publish a Kansas credential with aggregate data
		/// Based on Cofferville <see cref="https://ksdegreestats.org/ProspectusController?app=prospectus&ficeInstId=001910&programNbr=283"/>
		/// credential: MEDICAL LABORATORY TECHNOLOGY AAS <see cref="https://credentialfinder.org/credential/7381/MEDICAL_LABORATORY_TECHNOLOGY-ASSOCIATE_OF_APPLIED_SCIENCE"/>
		/// sandbox registry: <see cref="https://sandbox.credentialengineregistry.org/graph/ce-bd860014-039c-9c32-f816-e4f429b17f40"/>
		/// sandbox finder: <see cref="https://sandbox.credentialengine.org/finder/credential/ce-bd860014-039c-9c32-f816-e4f429b17f40"/>
		/// <param name="requestType">Format or Publish</param>
		public void CredentialWithAggregateDataProfile( string requestType = "format" )
		{

			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
				//		
			}
			//coffeyville
			var organizationIdentifierFromAccountsSite = "ce-3382bacb-2f29-4037-874e-9a53e2398661";// SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
			RequestHelper helper = new RA.Models.RequestHelper();
			//CTID from production
			var credCtid = "ce-bd860014-039c-9c32-f816-e4f429b17f40"; ;
			var myData = new Credential()
			{
				Name = "MEDICAL LABORATORY TECHNOLOGY - ASSOCIATE OF APPLIED SCIENCE",
				Ctid = credCtid,
				Description = "Associate Degree : A program that prepares individuals, under the supervision of clinical laboratory scientists/medical technologists, to perform routine medical laboratory procedures and tests and to apply preset strategies to record and analyze data.  Includes instruction in general laboratory procedures and skills; laboratory mathematics; medical computer applications; interpersonal and communications skills; and the basic principles of hematology, medical microbiology, immunohematology, immunology, clinical chemistry, and urinalysis.",
				SubjectWebpage = "http://www.coffeyville.edu/",
				CredentialType = "AssociateDegree",
				CredentialStatusType = "Active"
			};
			//Coffeyville Community College doesn't exist in sandbox, so using a reference org.
			myData.OwnedBy = new List<OrganizationReference>()
			{
				new OrganizationReference()
				{
					Type="CredentialOrganization",
					CTID="ce-3382bacb-2f29-4037-874e-9a53e2398661", //now published, only need CTID
					//Name="Coffeyville Community College",
					//Description="Coffeyville Community College is dedicated to identifying and addressing community and area needs, providing accessible, affordable quality education and training, and promoting opportunities for lifelong learning. CCC offers a wide variety of traditional and technical classes to serve our diverse student population. CCC strives to provide educational classes that are beneficial to the individual student and encourage a healthy engagement in the community. CCC collaborates with area business and industry to train and develop future employees for the area.",
					//SubjectWebpage="http://www.coffeyville.edu/"
				}
			};
			//where owner also offers:
			myData.OfferedBy = myData.OwnedBy;

			//duration
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					Description="The anticipated time for a full-time student (30 annual semester hours) to complete this program of study.",
					ExactDuration = new DurationItem()
					{
						Years=2
					}
				}
			};


			/*
			 * The AggregateDataProfile is an entity describing the count and related statistical information for a given credential. 
			 * The profile has high level statistical information plus a relevantDataset (a list for multiple) which is the DataSetProfile class 
			have to include all the details of a DataSetProfile with DataProfile(s)
			 * Earnings: 
			 *		LowEarnings, MedianEarnings, HighEarnings
			 * NumberAwarded	- Number of credentials awarded.
			 * JobsObtained		- Number of jobs obtained in the region during a given timeframe.
			 */

			var aggregateProfile = new AggregateDataProfile()
			{
				Description = "Median Earnings of Program Graduates in Region upon entry",
				MedianEarnings = 44439,
				JobsObtained = new List<QuantitativeValue>()
				{
					new QuantitativeValue()
					{
						Percentage=88,
						Description="% of Program Graduates Employed in the Region"
					}
				},
				PostReceiptMonths = 0
			};



			myData.AggregateData = new List<AggregateDataProfile>() { aggregateProfile };
			var aggregateProfile2 = new AggregateDataProfile()
			{
				Description = "Median Earnings of Program Graduates in Region after 5 years.",
				MedianEarnings = 44439,
				PostReceiptMonths = 60
			};
			myData.AggregateData.Add( aggregateProfile2 );

			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};
			

			//create a literal to hold data to use with ARC
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API
			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.Ctid.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );

			//LoggingHelper.WriteLogFile( 2, string.Format( "coffeyfille_{0}_payload.json", myRequest.Credential.Ctid ), req.FormattedPayload, "", false );

		}

	}
}
