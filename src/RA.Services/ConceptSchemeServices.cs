using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RJ = RA.Models.JsonV2;
using RA.Models.Input;
using CASSEntityRequest = RA.Models.Input.CASSConceptSchemeRequest;
using EntityRequest = RA.Models.Input.ConceptSchemeRequest;
using InputEntity = RA.Models.Input.ConceptScheme;
using OutputEntity = RA.Models.JsonV2.ConceptScheme;
using OutputGraph = RA.Models.JsonV2.ConceptSchemeGraph;
using OutputGraphEntity = RA.Models.JsonV2.ConceptScheme;
using GraphContainer = RA.Models.JsonV2.GraphContainer;
using Newtonsoft.Json;
using CER = RA.Services.RegistryServices;
using Utilities;

namespace RA.Services
{
	public class ConceptSchemeServices : ServiceHelper
	{
		static string status = "";
		static List<string> warnings = new List<string>();
		List<OutputEntity> outputConcepts = new List<OutputEntity>();

		#region from a graph
		/// <summary>
		/// A request from CASS will come already formatted
		/// </summary>
		/// <param name="request"></param>
		/// <param name="isValid"></param>
		/// <param name="helper"></param>
		public void PublishGraph( CASSEntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			var output = new OutputGraph();
			OutputGraphEntity oc = new OutputGraphEntity();
			List<OutputGraphEntity> compList = new List<OutputGraphEntity>();

			ConceptSchemeGraph input = request.ConceptSchemeGraph;// 
			if ( ToMapFromGraph( input, output, ref messages ) )
			{
				if ( string.IsNullOrWhiteSpace( output.CTID) && output.CtdlId.IndexOf( "/ce-" ) > -1)
				{
					output.CTID = output.CtdlId.Substring( output.CtdlId.IndexOf( "/ce-" ) + 1 );
					request.CTID = output.CTID;
				}
				
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

				//will need to extract a ctid?
				CER cer = new CER( "ConceptScheme", output.Type, output.CTID, helper.SerializedInput );
				cer.PublisherAuthorizationToken = helper.ApiKey;
				cer.PublishingForOrgCtid = helper.OwnerCtid;
				cer.SkippingValidation = true;

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;

				string identifier = "ConceptScheme_" + output.CTID;
				if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
					//for now need to ensure envelopid is returned
					helper.RegistryEnvelopeId = crEnvelopeId;

					string msg = string.Format( "<p>Published Concept Scheme</p><p>CTID: {0}</p> <p>EnvelopeId: {1}</p> ", output.CTID, crEnvelopeId );
					NotifyOnPublish( "ConceptScheme", msg );
				}
				else
				{
					messages.Add( status );
					isValid = false;
				}
				//}
			}
			else
			{
				helper.HasErrors = true;
				isValid = false;
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}
			helper.SetMessages( messages );
			return;
		}


		/// <summary>
		/// Input from CASS should already be properly formatted.
		/// Ensure a conceptScheme exists
		/// </summary>
		/// <param name="input"></param>
		/// <param name="requestCTID"></param>
		/// <param name="output"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public bool ToMapFromGraph( ConceptSchemeGraph input, OutputGraph output, ref List<string> messages )
		{
			CurrentEntityType = "CASSConceptScheme";
			bool isValid = true;

			//TODO - if from CASS, just pass thru, with minimum validation
			output.Graph = input.Graph;
			int conceptsCount = 0;
			try
			{
				RJ.ConceptScheme conceptScheme = GetConceptScheme( input.Graph, ref conceptsCount );

				string ctid = "";
				if ( conceptScheme.CtdlId.IndexOf( "/ce-" ) > -1 )
				{
					ctid = conceptScheme.CtdlId.Substring( conceptScheme.CtdlId.IndexOf( "/ce-" ) + 1 );
				}
				if ( conceptScheme == null  )
				{
					messages.Add( "A ceasn:ConceptScheme document was not found." );
				}
				else
				{
					//if ( !string.IsNullOrWhiteSpace( requestCTID ) )
					//{
					//	output.CTID = requestCTID;
					//}
					//else
					//if ( !string.IsNullOrWhiteSpace( conceptScheme.CTID ) )
					//{
					//	//LoggingHelper.DoTrace( 4, string.Format( "CompetencyServices.PublishFromCASS. DIFFERENCES IN CTIDs. request.CTID: {0}, conceptScheme.CTID: {1}", ( requestCTID ?? "" ), conceptScheme.CTID ) );
					//	output.CTID = conceptScheme.CTID;
					//}
					//else
					//{
					//	messages.Add( "A CTID for the competency conceptScheme was not found in the request object or competency conceptScheme." );
					//}
					if ( conceptsCount == 0 )
						messages.Add( "No documents of type ceasn:Competency were found." );
				}
				output.CtdlId = credRegistryGraphUrl + ctid;
				output.CTID = ctid;
			}
			catch ( Exception ex )
			{
				LogError( ex, "CompetencyServices.ToMapFromCASS" );
				messages.Add( ex.Message );
			}
			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}

		private RJ.ConceptScheme GetConceptScheme( object graph, ref int conceptsCount )
		{
			//string ctid = "";
			conceptsCount = 0;
			if ( graph == null )
			{
				return null;
			}
			RJ.ConceptScheme entity = new RJ.ConceptScheme();
			Newtonsoft.Json.Linq.JArray jarray = ( Newtonsoft.Json.Linq.JArray )graph;
			foreach ( var token in jarray )
			{
				if ( token.GetType() == typeof( Newtonsoft.Json.Linq.JObject ) )
				{
					if ( token.ToString().IndexOf( "skos:ConceptScheme" ) > -1 || token.ToString().IndexOf( "ConceptScheme" ) > -1 )
					{
						entity = ( ( Newtonsoft.Json.Linq.JObject )token ).ToObject<RJ.ConceptScheme>();

						//RJ.ConceptScheme cf = ( RJ.ConceptScheme ) JsonConvert.DeserializeObject( token.ToString() );
						if ( conceptsCount == 0 && jarray.Count > 1 )
						{
							//18-09-25 the conceptScheme is now first in the export document
							conceptsCount = jarray.Count - 1;
						}
						return entity;
					}
					else if ( token.ToString().IndexOf( "skos:Concept" ) > -1 || token.ToString().IndexOf( "Concept" ) > -1 )
					{
						conceptsCount++;
						//ignore
						//var c1 = token.ToString().Replace( "exactMatch", "exactAlignment" );
						//var c2 = ( ( Newtonsoft.Json.Linq.JObject ) c1 ).ToObject<RJ.CompetencyInput>();

					}
				}

				else
				{
					//error
				}
			}
			//no ctid found, so????
			return entity;
		}

		#endregion

		#region direct publish
		public void Publish( EntityRequest request, ref bool isValid, RA.Models.RequestHelper helper )
		{
			isValid = true;
			string crEnvelopeId = request.RegistryEnvelopeId;

			//submitter is not a person for this api, rather the organization
			//may want to do a lookup via the api key?
			string submitter = "";
			List<string> messages = new List<string>();
			var output = new OutputEntity();
			GraphContainer og = new GraphContainer();

			/*
             * current approach:
             * - multiple publish
             * - framework
             * - one envelope for each competency
             * 
             */
			if ( ToMap( request, output, ref messages ) )
			{
				if ( warnings.Count > 0 )
					messages.AddRange( warnings );

				og.Graph.Add( output );
				//add competencies
				if ( outputConcepts != null && outputConcepts.Count > 0 )
				{
					foreach ( var item in outputConcepts )
					{
						og.Graph.Add( item );
					}
				}

				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );

				CER cer = new CER( "ConceptScheme", output.Type, output.CTID, helper.SerializedInput );
				cer.PublisherAuthorizationToken = helper.ApiKey;
				cer.PublishingForOrgCtid = helper.OwnerCtid;

				if ( cer.PublisherAuthorizationToken != null && cer.PublisherAuthorizationToken.Length >= 32 )
					cer.IsManagedRequest = true;

				string identifier = "ConceptScheme_" + request.ConceptScheme.CTID;

				if ( cer.Publish( helper.Payload, submitter, identifier, ref status, ref crEnvelopeId ) )
				{
					//for now need to ensure envelopid is returned
					helper.RegistryEnvelopeId = crEnvelopeId;

					//string msg = string.Format( "<p>Published ConceptScheme: {0}</p><p>sourcewebpage: {1}</p><p>CTID: {2}</p> <p>EnvelopeId: {3}</p> ", output.name, output.source, output.CTID, crEnvelopeId );
					//NotifyOnPublish( "ConceptScheme", msg );
				}
				else
				{
					messages.Add( status );
					isValid = false;
				}
			}
			else
			{
				isValid = false;
				if ( !string.IsNullOrWhiteSpace( status ) )
					messages.Add( status );
				helper.Payload = JsonConvert.SerializeObject( output, ServiceHelper.GetJsonSettings() );
			}

			helper.SetMessages( messages );
		}
		public bool ToMap( EntityRequest request, OutputEntity output, ref List<string> messages )
		{
			CurrentEntityType = "ConceptScheme";
			bool isValid = true;
			RJ.EntityReferenceHelper helper = new RJ.EntityReferenceHelper();
			InputEntity input = request.ConceptScheme;
			bool hasDefaultLanguage = false;
			
			try
			{ 

			}
			catch ( Exception ex )
			{
				LogError( ex, "CompetencyServices.ToMap" );
				messages.Add( ex.Message );
			}

			if ( messages.Count > 0 )
				isValid = false;

			return isValid;
		}
		#endregion
	}
}
