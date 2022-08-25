using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models.Input
{
	public class RubricRequest : BaseRequest
	{
		public RubricRequest()
		{
		}
		public Rubric Rubric { get; set; } = new Rubric();

		public List<RubricCriterion> RubricCriterion { get; set; } = new List<RubricCriterion>();
		public List<CriterionCategory> CriterionCategory { get; set; } = new List<CriterionCategory>();
		public List<CriterionLevel> CriterionLevel { get; set; } = new List<CriterionLevel>();
	}

	/// <summary>
	/// A rubric is typically an evaluation tool or set of guidelines used to promote the consistent application of learning expectations, learning objectives, or learning standards in the classroom, or to measure their attainment against a consistent set of criteria. In instructional settings, rubrics clearly define academic expectations for students and help to ensure consistency in the evaluation of academic work from student to student, assignment to assignment, or course to course. Rubrics are also used as scoring instruments to determine grades or the degree to which learning standards have been demonstrated or attained by students.
	/// <see cref="http://standards.asn.desire2learn.com/rubric.html"/>
	/// </summary>
	public class Rubric
	{
		public Rubric()
		{
		}
		/// <summary>
		/// CTID
		/// Required
		/// </summary>
		public string CTID { get; set; }

		/// <summary>
		/// An account of the resource.
		/// </summary>
		//[JsonProperty( "dcterms:description" )]
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		//????these are URIs - could imply RubricCriterion is to be a top level class
		/// <summary>
		/// RubricCriterian referenced defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
		/// List of CTIDs/URIs to a RubricCriterion
		/// </summary>
		//[JsonProperty( "asn:hasCriterion" )]
		public List<string> HasCriterion { get; set; } = new List<string>();

		/// <summary>
		/// Has Criterion Category
		/// Resource referenced by the Rubric that defines categories for clustering logical sets of RubricCriterion.
		/// </summary>
		//[JsonProperty( "asn:hasCriterionCategory" )]
		public List<string> HasCriterionCategory { get; set; } = new List<string>();

		/// <summary>
		/// Reference to a progression model used.
		/// TBD - string, or class, or ???
		/// </summary>
		//[JsonProperty( "asn:hasProgressionModel" )]  //??? asn example has no namespace
		public string HasProgressionModel { get; set; }

		/// <summary>
		/// Description of what the rubric's creator intended to assess or evaluate.
		/// </summary>
		//[JsonProperty( "asn:hasScope" )]
		public string HasScope { get; set; }
		public LanguageMap HasScope_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// An unambiguous reference to the resource within a given context.
		/// Recommended practice is to identify the resource by means of a string conforming to an identification system. Examples include International Standard Book Number (ISBN), Digital Object Identifier (DOI), and Uniform Resource Name (URN). Persistent identifiers should be provided as HTTP URIs.
		/// </summary>
		//[JsonProperty( "dcterms:identifier" )]
		public string Identifier { get; set; }

		//[JsonProperty( "dcterms:Language" )]
		public List<string> Language { get; set; } = new List<string>();

		/// <summary>
		/// Original resource on which this resource is based or derived from.
		/// </summary>
		//[JsonProperty( "dcterms:source" )]  //??? 
		public string Source { get; set; }  //URI

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		//[JsonProperty( "dcterms:title" )]
		public string Title { get; set; }
		public LanguageMap Title_Map { get; set; } = new LanguageMap();

	}

	public class RubricCriterion
	{
		public RubricCriterion()
		{
		}

		#region Base properties

		//[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// An account of the resource.
		/// </summary>
		//[JsonProperty( "dcterms:description" )]
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		//[JsonProperty( "dcterms:title" )]
		public string Title { get; set; }
		public LanguageMap Title_Map { get; set; } = new LanguageMap();
		#endregion

		#region relationship properties

		/// <summary>
		/// Reference to the Rubric to which the RubricCriteria being described belongs.
		/// /// List of Rubric URIs
		/// </summary>
		//[JsonProperty( "asn:criterionFor" )]
		public List<string> CriterionFor { get; set; } = new List<string>();

		/// <summary>
		/// Resource description of a level of performance based on a RubricCriterion.
		/// List of CriterionLevel
		/// </summary>
		//[JsonProperty( "asn:hasLevel" )]
		public List<string> HasLevel { get; set; } = new List<string>();
		#endregion`


	}

	public class CriterionCategory
	{
		public CriterionCategory()
		{
		}

		//[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// An account of the resource.
		/// </summary>
		//[JsonProperty( "dcterms:description" )]
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		//[JsonProperty( "dcterms:Language" )]
		public List<string> Language { get; set; } = new List<string>();

		/// <summary>
		/// A name given to the resource.
		/// </summary>
		//[JsonProperty( "dcterms:title" )]
		public string Title { get; set; }
		public LanguageMap Title_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Resource referenced is a Rubric to which this CriterionCategory belongs.
		/// List of Rubric URIs
		/// </summary>
		//[JsonProperty( "asn:criterionCategoryOf" )]
		public List<string> CriterionCategoryOf { get; set; } = new List<string>();

		/// <summary>
		/// RubricCriterian referenced defines a principle or standard to be met that demonstrates quality in performance of a task or obtaining an objective.
		/// List of RubricCriterian
		/// </summary>
		//[JsonProperty( "asn:hasCriterion" )]
		public List<string> HasCriterion { get; set; } = new List<string>();

	}

	public class CriterionLevel
	{
		public CriterionLevel()
		{
		}

		#region base properties

		//[JsonProperty( "ceterms:ctid" )]
		public string CTID { get; set; }

		/// <summary>
		/// Description of a level of achievement in performance of a task defined by the RubricCriterion.
		/// </summary>
		//[JsonProperty( "asn:benchmark" )]
		public string Benchmark { get; set; }
		public LanguageMap Benchmark_Map { get; set; } = new LanguageMap();


		//[JsonProperty( "dcterms:description" )]
		public string Description { get; set; }
		public LanguageMap Description_Map { get; set; } = new LanguageMap();

		//[JsonProperty( "dcterms:Language" )]
		public List<string> Language { get; set; } = new List<string>();

		/// <summary>
		/// Qualitative description of the degree of achievement used for a column header in a tabular rubric.
		/// </summary>
		//[JsonProperty( "asn:qualityLabel" )]
		public string QualityLabel { get; set; }
		public LanguageMap QualityLabel_Map { get; set; } = new LanguageMap();

		/// <summary>
		/// Points to be awarded for achieving this level for a RubricCriterion.
		/// </summary>
		//[JsonProperty( "asn:score" )]
		public decimal Score { get; set; }

		/// <summary>
		/// Numeric value representing the resource's position in a list (array) of resources.
		/// </summary>
		//[JsonProperty( "asn:sequence" )]
		public int Sequence { get; set; }

		#endregion

		#region relationship properties

		/// <summary>
		/// Reference to the RubricCriterion to which the CriterionLevel being described belongs.
		/// </summary>
		//[JsonProperty( "asn:levelFor" )]
		public List<string> LevelFor { get; set; } = new List<string>();

		/// <summary>
		/// Reference to a progression model used.
		/// </summary>
		//[JsonProperty( "asn:hasProgressionLevel" )]
		public List<string> HasProgressionLevel { get; set; } = new List<string>();

		#endregion
	}
}

