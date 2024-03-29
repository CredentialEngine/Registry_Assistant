﻿using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RA.SamplesForDocumentation;


namespace RA.SamplePublishingProject
{
	[TestClass]
	public class TransferValueTesting
	{
		#region TransferIntermediary
		[TestMethod]
		public void TransferIntermediaryPublish()
		{
			//just with IntermediaryFor
			new PublishTransferIntermediary().PublishWithIntermediaryFor();
		}

		[TestMethod]
		public void TransferIntermediaryBulkPublish()
		{
			//using simple post
			new TransferIntermediaryBulkPublish().PublishWithRelatedTransferValues();

			//just with IntermediaryFor
			//new PublishTransferIntermediary().PublishWithIntermediaryFor();
		}

		#endregion

		[TestMethod]
		public void TestPublishTransferValue()
		{
			//using simple post
			new PublishTransferValueProfile().PublishSimpleRecord(false);
		}

		[TestMethod]
		public void TestPublishTransferValue2()
		{
			//using better post with details
			new PublishTransferValueProfile().PublishTVPSameAsRecord(false);
		}

		/// <summary>
		/// Coming soon batch publishing
		/// </summary>
		[TestMethod]
		public void TestBatchPublishTransferValueBulkPublish()
		{
			//using better post with details
			new PublishTransferValueProfileList().PublishList( false );
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
			new PublishACEFrameworks().PublishACEPrinFin( true );
			//
			new PublishACEFrameworks().PublishACEIntroductorySociology( true );

			new ACETransferValues().ACEIntroductorySociologyTVP( true );

			new ACETransferValues().ACEPrinciplesOfFinanceTVP( true );
		}
		#endregion
	}
}
