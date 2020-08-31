using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;
namespace RA.SamplesForDocumentation
{
	public class Industries
	{
		/// <summary>
		/// Example for populating Industries for a Credential Request. 
		/// The same approach would be used for other classes that support Industries such as Assessments and LearningOpportunities. 
		/// Possible Input Types
		/// - List of frameworks
		/// - list of industry names
		/// - List of NAICS codes
		/// </summary>
		/// <param name="request"></param>
		public static void PopulateIndustries( Credential request )
		{
			request.IndustryType = new List<FrameworkItem>
			{
				//Using existing frameworks such as NAICS
				//occupations from a framework like NAICS - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://www.naics.com/",
					FrameworkName = "NAICS - North American Industry Classification System",
					Name = "National Security",
					TargetNode = "https://www.naics.com/naics-code-description/?code=928110",
					CodedNotation = "928110",
					Description = "This industry comprises government establishments of the Armed Forces, including the National Guard, primarily engaged in national security and related activities."
				},
				new FrameworkItem()
				{
					Framework = "https://www.naics.com/",
					FrameworkName = "NAICS - North American Industry Classification System",
					Name = "Regulation and Administration of Transportation Programs",
					TargetNode = "https://www.naics.com/naics-code-description/?code=926120",
					CodedNotation = "926120",
					Description = "This industry comprises government establishments primarily engaged in the administration, regulation, licensing, planning, inspection, and investigation of transportation services and facilities. Included in this industry are government establishments responsible for motor vehicle and operator licensing, the Coast Guard (except the Coast Guard Academy), and parking authorities."
				}
			};


			//Industries not in a known framework
			//Industries that are not in a framework can still be published using a list of strings.
			request.AlternativeIndustryType = new List<string>() { "Cybersecurity", "Forensic Science", "Forensic Anthropology" };

			//NAICS helper - ALternately provided a list of NAICS codes. 
			//The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
			request.Naics = new List<string>() { "9271", "927110", "9281", "928110" };
		}
	}
}
