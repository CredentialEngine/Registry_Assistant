using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;

namespace RA.SamplesForDocumentation.SupportingData
{
    public class ProcessProfiles
    {
        public static List<ProcessProfile> GetProcessProfileList()
        {
            var output = new List<ProcessProfile>();
            output.Add( GetProcessProfile() );

            return output;
        }

        public static ProcessProfile GetProcessProfile()
        {
            var output = new ProcessProfile()
            {
                Description = "Description of the development process for this transfer value profile",
                DateEffective = "1991-05-16",
                SubjectWebpage = "https://example.org?t=tvpswp",
                ExternalInputType = new List<string>() { "Business", "Education Administrators", "Associations" },
                ProcessFrequency = "New development occurs on an as needed basis."
            };

            return output;
        }

        public static ProcessProfile GetAdministrativeProcessProfile( string processingAgentCTID)
        {
            var output = new ProcessProfile()
            {
                Description = "This NOCTI assessment is delivered in an online format and consists of a written component which is used to measure certain aspects of occupational competence such as factual knowledge and theoretical knowledge. NOCTI assessments must be administered in a proctored environment and can be delivered in one, two, or three sessions. Paper-based administration is available upon request.",
                DateEffective = "2016-05-16",
                SubjectWebpage = "https://example.org?t=adminProcess",
                DataCollectionMethodType = new List<string> { "AdministrativeRecordMatching", "SupplementalMethod" },
                ExternalInputType = new List<string>() { "Business", "Business or Industry Association", "Education Administrators" },
                ProcessFrequency = "Administration frequency is determined by the testing site.",
                VerificationMethodDescription = "A key strength of all assessments concerns content validity. That validity is based upon the fact that each assessment is built upon national/industry standards and reflects the critical core competencies required in the occupation as reflected in associated job-task analyses performed during the development/revision process. ",
            };
            output.ProcessingAgent = new List<OrganizationReference>
            { 
                new OrganizationReference() { CTID = processingAgentCTID } 
            };
            output.Jurisdiction = Jurisdictions.SampleJurisdictions();

            output.ScoringMethodDescription = "Each multiple-choice item is worth one point. If an individual answers an item correctly, one point is given. If an individual answers an item incorrectly, no points are given. All items that are not answered are counted as incorrect. Scores are calculated by standard area and by total. Standard scores are determined by dividing the number of questions answered correctly by the number possible within the standard. Standard scores are considered weighted because each standard does not contain the same number of questions. Scores are represented by percentages. A total score is calculated by dividing the total number of questions answered correctly across all standard areas by the total number of points possible across standard areas.";
            output.ScoringMethodExampleDescription = "Textual example of the method or tool used to score the assessment.";
            output.ScoringMethodExample = "https://example.org/?t=ScoringMethodExample";

            output.ProcessMethod = "https://example.org/?t=ProcessMethod";
            output.ProcessStandards = "https://example.org/?t=ProcessMethod";
            output.ProcessStandardsDescription = "Textual description of the criteria, standards, and/or requirements used with a process";
            return output;
        }
        public static ProcessProfile GetDevelopementProcessProfile()
        {
            var output = new ProcessProfile()
            {
                Description = "NOCTI relies upon the NOCTI-authorized testing site to receive and evaluate appeals and to communicate such requests to NOCTI for guidance, assistance, and processing.",
                DateEffective = "1991-05-16",
                SubjectWebpage = "https://example.org?t=devProcess",
                ExternalInputType = new List<string>() { "Business", "Business or Industry Association", "Education Administrators" },
                ProcessFrequency = "New development occurs on an as needed basis."
            };

            return output;
        }
    }
}
