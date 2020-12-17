using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class Testing
	{
		[TestMethod]
		public void PublishNCCRSStuff()
		{
			//new NCCRSTransferValues().PublishNCCRS( true );
			//
			new NCCRSTransferValues().AdvancedLegalIssuesFraudInvestigationTVP( true );

		}


			[TestMethod]
			public void PublishIndianaTVPs()
			{
				new IndianaTransferValues().IntroductiontoBusinessTVP( true );
				//


			}
		}
}
