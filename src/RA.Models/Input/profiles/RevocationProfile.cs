using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Revocation Profile
	/// The conditions and methods by which a credential can be removed from a holder.
	/// </summary>
	public class RevocationProfile
	{
		public RevocationProfile()
		{
		}
		/// <summary>
		/// Profile Description 
		/// REQUIRED and must be a minimum of 15 characters.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		
		/// <summary>
		/// Effective date of the content of this profile
		/// ceterms:dateEffective
		/// </summary>
		public string DateEffective { get; set; }
		 
		/// <summary>
		/// Jurisdiction Profile
		/// Geo-political information about applicable geographic areas and their exceptions.
		/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
		/// </summary>
		public List<JurisdictionProfile> Jurisdiction { get; set; } = new List<JurisdictionProfile>();

		/// <summary>
		/// Webpage or online document that provides information about the removal criteria for an awarded credential.
		/// URI
		/// </summary>
		public string RevocationCriteria { get; set; }

		/// <summary>
		/// Textual description providing information about the removal criteria for an awarded credential.
		/// Optional
		/// Minimum of 15 characters when present, but should be clear.
		/// </summary>
		public string RevocationCriteriaDescription { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap RevocationCriteriaDescription_Map { get; set; } = new LanguageMap();

	}
}
