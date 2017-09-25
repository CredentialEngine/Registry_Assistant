using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace RA.Models.Json
{
		public class JsonLDDocument 
	{   
		public JsonLDDocument()
		{
			Context = new Dictionary<string, object>()
			{
				{ "ceterms", "http://purl.org/ctdl/terms/" },
				{ "dc", "http://purl.org/dc/elements/1.1/" },
				{ "dct", "http://purl.org/dc/terms/" },
				{ "foaf", "http://xmlns.com/foaf/0.1/" },
				{ "obi", "https://w3id.org/openbadges#" },
				{ "owl", "http://www.w3.org/2002/07/owl#" },
				{ "rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#" },
				{ "rdfs", "http://www.w3.org/2000/01/rdf-schema#" },
				{ "schema", "http://schema.org/" },
				{ "skos", "http://www.w3.org/2004/02/skos/core#" },
				{ "vs", "https://www.w3.org/2003/06/sw-vocab-status/ns" },
				{ "xsd", "http://www.w3.org/2001/XMLSchema#" },
				{ "lrmi", "http://purl.org/dcx/lrmi-terms/" },
				{ "asn", "http://purl.org/ASN/schema/core/" },
				{ "vann", "http://purl.org/vocab/vann/" },
				{ "actionStat", "http://purl.org/ctdl/vocabs/actionStat/" },
				{ "agentSector", "http://purl.org/ctdl/vocabs/agentSector/" },
				{ "serviceType", "http://purl.org/ctdl/vocabs/serviceType/" },
				{ "assessMethod", "http://purl.org/ctdl/vocabs/assessMethod/" },
				{ "assessUse", "http://purl.org/ctdl/vocabs/assessUse/" },
				{ "audience", "http://purl.org/ctdl/vocabs/audience/" },
				{ "claimType", "http://purl.org/ctdl/vocabs/claimType/" },
				{ "costType", "http://purl.org/ctdl/vocabs/costType/" },
				{ "credentialStat", "http://purl.org/ctdl/vocabs/credentialStat/" },
				{ "creditUnit", "http://purl.org/ctdl/vocabs/creditUnit/" },
				{ "deliveryType", "http://purl.org/ctdl/vocabs/deliveryType/" },
				{ "inputType", "http://purl.org/ctdl/vocabs/inputType/" },
				{ "learnMethod", "http://purl.org/ctdl/vocabs/learnMethod/" },
				{ "orgType", "http://purl.org/ctdl/vocabs/orgType/" },
				{ "residency", "http://purl.org/ctdl/vocabs/residency/" },
				{ "score", "http://purl.org/ctdl/vocabs/score/" },
				{ "@language", "en-US" } //Default. May change later if/when we implement language selection
			};
		}
		[JsonProperty( "@context" )]
		public Dictionary<string, object> Context { get; set; }
	}
}
