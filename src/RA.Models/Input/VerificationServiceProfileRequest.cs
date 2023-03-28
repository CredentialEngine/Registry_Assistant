using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    public class VerificationServiceProfileRequest : BaseRequest
    {
        /// <summary>
        /// constructor
        /// </summary>
        public VerificationServiceProfileRequest()
        {
            VerificationServiceProfile = new VerificationServiceProfile();
        }
        /// <summary>
        /// VerificationServiceProfile Input Class
        /// </summary>
        public VerificationServiceProfile VerificationServiceProfile { get; set; }
    }

}
