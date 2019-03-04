using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;
using OrgRequest = RA.Models.Input.OrganizationRequest;

namespace RA.SamplesForDocumentation.InputSamples
{
	public class Frameworks
	{
		public void AssignFrameworks()
		{
			
			Enumeration occupations = new Enumeration();


			Credential request = new Credential();
			FrameworkItem fi = new FrameworkItem();
			List<FrameworkItem> occupationTypes = new List<FrameworkItem>();
			string frameworkName = "The Occupational Information Network (O*NET)";
			string framework = "https://www.onetonline.org";
			//get all relevent occupations for internal source
			foreach ( var item in occupations.Items )
			{
				fi = new FrameworkItem();
				fi.FrameworkName = frameworkName;
				fi.Framework = framework;
				fi.CodedNotation = item.CodedNotation;
				fi.Name = item.Name;
				fi.Description = item.Description ?? "";
				fi.TargetNode = "https://www.onetonline.org/link/summary/" + fi.CodedNotation;

				request.OccupationType.Add( fi );
			}

			//occupations, etc. that are not part of a formal framework. The only required property for a framework item is the name (or targetNodeName)
			request.OccupationType.Add( new FrameworkItem() { Name = "Cybersecurity Specialist" } );
		}

		public void AssignAlternativeFrameworks()
		{
			Credential request = new Credential();

			request.AlternativeIndustryType.Add( "Cyber security" );
			request.AlternativeIndustryType.Add( "Some industry name not found in NAICS" );

			//using language map lists 
			request.AlternativeIndustryType_Map.Add( "en-us", new List<string>() { "Cyber security", "Analysts" } );
			request.AlternativeIndustryType_Map.Add( "fr", new List<string>() { "La cyber-sécurité", "Les analystes" } );
		}


		public void HelperAssignCIP()
		{
			Credential request = new Credential();

			request.CIP_Codes.Add( "Cyber security" );
			request.AlternativeIndustryType.Add( "Some industry name not found in NAICS" );
		}
	}

	public class Enumeration
	{
		public Enumeration()
		{
			Items = new List<EnumeratedItem>();
		}
		public string Name { get; set; }
		public string SchemaName { get; set; }
		public string Description { get; set; }
		public int ParentId { get; set; }
		public string Url { get; set; }
		public string FrameworkVersion { get; set; }
		public List<EnumeratedItem> Items { get; set; }
	}
	//

	public class EnumeratedItem
	{
		public EnumeratedItem()
		{
			
		}
	
		/// <summary>
		/// Displayed name
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Url - optional
		/// </summary>
		public string URL { get; set; }
		/// <summary>
		/// Description (if applicable)
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Schema-based name. Should not contain spaces. May not be necessary.
		/// </summary>
		public string SchemaName { get; set; }
		/// <summary>
		/// Schema-based name of the parent code, where present.
		/// </summary>
		public string ParentSchemaName { get; set; }

		/// <summary>
		/// Value referenced in "value" property of HTML objects
		/// </summary>
		public string Value { get; set; }
		public string CodedNotation {
			get { return Value; }
			set { Value = value; }
		}
		public bool IsSpecialValue { get; set; }

		/// <summary>
		/// URL to schema descriptor - can probably delete this
		/// </summary>
		public string SchemaUrl { get; set; }
		
										  
	}
}
