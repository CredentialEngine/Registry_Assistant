using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace RA.Models.BusObj
{
	public class Schema
	{
		public enum SchemaCode { CTDL, CTDLASN, QDATA }

		// Probably okay to hard-code these, as they should work regardless of environment and should never change
		// Also makes it easier to retrieve schema identifier data by enum, or by short prefix, or by URI, etc.
		public static List<SchemaIdentifierSet> SchemaList
		{
			get
			{
				return new List<SchemaIdentifierSet>()
				{
					new SchemaIdentifierSet(){ SchemaCode = SchemaCode.CTDL, ShortURIPrefix = "ceterms:", FullURIPrefix = "https://credreg.net/ctdl/" },
					new SchemaIdentifierSet(){ SchemaCode = SchemaCode.CTDLASN, ShortURIPrefix = "ceasn:", FullURIPrefix = "https://credreg.net/ctdlasn/" },
					new SchemaIdentifierSet(){ SchemaCode = SchemaCode.QDATA, ShortURIPrefix = "qdata:", FullURIPrefix = "https://credreg.net/qdata/" }
				};
			}
		}

		public class SchemaIdentifierSet
		{
			public SchemaCode SchemaCode { get; set; }

			public string ShortURIPrefix { get; set; }

			public string FullURIPrefix { get; set; }
		}

		[Serializable]
		public class SchemaJSON
		{
			public SchemaJSON()
			{
				RawContextData = new JObject();
				RawSchemaData = new JObject();
				Classes = new List<JObject>();
				Properties = new List<JObject>();
				ConceptSchemes = new List<JObject>();
				Concepts = new List<JObject>();
			}

			public SchemaIdentifierSet SchemaIdentifierSet { get; set; }

			public JObject RawContextData { get; set; }

			public JObject RawSchemaData { get; set; }

			public JObject Context { get; set; }

			public List<JObject> AllTerms { get; set; }

			public List<JObject> Classes { get; set; }

			public List<JObject> Properties { get; set; }

			public List<JObject> ConceptSchemes { get; set; }

			public List<JObject> Concepts { get; set; }
		}

		[Serializable]
		public class SchemaData
		{
			public SchemaIdentifierSet SchemaIdentifierSet { get; set; }

			public SchemaContext Context { get; set; }

			public List<SchemaClass> Classes { get; set; }

			public List<SchemaProperty> Properties { get; set; }

			public List<SchemaConceptScheme> ConceptSchemes { get; set; }

			public List<SchemaConcept> Concepts { get; set; }
		}

		public class SchemaItem
		{
			public string LastPartURI { get; set; }

			public string ShortURI { get; set; }

			public string FullURI { get; set; }

			public string PURLURI { get; set; }

			public JObject JSON { get; set; }

			public string Name { get; set; }

			public string Description { get; set; }
		}

		public class SchemaContext
		{
			public List<PrefixMap> PrefixMap { get; set; }
		}

		public class PrefixMap
		{
			public string Prefix { get; set; }

			public string FullURI { get; set; }

			public string PURLURI { get; set; }
		}

		public class SchemaClass : SchemaItem
		{
			public List<string> PropertyURIs { get; set; }
		}

		public class SchemaProperty : SchemaItem
		{
			public List<string> DomainURIs { get; set; }

			public List<string> RangeURIs { get; set; }

			public List<string> TargetSchemeURIs { get; set; }
		}

		public class SchemaConceptScheme : SchemaItem
		{
			public List<string> ConceptURIs { get; set; }
		}

		public class SchemaConcept : SchemaItem
		{
			public List<string> InSchemeURIs { get; set; }
		}
	}
}
