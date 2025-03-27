using Newtonsoft.Json;

namespace RA.Models.Input
{
    public class CompetencyFrameworkGraph
    {

        [JsonProperty( "@context" )]
        public string Context { get; set; }

        [JsonProperty( "@id" )]
        public string Id { get; set; }

        /// <summary>
        /// Main graph object
        /// </summary>
        [JsonProperty( "@graph" )]
        public object Graph { get; set;  }

    }
}
