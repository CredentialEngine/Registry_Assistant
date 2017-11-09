using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
using Utilities;

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
			//	return false;

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
					//Name =  data.skos_prefLabel != null ? data.skos_prefLabel.ToString() : "",
					//Description = ( data.skos_definition ?? "" ),
					ParentSchemaName = parentSchema
					//UsageNote = ( data.skos_scopeNote.FirstOrDefault() ?? "" ),
					//Comment = string.IsNullOrWhiteSpace( data.dcterms_description ) ? "" : data.dcterms_description
				};
				//var result2 = new ToolTipData()
				//{
				//	Term = data.id,
				//	Name = data.skos_prefLabel,
				//	Definition = ( data.skos_definition ?? "" ),
				//	UsageNote = ( data.skos_scopeNote.FirstOrDefault() ?? "" ),
				//	Comment = string.IsNullOrWhiteSpace( data.dcterms_description ) ? "" : data.dcterms_description
				//};

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

			//			if ( CtdlHelper.CodesManager.IsPropertySchemaValid( categoryCode, property, ref code ) == false )
			//	return false;

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

				var targetVocab = ctdlProperty.Contains( ':' ) ? ctdlProperty.Split( ':' )[ 1 ] : ctdlProperty;

				targetVocab = GetVocabularyConceptScheme( ctdlProperty );
				if ( string.IsNullOrWhiteSpace( targetVocab ) )
				{
					//what to do ??
					isValid = false;
					return null;
				}
				string concept = targetVocab.Contains( ':' ) ? targetVocab.Split( ':' )[ 1 ] : targetVocab;

				var targetTerm = term.Contains( ':' ) ? term.Split( ':' )[ 1 ] : term;
				var rawJson = new HttpClient().GetAsync( ctdlUrl + concept + "/" + targetTerm + "/json" ).Result.Content.ReadAsStringAsync().Result;
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
					ParentSchemaName = targetVocab
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

	}


}
