using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Utilities;

namespace RA.SamplePublishingProject
{
	[TestClass]
	public class ToolTesting
	{
		[TestMethod]
		public void GenerateCTIDsFromBaseCTID()
		{
			/*
			 * As a means to minimize having to store CTIDs for related classes, it 'appears' save to simply create a CTID based on a CTID
			 * This could be useful where a base CTID, say for a credential is stored in the database, and then the CTID for a related DatasetProfile could be generated from the credential CTID
			 * credential
			 *			DatasetProfile CTID from credential CTID
			 * 
			 */
			string ctid = "ce-3e7df7ec-1a9b-4503-9ff3-21256022b515";
			//do
			var holdersProfilectid = UtilityManager.CreateCtidFromString( ctid );
			LoggingHelper.DoTrace( 1, "holdersProfilectid: " + holdersProfilectid );
			//
			var datasetProfilectid = UtilityManager.CreateCtidFromString( holdersProfilectid );
			LoggingHelper.DoTrace( 1, "datasetProfilectid: " + datasetProfilectid );

			//now if multiple classes (that require a CTID) need to be published, then a similar approach can used with any dependable unique identifer. 
		}

	}
}
