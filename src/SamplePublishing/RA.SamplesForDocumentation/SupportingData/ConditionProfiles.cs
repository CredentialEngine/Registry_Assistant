using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RA.Models.Input;

namespace RA.SamplesForDocumentation
{
	public class ConditionProfiles
	{
		/// <summary>
		/// Sample data for most properties of a condition profile
		/// Required:
		/// - Description
		/// 
		/// Recommended
		/// - Name: Name or title of the resource.
		/// - Submission Of: Artifact to be submitted such as a transcript, portfolio, or an affidavit.
		/// - Condition: Single constraint, prerequisite, entry condition, requirement, or cost.
		/// - Subject Webpage: Webpage that describes this entity.
		/// - Target Entity
		///			In the context of requirements, this points to the entity or entities that are required 
		///			for the credential, assessment, or learning opportunity that is using this Condition Profile. 
		///			In other contexts, these properties connect their target to the entity using this Condition Profile.
		///		<see href="https://credreg.net/ctdl/terms/ConditionProfile">ConditionProfile</see>
		/// </summary>
		/// <returns></returns>
		public ConditionProfile PopulateRequires()
		{

			var output = new ConditionProfile()
			{
				Description = "To earn this credential the following conditions must be met, and the target learning opportunity must be completed.",
				SubjectWebpage = "https://example.org/mypage",
				AudienceType = new List<string>() { "Citizen", "audience:NonResident" },
				AudienceLevelType = new List<string>() { "BeginnerLevel", "audLevel:IntermediateLevel" },
				Condition = new List<string>() { "Complete High School", "Have a drivers licence." },
				Experience = "A little life experience is preferred.",
				MinimumAge =18,
				YearsOfExperience=3,
				Weight =.95M,

			};
			//name is optional
			output.Name = "A useful name for this condition profile";
			//AssertedBy is a list of OrganizationReference. If the organization exists in the registry, only the CTID needs to be provided.
			output.AssertedBy = new List<OrganizationReference>()
			{				
				new OrganizationReference()
				{
					Type= "Organization",
					Name="Asserting Organization",
					SubjectWebpage="https://example.org/assertingOrg"
				}, 
				new OrganizationReference()
                {
					CTID="ce-231a1022-43c7-41ed-9b0d-9a4c6b59ffc3"
				}
			};
			//CreditValue. see: https://credreg.net/ctdl/terms/creditValue
			//CreditValue is of type ValueProfile (https://credreg.net/ctdl/terms/ValueProfile)
			output.CreditValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					//CreditUnitType- The type of credit associated with the credit awarded or required.
					//			ConceptScheme: ceterms:CreditUnit (https://credreg.net/ctdl/terms/CreditUnit#CreditUnit)
					//			Concepts: provide with the namespace (creditUnit:SemesterHour) or just the text (SemesterHour). examples
					//			creditUnit:ClockHour, creditUnit:ContactHour, creditUnit:DegreeCredit
					CreditUnitType = new List<string>() {"SemesterHour"},
					Value=10
				}
			};
			output.CreditUnitTypeDescription = "Perhaps a clarification on rules for credit value.";
			//Submission Of - Artifact to be submitted such as a transcript, portfolio, or an affidavit.
			output.SubmissionOf = new List<string>() { "https://example.com/howtoapply", "https://example.com/usefulinfo" };
			//Name, label, or description of an artifact to be submitted such as a transcript, portfolio, or an affidavit.
			output.SubmissionOfDescription = "As a companion to SubmissionOf, or where is no online resource, provide useful information.";

			//costs
			output.EstimatedCost.Add( CostProfiles.PopulateCostProfile());

			// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization
			output.CommonCosts = new List<string>()
			{
				"ce-c9ced959-2924-481a-b8dd-7590b37dd07f"
			};

			//jurisdictions
			output.Jurisdiction.Add( Jurisdictions.SampleJurisdiction() );
			output.ResidentOf.Add( Jurisdictions.SampleJurisdiction() );


			//target entities.
			//Often a credential must be preceded by a learning opportunity and/or an assessment. Use an EntityFramework to provide the resource, either using just a CTID for a resource in the Credential Registry, or minimally the type, name, and subject webpage for other resources. 
			output.TargetLearningOpportunity = new List<EntityReference>()
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
					Name="Another required learning opportunity (external)",
					Description="A required learning opportunity that has not been published to Credential Registry. The type, name, and subject webpage are required. The description while useful is optional. ",
					SubjectWebpage="https://example.org?t=anotherLopp",
						CodedNotation="Learning 101"
				},
				new EntityReference()
				{
					//New: LearningOpportunities now have two subclasses: Course and LearningProgram
					//The type, name, and subject webpage are required. The description while useful is optional.
					Type="Course",
					Name="Name for a course that is required for this entity",
					Description="A description for a course that has not been published to Credential Registry. The type, name, and subject webpage are required. The description while useful is optional. ",
					SubjectWebpage="https://example.org?t=aCousre",
						CodedNotation="Course 200"
				}
			};
			//same idea for assessments and credentials
			output.TargetAssessment = new List<EntityReference>();
			output.TargetCredential = new List<EntityReference>();
			//target competencies are Credential Alignment Objects
			output.TargetCompetency = new List<CredentialAlignmentObject>()
			{
				//just the CTID for a competency in the registry
				new CredentialAlignmentObject()
				{
					CTID="ce-07a645c7-1f5b-4a23-9733-13d9dcf4290e"
				},
				//if not in the registry, include at least the TargetNodeName
				new CredentialAlignmentObject()
				{
					TargetNode="https://example.org/optionalCompetencyURL",
					TargetNodeName="Outcome item",
					TargetNodeDescription="Description of this outcome item",
					FrameworkName="Optional name of this framework",
					Framework="https://example.org/optionalCompetencyFrameworkURL"
				}
			};

			//Alternative Condition
			//Constraints, prerequisites, entry conditions, or requirements in a context where more than one alternative condition or path has been defined and from which any one path fulfills the parent condition.

			//when use the description would probably provide some direction such as
			//			"...one of the programs referenced in the AlternativeCondition must be completed."
			output.AlternativeCondition = GetAlternativeConditions();
			return output;
		}

		private List<ConditionProfile> GetAlternativeConditions()
        {
			var alternativeConditions = new List<ConditionProfile>()
			{
				new ConditionProfile()
				{
					Name="campus 1",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-bdd69580-1dba-4aad-a2fe-608ae14e19a0"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 2",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-34d92f0d-1a09-4bab-8144-41147df5f531"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 3",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-35fdd620-4fa6-4919-9ab8-33ded8f0f0b8"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 4",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-6a8dc43a-5347-4160-a4c6-804f13f02113"
						}
					}
				},
				new ConditionProfile()
				{
					Name="campus 5",
					Description = "To earn this credential the following program is one that would satisfy this requirement.",
					TargetLearningOpportunity = new List<EntityReference>()
					{
						new EntityReference()
						{
							CTID="ce-38614e2e-dc03-454f-834a-971107268aa7"
						}
					}
				}
			};
			return alternativeConditions;
		}
	}
}
