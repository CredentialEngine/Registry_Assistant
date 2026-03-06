using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	/// <summary>
	/// Registry Assistant Response
	/// </summary>
	public class RegistryAssistantResponse
	{
		public RegistryAssistantResponse()
		{
			Messages = new List<string>();
			Payload = string.Empty;
		}

		/// True if action was successful, otherwise false
		public bool Successful { get; set; }

		/// True if registry resource was updated, false if the input data was the same as the currently saved data.
		public bool WasChanged { get; set; }

		/// <summary>
		/// List of error or warning messages
		/// </summary>
		public List<string> Messages { get; set; }

		public string CTID { get; set; }

		public DateTime ResponseDate { get; set; } = DateTime.Now;

		/// <summary>
		/// URL for the registry envelope that contains the document just add/updated
		/// </summary>
		public string EnvelopeUrl { get; set; }

		/// <summary>
		/// URL for the graph endpoint for the document just add/updated
		/// </summary>
		public string GraphUrl { get; set; }

		/// <summary>
		/// Credential Finder Detail Page URL for the document just published (within 30 minutes of publishing)
		/// </summary>
		public string CredentialFinderUrl { get; set; }

		/// <summary>
		/// Identifier for the registry envelope that contains the document just add/updated
		/// </summary>
		public string RegistryEnvelopeIdentifier { get; set; }

		/// <summary>
		/// Payload of request to registry, containing properties formatted as CTDL - JSON-LD
		/// </summary>
		public string Payload { get; set; }
	}

	public class RegistryAssistantFormatResponse
	{
		public RegistryAssistantFormatResponse()
		{
			Messages = new List<string>();
			Payload = string.Empty;
		}

		/// True if action was successfull, otherwise false
		public bool Successful { get; set; }

		/// <summary>
		/// List of error or warning messages
		/// </summary>
		public List<string> Messages { get; set; }

		/// <summary>
		/// Payload of request to registry, containing properties formatted as CTDL - JSON-LD
		/// </summary>
		public string Payload { get; set; }
	}

	public class RegistryAssistantDeleteResponse
	{
		public RegistryAssistantDeleteResponse()
		{
			Messages = new List<string>();
		}

		/// <summary>
		/// True if delete was successfull, otherwise false
		/// </summary>
		public bool Successful { get; set; }

		/// <summary>
		/// List of error or warning messages
		/// </summary>
		public List<string> Messages { get; set; }
	}

	public class ValidateResponse : RegistryAssistantDeleteResponse
	{
	}
}
