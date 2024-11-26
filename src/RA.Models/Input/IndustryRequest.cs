using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
    /// <summary>
    /// Request class for publishing an industry
    /// </summary>
    public class IndustryRequest : BaseRequest
    {
        /// <summary>
        /// Broad category of economic activities encompassing various sectors.
        /// </summary>
        public Industry Industry { get; set; } 


    }
}
