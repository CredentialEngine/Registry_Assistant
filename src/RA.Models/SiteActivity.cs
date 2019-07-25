using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	public class SiteActivity 
	{
		public static string AssessmentType = "AssessmentProfile";
		public static string LearningOpportunity = "LearningOpportunity";

		public SiteActivity()
		{
			ActivityType = "RegistryAssistant";
		}
		
		public string ActivityType { get; set; }
		public string Activity { get; set; }
		public string Event { get; set; }
		public string Comment { get; set; }
		public string DataOwnerCTID { get; set; }
		public string ActionByUser { get; set; }
		public string IPAddress { get; set; }
		public string Referrer { get; set; }

	}
}
