using Newtonsoft.Json;

namespace RA.Models.Input
{
    public class SchemaDate
    {
        [JsonProperty( "@type" )]
        public string Type { get; set; } = "schema:Date";

        [JsonProperty( "@id" )]
        public string Id { get; set; }

        [JsonProperty( "@value" )]
        public string Value { get; set; }

    }
}
