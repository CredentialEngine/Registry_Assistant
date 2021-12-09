using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class PathwaysTesting
	{
		[TestMethod]
		public void DoAFormatRequest()
		{
			//using simple post
			new PublishPathway().PublishSimpleRecord( "format" );

			new PublishCredentialWithOutcomes().CredentialWithOutcomeData( "format" );
			//temp
			new KansasExamples().CredentialWithAggregateDataProfile( "format" );
		}
	}
}
