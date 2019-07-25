using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	public class ConceptSchemes
	{
		public List<ConceptScheme> ConceptScheme { get; set; } = new List<ConceptScheme>();
	}
	public class ConceptScheme
	{
		public string Label { get; set; }
		public string Scheme { get; set; }
		//public string Definition { get; set; }
		public List<Concept> Concepts { get; set; } = new List<Concept>();
	}
	public class Concept
	{
		public Concept()
		{
		}

		public string Label { get; set; }

		public string Definition { get; set; }
		public string InScheme { get; set; }
		public string SchemaLabel { get; set; }

	}
}
