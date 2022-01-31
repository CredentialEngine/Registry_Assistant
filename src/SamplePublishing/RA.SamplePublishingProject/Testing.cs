using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;
using RA.SamplesForDocumentation.Employment;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class Testing
	{
		[TestMethod]
		public void LearningOpportunityWithOutcomeData()
		{
			new PublishLearningOpportunityWithOutcomes().LearningOpportunityWithOutcomeData( "publish" );
		}
		#region Collection publishing
		[TestMethod]
		public void PublishCollection()
		{
			new RA.SamplesForDocumentation.Collections.PublishCollection().Simple( "publish" );

			//PublishCollectionMembers();
		}


		[TestMethod]
		public void PublishCollectionMembers()
		{
			new RA.SamplesForDocumentation.Collections.PublishCollection().PublishWithCollectionMembers( "publish" );
		}
		#endregion

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


		#region Job publishing
		[TestMethod]
		public void PublishJob()
		{
			new RA.SamplesForDocumentation.Employment.Jobs().PublishJob( "format" );

		}


		[TestMethod]
		public void PublishJobList()
		{
			new RA.SamplesForDocumentation.Employment.Jobs().PublishJobList();
		}
		#endregion

	}
}
