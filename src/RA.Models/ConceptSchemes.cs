using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	public class ConceptSchemes
	{
		public List<ConceptSchemeDTO> CTDLConceptSchemes { get; set; } = new List<ConceptSchemeDTO>();
	}
	public class ConceptSchemeDTO
	{
		public string Label { get; set; }
		public string ConceptSchemeId { get; set; }
		//public string Definition { get; set; }
		public List<ConceptDTO> Concepts { get; set; } = new List<ConceptDTO>();
	}
	public class ConceptDTO
	{
		public ConceptDTO()
		{
			SortOrder = 10;
		}

		public string Label { get; set; }

		public string Definition { get; set; }
		public string InScheme { get; set; }
		public string SchemaLabel { get; set; }
		public int SortOrder { get; set; }

	}
}
