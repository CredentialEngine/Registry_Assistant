﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class OccupationsHelper
	{

        /// <summary>
        /// Example for populating Occupations for a Credential Request. 
        /// The same approach would be used for other classes that support Occupations such as Assessments and LearningOpportunities. 
        /// Possible Input Types
        /// - List of frameworks
        /// - list of occupation names
        /// - List of SOC codes
        /// </summary>
        /// <param name="AlternativeTypes">If applicable, will return a list of occupations (name only) for use with request.AlternativeInstructionalProgramType</param>
        /// <param name="ONET_Codes">If applicable, will return a list of SOC codes for use with request.ONET_Codes</param>
        /// <returns></returns>
        public static List<FrameworkItem> PopulateOccupations( ref List<string> AlternativeTypes, ref List<string> ONET_Codes )
		{
			var OccupationType = new List<FrameworkItem>();

			//Using existing frameworks such as O*Net
			//occupations from a framework like ONet - where the information is stored locally and can be included in publishing
			OccupationType.Add( new FrameworkItem()
			{
				Framework = "https://www.onetcenter.org/taxonomy.html",
				FrameworkName = "Standard Occupational Classification",
				Name = "Information Security Analysts",
				TargetNode = "https://www.onetonline.org/link/summary/15-1122.00",
				CodedNotation = "15-1122.00",
				Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
			} );
			OccupationType.Add( new FrameworkItem()
			{
				Framework = "https://www.onetcenter.org/taxonomy.html",
				FrameworkName = "Standard Occupational Classification",
				Name = "Computer Network Support Specialists",
				TargetNode = "https://www.onetonline.org/link/summary/15-1152.00",
				CodedNotation = "15-1152.00",
				Description = "Plan, implement, upgrade, or monitor security measures for the protection of computer networks and information. May ensure appropriate security controls are in place that will safeguard digital files and vital electronic infrastructure. May respond to computer security breaches and viruses."
			} );
			//or if want to just reference the Job family
			OccupationType.Add( new FrameworkItem()
			{
				Framework = "https://www.onetcenter.org/taxonomy.html",
				FrameworkName = "Standard Occupational Classification",
				Name = "Construction and Extraction",
				TargetNode = "https://www.onetonline.org/find/family?f=47",
				CodedNotation = "47"
			} );

			////Occupations not in a known framework
			////Occupations that are not in a framework can still be published using a list of strings.
			AlternativeTypes = new List<string>() { "Cybersecurity", "Forensic Scientist", "Forensic Anthropologist" };

            //O*Net helper - ALternately provided a list of O*Net codes. 
            //The Assistant API will validate the codes and format the output including the framework name and URL, the occupation, description, and code
            //request.ONET_Codes 
            ONET_Codes = new List<string>() { "13-2099.01", "13-2052.00", "13-2061.00", "13-2051.00" };
			return OccupationType;
		}

	}
}
