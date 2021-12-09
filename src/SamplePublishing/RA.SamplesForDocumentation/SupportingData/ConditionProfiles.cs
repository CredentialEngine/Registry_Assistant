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
		/// </summary>
		/// <returns></returns>
		public ConditionProfile PopulateRequires()
		{
			/*
			 * 
			 * 
			 * Submission Of
					Artifact to be submitted such as a transcript, portfolio, or an affidavit.
			 * 
			 */
			var output = new ConditionProfile()
			{
				Description = "To earn this credential the following conditions must be met, and the target learning opportunity must be completed.",
				SubjectWebpage = "https://example.org/mypage",
				Condition = new List<string>() { "Complete High School", "Have a drivers licence." }
			};

			output.SubmissionOf = new List<string>() { "https://example.com/howtoapply", "https://example.com/usefulinfo" };
			//Name, label, or description of an artifact to be submitted such as a transcript, portfolio, or an affidavit.
			output.SubmissionOfDescription = "As a companion to SubmissionOf, or where is no online resource, provide useful information.";

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

			//credit Value
			output.CreditValue = new List<ValueProfile>()
			{
				new ValueProfile()
				{
					CreditUnitType = new List<string>() {"SemesterHour"},
					Value=10
				}
			};


			return output;
		}
	}
}
