using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			new KansasExamples().CredentialWithAggregateDataProfile( "format" );
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
				Ctid="ce-" + Guid.NewGuid().ToString().ToLower(),
				CredentialType = "BachelorDegree",
				SubjectWebpage ="https://example.com?type=thisTest", 
				Keyword = new System.Collections.Generic.List<string>() { "Engineering","Mechanical Engineering","Bachelor"}

			};

			//using simple post
			new PublishCredential().PublishFromInputClass( input ) ;
		}
	}
}
