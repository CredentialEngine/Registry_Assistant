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
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();
		//public List<string> CredentialProfiled { get; set; }
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
		public List<Jurisdiction> Jurisdiction { get; set; } = new List<Jurisdiction>();
		public string RevocationCriteria { get; set; }
		public string RevocationCriteriaDescription { get; set; }
		public LanguageMap RevocationCriteriaDescription_Map { get; set; } = new LanguageMap();

	}
}
