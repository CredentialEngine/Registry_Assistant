using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RA.Models.Json
{
    class CompetencyFramework : JsonLDDocument
    {
        [JsonIgnore]
        public static string classType = "ceasn:CompetencyFramework";
        public CompetencyFramework()
        {
            Type = classType;
        }
        [JsonProperty( "@type" )]
        public string Type { get; set; }

        [JsonProperty( "@id" )]
        public string CtdlId { get; set; }

        [JsonProperty( PropertyName = "ceasn:ctid" )]
        public string Ctid { get; set; }



        [JsonProperty( PropertyName = "ceasn:alignFrom" )]
        public List<IdProperty> alignFrom { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:alignTo" )]
        public List<IdProperty> alignTo { get; set; } = new List<IdProperty>();

        [JsonProperty( "@author" )]
        public List<string> author { get; set; } = new List<string>();

        

        [JsonProperty( PropertyName = "ceasn:conceptKeyword" )]
        public List<LanguageMap> conceptKeyword { get; set; }

        [JsonProperty( PropertyName = "ceasn:conceptTerm" )]
        public List<IdProperty> conceptTerm { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:creator" )]
        public List<IdProperty> creator { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:dateCopyrighted" )]
        public string dateCopyrighted { get; set; }

        [JsonProperty( PropertyName = "ceasn:dateCreated" )]
        public string dateCreated { get; set; }


        [JsonProperty( PropertyName = "ceasn:dateValidFrom" )]
        public string dateValidFrom { get; set; }

        [JsonProperty( PropertyName = "ceasn:dateValidUntil" )]
        public string dateValidUntil { get; set; }

        [JsonProperty( PropertyName = "ceasn:derivedFrom" )]
        public List<IdProperty> derivedFrom { get; set; } = new List<IdProperty>();

        //???language map??
        [JsonProperty( PropertyName = "ceasn:description" )]
        public LanguageMap description { get; set; } = new LanguageMap();

        [ JsonProperty( PropertyName = "ceasn:educationLevelType" )]
        public List<IdProperty> educationLevelType { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:hasTopChild" )]
        public List<IdProperty> hasTopChild { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:identifier" )]
        public List<IdProperty> identifier { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:inLanguage" )]
        public List<string> inLanguage { get; set; } = new List<string>();

        [JsonProperty( PropertyName = "ceasn:license" )]
        public IdProperty license { get; set; }

        [JsonProperty( PropertyName = "ceasn:localSubject" )]
        public List<LanguageMap> localSubject { get; set; } = new List<LanguageMap>();


        [JsonProperty( PropertyName = "ceasn:name" )]
        public LanguageMap name { get; set; } = new LanguageMap();

        [ JsonProperty( PropertyName = "ceasn:publicationStatusType" )]
        public List<IdProperty> publicationStatusType { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:publisher" )]
        public List<IdProperty> publisher { get; set; } = new List<IdProperty>();

        [JsonProperty( PropertyName = "ceasn:publisherName" )]
        public List<LanguageMap> publisherName { get; set; } = new List<LanguageMap>();
        //

        [JsonProperty( PropertyName = "ceasn:repositoryDate" )]
        public string repositoryDate { get; set; }

        [JsonProperty( PropertyName = "ceasn:rights" )]
        public IdProperty rights { get; set; }

        [JsonProperty( PropertyName = "ceasn:rightsHolder" )]
        public IdProperty rightsHolder { get; set; }

        [JsonProperty( PropertyName = "ceasn:source" )]
        public List<IdProperty> source { get; set; } = new List<IdProperty>();
   
        //
        [JsonProperty( PropertyName = "ceasn:tableOfContents" )]
        public List<LanguageMap> tableOfContents { get; set; } = new List<LanguageMap>();
    }
}
