using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class InstructionalPrograms
	{
		/// <summary>
		/// Example for populating Instructional Programs (example CIP) for a Credential Request. 
		/// The same approach would be used for other classes that support Instructional Programs such as Assessments and LearningOpportunities. 
		/// Possible Input Types
		/// - List of frameworks
		/// - list of program names
		/// - List of CIP codes
		/// </summary>
		/// <param name="request"></param>
		public static List<FrameworkItem> PopulatePrograms(ref List<string> AlternativeTypes, ref List<string> Codes )
		{
			var InstructionalProgramType = new List<FrameworkItem>
			{
				//Using existing frameworks such as CIP
				//programs from a framework like Classification of Instructional Program - where the information is stored locally and can be included in publishing
				new FrameworkItem()
				{
					Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					FrameworkName = "Classification of Instructional Program",
					Name = "Medieval and Renaissance Studies",
					TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.1301",
					CodedNotation = "30.1301",
					Description = "A program that focuses on the  study of the Medieval and/or Renaissance periods in European and circum-Mediterranean history from the perspective of various disciplines in the humanities and social sciences, including history and archeology, as well as studies of period art and music."
				},
				new FrameworkItem()
				{
					Framework = "https://nces.ed.gov/ipeds/cipcode/search.aspx?y=56",
					FrameworkName = "Classification of Instructional Program",
					Name = "Classical, Ancient Mediterranean and Near Eastern Studies and Archaeology",
					TargetNode = "https://nces.ed.gov/ipeds/cipcode/cipdetail.aspx?y=56&cip=30.2202",
					CodedNotation = "30.2202",
					Description = "A program that focuses on the cultures, environment, and history of the ancient Near East, Europe, and the Mediterranean basin from the perspective of the humanities and social sciences, including archaeology."
				}
			};


            //Instructional Programs not in a known framework
            //Instructional Programs that are not in a framework can still be published using a list of strings.
            AlternativeTypes = new List<string>() { "Cybersecurity 101", "Forensic Science 120", "Forensic Anthropology 400" };

            //CIP code helper - ALternately provided a list of CIP codes. 
            //The Assistant API will validate the codes and format the output including the framework name and URL, the name, description, and code
            Codes = new List<string>() { "31.0504", "31.0505", "31.0599", "31.9999" };

            return InstructionalProgramType;
		}
	}
}
