using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;
using RA.SamplesForDocumentation.Employment;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class Testing
	{
		#region transfer value publishing
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
		#endregion

		#region Occupation publishing
		[TestMethod]
		public void PublishOccupation()
		{
			new Occupations().PublishOccupation();

		}


		[TestMethod]
		public void PublishOccupationList()
		{
			new Occupations().PublishOccupationList();
		}
		#endregion


	}
}
