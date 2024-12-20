﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{

	public class PathwaySetRequest : BaseRequest
	{
		public PathwaySetRequest()
		{
		}

		/// <summary>
		/// Pathway Set - a roadmap of pathways
		/// </summary>
		public PathwaySet PathwaySet { get; set; } = new PathwaySet();

		/// <summary>
		/// List of pathways that are part of the pathway set.
		/// The PathwaySet.HasPathway is a list of CTIDs that will reference pathways in this list.
		/// </summary>
		public List<PathwayRequest> Pathways { get; set; } = new List<PathwayRequest>();

	}//


	public class PathwaySet
	{
		public PathwaySet() { }

		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// Pathway Set Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Pathway Set Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// The webpage that describes this entity.
		/// URL
		/// </summary>
		public string SubjectWebpage { get; set; }

		/// <summary>
		/// List of Pathways - CTIDs
		/// This could refer to pathways in the request or previously published pathways.
		/// Could be used as an option to just provide ctids/uris
		/// </summary>
		public List<string> HasPathway { get; set; } = new List<string>();

		/// <summary>
		/// Organization(s) that owns this resource
		/// </summary>
		public List<OrganizationReference> OwnedBy { get; set; } = new List<OrganizationReference>();
		//OR
		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();

        #region -- Process Profiles --

        /// <summary>
        /// Description of a process by which a resource was created.
        /// </summary>
        public List<ProcessProfile> DevelopmentProcess { get; set; }

        /// <summary>
        ///  Description of a process by which a resource is maintained, including review and updating.
        /// </summary>
        public List<ProcessProfile> MaintenanceProcess { get; set; }

        /// <summary>
        /// Description of a process by which a resource is reviewed.
        /// </summary>
        public List<ProcessProfile> ReviewProcess { get; set; }

        #endregion
    } 


}
