using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using RA.Models.Input;
using YourCredential = RA.SamplesForDocumentation.SampleModels.Credential;
using APIRequestCredential = RA.Models.Input.Credential;
using APIRequest = RA.Models.Input.CredentialRequest;

namespace RA.SamplesForDocumentation
{
    public class CredentialWithMultipleLanguages
    {
		public string PublishDetailedRecord( string requestType = "publish" )
		{
			//Holds the result of the publish action
			var result = "";

			// Assign the api key - acquired from organization account of the organization doing the publishing
			var apiKey = SampleServices.GetMyApiKey();
			if ( string.IsNullOrWhiteSpace( apiKey ) )
			{
				//ensure you have added your apiKey to the app.config
			}

			// This is the CTID of the organization that owns the data being published
			var organizationIdentifierFromAccountsSite = SampleServices.GetMyOrganizationCTID();
			if ( string.IsNullOrWhiteSpace( organizationIdentifierFromAccountsSite ) )
			{
				//ensure you have added your organization account CTID to the app.config
			}//


			//Assign a CTID for the entity being published and keep track of it
			var myCTID = "ce-cef8a76d-f168-0513-8fac-0b8dc60ef853"; //"ce-" + Guid.NewGuid().ToString();
																	//set up language helper
			var languages = new List<string>() { "ar-SA", "en" };

			//A simple credential object - see below for sample class definition
			var myData = new Credential()
			{
				CTID = myCTID,
				SubjectWebpage = "https://www.bachelorstudies.com/Bachelor-of-Science-in-Accounting/Saudi-Arabia/Dar-Al-Hekma-University/",
				CredentialType = "ceterms:BachelorDegree",
				CredentialStatusType = "Active",
				InLanguage = languages,
			};
			//typically the ownedBy is the same as the CTID for the data owner
			myData.OwnedBy.Add( new OrganizationReference()
			{
				CTID = organizationIdentifierFromAccountsSite
			} );
			//add name languages
			myData.Name_Map = new LanguageMap();
			myData.Name_Map.Add( "ar-SA", "بكالوريوس العلوم في المحاسبة" );
			myData.Name_Map.Add( "en", "Bachelor of Science in Accounting" );
			//add desc languages
			myData.Description_Map = new LanguageMap();
			myData.Description_Map.Add( "ar-SA", "المحاسبة من أكثر الدرجات تنوعًا في مجال الأعمال. يقدم برنامج المحاسبة في دار الحكمة برنامجًا أكاديميًا شاملاً وصارمًا في الدراسات المحاسبية. سيحصل الطلاب على فهم متعمق للجوانب التقنية والنظرية للمحاسبة ، بما في ذلك التدقيق والضرائب والإدارة المالية وعلوم الإدارة والاقتصاد والتسويق. سيقوم البرنامج بإعداد الطلاب لمجموعة متنوعة من فرص العمل في العديد من قطاعات الاقتصاد. أيضًا ، يمكن أن يساعد الطلاب على تلبية متطلبات التعيين المهني ، بما في ذلك المحاسب المحترف المعتمد (CPA) ، والمحاسب الإداري المعتمد (CMA) والمحلل المالي المعتمد (CFA). \n\n يتزايد الطلب على المحاسبين في المملكة العربية السعودية بسبب تطبيق المعايير الدولية لإعداد التقارير المالية (IFRS) للمؤسسات الصغيرة و" );
			myData.Description_Map.Add( "en", "The Bachelor of Science in Accounting is considered one of the most versatile degrees in business. The Accounting program at Dar Al-Hekma offers a thorough and rigorous academic program in accounting studies. Students will get an in-depth understanding of the technical and theoretical aspects of accounting, including auditing, taxation, financial management, management science, economics, and marketing. The program will prepare students for a variety of job opportunities in numerous sectors of the economy. Also, it can help students meet professional designation requirements, including the Chartered Professional Accountant (CPA), the Certified Management Accountant (CMA) and the Chartered Financial Analyst (CFA). \n\nAccountants are in high demand in Saudi Arabia, due to the implementation of the International Financial Reporting Standards (IFRS) for small and medium enterprises (SMEs)." );

			//add keywords
			myData.Keyword_Map = new LanguageMapList( new List<string>() { "Accounting", "Taxation", "financial management" } );
			myData.Keyword_Map.Add( "ar-SA", new List<string>() { "محاسبة", "تحصيل الضرائب", "ادارة مالية" } );
            //==================== QUALITY ASSURANCE RECEIVED ====================

            //CTID for accreditation
   //         myData.AccreditedBy.Add( new OrganizationReference()
			//{
			//	CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			//} );
			//Add organization that is NOT in the credential registry
			myData.AccreditedBy.Add( new OrganizationReference()
			{
				Type = "QACredentialOrganization",
				Name_Map = FormatLanguages( languages, new List<string>() { "بعض هيئات الاعتماد" , "Some Accreditation Body" } ),
				SubjectWebpage = "https://www.cswe.org/"
			} );
			//TBD on addresses
			myData.AvailableAt = new List<Place>()
			{
				new Place()
				{
					Address1="One University Plaza",
					City="Springfield",
					PostalCode="62703",
					AddressRegion="IL",
					Country="United States"
				}
			};
			//==================== CONDITION PROFILE ====================
			// add a requires Condition profile with conditions and a required learning opportunity. 
			// See the code sample for a ConditionProfile for more detailed information
			//	https://github.com/CredentialEngine/Registry_Assistant/blob/master/src/SamplePublishing/RA.SamplesForDocumentation/SupportingData/ConditionProfiles.cs
			/*Scenario: 
				- The learning opportunity will be published to the credential registry
				- The credential must be published before the learning opportunity
				- The learning opportunity is referenced using the Condition Profile property of TargetLearningOpportunity
				- Only the CTID need be provided for a learning opportunity that will be published

			*/
			myData.Requires.Add( AddConditionProfile1( languages ) );
			myData.Requires.Add( AddConditionProfile2( languages ) );

			//duration for a range from 8 to 12 weeks
			myData.EstimatedDuration = new List<DurationProfile>()
			{
				new DurationProfile()
				{
					MinimumDuration = new DurationItem()
					{
						Weeks=8
					},
					MaximumDuration = new DurationItem()
					{
						Weeks=12
					}, 
					Description_Map=FormatLanguages(languages, new List<string>() {"المدة المقدرة من 8 إلى 12 أسبوعًا.", "Estimated duration of 8 to 12 weeks."})
				}
			};
			//====================	COSTS	====================
			//Must be a valid CTDL cost type.
			// Example: Tuition, Application, AggregateCost, RoomOrResidency
			//see: https://credreg.net/ctdl/terms#CostType
			myData.EstimatedCost.Add( new CostProfile()
			{
				Description_Map = FormatLanguages(languages, new List<string>() { "الوصف المطلوب لملف تعريف التكلفة", "A required description of the cost profile" }),
				CostDetails = "https://example.com/t=loppCostProfile",
				Currency = "USD", //does this need to be a language map?
				CostItems = new List<CostProfileItem>()
				 {
					 new CostProfileItem()
					 {
						 DirectCostType="Application",
						 Price=100,
					 },
					 new CostProfileItem()
					 {
						 DirectCostType="Tuition",
						 Price=12999,
						 PaymentPattern_Map= FormatLanguages(languages, new List<string>() {"كامل المبلغ المستحق وقت التسجيل","Full amount due at time of registration"})
					 }
				 }
			} );

			//====================	OCCUPATIONS ====================
			//PopulateOccupations( myData );
			//====================	INDUSTRIES	====================
			//PopulateIndustries( myData );
			//====================	PROGRAMS	====================
			//PopulatePrograms( myData );

			//====================	CONNECTIONS ====================
			//Connections between credentials can be published using properties such as
			//- isPreparationFor, PreparationFrom, isAdvancedStandingFor, AdvancedStandingFrom, IsRequiredFor, and IsRecommendedFor. 
			//example of a connection to a credential for which the current credential will prepare a student.
			//var isPreparationFor = new Connections
			//{
			//	Description = "This certification will prepare a student for the target credential",
			//	TargetCredential = new List<EntityReference>()
			//	{
			//		//the referenced credential could be for an external credential, not known to be in the credential registry
			//		new EntityReference()
			//		{
			//			Type="MasterDegree",
			//			Name="Cybersecurity Technology Master's Degree  ",
			//			Description="A helpful description",
			//			SubjectWebpage="https://example.org?t=masters"
			//		}
			//	}
			//};
			//myData.IsPreparationFor.Add( isPreparationFor );

			//add credential that prepares for this credential. 
			//var preparationFrom = new Connections
			//{
			//	Description = "This credential will prepare a student for this credential",
			//	TargetCredential = new List<EntityReference>()
			//	{
			//		//the referenced credential is known to be in the credential registry, so only the CTID need be provided
			//		new EntityReference()
			//		{
			//			CTID="ce-40c3e860-5034-4375-80e8-f7455ff86a48"
			//		}
			//	}
			//};
			//myData.PreparationFrom.Add( preparationFrom );

			//====================	CREDENTIAL REQUEST ====================
			//This holds the credential and the identifier (CTID) for the owning organization
			var myRequest = new APIRequest()
			{
				Credential = myData,
				DefaultLanguage = "en-us",
				PublishForOrganizationIdentifier = organizationIdentifierFromAccountsSite
			};

			//Serialize the credential request object
			//Preferably, use method that will exclude null/empty properties
			string payload = JsonConvert.SerializeObject( myRequest, SampleServices.GetJsonSettings() );

			//call the Assistant API

			SampleServices.AssistantRequestHelper req = new SampleServices.AssistantRequestHelper()
			{
				EndpointType = "credential",
				RequestType = requestType,
				OrganizationApiKey = apiKey,
				CTID = myRequest.Credential.CTID.ToLower(),   //added here for logging
				Identifier = "testing",     //useful for logging, might use the ctid
				InputPayload = payload
			};

			bool isValid = new SampleServices().PublishRequest( req );
			//Return the result
			return req.FormattedPayload;
		}

		public ConditionProfile AddConditionProfile1( List<string> languageCodes )
		{
			var output = new ConditionProfile()
			{

				Description_Map = FormatLanguages( languageCodes, new List<string>() { "للحصول على هذا الاعتماد ، يجب استيفاء الشروط التالية ، ويجب إكمال فرصة التعلم المستهدفة.", "To earn this credential the following conditions must be met, and the target learning opportunity must be completed." } ),
				TargetLearningOpportunity = new List<EntityReference>()
					{
						//if the target learning opportunity exists in the registry, then only the CTID has to be provided in the EntityReference
						new EntityReference()
						{
							CTID="ce-ccd00a32-d5ad-41e7-b14c-5c096bc9eea0"
						},
						new EntityReference()
						{
							//Learning opportunities not in the registry may still be published as 'blank nodes'
							//The type, name, and subject webpage are required. The description while useful is optional.
							Type="LearningOpportunity",
							Name_Map= FormatLanguages(languageCodes,new List<string>(){ "فرصة التعلم المطلوبة.", "A required learning opportunity." }),
							Description_Map= FormatLanguages(languageCodes,new List<string>(){ "الوصف بينما مفيد هو اختياري.", "The description while useful is optional. " }),
							SubjectWebpage="https://example.org?t=anotherLopp",
							 CodedNotation="Learning 101" //string, single value, no language map
						}
					}
			};
			output.Condition_Map = new LanguageMapList();
			output.Condition_Map.Add( "ar-SA", new List<string>() { "إكمال المدرسة الثانوية", "لديك رخصة قيادة" } );
			output.Condition_Map.Add( "en", new List<string>() { "Complete High School", "Have a drivers licence" } );

			return output;
		}

		public ConditionProfile AddConditionProfile2( List<string> languageCodes )
		{
			var output = new ConditionProfile()
			{
				Description_Map = FormatLanguages( languageCodes, new List<string>() { "للحصول على هذا الاعتماد يجب أن تتحقق الشروط التالية.", "To earn this credential the following conditions must be met" } ),
				//credit Value
				CreditValue = new List<ValueProfile>()
					{
						new ValueProfile()
						{
							//CreditUnitType- The type of credit associated with the credit awarded or required.
							// - ConceptScheme: ceterms:CreditUnit (https://credreg.net/ctdl/terms/CreditUnit#CreditUnit)
							// - Concepts: provide with the namespace (creditUnit:SemesterHour) or just the text (SemesterHour). examples
							// - creditUnit:ClockHour, creditUnit:ContactHour, creditUnit:DegreeCredit
							CreditUnitType = new List<string>() {"SemesterHour"}, //fixed vocabulary
							Value=10,
							Description_Map=FormatLanguages( languageCodes, new List<string>() { "وحدة الائتمان في ساعات الفصل الدراسي.", "Credit unit is in semester hours." })
						}
					}
			};

			return output;
		}
		public LanguageMap FormatLanguages(List<string> languageCodes, List<string> text)
        {
			var output = new LanguageMap();
			if (languageCodes?.Count == 0 || languageCodes?.Count != text?.Count)
            {
				//bad request
				return output;
            }
			int cntr = 0;
			foreach (var item in text)
            {
				output.Add( languageCodes[cntr], item );
				cntr++;
            }

			return output;
        }
	}
}
