using System;
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

		public List<PathwayRequest> Pathways { get; set; } = new List<PathwayRequest>();
		//TBD - consider option to just provide a list of ctids/uris
		//one or the other but not both??
		public List<string> PathwayReferences { get; set; } = new List<string>();

	}//


	public class PathwaySet
	{
		public PathwaySet() { }

		public string Ctid { get; set; }

		/// <summary>
		/// Pathway Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Pathway Description 
		/// Required
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
	} //


}
