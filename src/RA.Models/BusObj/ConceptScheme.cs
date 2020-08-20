using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.BusObj
{
	//Used for code tables, etc.
	public class ConceptScheme : BaseRdfEntity
	{
		public ConceptScheme()
		{
			LoadedConcepts = new List<Concept>();
		}
		public List<string> ConceptURIs { get; set; }
		public List<Concept> LoadedConcepts { get; set; } //Convenience - should not be used to store data
	}
	//

	public class Concept : BaseRdfEntity
	{

	}
	//
	//

	public class BaseRdfEntity 
	{
		public string Uri { get; set; }
		public string Label { get; set; }
		public string Description { get; set; }
	}
}
