using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    public class VerifyRequest
    {
        /// <summary>
        /// Identifier for Organization which Owns the data being verified
        /// </summary>
        public string PublishForOrganizationIdentifier { get; set; }

        /// <summary>
        /// The CTID of the resource to be verified.
        /// </summary>
        public string CTID { get; set; }

        /// <summary>
        /// The community/private registry where the resource to be verified is located.
        /// Optional. If not provided, the default registry will be used. 
        /// </summary>
        public string Community { get; set; }
    }
}
