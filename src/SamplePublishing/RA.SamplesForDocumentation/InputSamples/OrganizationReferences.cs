using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RA.Models.Input;
using OrgRequest = RA.Models.Input.OrganizationRequest;

namespace RA.SamplesForDocumentation.InputSamples
{
	public class OrganizationReferences
	{
		OrgRequest request = new OrganizationRequest();
		Organization myOrg = new Organization();
		public void Assign()
		{
			//if you know the CTID, then only specify CTID
			myOrg.AccreditedBy.Add( new OrganizationReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );
			//if the CTID is not known, or if not sure a QA organization is in the registry, use a refer
			myOrg.Department.Add( new OrganizationReference()
			{
				Name = "A Quality Assurance Organization",
				SubjectWebpage = "http://example.com/qualityAssuranceIsUs",
				Type = OrganizationReference.QACredentialOrganization
			} );
		}
	}

	public class EntityReferences
	{
		Organization myQAOrgRequest = new Organization();
		public void QAOrgAssign()
		{
			//if you know the CTID, then only specify CTID for a credential that this QA org accredits

			myQAOrgRequest.Accredits.Add( new EntityReference()
			{
				CTID = "ce-541da30c-15dd-4ead-881b-729796024b8f"
			} );

			//if the CTID is not known, or if not sure a credential is in the registry, use a reference to an entity
			myQAOrgRequest.Approves.Add( new EntityReference()
			{
				Type = "ceterms:Certification",
				Name = "A certification that is approved by our ORG",
				Description = "A helpful but optional description of this certification",
				SubjectWebpage = "http://example.com/certification"
				
			} );
		}
	}
}
