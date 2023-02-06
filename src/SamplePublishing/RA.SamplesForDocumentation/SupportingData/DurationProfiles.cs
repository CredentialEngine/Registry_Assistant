using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using RA.Models.Input;
namespace RA.SamplesForDocumentation
{
	public class DurationProfiles
	{
        /// <summary>
        /// Sample Duration profiles
        /// </summary>
        /// <returns></returns>
        public List<DurationProfile> SampleDurationProfiles()
        {
            //duration for a range from 8 to 12 weeks
            var estimatedDurations = new List<DurationProfile>()
            {
                new DurationProfile()
                {
                    MinimumDuration = new DurationItem()
                    {
                        Weeks=8
                    },
                    MaximumDuration = new DurationItem()
                    {
                        Weeks=12
                    }
                }
            };

            //duration for a program that is exactly 9 months
            estimatedDurations.Add( new DurationProfile()
            {
                ExactDuration = new DurationItem()
                {
                    Months = 9
                }

            } );
            //duration profile with just a description
            estimatedDurations.Add( new DurationProfile()
            {
                Description = "This course typically takes 6 weeks full time or 8 to 12 weeks part-time."
            } );

            //duration use the ISO8601 coded format
            estimatedDurations.Add( new DurationProfile()
            {
                ExactDuration = new DurationItem()
                {
                    Duration_ISO8601 = "PT10H"  //10 hours
                }
            } );
            return estimatedDurations;
        }
	}
}
