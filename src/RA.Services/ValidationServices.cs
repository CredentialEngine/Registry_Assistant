using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

//may want an alternative to this
//using CtdlHelper = Factories;

//using Models;

namespace RA.Services
{
    public class ValidationServices
	{
		#region codes

		static List<string> credentialTypes = new List<string>()
		{
			"ceterms:ApprenticeshipCertificate",
			"ceterms:AssociateDegree",
			"ceterms:BachelorDegree",
			"ceterms:Badge",
			"ceterms:Certificate",
			"ceterms:Certification",
			"ceterms:DigitalBadge",
			"ceterms:DoctoralDegree",
			"ceterms:GeneralEducationDevelopment",
			"ceterms:JourneymanCertificate",
			"ceterms:License",
			"ceterms:MasterCertificate",
			"ceterms:MasterDegree",
			"ceterms:MicroCredential",
			"ceterms:OpenBadge",
			"ceterms:ProfessionalDoctorate",
			"ceterms:QualityAssuranceCredential",
			"ceterms:ResearchDoctorate",
			"ceterms:SecondarySchoolDiploma"
		};

		static List<string> otherClassTypes = new List<string>()
		{
			"ceterms:CredentialOrganization",
			"ceterms:QACredentialOrganization",
			"ceterms:AssessmentProfile",
			"ceterms:LearningOpportunityProfile",
			"ceterms:ConditionManifest",
			"ceterms:CostManifest"
		};
		#endregion

		#region validation with code tables
		/// <summary>
		/// Check if the property exists
		/// the proper case adjusted property name will be returned
		/// </summary>
		/// <param name="vocabulary"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static bool IsCredentialTypeValid( string vocabulary, ref string property )
		{

            //if ( CtdlHelper.CodesManager.IsPropertySchemaValid( categoryCode, ref property ) == false )
            //    return false;

            //CodeItem ci = GetVocabularyTermJson(vocabulary, property, ref isValid );
            try
			{
				var targetVocab = vocabulary.Contains( ':' ) ? vocabulary.Split( ':' )[ 1 ] : vocabulary;
				targetVocab = GetVocabularyConceptScheme( vocabulary );
				if ( string.IsNullOrWhiteSpace( targetVocab ) )
				{
					//what to do ??
					return false;
				}

				var vocabsUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegVocabsApi", "http://credreg.net/ctdl/vocabs/" );
				var targetTerm = property.Contains( ':' ) ? property.Split( ':' )[ 1 ] : property;
				var rawJson = new HttpClient().GetAsync( vocabsUrl + targetVocab + "/" + targetTerm + "/json" ).Result.Content.ReadAsStringAsync().Result;
				var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiTermResult>( rawJson );
				var data = deserialized.graph.First();
				//could have a check that returned Id matches the request, as if not found, returns the first item
				if ( data != null && data.id.ToLower().IndexOf( property.ToLower() ) == -1 )
				{
					//what to do ??
					return false;
				}
				string parentSchema = GetVocabularyConceptScheme( vocabulary );
				var result = new CodeItem()
				{
					SchemaName = data.id,
					Name = GetLabelWithoutLanguage(data),
					ParentSchemaName = parentSchema

				};
	

				return true;
			}
			catch ( Exception ex )
			{
				return false;
			}

		}

		public static bool IsValidCredentialType( string credentialType, ref string validSchema )
		{
			bool isValid = false;
			//prefix if necessary
			if ( credentialType.IndexOf( "ceterms" ) == -1 )
				credentialType = "ceterms:" + credentialType;
			validSchema = "";
			string exists = credentialTypes.FirstOrDefault( s => s == credentialType );
			//or maybe loop thru and check case independent
			foreach ( string s in credentialTypes )
			{
				if ( s == credentialType ||
					s.ToLower() == credentialType.ToLower() )
				{
					validSchema = s;
					isValid = true;
					break;
				}
			}

			return isValid;
		}

		/// <summary>
		/// Validate property
		/// </summary>
		/// <param name="vocabulary"></param>
		/// <param name="property"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static bool IsTermValid( string ctdlProperty, string term, ref CodeItem code )
		{
			bool isValid = true;

			code = GetVocabularyTermJson( ctdlProperty, term, ref isValid );

			return isValid;
		}

		//public static bool IsAgentServiceCodeValid( string categoryCode, string property, ref CodeItem code )
		//{
		//	bool isValid = true;

		//	if ( CtdlHelper.OrganizationServiceManager.IsPropertySchemaValid( categoryCode, property, ref code ) == false )
		//		return false;


		//	return isValid;
		//}

		public static bool IsSchemaNameValid( string classSchema, ref string validSchema )
		{
			bool isValid = false;
			validSchema = "";
			if ( string.IsNullOrWhiteSpace( classSchema ))
			{
				validSchema = "";
				return false;
			}
				
			//prefix if necessary
			if ( classSchema.IndexOf( "ceterms" ) == -1 )
				classSchema = "ceterms:" + classSchema;
			
			string exists = otherClassTypes.FirstOrDefault( s => s == classSchema );
			//or maybe loop thru and check case independent
			foreach ( string s in otherClassTypes )
			{
				if ( s == classSchema ||
					s.ToLower() == classSchema.ToLower() )
				{
					validSchema = s;
					isValid = true;
					//break;
					return true;
				}
			}
			if ( validSchema == "")
			{
				foreach ( string s in credentialTypes )
				{
					if ( s == classSchema ||
						s.ToLower() == classSchema.ToLower() )
					{
						validSchema = s;
						isValid = true;
						break;
					}
				}
			}
			
			return isValid;
		}
        #endregion

        #region  validation using code tables - really
        /// <summary>
        /// Check if the provided property schema is valid
        /// </summary>
        /// <param name="category"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static bool IsPropertySchemaValid( string categoryCode, ref string schemaName )
        {
            CodeItem item = GetPropertyBySchema( categoryCode, schemaName );

            if ( item != null && item.Id > 0 )
            {
                //the lookup is case insensitive
                //return the actual schema name value
                schemaName = item.SchemaName;
                return true;
            }
            else
                return false;
        }

        public static bool IsPropertySchemaValid( string categoryCode, string schemaName, ref CodeItem item )
        {
            item = GetPropertyBySchema( categoryCode, schemaName );

            if ( item != null && item.Id > 0 )
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Get a single property using the category code, and property schema name
        /// </summary>
        /// <param name="category"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public static CodeItem GetPropertyBySchema( string categoryCode, string schemaName )
        {
            CodeItem code = new CodeItem();

            //using ( var context = new EM.CTIEntities() )
            //{
            //    //for the most part, the code schema name should be unique. We may want a extra check on the categoryCode?
            //    //TODO - need to ensure the schemas are accurate - and not make sense to check here
            //    Codes_PropertyCategory category = context.Codes_PropertyCategory
            //                .FirstOrDefault( s => s.SchemaName.ToLower() == categoryCode.ToLower() && s.IsActive == true );

            //    Data.Codes_PropertyValue item = context.Codes_PropertyValue
            //        .FirstOrDefault( s => s.SchemaName == schemaName );
            //    if ( item != null && item.Id > 0 )
            //    {
            //        //could have an additional check that the returned category is correct - no guarentees though
            //        code = new CodeItem();
            //        code.Id = ( int )item.Id;
            //        code.CategoryId = item.CategoryId;
            //        code.Title = item.Title;
            //        code.Description = item.Description;
            //        code.URL = item.SchemaUrl;
            //        code.SchemaName = item.SchemaName;
            //        code.ParentSchemaName = item.ParentSchemaName;
            //        code.Totals = item.Totals ?? 0;
            //    }
            //}
            return code;
        }
        #endregion

        #region  validation using webservice
        /// <summary>
        /// Get concept scheme for a cdtl property, and verify the passed term is a valid concept
        /// </summary>
        /// <param name="ctdlProperty"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static CodeItem GetVocabularyTermJson( string ctdlProperty, string term, ref bool isValid )
		{
			isValid = true;
			try
			{
				//may want to be configurable to use pending
				var ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegVocabsApi", "http://credreg.net/ctdl/vocabs/");

				//var targetVocab = ctdlProperty.Contains( ':' ) ? ctdlProperty.Split( ':' )[ 1 ] : ctdlProperty;
                //actually full conceptScheme (property was in cache returns without ceterms
				var targetVocab = GetVocabularyConceptScheme( ctdlProperty );
				if ( string.IsNullOrWhiteSpace( targetVocab ) )
				{
					//what to do ??
					isValid = false;
					return null;
				}
                //plain conceptScheme
				string conceptSchemePlain = targetVocab.Contains( ':' ) ? targetVocab.Split( ':' )[ 1 ] : targetVocab;

				var targetTerm = term.Contains( ':' ) ? term.Split( ':' )[ 1 ] : term;
				var rawJson = new HttpClient().GetAsync( ctdlUrl + conceptSchemePlain + "/" + targetTerm + "/json" ).Result.Content.ReadAsStringAsync().Result;
				//just getting minimum properties
				var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiTermResult>( rawJson );
				var data = deserialized.graph.First();
				//could have a check that returned Id matches the request, as if not found, returns the first item
				if ( data != null && data.id.ToLower().IndexOf( targetTerm.ToLower() ) == -1 )
				{
					//what to do ??
					isValid = false;
					return null;
				}
				//we only care if found

				var result = new CodeItem()
				{
					SchemaName = data.id,
					Name = GetLabelWithoutLanguage( data ),
					//Name = data.skos_prefLabel != null ? data.skos_prefLabel.ToString() : "",
					//Description = ( data.skos_definition ?? "" ),
					ParentSchemaName = targetVocab,
                    ConceptSchemaPlain = conceptSchemePlain
                    //UsageNote = ( data.skos_scopeNote.FirstOrDefault() ?? "" ),
                    //Comment = string.IsNullOrWhiteSpace( data.dcterms_description ) ? "" : data.dcterms_description
                };
			

				return result;
			}
			catch ( Exception ex )
			{
				isValid = false;
				return null;
			}
		}

		public static string GetLabelWithoutLanguage( ApiTerm term )
		{
			if ( term == null )
				return "";
			string label = "";
			char[] charsToTrim = { '*', ' ', '\'', '"', '}', '\r', '\n' };

			if ( term.skos_prefLabel != null )
			{
				label = term.skos_prefLabel.ToString();
				//for now work around the language: en-US: "Certificate Credit"

				label = label.Contains( ':' ) ? label.Split( ':' )[ 1 ] : label;
				label = label.Trim( charsToTrim );
				return label;
			}
			if ( term.id == null )
				return "";

			label = term.id.Contains( ':' ) ? term.id.Split( ':' )[ 1 ] : term.id;
			return "";
		}

		public static string GetVocabularyConceptScheme( string vocabulary )
		{
			var concept = "";
			bool isValid = false;
			//vocabulary = Char.ToLowerInvariant( vocabulary[ 0 ] ) + vocabulary.Substring( 1 );

			string key = "vocabulary_" + vocabulary;
			//check cache for vocabulary
			if ( HttpRuntime.Cache[ key ] != null )
			{
				concept = ( string ) HttpRuntime.Cache[ key ];
				return concept;
			}
			else
			{
				var targetTerm = GetVocabularyFromTermJson( vocabulary, ref isValid );
				if ( isValid )
				{
					concept = targetTerm.Contains( ':' ) ? targetTerm.Split( ':' )[ 1 ] : targetTerm;
					HttpRuntime.Cache.Insert( key, concept );
					return targetTerm;
				}
				else
				{
					if ( vocabulary.ToLower() == "audienceleveltype" )
					{
						concept = "AudienceLevel";
						HttpRuntime.Cache.Insert( key, concept );
						return concept;
					}
				}
			}
			return "";
		} //

		public static string GetVocabularyFromTermJson( string term, ref bool isValid )
		{
			string rawJson = "";
			
			try
			{
				isValid = false;
				var ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegTermsApi", "http://credreg.net/ctdl/terms/" );

				var target = term.Contains( ':' ) ? term.Split( ':' )[ 1 ] : term;
				rawJson = new HttpClient().GetAsync( ctdlUrl + target + "/json" ).Result.Content.ReadAsStringAsync().Result;
				var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiTermResult>( rawJson );
				var data = deserialized.graph.First();
				//seems like another change
				if ( data.targetScheme != null &&  data.targetScheme.Count > 0 ) 
				{
					isValid = true;
					//should only be one
					return data.targetScheme[0];
				}
				else
				{
					return "";
				}
			}
			catch ( Exception ex )
			{
				return "";
			}
		}

		public class ApiTermResult
		{
			public ApiTermResult()
			{
				graph = new List<ApiTerm>();
			}
			[JsonProperty( "@graph" )]
			public List<ApiTerm> graph { get; set; }
		}
		public class ApiTerm
		{
			public ApiTerm()
			{
				//targetConceptScheme = new JsonLDUri();
				//skos_scopeNote = new List<string>();
				rdfs_rangeIncludes = new List<string>();
				targetScheme = new List<string>();
			}
			[JsonProperty( "@id" )]
			public string id { get; set; } //The @id field contains the term with prefix
			[JsonProperty( "@type" )]
			public string type { get; set; }
			//[JsonProperty( "rdfs:label" )]
			//public object rdfs_label { get; set; } //Name

			[JsonProperty( "skos:prefLabel" )]
			public object skos_prefLabel { get; set; } //Name (vocab term)

			[JsonProperty( "rdfs:rangeIncludes" )]
			public List<string> rdfs_rangeIncludes { get; set; } //Name


			[JsonProperty( "meta:targetScheme" )]
			public List<string> targetScheme { get; set; } //Used by terms that point to vocabularies

			//[JsonProperty( "rdfs:comment" )]
			//public string rdfs_comment { get; set; } //Definition

			//[JsonProperty( "vann:usageNote" )]
			//public string skos_scopeNote { get; set; } //Usage Note

			//[JsonProperty( "dcterms:description" )]
			//public string dcterms_description { get; set; } //Comment
		
			//[JsonProperty( "skos:definition" )]
			public string skos_definition { get; set; } //Definition (vocab term)


			//[JsonProperty( "meta:targetConceptScheme" )]
			//public JsonLDUri targetConceptScheme { get; set; } //Used by terms that point to vocabularies
		}
		public class LanguageString
		{
			public LanguageString()
			{
				language = "";
				value = "";
			}
			[JsonProperty( "@language" )]
			public string language { get; set; }
			[JsonProperty( "@value" )]
			public string value { get; set; }
		}
		public class JsonLDUri
		{
			[JsonProperty( "@id" )]
			public string id { get; set; }
		}

		public class ToolTipData
		{
			public string Term { get; set; }
			public string Name { get; set; }
			public string Definition { get; set; }
			public string UsageNote { get; set; }
			public string Comment { get; set; }
		}

		#endregion
	}

	public class CodeItem
	{
		public CodeItem()
		{
		}

		//the code PK is either Id or Code - the caller will know the context
		public int Id { get; set; }
		/// <summary>
		/// Code is a convenience property to handle where a code item has a character key, or where need to use a non-integer display - rare
		/// </summary>
		public string Code { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
		public string SchemaName { get; set; }
		public string ParentSchemaName { get; set; }
        /// <summary>
        /// will contain conceptScheme without prefix. ParentSchemaName may be the same, but this is ready for use as framework URI
        /// </summary>
        public string ConceptSchemaPlain { get; set; }

    }


}
