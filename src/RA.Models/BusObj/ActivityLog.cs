using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.BusObj
{
	 
	public class ActivityLog
	{
		public static string AssessmentType = "AssessmentProfile";
		public static string LearningOpportunity = "LearningOpportunity";

		public ActivityLog()
		{
			Application = "RegistryAssistant";
			ActivityType = "Audit";
		}
		//public int Id { get; set; }
		public DateTime Created { get; set; }
		public bool IsExternalActivity { get; set; }

		public string Application { get; set; }
		public string ActivityType { get; set; }
		public string DataOwnerCTID { get; set; }
		public string ActivityObjectCTID { get; set; }
		public string Activity { get; set; }
		public string Event { get; set; }
		public string Comment { get; set; }

		public string IPAddress { get; set; }
		public string Referrer { get; set; }
		//public bool IsBot { get; set; }

		//for search results
		//public int ParentEntityTypeId { get; set; }
		//public string ParentObject { get; set; }
		//public string Organization { get; set; }
		//public string OrganizationUID { get; set; }
	}
}
