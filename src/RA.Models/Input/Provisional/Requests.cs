namespace RA.Models.Input.Provisional
{

	#region Credential
	public class CredentialRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public CredentialRequest()
		{
			Credential = new Credential();
		}

		/// <summary>
		/// Credential Input Class
		/// </summary>
		public Credential Credential { get; set; }
	}

	public class Credential : BaseProvisional
	{
		/// <summary>
		/// constructor
		/// </summary>
		public Credential()
		{
			Type = "Credential";
		}
		/// <summary>
		/// The credential type as defined in CTDL
		/// <see href="https://credreg.net/page/typeslist"/>
		/// Required
		/// NOTE: The following types are 'top level' types that may not be published:
		///		Credential, Degree
		/// Only the sub-types under the latter may be used in publishing
		/// </summary>
		public string CredentialType { get; set; }

	}
	#endregion

	#region Assessment

	public class AssessmentRequest : BaseRequest
	{
		/// <summary>
		/// constructor
		/// </summary>
		public AssessmentRequest()
		{
			Assessment = new BaseProvisional();
		}

		/// <summary>
		/// Assessment Input Class
		/// </summary>
		public BaseProvisional Assessment { get; set; }
	}
	#endregion

	#region LearningOpportunity
	public class LearningOpportunityRequest : BaseRequest
	{
		public LearningOpportunityRequest()
		{
			LearningOpportunity = new LearningOpportunity();
		}

		public LearningOpportunity LearningOpportunity { get; set; }
	}

	public class LearningProgramRequest : LearningOpportunityRequest
	{
		public LearningProgramRequest()
		{
			LearningOpportunity.Type = "LearningProgram";
		}
	}

	public class CourseRequest : LearningOpportunityRequest
	{
		public CourseRequest()
		{
			LearningOpportunity.Type = "Course";
		}
	}

	/// <summary>
	/// TBD on separate classes for lopp related
	/// </summary>
	public class LearningOpportunity : BaseProvisional
	{
		/// <summary>
		/// constructor
		/// </summary>
		public LearningOpportunity()
		{
			Type = "LearningOpportunity";
		}

		/// <summary>
		/// Set of alpha-numeric symbols that uniquely identifies an item and supports its discovery and use.
		/// Course only?
		/// ceterms:codedNotation
		/// </summary>
		public string CodedNotation { get; set; }

		/// <summary>
		/// An inventory or listing of resources that includes this resource.
		/// Course only?
		/// </summary>
		public string InCatalog { get; set; }
	}

	#endregion
}
