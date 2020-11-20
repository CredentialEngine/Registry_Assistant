using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class CredentialTesting
	{
		[TestMethod]
		public void DoAFormatRequest()
		{
			//using simple post
			new PublishCredential().PublishSimpleRecord("format");
		}

		[TestMethod]
		public void DoAPublishRequest()
		{
			//using simple post
			new PublishCredential().PublishSimpleRecord( "publish" );
		}
		[TestMethod]
		public void CredentialWithHoldersProfile()
		{
			//using simple post
			new PublishCredentialWithOutcomes().CredentialWithHoldersProfile( "publish" );
		}
		[TestMethod]
		public void PublishQACredential()
		{
			//using simple post
			new PublishCredential().PublishQACredentialWithETPL( "publish" );
		}
		[TestMethod]
		public void PublishProPathCredential()
		{
			//
			//new ProPathExamples().CredentialWithHoldersProfile( "format" );
			new ProPathExamples().CredentialWithHoldersProfile( "publish" );
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
			new PublishCredential().PublishFromInput( input ) ;
		}
	}
}
