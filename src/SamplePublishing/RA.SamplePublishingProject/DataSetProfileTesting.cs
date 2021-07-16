using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;
using RA.SamplesForDocumentation.PerformanceExamples;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class DataSetProfileTesting
	{
		[TestMethod]
		public void PublishIndianaPassrages()
		{
			//
			new IndianaPassRates().BallStateUniversity( "format" );

			new IndianaPassRates().AndersonUniversity( "format" );
		}

		[TestMethod]
		public void PublishExample()
		{
			//
			new WIOAExamples().Example1( "publish" );

		}
	}
}
