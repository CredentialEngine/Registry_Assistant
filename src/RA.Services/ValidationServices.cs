using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Utilities;
using RA.Models.Input;
//may want an alternative to this
//using CtdlHelper = Factories;


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
			"ceterms:CertificateOfCompletion",
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
		static List<string> credentialGroupTypes = new List<string>()
		{
			"ceterms:Diploma",
			"ceterms:Degree",
			"ceterms:Doctorate"
		};
		/*
		 * Badge, Certificate, Diploma, Degree, Doctorate
		 * 
		 */
		static List<string> otherClassTypes = new List<string>()
		{
			"ceterms:CredentialOrganization",
			"ceterms:QACredentialOrganization",
			"ceterms:AssessmentProfile",
			"ceterms:LearningOpportunityProfile",
			"ceterms:ConditionManifest",
			"ceterms:CostManifest",
			"ceterms:Job",
			"ceterms:Occupation",
			"ceterms:Pathway",
			"ceterms:PathwaySet",
			"ceterms:Rubric",
			"ceterms:Task",
			"ceterms:TransferValueProfile",
			"ceterms:WorkRole",
		};
		#endregion
		static List<string> statusTypes = new List<string>()
		{
			"lifecycle:Developing",
			"lifecycle:Active",
			"lifecycle:Suspended",
			"lifecycle:Ceased",
		};
		#region validation with code tables
		/// <summary>
		/// Check if the property exists
		/// the proper case adjusted property name will be returned
		/// </summary>
		/// <param name="ctdlProperty"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static bool IsCredentialTypeValid( string ctdlProperty, ref string property )
		{
			//var credentialTypes = SchemaServices.GetConceptSchemeFromPropertyRange( "http://credreg.net/ctdl/schema/encoding/json", "ceterms:credentialType" );

			//if ( CtdlHelper.CodesManager.IsPropertySchemaValid( categoryCode, ref property ) == false )
			//    return false;

			//CodeItem ci = GetConceptSchemeTermJson(ctdlProperty, property, ref isValid );
			try
			{
				var targetVocab = ctdlProperty.Contains( ':' ) ? ctdlProperty.Split( ':' )[ 1 ] : ctdlProperty;
				targetVocab = GetConceptSchemeForProperty( ctdlProperty );
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
				string parentSchema = GetConceptSchemeForProperty( ctdlProperty );
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

		/// <summary>
		/// 
		/// 20-06-04 Upon request formatting the property as a URI like https://purl.org/ctdl/terms/CertificateOfCompletion
		/// </summary>
		/// <param name="credentialType"></param>
		/// <param name="validSchema"></param>
		/// <returns></returns>
		public static bool IsValidCredentialType( string credentialType, ref string validSchema, bool formattingAsUri = false )
		{
			bool isValid = false;
			if ( string.IsNullOrWhiteSpace( credentialType ) )
				return false;
			//prefix if necessary
			if ( credentialType.IndexOf( "ceterms" ) == -1 )
				credentialType = "ceterms:" + credentialType.Trim();
			if ( credentialType.IndexOf("'") > -1 || credentialType.IndexOf(" ") > -1)
				credentialType = credentialType.Replace( " ", "" ).Replace( "'", "" );
			//some helper corrections
			credentialType = credentialType.Replace( "AssociatesDegree", "AssociateDegree" );
			credentialType = credentialType.Replace( "BachelorsDegree", "BachelorDegree" );
			credentialType = credentialType.Replace( "MastersDegree", "MasterDegree" );
			validSchema = "";
			string exists = credentialTypes.FirstOrDefault( s => s.ToLower() == credentialType.ToLower() );
			//or maybe loop thru and check case independent
			foreach ( string s in credentialTypes )
			{
				if ( s == credentialType ||
					s.ToLower() == credentialType.ToLower() )
				{
					if ( formattingAsUri )
					{
						validSchema = "https://purl.org/ctdl/terms/" + s.Replace( "ceterms:", "");
					}
					else
						validSchema = s;
					return true;
					//break;
				}
			}
			if (!isValid)
			{
				var groupTypes = UtilityManager.GetAppKeyValue( "includeGeneralCredentialTypes" );
				if (groupTypes.IndexOf(credentialType) > -1)
				{
					validSchema = credentialType;
					isValid = true;
				}
			}
			return isValid;
		}

		/// <summary>
		/// Probably temporary
		/// </summary>
		/// <param name="statusType"></param>
		/// <param name="validSchema"></param>
		/// <returns></returns>
		public static bool IsValidLifecycleType(string statusType, ref string validSchema)
		{
			bool isValid = false;
			//prefix if necessary
			//??changing? lifecycleStatusType
			//if ( statusType.IndexOf( "lifecycle" ) == -1 )
			//	statusType = "lifecycle:" + statusType;
			if ( statusType.IndexOf( "lifecycle" ) == -1 )
				statusType = "lifecycle:" + statusType;

			validSchema = "";
			string exists = statusTypes.FirstOrDefault( s => s.ToLower() == statusType.ToLower() );
			//or maybe loop thru and check case independent
			foreach ( string s in statusTypes )
			{
				if ( s == statusType ||
					s.ToLower() == statusType.ToLower() )
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
		/// <param name="ctdlProperty"></param>
		/// <param name="property"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static bool IsTermValid( string ctdlProperty, string term, ref CodeItem code, string schemaType= "ctdl" )
		{
			bool isValid = true;

			code = GetConceptSchemeTermJson( ctdlProperty, term, ref isValid, schemaType );

			return isValid;
		}

		//public static bool IsAgentServiceCodeValid( string categoryCode, string property, ref CodeItem code )
		//{
		//	bool isValid = true;

		//	if ( CtdlHelper.OrganizationServiceManager.IsPropertySchemaValid( categoryCode, property, ref code ) == false )
		//		return false;


		//	return isValid;
		//}

		public static bool IsSchemaNameValid( string classSchema, ref string validSchema, string prefix="ceterms" )
		{
			bool isValid = false;
			validSchema = "";
			if ( string.IsNullOrWhiteSpace( classSchema ))
			{
				validSchema = "";
				return false;
			}
				
			//prefix if necessary
			if ( classSchema.IndexOf( prefix ) == -1 )
				classSchema = prefix + ":" + classSchema;
			
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
		public static CodeItem GetConceptSchemeTermJson( string ctdlProperty, string term, ref bool isValid, string schemaType="ctdl" )
		{
			isValid = true;
			try
			{
				//may want to be configurable to use pending
				var ctdlConceptVocabUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegVocabsApi", "http://credreg.net/ctdl/vocabs/");
				if ( schemaType == "qdata")
					ctdlConceptVocabUrl = Utilities.UtilityManager.GetAppKeyValue( "qdataVocabsApi", "http://credreg.net/qdata/vocabs/" );

				//var targetVocab = ctdlProperty.Contains( ':' ) ? ctdlProperty.Split( ':' )[ 1 ] : ctdlProperty;
				//actually full conceptScheme (property was in cache returns without ceterms
				var targetVocab = GetConceptSchemeForProperty( ctdlProperty, schemaType );
				if ( string.IsNullOrWhiteSpace( targetVocab ) )
				{
					//what to do ??
					isValid = false;
					return null;
				}
                //plain conceptScheme
				string conceptSchemePlain = targetVocab.Contains( ':' ) ? targetVocab.Split( ':' )[ 1 ] : targetVocab;

				var targetTerm = term.Contains( ':' ) ? term.Split( ':' )[ 1 ] : term;
				//???
				var nextTarget = conceptSchemePlain;
				if( conceptSchemePlain == "FinancialAssistance" )
					nextTarget = "financialAid";
				var rawJson = new HttpClient().GetAsync( ctdlConceptVocabUrl + nextTarget + "/" + targetTerm + "/json" ).Result.Content.ReadAsStringAsync().Result;
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

		/// <summary>
		/// Get the concept scheme for a property name
		/// </summary>
		/// <param name="ctdlProperty"></param>
		/// <returns></returns>
		public static string GetConceptSchemeForProperty( string ctdlProperty, string schemaType = "ctdl" )
		{
			var concept = "";
			bool isValid = false;
			//ctdlProperty = Char.ToLowerInvariant( ctdlProperty[ 0 ] ) + ctdlProperty.Substring( 1 );

			string key = "ctdlProperty_" + ctdlProperty;
			//check cache for ctdlProperty
			if ( HttpRuntime.Cache[ key ] != null )
			{
				concept = ( string ) HttpRuntime.Cache[ key ];
				return concept;
			}
			else
			{
				var targetTerm = GetConceptSchemeFromTermJson( ctdlProperty, ref isValid, schemaType );
				if ( isValid )
				{
					concept = targetTerm.Contains( ':' ) ? targetTerm.Split( ':' )[ 1 ] : targetTerm;
					HttpRuntime.Cache.Insert( key, concept );
					return targetTerm;
				}
				else
				{
					if ( ctdlProperty.ToLower() == "audienceleveltype" )
					{
						concept = "AudienceLevel";
						HttpRuntime.Cache.Insert( key, concept );
						return concept;
					}
				}
			}
			return "";
		} //

		/// <summary>
		/// Get concept scheme for a property
		/// </summary>
		/// <param name="term"></param>
		/// <param name="isValid"></param>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static string GetConceptSchemeFromTermJson( string ctdlProperty, ref bool isValid, string schemaType = "ctdl" )
		{
			string rawJson = "";
			
			try
			{
				isValid = false;
				var ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "credRegTermsApi", "http://credreg.net/ctdl/terms/" );
				if ( schemaType == "qdata" )
				{
					ctdlUrl = Utilities.UtilityManager.GetAppKeyValue( "qdataTermsApi", "http://credreg.net/qdata/terms/" );
				}
				var target = ctdlProperty.Contains( ':' ) ? ctdlProperty.Split( ':' )[ 1 ] : ctdlProperty;
				rawJson = new HttpClient().GetAsync( ctdlUrl + target + "/json" ).Result.Content.ReadAsStringAsync().Result;
				var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiTermResult>( rawJson );
				var data = deserialized.graph.First();
				//seems like another change
				//20-02-05 - targetScheme is not returned for FinancialAid, or Audience or qdata
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
			/// <summary>
			/// label is for concept scheme
			/// </summary>
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

		/// <summary>
		/// Resolve a list of Onet codes as FrameworkItems
		/// </summary>
		/// <param name="codes"></param>
		/// <param name="messages"></param>
		/// <param name="warnings">For now, only warn where O*Net code not found</param>
		/// <returns></returns>
		public static List<FrameworkItem> ResolveOnetCodes(List<string> codes, ref List<string> messages, ref List<string> warnings )
		{
			if (codes == null || codes.Count() == 0)
				return null;
			//O*NET-SOC Taxonomy
			string frameworkName = "Standard Occupational Classification";
			string framework = "https://www.onetcenter.org/taxonomy.html";//" https://www.bls.gov/soc/";
			//string template = "http://www.onetonline.org/link/summary/";
			bool doingBulk = true;
			var output = new List<FrameworkItem>();
			//might be better to do one at a time to be able to mark any not found!
			//intial
			try
			{
				//string result = CtdlHelper.CodesManager.SOC_SearchAsObject( codes, ref warnings );
				//var list = JsonConvert.DeserializeObject<List<CodeItem>>( result );
				//var fi = new FrameworkItem();

				//foreach ( var item in list )
				//{
				//	if ( item == null )
				//		continue;
				//	//not sure
				//	fi = new FrameworkItem()
				//	{
				//		Framework = framework,
				//		FrameworkName = frameworkName,
				//		Name = item.Name,
				//		Description = item.Description,
				//		CodedNotation = item.Code,
				//		TargetNode = item.URL
				//	};
				//	output.Add( fi );
				//}
			} catch (Exception ex)
			{
				//if exception is encountered, add warning and allow publish to continue
				warnings.Add( "Exception occured resolving O*Net codes. Ignored during beta period. All O*Net codes may not have been published." );
				LoggingHelper.LogError( ex, "ValidationServices.ResolveOnetCodes", true );
			}
			return output;
		}

		/// <summary>
		/// Resolve a list of NAICS codes as FrameworkItems
		/// </summary>
		/// <param name="codes"></param>
		/// <param name="messages"></param>
		/// <param name="warnings">For now, only warn where NAICS code not found</param>
		/// <returns></returns>
		public static List<FrameworkItem> ResolveNAICSCodes(List<string> codes, ref List<string> messages, ref List<string> warnings)
		{
			if ( codes == null || codes.Count() == 0 )
				return null;
			string frameworkName = "North American Industry Classification System";
			string framework = "https://www.census.gov/eos/www/naics/index.html";
			string template = "https://www.census.gov/cgi-bin/sssd/naics/naicsrch?code={0}&search=2017";
			//for now not doing bulk search as allows for returning messages where not found
			bool doingBulkSearch = false;
			var output = new List<FrameworkItem>();

			try
			{
				//string result = CtdlHelper.CodesManager.NAICS_SearchAsObject( codes, ref warnings, doingBulkSearch );
				//var list = JsonConvert.DeserializeObject<List<CodeItem>>( result );
				//var fi = new FrameworkItem();

				//foreach ( var item in list )
				//{
				//	if ( item == null )
				//		continue;
				//	//not sure
				//	fi = new FrameworkItem()
				//	{
				//		Framework = framework,
				//		FrameworkName = frameworkName,
				//		Name = item.Name,
				//		Description = item.Description,
				//		CodedNotation = item.CodedNotation,
				//		TargetNode = item.URL
				//	};
				//	output.Add( fi );
				//}
			}
			catch ( Exception ex )
			{
				//if exception is encountered, add warning and allow publish to continue
				warnings.Add( "Exception occured resolving NAICS codes. Ignored during beta period. All NAICS codes may not have been published." );
				LoggingHelper.LogError( ex, "ValidationServices.ResolveNAICSCodes", true );
			}
			return output;
		}



		/// <summary>
		/// Resolve a list of CIP codes as FrameworkItems
		/// </summary>
		/// <param name="codes"></param>
		/// <param name="messages"></param>
		/// <param name="warnings">For now, only warn where O*Net code not found</param>
		/// <returns></returns>
		public static List<FrameworkItem> ResolveCipCodes(List<string> codes, ref List<string> messages, ref List<string> warnings)
		{
			if ( codes == null || codes.Count() == 0 )
				return null;
			string frameworkName = "Classification of Instructional Programs";
			string framework = "https://nces.ed.gov/ipeds/cipcode/Default.aspx?y=55";
			var output = new List<FrameworkItem>();
			//any codes not found will be marked as warnings
			try
			{
				//string result = CtdlHelper.CodesManager.CIP_SearchAsObject( codes, ref warnings );
				//var list = JsonConvert.DeserializeObject<List<CodeItem>>( result );
				//var fi = new FrameworkItem();

				//foreach ( var item in list )
				//{
				//	if ( item == null )
				//		continue;
				//	//not sure
				//	fi = new FrameworkItem()
				//	{
				//		Framework = framework,
				//		FrameworkName = frameworkName,
				//		Name = item.Name,
				//		Description = item.Description,
				//		CodedNotation = item.CodedNotation,
				//		TargetNode = item.URL
				//	};
				//	output.Add( fi );
				//}
			}
			catch ( Exception ex )
			{
				//if exception is encountered, add warning and allow publish to continue
				warnings.Add( "Exception occured resolving CIP codes. Ignored during beta period. All CIP codes may not have been published." );
				LoggingHelper.LogError( ex, "ValidationServices.ResolveCipCodes", true );
			}
			return output;
		}
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
		public string CodedNotation { get; set; }
		public string Name { get; set; }

		public string Description { get; set; }
		public string URL { get; set; }
		public string SchemaName { get; set; }
		public string ParentSchemaName { get; set; }
        /// <summary>
        /// will contain conceptScheme without prefix. ParentSchemaName may be the same, but this is ready for use as framework URI
        /// </summary>
        public string ConceptSchemaPlain { get; set; }

    }


}
