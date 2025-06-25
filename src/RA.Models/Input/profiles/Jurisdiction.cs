using System.Collections.Generic;

namespace RA.Models.Input
{
	/// <summary>
	/// Jurisdiction Profile
	/// Geo-political information about applicable geographic areas and their exceptions.
	/// A Jurisdiction Profile must have at least one of the following:
	///		A global jurisdiction of true</a>,
	///		or just a description,
	///		or a <a href="https://credreg.net/ctdl/terms/mainJurisdiction" target="credreg">main Jurisdiction</a> with possible exceptions (<a href="https://credreg.net/ctdl/terms/jurisdictionException" target="credreg">jurisdiction exception</a>).
	/// <see cref="https://credreg.net/ctdl/terms/JurisdictionProfile"/>
	/// </summary>
	public class JurisdictionProfile
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public JurisdictionProfile()
		{
			JurisdictionException = new List<Place>();
		}

		/// <summary>
		/// List of Organizations that asserts this condition
		/// Required
		/// </summary>
		public List<OrganizationReference> AssertedBy { get; set; } = new List<OrganizationReference>();

		/// <summary>
		/// Whether or not the resource is useful, applicable or recognized everywhere.
		/// </summary>
		public bool? GlobalJurisdiction { get; set; }

		/// <summary>
		/// Statement, or characterization for the Jurisdiction.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// TBD - does it make sense to offer providing the full GeoCoordinates.
		/// Will be useful where the request can be populated programatically.
		///  <see cref="https://credreg.net/ctdl/terms/mainJurisdiction"/>
		/// </summary>
		public Place MainJurisdiction { get; set; } = new Place();

		/// <summary>
		/// Geographic or political region in which the credential is not formally recognized or an organization has no authority to act.
		/// <see cref="https://credreg.net/ctdl/terms/jurisdictionException"/>
		/// </summary>
		public List<Place> JurisdictionException { get; set; }
	}

	/// <summary>
	/// Geographic Coordinates
	/// Geographic coordinates of a place or event including latitude and longitude as well as other locational information.
	/// Not currently used.
	/// </summary>
	public class GeoCoordinates
	{
		public GeoCoordinates()
		{
			Name = string.Empty;
		}

		public string Name { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		/// <summary>
		/// ceterms:geoURI
		/// Entity that describes the longitude, latitude and other location details of a place.
		/// </summary>
		public string GeoURI { get; set; }
	}
}
