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


}
