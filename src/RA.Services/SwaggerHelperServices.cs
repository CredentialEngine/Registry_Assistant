using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RA.Models.Input;
using RAI = RA.Models.Input;
using RA.Models.Json;
using Models;
using MC = Models.Common;
using InputEntity = RA.Models.Input.Credential;
using OutputEntity = RA.Models.Json.Credential;

using MP = Models.ProfileModels;

namespace RA.Services
{
	public class SwaggerHelperServices
	{
		#region Credential
		public static object SwaggerForCredential( int id )
		{
			try
			{
				var commonCredential = CTIServices.CredentialServices.GetCredentialDetail( id, new AppUser { Id = 10 } );

				var input = new InputEntity();
				MapToCredential( commonCredential, input );

				List<string> messages = new List<string>();
				var output = new OutputEntity();
				if ( CredentialServices.ToMap( input, output, ref messages ) )
				{
					var value = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
					return JObject.Parse( value );
				}

				return new Models.RegistryAssistantResponse { Successful = false, Messages = messages, Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() ) };
			}
			catch ( Exception ex )
			{
				return new Models.RegistryAssistantResponse { Successful = false, Payload = JsonConvert.SerializeObject( ex, ServiceHelper.GetJsonSettings() ) };
			}
		}

		private static void MapToCredential( MC.Credential from, RA.Models.Input.Credential to )
		{
			to.Name = from.Name;
			to.Description = from.Description;
			to.AlternateName = from.AlternateName;

			to.CodedNotation = from.CodedNotation;
			to.CredentialId = from.CredentialId;
			to.CredentialType = from.CredentialType.Items[ 0 ].SchemaName;
			to.Ctid = from.ctid;
			to.DateEffective = from.DateEffective;

			to.Image = from.ImageUrl;
			to.InLanguage = from.InLanguage;
			//new method to map these
			to.Subject = MapHelper.MapToString( from.Subject );
			to.Keyword = MapHelper.MapToString( from.Keyword );

			to.DegreeConcentration = MapHelper.MapToString( from.DegreeConcentration );
			to.DegreeMajor = MapHelper.MapToString( from.DegreeMajor );
			to.DegreeMinor = MapHelper.MapToString( from.DegreeMinor );

			to.SubjectWebpage = from.SubjectWebpage;

			to.AvailableOnlineAt = from.AvailableOnlineAt;
			to.AvailabilityListing = from.AvailabilityListing;
			to.LatestVersion = from.LatestVersion;
			to.PreviousVersion = from.PreviousVersion;
			to.VersionIdentifier = from.VersionIdentifier;

			to.AudienceLevel = MapHelper.MapEnumermationToString( from.AudienceLevelType );
			to.ProcessStandards = from.ProcessStandards;
			to.ProcessStandards = from.ProcessStandardsDescription;

			to.Industries = MapHelper.MapEnumermationToFrameworkItem( from.Industry );
			//handle others
			if ( from.OtherIndustries != null && from.OtherIndustries.Count > 0 )
			{
				to.Industries.AddRange( MapHelper.MapTextValueProfileToFrameworkItem( from.OtherIndustries ) );
			}
			to.Occupations = MapHelper.MapEnumermationToFrameworkItem( from.Occupation );
			//handle others
			if ( from.OtherOccupations != null && from.OtherOccupations.Count > 0 )
			{
				to.Occupations.AddRange( MapHelper.MapTextValueProfileToFrameworkItem( from.OtherOccupations ) );
			}

			to.OwningOrganization = MapHelper.MapToOrgRef( from.OwningOrganization );
			to.EstimatedCost = MapHelper.MapToEstimatedCosts( from.EstimatedCosts );
			to.EstimatedDuration = MapHelper.MapToEstimatedDuration( from.EstimatedDuration );
		}

		#endregion

		#region Organization
		public static object SwaggerForOrganization( int id )
		{
			try
			{
				var commonOrganization = CTIServices.OrganizationServices.GetOrganizationDetail( id, new AppUser { Id = 10 } );
				var organization = MapToOrg( commonOrganization );
				organization.Type = "CredentialOrganization";

				List<string> messages = new List<string>();
				var agent = new Agent();
				if ( AgentServices.ToMap( organization, agent, ref messages ) )
				{
					var value = JsonConvert.SerializeObject( agent, ServiceHelper.GetJsonSettings() );
					return JObject.Parse( value );
				}

				return new Models.RegistryAssistantResponse { Successful = false, Messages = messages, Payload = JsonConvert.SerializeObject( agent, ServiceHelper.GetJsonSettings() ) };
			}
			catch ( Exception ex )
			{
				return new Models.RegistryAssistantResponse { Successful = false, Payload = JsonConvert.SerializeObject( ex, ServiceHelper.GetJsonSettings() ) };
			}
		}

		private static RA.Models.Input.Organization MapToOrg( MC.Organization org )
		{
			RA.Models.Input.Organization or = new RA.Models.Input.Organization();
			or.Name = org.Name;
			or.Description = org.Description;
			or.Image = org.ImageUrl;
			or.SubjectWebpage = org.SubjectWebpage;

			or.Keyword = MapHelper.MapToString( org.Keyword );
			or.Email = MapHelper.MapToString( org.Emails );
			or.SocialMedia = MapHelper.MapToString( org.SocialMediaPages );

			or.AgentType = MapHelper.MapEnumermationToString( org.OrganizationType );
			or.AgentSectorType = MapHelper.MapEnumermationToString( org.AgentSectorType );
			or.ServiceType = MapHelper.MapEnumermationToFrameworkItem( org.ServiceType );

			or.FoundingDate = org.FoundingDate;
			or.AvailabilityListing.Add( org.AvailabilityListing );
			or.Ctid = org.CTID;
			or.AlternativeIdentifier = org.AlternativeIdentifier;
			or.MissionAndGoalsStatement = org.MissionAndGoalsStatement;
			or.MissionAndGoalsStatementDescription = org.MissionAndGoalsStatementDescription;
			or.AgentPurpose = org.AgentPurpose;
			or.AgentPurposeDescription = org.AgentPurposeDescription;

			or.Duns = org.ID_DUNS;
			or.Fein = org.ID_FEIN;
			or.IpedsId = org.ID_IPEDSID;
			or.OpeId = org.ID_OPEID;

			//or.Jurisdiction = org.Jurisdiction;
			foreach ( var address in org.Addresses )
			{
				or.Address.Add( new RA.Models.Input.Address
				{
					Name = address.Name,
					Address1 = address.Address1,
					Address2 = address.Address2,
					City = address.City,
					Country = address.Country,
					AddressRegion = address.AddressRegion,
					PostalCode = address.PostalCode
				} );
			}

			foreach ( var cp in org.Auto_TargetContactPoint )
			{
				or.ContactPoint.Add( new RA.Models.Input.ContactPoint
				{
					Name = cp.Name,
					ContactType = cp.ContactType,
					Emails = MapHelper.MapToString( cp.Emails ),
					ContactOption = MapHelper.MapToString( cp.Auto_ContactOption ),
					SocialMediaPages = MapHelper.MapToString( cp.SocialMediaPages )
				} );
			}
			return or;
		}
		#endregion

		#region Assessment

		#endregion


		#region Learning Opportunity

		#endregion
	}
}
