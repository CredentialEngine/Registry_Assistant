using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	/// <summary>
	/// Offering of a Learning Opportunity or Assessment with associated schedule.
	/// Comment: Provides data specific to a given scheduled offering; the Learning Opportunity (including Course and Learning Program) or Assessment being offered provides all data that is shared between different offerings.
	/// Usage Note: Rather than repeating data, only use the properties in this class when and where their data differs from the data provided by the resource for which this is an offer.
	/// </summary>
	public class ScheduledOffering
	{
		public string CTID { get; set; }

		public string Name { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Name_Map { get; set; } = new LanguageMap();
		/// <summary>
		/// Description 
		/// Required
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Alternately can provide a language map
		/// </summary>
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// List of Alternate Names for this resource
		/// </summary>
		public List<string> AlternateName { get; set; } = new List<string>();
		/// <summary>
		/// LanguageMap for AlternateName
		/// </summary>
		public LanguageMapList AlternateName_Map { get; set; } = new LanguageMapList();


		/// <summary>
		///  Resource containing summary/statistical employment outcome, earnings, and/or holders information.
		///  For deeper information, include qdata:DataSetProfile.
		/// </summary>
		public List<AggregateDataProfile> AggregateData { get; set; } = new List<AggregateDataProfile>();

		/// <summary>
		/// Online location where the credential, assessment, or learning opportunity can be pursued.
		/// URL
		/// </summary>
		public List<string> AvailableOnlineAt { get; set; }

		/// <summary>
		/// Listing of online and/or physical locations where a credential can be pursued.
		/// URL
		/// </summary>
		public List<string> AvailabilityListing { get; set; }

		/// <summary>
		/// Physical location where the credential, assessment, or learning opportunity can be pursued.
		/// </summary>
		public List<Place> AvailableAt { get; set; } = new List<Place>();

		/// <summary>
		/// Type of means by which a learning opportunity or assessment is delivered to credential seekers and by which they interact; select from an existing enumeration of such types.
		/// deliveryType:BlendedDelivery deliveryType:InPerson deliveryType:OnlineOnly
		/// <see href="https://credreg.net/ctdl/terms/Delivery"/>
		/// </summary>
		public List<string> DeliveryType { get; set; } = new List<string>();

		/// <summary>
		/// Detailed description of the delivery type of an assessment or learning opportunity.
		/// </summary>
		public string DeliveryTypeDescription { get; set; }
		public LanguageMap DeliveryTypeDescription_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// List of CTIDs (recommended) or full URLs for a CostManifest published by the owning organization.
		/// Set of costs maintained at an organizational or sub-organizational level, which apply to this learning opportunity.
		/// </summary>
		public List<string> CommonCosts { get; set; } = new List<string>();

		/// <summary>
		/// Estimated cost of a credential, learning opportunity or assessment.
		/// </summary>
		public List<CostProfile> EstimatedCost { get; set; } = new List<CostProfile>();

		/// <summary>
		/// Estimated time it will take to complete a credential, learning opportunity or assessment.
		/// </summary>
		public List<DurationProfile> EstimatedDuration { get; set; } = new List<DurationProfile>();

        /// <summary>
        /// Reference to a relevant support service.
        /// List of CTIDs that reference one or more published support services
        /// </summary>
        public List<string> HasSupportService { get; set; }

        /// <summary>
        /// Type of frequency at which a resource is offered; select from an existing enumeration of such types.
        /// ConceptScheme: ceterms:ScheduleFrequency
        /// scheduleFrequency:Annually scheduleFrequency:BiMonthly scheduleFrequency:EventBased scheduleFrequency:Irregular scheduleFrequency:Monthly scheduleFrequency:MultiplePerWeek scheduleFrequency:OnDemand scheduleFrequency:OpenEntryExit scheduleFrequency:Quarterly scheduleFrequency:SelfPaced scheduleFrequency:SemiAnnually scheduleFrequency:SingleInstance scheduleFrequency:Weekly
        /// </summary>
        public List<string> OfferFrequencyType { get; set; } = new List<string>();

		/// <summary>
		/// Type of frequency with which events typically occur; select from an existing enumeration of such types.
		/// ConceptScheme: ceterms:ScheduleFrequency
		/// scheduleFrequency:Annually scheduleFrequency:BiMonthly scheduleFrequency:EventBased scheduleFrequency:Irregular scheduleFrequency:Monthly scheduleFrequency:MultiplePerWeek scheduleFrequency:OnDemand scheduleFrequency:OpenEntryExit scheduleFrequency:Quarterly scheduleFrequency:SelfPaced scheduleFrequency:SemiAnnually scheduleFrequency:SingleInstance scheduleFrequency:Weekly
		/// </summary>
		public List<string> ScheduleFrequencyType { get; set; } = new List<string>();

		/// <summary>
		/// Type of time at which events typically occur; select from an existing enumeration of such types.
		/// ConceptScheme: ceterms:ScheduleTiming
		/// scheduleTiming:Daytime scheduleTiming:Evening scheduleTiming:Weekdays scheduleTiming:Weekends
		/// </summary>
		public List<string> ScheduleTimingType { get; set; } = new List<string>();

		/// <summary>
		/// Organization(s) that offer this resource
		/// </summary>
		public List<OrganizationReference> OfferedBy { get; set; } = new List<Input.OrganizationReference>();

		public string SubjectWebpage { get; set; } //URL
	}
}
