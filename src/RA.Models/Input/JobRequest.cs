﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MJ = RA.Models.JsonV2;

namespace RA.Models.Input
{

    /// <summary>
    /// Request class for publishing a Job
    /// </summary>
    public class JobRequest : BaseRequest
    {
        /// <summary>
        /// Set of responsibilities based on work roles within an occupation as defined by an employer.
        /// </summary>
        public Job Job { get; set; } = new Job();


		/// <summary>
		/// Job already formatted as JSON-LD
		/// ONLY USED WITH PUBLISH LIST and request.HasLanguageMaps must be set to true.
		/// </summary>
		public MJ.Job FormattedJob { get; set; } = new MJ.Job();
    }

    /// <summary>
    /// Request class for publishing a list of Jobs
    /// </summary>
    public class JobListRequest : BaseRequest
    {
        /// <summary>
        /// List of Jobs
        /// /// Using data type of object to allow handling plain requests or those with language maps
        /// </summary>
        public List<object> JobList { get; set; } = new List<object>();
        /// <summary>
        /// HasLanguageMaps
        /// If false, will format input using the plain Job classes otherwise the JSON-LD class
        /// </summary>
        public bool HasLanguageMaps { get; set; }
    }
}
