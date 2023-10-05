using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MJ = RA.Models.JsonV2;

namespace RA.Models.Input
{
    public class CredentialingActionRequest : BaseRequest
    {
        /// <summary>
        /// constuctor
        /// </summary>
        public CredentialingActionRequest()
        {
        }

        public CredentialingAction CredentialingAction { get; set; } = new CredentialingAction();
    }

	//public class WorkforceDemandAction : CredentialingAction
	//{
	//	public WorkforceDemandAction() { }

	//	/// <summary>
	//	/// Only for Workforce Demand Action, so likely handled differently, maybe inherit this class, and add CTID
	//	/// </summary>
	//	public string CTID { get; set; }
	//}

}
