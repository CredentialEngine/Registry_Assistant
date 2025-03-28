﻿using RA.Models.Input.profiles.QData;

namespace RA.Models.Input
{
    public class DataSetProfileRequest : BaseRequest
    {
        /// <summary>
        /// constructor
        /// </summary>
        public DataSetProfileRequest()
        {
            DataSetProfile = new DataSetProfile();
        }
        /// <summary>
        /// DataSetProfile Input Class
        /// </summary>
        public DataSetProfile DataSetProfile { get; set; }

    }

}
