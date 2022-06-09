	public class ExampleQuery
	{
		//Class for the query
		public class QueryWrapper
		{
			public int Skip { get; set; }
			public int Take { get; set; }
			public JObject Query { get; set; }
			public string DescriptionSetType { get; set; }
		}

		//Classes for the response
		public class SearchResponse
		{
			public List<JObject> data { get; set; }
			public bool valid { get; set; }
			public string status { get; set; }
			public SearchResponseExtra extra { get; set; }
		}
		public class SearchResponseExtra
		{
			//Always included
			public int TotalResults { get; set; }

			//Only if you request description set data
			public List<RelatedItemsWrapper> RelatedItemsMap { get; set; }
			public List<JObject> RelatedItems { get; set; }
		}

		public static SearchResponse RunSampleQuery( string ownerCTID, List<string> ctdlTypeURIs, int skip, int take )
		{
			//Build the query
			var wrapper = new QueryWrapper()
			{
				Skip = skip,
				Take = take,
				Query = new JObject()
				{
					{ "@type", JArray.FromObject( ctdlTypeURIs ) }, //e.g., new List<string>() { "ceterms:AssociateDegree", "ceterms:BachelorDegree" }
					{ "ceterms:ownedBy", new JObject()
						{
							{ "ceterms:ctid", ownerCTID } //e.g., "ce-2ecc2ce8-b134-4a3a-8b17-863aa118f36e"
						}
					}
				}
			};

			//In case you need to add something later in the code, you can access the JSON tree directly
			wrapper.Query[ "ceterms:ownedBy" ][ "ceterms:name" ] = new JObject()
			{
				{ "search:value", "Some College Name" },
				{ "search:matchType", "search:exactMatch" }
			};

			//Format the request
			var queryJSON = JsonConvert.SerializeObject( wrapper, new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore } );

			//Do the request
			var client = new HttpClient();
			var apiKey = "Your API Key Here"; //Probably get this from a configuration file
			var apiURL = "https://apps.credentialengine.org/assistant/search/ctdl"; //Ditto
			client.DefaultRequestHeaders.TryAddWithoutValidation( "Authorization", "ApiToken " + apiKey );
			var rawResult = client.PostAsync( apiURL, new StringContent( queryJSON, Encoding.UTF8, "application/json" ) ).Result;

			//Process the response
			if ( rawResult.IsSuccessStatusCode )
			{
				var response = JsonConvert.DeserializeObject<SearchResponse>( rawResult.Content.ReadAsStringAsync().Result, new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None } );
				return response;
			}
			else
			{
				//Error logging goes here
				return null;
			}

		}

		public static void Example()
		{
			var result = RunSampleQuery( "ce-2ecc2ce8-b134-4a3a-8b17-863aa118f36e", new List<string>() { "ceterms:AssociateDegree", "ceterms:BachelorDegree" }, 0, 10 );
		}

	}
