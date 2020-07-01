using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA.Models
{
	public class RequestHelper
	{
		public RequestHelper()
		{
			Messages = new List<RequestMessage>();
			HasErrors = false;
			CodeValidationType = "warn";
			IsPublishRequestType = true;
		}
        public string ApiKey { get; set; } = "";
        public string OwnerCtid { get; set; } = "";
        public string ClientIdentifier { get; set; } = "";

		//true if originates from publisher
		public bool IsPublisherRequest { get; set; }
		public bool IsPublishRequestType { get; set; }
		/// <summary>
		/// CodeValidationType - actions for code validation
		/// rigid-concepts must match ctdl 
		/// warn - allow exceptions, return a warning message
		/// skip - no validation of concept scheme concepts
		/// </summary>
		public string CodeValidationType { get; set; }
        public string SerializedInput { get; set; }
        public string Payload { get; set; }
		/// <summary>
		/// return the registr envelope id
		/// </summary>
		public string RegistryEnvelopeId { get; set; }
		public string EnvelopeUrl { get; set; }
		public string GraphUrl { get; set; }

		public List<RequestMessage> Messages { get; set; }
		public bool HasErrors { get; set; }
		public void AddError( string message )
		{
			Messages.Add( new Models.RequestMessage() { Message = message});
			HasErrors = true;
		}
		public void AddWarning( string message )
		{
			Messages.Add( new Models.RequestMessage() { Message = message, IsWarning = true } );
		}
		
		public bool WasChanged { get; set; }

		public List<string> GetAllMessages()
		{
			List<string> messages = new List<string>();
			string prefix = "";
			foreach(RequestMessage msg in Messages.OrderBy(m => m.IsWarning))
			{
				prefix = "";
				if ( msg.IsWarning )
				{
					if ( msg.Message.ToLower().IndexOf( "warning" ) == -1 && msg.Message.ToLower().IndexOf( "note" ) == -1 )
						prefix = "Warning - ";
				}
				else if ( msg.Message.ToLower().IndexOf( "error" ) == -1 && msg.Message.ToLower().IndexOf( "warning" ) == -1 && msg.Message.ToLower().IndexOf( "note" ) == -1 )
					prefix = "Error - ";

				messages.Add( prefix + msg.Message );
			}
			

			return messages;
		}
		public void SetWarningMessages( List<string> messages )
		{
			foreach ( string msg in messages )
			{
				AddWarning( msg );
			}

		}
		public void SetMessages( List<string> messages )
		{
			foreach ( string msg in messages )
			{
				if ( msg.ToLower().IndexOf( "warning" ) > -1 || msg.ToLower().IndexOf( "note" ) == 0 )
					AddWarning( msg );
				else
					AddError( msg );
			}

		}
	}

	public class RequestMessage
	{
		public string Message { get; set; }
		public bool IsWarning { get; set; }
	}
}
