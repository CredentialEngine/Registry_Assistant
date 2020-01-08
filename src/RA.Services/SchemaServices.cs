using System;
using System.Linq;

using System.Net.Http;
using System.Runtime.Caching;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RA.Models.BusObj;
using Utilities;

namespace RA.Services
{
    public class SchemaServices
	{
		public static string GetSchemaJSON( string uri )
		{
			var cache = MemoryCache.Default;
			var cachedData = cache[ uri ];
			if( cachedData == null )
			{
				var data = new HttpClient().GetAsync( uri ).Result.Content.ReadAsStringAsync().Result;
				var policy = new CacheItemPolicy();
				policy.AbsoluteExpiration = DateTimeOffset.Now.AddHours( 24 );
				cache.Remove( uri );
				cache.Add( uri, data, policy );
				return data;
			}
			else
			{
				return (string) cachedData;
			}
		}
		//

		public static ConceptScheme GetConceptScheme( string uri )
		{
			var result = new ConceptScheme();
			try
			{
				var raw = GetSchemaJSON( uri );
				var json = JObject.Parse( raw );
				var graph = ( JArray ) json[ "@graph" ];
				var scheme = ( graph ).FirstOrDefault( ( dynamic m ) => m[ "@type" ] == "skos:ConceptScheme" );

				result = new ConceptScheme()
				{
					Label = scheme[ "rdfs:label" ][ "en-US" ],
					Description = scheme[ "rdfs:comment" ][ "en-US" ],
					ConceptURIs = ( ( JArray ) scheme[ "meta:hasConcept" ] ).ToList().ConvertAll( m => ( string ) m ).ToList(),
					Uri = scheme[ "@id" ]
				};

				foreach ( var item in graph.Where( ( dynamic m ) => m[ "@type" ] == "skos:Concept" ).ToList() )
				{
					result.LoadedConcepts.Add( new Concept()
					{
						Label = item[ "skos:prefLabel" ][ "en-US" ],
						Description = item[ "skos:definition" ][ "en-US" ],
						Uri = item[ "@id" ]
					} );
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "SchemaServices.GetConceptScheme");
			}
			return result;
		}
		//

		public static ConceptScheme GetConceptSchemeFromPropertyRange( string wholeSchemaURI, string propertyURI )
		{
			var result = new ConceptScheme();

			try
			{
				var wholeSchemaRaw = GetSchemaJSON( wholeSchemaURI );
				var wholeSchema = JObject.Parse( wholeSchemaRaw );
				var schemaGraph = ( JArray ) wholeSchema[ "@graph" ];

				var targetTerm = schemaGraph.FirstOrDefault( ( dynamic m ) => m[ "@id" ] == propertyURI );
				result = new ConceptScheme()
				{
					Label = targetTerm[ "rdfs:label" ][ "en-US" ],
					Description = targetTerm[ "rdfs:comment" ][ "en-US" ],
					ConceptURIs = (( JArray ) targetTerm[ "schema:rangeIncludes" ]).ToList().ConvertAll( m => ( string ) m ).ToList(),
					Uri = targetTerm[ "@id" ]
				};

				foreach( var itemURI in result.ConceptURIs )
				{
					var match = schemaGraph.FirstOrDefault( ( dynamic m ) => m[ "@id" ] == itemURI );
					if( match != null )
					{
						result.LoadedConcepts.Add( new Concept()
						{
							Label = match[ "rdfs:label" ][ "en-US" ],
							Description = match[ "rdfs:comment" ][ "en-US" ],
							Uri = match[ "@id" ]
						} );
					}
				}
			}
			catch ( Exception ex )
			{
				LoggingHelper.LogError( ex, "SchemaServices.GetConceptSchemeFromPropertyRange" );
			}

			return result;
		}
		//
	}
}