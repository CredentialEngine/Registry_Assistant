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
		public void PublishACEIntroductorySociologyTVP()
		{
			//
			new ACETransferValues().ACEIntroductorySociologyTVP(true);

			PublishACEPrinciplesOfFinanceTVP();
		}
		[TestMethod]
		public void PublishACEPrinciplesOfFinanceTVP()
		{
			//ACEPrinciplesOfFinanceTVP
			new ACETransferValues().ACEPrinciplesOfFinanceTVP( true );
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
	}
}
