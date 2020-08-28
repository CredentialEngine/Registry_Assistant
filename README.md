# Registry Assistant
Web service API for formatting and publishing to the Credential Registry.
Detailed documentation of the background of the API can be found on the credReg.net site at:
http://credreg.net/registry/assistant

## Updates
### August 28, 2020
- FinancialAssistance
  - Added Financial Assistance Type
  - Added Financial Assistance Value
- RA.SamplesForDocumentation
  - Reorganized the project to have separate folders for each publishing type
  - Added the folder: SupportingData that contains samples for common classes including:
    - FinancialAssistanceProfile
    - Occupations
    - Industries
    - Instructional Programs (i.e. CIP)
## Quick Start
### Import
- Read the material at: http://credreg.net/registry/assistant
- Download the input classes
  https://github.com/CredentialEngine/Registry_Assistant/tree/master/src/RA.Models/Input
- Review the sample code for publishing
  https://github.com/CredentialEngine/Registry_Assistant/tree/master/src/SamplePublishing/RA.SamplesForDocumentation
-The project: RA.SamplePublishingProject was added to easily test the sample code. This project is under construction. Additional test methods will be added in the coming weeks. 
  https://github.com/CredentialEngine/Registry_Assistant/tree/master/src/SamplePublishing/RA.SamplePublishingProject
  - The latter project includes an AppConfig file for storing your ApiKey, organization CTIDs, etc. 
  - For initial testing, call the 'format' endpoint of the API
