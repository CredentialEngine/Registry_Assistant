﻿using System.Collections.Generic;

using Newtonsoft.Json;

namespace CredentialRegistry
{
	public class Envelope
	{
		public Envelope()
		{
			NodeHeader = new CredentialRegistry.NodeHeader();
		}

		[JsonProperty( PropertyName = "envelope_type" )]
		public string EnvelopeType { get; set; }

		[JsonProperty( PropertyName = "envelope_version" )]
		public string EnvelopeVersion { get; set; }

        [JsonProperty( PropertyName = "envelope_ceterms_ctid" )]
        public string EnvelopeCetermsCtid { get; set; }

        [JsonProperty( PropertyName = "envelope_ctdl_type" )]
        public string EnvelopeCtdlType { get; set; }

        /// <summary>
        /// Not used for an add - may be added back later
        /// </summary>
        //[JsonProperty( PropertyName = "envelope_id" )]
        //public string EnvelopeIdentifier { get; set; }

        [JsonProperty( PropertyName = "envelope_community" )]
		public string EnvelopeCommunity { get; set; }

		[JsonProperty( PropertyName = "resource" )]
		public string Resource { get; set; }

		[JsonProperty( PropertyName = "resource_format" )]
		public string ResourceFormat { get; set; }

		[JsonProperty( PropertyName = "resource_encoding" )]
		public string ResourceEncoding { get; set; }

		[JsonProperty( PropertyName = "resource_public_key" )]
		public string ResourcePublicKey { get; set; }

		[JsonProperty( PropertyName = "documentOwnedBy" )]
		public string documentOwnedBy { get; set; }

		[JsonProperty( PropertyName = "documentPublishedBy" )]
		public string documentPublishedBy { get; set; }

		/// <summary>
		/// We don't have access to this here
		/// </summary>
		[JsonProperty( PropertyName = "publisher_id" )]
		public string publisher_id { get; set; }

		[JsonProperty( PropertyName = "node_headers" )]
		public NodeHeader NodeHeader { get; set; }

		[JsonProperty( PropertyName = "changed" )]
		public bool Changed { get; set; }

	}
	public class UpdateEnvelope : Envelope
	{
		/// <summary>
		/// NOTE: at this time, the EnvelopeIdentifier is not provided when doing an initial publish (ie. an Add). It is only used for an update, and will contain the envelope identifier returned by the registry from the initial publish.
		/// </summary>
		[JsonProperty( PropertyName = "envelope_id" )]
		public string EnvelopeIdentifier { get; set; }

	}


	public class ReadEnvelope : Envelope
	{
		[JsonProperty( PropertyName = "envelope_id" )]
		public string EnvelopeIdentifier { get; set; }


		[JsonProperty( PropertyName = "decoded_resource" )]
		public object DecodedResource { get; set; }

		//probably don't care about the headers, but include for now
		//[JsonProperty( PropertyName = "node_headers" )]
		//public NodeHeader NodeHeaders { get; set; }

	}
	public class NodeHeader
	{
		[JsonProperty( PropertyName = "resource_digest" )]
		public string ResourceDigest { get; set; }

		[JsonProperty( PropertyName = "versions" )]
		public List<NodeVersion> NodeVersions { get; set; }

		[JsonProperty( PropertyName = "created_at" )]
		public string CreatedAt { get; set; }

		[JsonProperty( PropertyName = "updated_at" )]
		public string UpdatedAt { get; set; }

		[JsonProperty( PropertyName = "deleted_at" )]
		public string DeletedAt { get; set; }
	}

	public class NodeVersion
	{
		[JsonProperty( PropertyName = "head" )]
		public string head { get; set; }

		[JsonProperty( PropertyName = "event" )]
		public string EventType { get; set; }

		[JsonProperty( PropertyName = "created_at" )]
		public string CreatedAt { get; set; }

		[JsonProperty( PropertyName = "actor" )]
		public string Actor { get; set; }

		[JsonProperty( PropertyName = "url" )]
		public string Url { get; set; }

	}
	public class DeleteObject
	{
		public DeleteObject()
		{
			deleteLabel = "true";
		}
		[JsonProperty( PropertyName = "delete" )]
		public string deleteLabel { get; set; }

		[JsonProperty( PropertyName = "ctld:ctid" )]
		public string Ctid { get; set; }

		[JsonProperty( PropertyName = "deletedBy" )]
		public string Actor { get; set; }

	}

	public class DeleteEnvelope
	{
		//[JsonProperty( PropertyName = "envelope_community" )]
		//public string EnvelopeCommunity { get; set; }


		//[JsonProperty( PropertyName = "envelope_id" )]
		//public string EnvelopeIdentifier { get; set; }

		/// <summary>
		/// No particular value idenified at this time. 
		/// </summary>
		[JsonProperty( PropertyName = "delete_token" )]
		public string DeleteToken { get; set; }

		[JsonProperty( PropertyName = "delete_token_format" )]
		public string ResourceFormat { get; set; }

		[JsonProperty( PropertyName = "delete_token_encoding" )]
		public string ResourceEncoding { get; set; }

		[JsonProperty( PropertyName = "delete_token_public_key" )]
		public string ResourcePublicKey { get; set; }

	}


	public class RegistryResponse
	{
		[JsonProperty( PropertyName = "Content" )]
		public object content { get; set; }

		[JsonProperty( PropertyName = "Headers" )]
		public object Headers { get; set; }

		[JsonProperty( PropertyName = "IsSuccessStatusCode" )]
		public bool IsSuccessStatusCode { get; set; }

		[JsonProperty( PropertyName = "ReasonPhrase" )]
		public string ReasonPhrase { get; set; }

		[JsonProperty( PropertyName = "RequestMessage" )]
		public object RequestMessage { get; set; }

		[JsonProperty( PropertyName = "StatusCode" )]
		public string StatusCode { get; set; }

		[JsonProperty( PropertyName = "Version" )]
		public string Version { get; set; }

	}


	public class RegistryResponseContent
	{
		[JsonProperty( PropertyName = "errors" )]
		public List<string> Errors { get; set; }

		[JsonProperty( PropertyName = "json_schema" )]
		public List<string> JsonSchema { get; set; }

	}

}
