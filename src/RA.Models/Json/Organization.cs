using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Json
{
    public class Organization : OrganizationBase
	{
        public Organization()
        {
            //Industries = new List<FrameworkItem>();
            AgentType = new List<string>();
            Keyword = new List<string>();
            ServiceType = new List<string>();
            //Address = new Address();
        }

        [JsonProperty( "ceterms:foundingDate" )]
        public string FoundingDate { get; set; }

        [JsonProperty( "@type" )]
        public string Ctid { get; set; }

        [JsonProperty( "ceterms:image" )]
        public string Image { get; set; } //Image URL

        [JsonProperty( "ceterms:subjectWebpage" )]
        public string SubjectWebpage { get; set; } //URL

        [JsonProperty( "ceterms:duns" )]
        public string Duns { get; set; }

        [JsonProperty( "ceterms:fein" )]
        public string Fein { get; set; }

        [JsonProperty( "ceterms:ipedsID" )]
        public string IpedsId{ get; set; }

        [JsonProperty( "ceterms:opeID" )]
        public string OpeId { get; set; }

        [JsonProperty( "ceterms:alternativeIdentifier" )]
        public string AlternativeIdentifier { get; set; }

        public string MissionAndGoalsStatement { get; set; }

        public string MissionAndGoalsStatementDescription { get; set; }

        public string AgentPurpose { get; set; }

        public string AgentPurposeDescription { get; set; }

        /// <summary>
        /// Need process to provide valid types (orgType)
        /// </summary>
        public List<string> AgentType { get; set; }

        /// <summary>
        /// Need process to provide valid types (agentSectorType)
        /// </summary>
        public List<string> AgentSectorType { get; set; }
        //public List<FrameworkItem> Industries { get; set; }
        public List<string> Keyword { get; set; }
        public List<string> Email { get; set; }
        public List<string> SocialMedia { get; set; }
        public List<string> AvailabilityListing { get; set; }
        public List<string> ServiceType { get; set; }
        //public Jurisdiction Jurisdiction { get; set; }
        //public Address Address { get; set; }
        
    }
}
