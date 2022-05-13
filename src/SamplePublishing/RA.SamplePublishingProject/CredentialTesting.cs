using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.Models.Input;
using RA.SamplesForDocumentation;

using RA.SamplesForDocumentation.Credentials;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class CredentialTesting
	{
		[TestMethod]
		public void DoAFormatRequest()
		{
			//using simple post
			new PublishCredential().PublishDetailedRecord( "format");

			new PublishCredentialWithOutcomes().CredentialWithOutcomeData( "format" );
			//temp
			new KansasExamples().CredentialWithAggregateDataProfile( "format" );
		}

		[TestMethod]
		public void DoAPublishRequest()
		{
			//using simple post
			new PublishCredential().PublishDetailedRecord( "publish" );
		}
		[TestMethod]
		public void CredentialWithMultipleLanguages()
		{
			//using simple post
			new CredentialWithMultipleLanguages().PublishDetailedRecord( "publish" );
		}
		//
		[TestMethod]
		public void CredentialWithOutcomeData()
		{
			//using simple post
			new PublishCredentialWithOutcomes().CredentialWithOutcomeData( "publish" );
		}
		//
		[TestMethod]
		public void CredentialWithAlternativeConditions()
		{
			//using simple post
			new PublishCredentialWithMultipleCampuses().PublishRecord( "publish" );
		}
		[TestMethod]
		public void CareerBridgeCredentialsCredentialWithHoldersProfile()
		{
			//using simple post
			new CareerBridgeCredentials().CredentialWithAggregateDataProfile( "publish" );
		}
		[TestMethod]
		public void KansasCredentialWithAggregateData()
		{
			//using simple post
			new KansasExamples().CredentialWithAggregateDataProfile( "publish" );
		}
		[TestMethod]
		public void PublishQACredential()
		{
			//using simple post
			new PublishQACredentialWithETPL().Publish( true );
		}
		[TestMethod]
		public void PublishProPathCredential()
		{
			//
			new ProPathExamples().CredentialWithAggregateDataProfile( "publish" );
			//new ProPathExamples().CredentialWithHoldersProfile( "publish" );
			//PublishProPathLearningOpportunity();
		}
		[TestMethod]
		public void PublishProPathLearningOpportunity()
		{
			//
			new ProPathExamples().RelatedLearningOpportunity( "publish" );
		}
		[TestMethod]
		public void DoAPublishRequestFromExternalData()
		{
			var input = new SamplesForDocumentation.SampleModels.Credential() 
			{ 
				Name="My internal credential",
				Description="Description of my credential",
				CTID="ce-" + Guid.NewGuid().ToString().ToLower(),
				CredentialType = "BachelorDegree",
				SubjectWebpage ="https://example.com?type=thisTest", 
				Keyword = new System.Collections.Generic.List<string>() { "Engineering","Mechanical Engineering","Bachelor"}

			};

			//using simple post
			new PublishCredential().PublishFromInputClass( input ) ;
		}

		[TestMethod]
		public void CredentialSingleDelete()
		{
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			var community = "";
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}

			// This is the CTID of the organization that owns the data being deleted
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
			var owningOrgCTID = "ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73";
			var ctidToDelete = "ce-7051cdf9-43b6-4e5b-8444-37cfeb64fdfe";
			//
			DeleteRequest dr = new DeleteRequest()
			{
				CTID = ctidToDelete.ToLower(),
				PublishForOrganizationIdentifier = owningOrgCTID.ToLower(),
				Community = community
			};
			string message = "";
			new SampleServices().DeleteRequest( dr, apiKey, "credential", ref message, community );
		}

		/// <summary>
		/// Request to delete a list of credentials
		/// NOT IMPLEMENTED COMING SOON
		/// </summary>
		[TestMethod]
		public void CredentialMultipleDelete()
		{
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			var community = "";
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}

			// This is the CTID of the organization that owns the data being deleted
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//
			var owningOrgCTID = "ce-a4041983-b1ae-4ad4-a43d-284a5b4b2d73";
			//
			DeleteRequest dr = new DeleteRequest()
			{
				CTIDList = new System.Collections.Generic.List<string>()
				{
					"ce-7051cdf9-43b6-4e5b-8444-37cfeb64fdfe", "", ""
				},
				PublishForOrganizationIdentifier = owningOrgCTID.ToLower(),
				Community = community
			};
			string message = "";
			new SampleServices().DeleteRequest( dr, apiKey, "credential", ref message, community );
		}
	}
}
