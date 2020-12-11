using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class TransferValueTesting
	{
		[TestMethod]
		public void TestPublishTransferValue()
		{
			//using simple post
			new PublishTransferValueProfile().PublishSimpleRecord();
		}

		[TestMethod]
		public void TestPublishTransferValue2()
		{
			//using better post with details
			new PublishTransferValueProfile().PublishSimpleRecord(false);
		}

		/// <summary>
		/// Coming soon batch publishing
		/// </summary>
		[TestMethod]
		public void TestBatchPublishTransferValueBulkPublish()
		{
			//using better post with details
			new PublishTransferValueProfile().PublishList( false );
		}


		#region ACE
		[TestMethod]
		public void PublishACEIntroductorySociologyTVP()
		{
			//
			new ACETransferValues().ACEIntroductorySociologyTVP( true );

			PublishACEPrinciplesOfFinanceTVP();
		}
		[TestMethod]


		public void PublishACEPrinciplesOfFinanceTVP()
		{
			//
			new ACETransferValues().ACEPrinciplesOfFinanceTVP( true );
		}
		[TestMethod]
		public void PublishACEPrinFinCompetencyamework()
		{
			//new PublishCompetencyFrameworks().PublishACEPrinFin( true );
			//
			new PublishCompetencyFrameworks().PublishACEIntroductorySociology( true );

			new ACETransferValues().ACEIntroductorySociologyTVP( true );

			new ACETransferValues().ACEPrinciplesOfFinanceTVP( true );
		}
		#endregion
	}
}
