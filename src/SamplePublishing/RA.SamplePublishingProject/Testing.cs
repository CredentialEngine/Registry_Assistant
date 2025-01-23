using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;
using RA.SamplesForDocumentation.Employment;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class Testing
	{
        #region Lopp types
        [TestMethod]
		public void LearningOpportunityWithOutcomeData()
		{
            Assert.IsTrue( new PublishLearningOpportunityWithOutcomes().LearningOpportunityWithOutcomeData( "publish" ) );
		}

		[TestMethod]
		public void PublishCourse()
		{
			new PublishLearningOpportunityTypes().PublishCourse( "publish" );
		}
		#endregion
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

		[TestMethod]
		public void PublishLikeAFramework()
		{
			new RA.SamplesForDocumentation.Collections.PublishCollection().PublishLikeAFrameworkWithCompetencies( "publish" );
		}
		//
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

        [TestMethod]
        public void PublishIndustry()
        {
           Assert.IsTrue( new RA.SamplesForDocumentation.PublishIndustry().DoPublish( "format" ) );

        }

        #region Job publishing
        [TestMethod]
		public void PublishJob()
		{
            Assert.IsTrue( new RA.SamplesForDocumentation.Employment.Jobs().PublishJob( "format" ) );

        }


        [TestMethod]
		public void PublishJobList()
		{
			new RA.SamplesForDocumentation.Employment.Jobs().PublishJobList();
		}
		#endregion


		#region Rubric publishing
		[TestMethod]
		public void PublishRubric()
		{
			new RA.SamplesForDocumentation.PublishRubric().Publish( "publish" );

		}

		#endregion
		#region ScheduledOffering publishing
		[TestMethod]
		public void PublishScheduledOffering()
		{
			new RA.SamplesForDocumentation.PublishScheduledOffering().Publish( "format" );

		}

        #endregion

        #region SupportService publishing
        [TestMethod]
        public void PublishSupportService()
        {
            new RA.SamplesForDocumentation.PublishSupportService().Publish( "format" );

        }


        [TestMethod]
        public void PublishSupportServiceList()
        {
            new RA.SamplesForDocumentation.PublishSupportServiceList().Publish();

        }

        #endregion

    }
}
