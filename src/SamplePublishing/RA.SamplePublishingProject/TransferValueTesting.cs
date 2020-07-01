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
		public void TestBatchPublishTransferValue2()
		{
			//using better post with details
			//new PublishTransferValueProfile().BatchPublish( false );
		}
	}
}
