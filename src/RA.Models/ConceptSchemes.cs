using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	/// <summary>
	/// Prototype
	/// Under development - process to get all concept schemes from credreg.net and cache for use in validation. 
	/// This process would then speed up the validation of concepts when publishing documents. 
	/// </summary>
	public class ConceptSchemes
	{
		public List<ConceptSchemeDTO> CTDLConceptSchemes { get; set; } = new List<ConceptSchemeDTO>();
	}

	public class ConceptSchemeDTO
	{
		public string Label { get; set; }

		public string ConceptSchemeId { get; set; }

		// public string Definition { get; set; }
		public List<ConceptDTO> Concepts { get; set; } = new List<ConceptDTO>();
	}

	public class ConceptDTO
	{
		public ConceptDTO()
		{
			SortOrder = 10;
		}

		public string Label { get; set; }

		public string ShorthandURI { get; set; }

		public string Definition { get; set; }

		public string InScheme { get; set; }

		public string SchemaURI { get; set; }

		public int SortOrder { get; set; }
	}
}
