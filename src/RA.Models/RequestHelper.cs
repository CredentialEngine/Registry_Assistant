using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RA.Models
{
    /// <summary>
    /// Helper class for publishing work flow.
    /// </summary>
    public class RequestHelper
    {
        public RequestHelper()
        {
            Messages = new List<RequestMessage>();
            HasErrors = false;
            CodeValidationType = "warn";
            IsPublishRequestType = true;
        }
        public string ApiKey { get; set; }
        public string OwnerCtid { get; set; }
        public string CurrentCTID { get; set; }
        public string ClientIdentifier { get; set; }
        public string EntityName { get; set; }
        public string IPAddress { get; set; }
        /// <summary>
        /// Flag to indicate if the data being published is from a primary source (true) or a secondary source (false)
        /// Decided to not use here, rather direct from request.
        /// </summary>
        public bool IsPrimarySourceRecord { get; set; } = true;
        /// <summary>
        /// true if originates from publisher
        /// </summary>
        public bool IsRequestFromPublisher { get; set; }
        /// <summary>
        /// True if a publish request
        /// False if a format (or validate request)
        /// </summary>
        public bool IsPublishRequestType { get; set; }

        public bool ResourceExistsInRegistry { get; set; }
        public DateTime RegistryResourceCreated { get; set; }
        public DateTime RegistryResourceLastUpdated { get; set; }

        public bool MappingFullResource { get; set; } = true;
        public string ExternalParentEntityType { get; set; }

        /// <summary>
        /// CodeValidationType - actions for code validation
        /// rigid-concepts must match ctdl 
        /// warn - allow exceptions, return a warning message
        /// skip - no validation of concept scheme concepts
        /// </summary>
        public string CodeValidationType { get; set; }
        // Serialized input document
        public string SerializedInput { get; set; }
        /// <summary>
        /// Serialized JSON-LD document
        /// </summary>
        public string Payload { get; set; }
        /// <summary>
        /// return the registr envelope id
        /// </summary>
        public string RegistryEnvelopeId { get; set; }
        public string EnvelopeUrl { get; set; }
        public string GraphUrl { get; set; }
        public List<PublishResults> AdditionalPublishedResults { get; set; }
        //========= custom handling ==============
        /// <summary>
        /// Request for FL staging registry
        /// Technically this is used for FVC and FloridaCommerce (formerly fdoe)
        /// </summary>
        public bool IsFloridaVirtualCampusRequest { get; set; }
        /// <summary>
        /// If true, a copy of the current request will also be published to the public registry
        /// only do if the request was for staging registry and the identifier is Public
        /// May always do for an org
        /// </summary>
        public bool FloridaVirtualCampusPublishPublic { get; set; }

        public PrivateRegistryRequest PrivateRegistryRequest { get; set; } = null;
        public string CurrentPrivateRegistry
        {
            get
            {
                if ( PrivateRegistryRequest != null
                    && ( PrivateRegistryRequest.IsATexasRegistryRequest || PrivateRegistryRequest.IsAFloridaRegistryRequest ))
                {
                    return PrivateRegistryRequest.PrivateRegistry;
                } 
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// A TBD for TX is whether data will always be published to both registries.
        /// The preference would be to a general indicator to "also publish to public" to avoid duplicate+ logic
        /// </summary>
        public bool IsTexasLibraryRequest
        {
            get
            {
                return PrivateRegistryRequest != null
                    && ( PrivateRegistryRequest.IsATexasRegistryRequest );
            }
        }
        /// <summary>
        /// Return true if any private registry request
        /// </summary>
        public bool IsPrivateRegistryRequest
        { 
            get 
            { 
                return PrivateRegistryRequest!= null 
                    && ( PrivateRegistryRequest.IsATexasRegistryRequest || PrivateRegistryRequest.IsAFloridaRegistryRequest ); 
            } 
        }

        public bool PrivateRegistryAndDoingPublicSync
        {
            get
            {
                return PrivateRegistryRequest != null
                    && ( PrivateRegistryRequest.AutoPublishingAllToPublic || PrivateRegistryRequest.DoingSyncOfResourceToPublic );
            }
        }
        //exception helper
        public string ExceptionAddendum { get; set; }
        //========= messages ==============
        public List<RequestMessage> Messages { get; set; }
        public bool HasErrors { get; set; }
        public void AddError( string message )
        {
            Messages.Add( new Models.RequestMessage() { Message = message } );
            HasErrors = true;
        }

        public void AddWarning( string message )
        {
            Messages.Add( new Models.RequestMessage() { Message = message, IsWarning = true } );
        }

        public bool WasChanged { get; set; }
        public void ResetHelper()
        {
            Messages = new List<RequestMessage>();
            HasErrors = false;
            Payload = string.Empty;
            RegistryEnvelopeId = string.Empty;
            EnvelopeUrl = string.Empty;
            GraphUrl = string.Empty;
            WasChanged = false;
        }

        /// <summary>
        /// Clone the current request helper for a related process. 
        /// </summary>
        /// <returns></returns>
        public RequestHelper CloneForRelatedRequest()
        {
            RequestHelper clone = new RequestHelper()
            {
                ApiKey = this.ApiKey,
                OwnerCtid = this.OwnerCtid,
                CurrentCTID = string.Empty,
                ClientIdentifier = this.ClientIdentifier,
                EntityName = string.Empty,
                IPAddress = this.IPAddress,
                IsPrimarySourceRecord = this.IsPrimarySourceRecord,
                IsRequestFromPublisher = this.IsRequestFromPublisher,
                IsPublishRequestType = this.IsPublishRequestType,
                ResourceExistsInRegistry = false,
                //not real relevent
                MappingFullResource = false,
                //this may not be relevent 
                ExternalParentEntityType = string.Empty,
                CodeValidationType = string.Empty,
                SerializedInput = string.Empty,
                Payload = string.Empty,
                RegistryEnvelopeId = string.Empty,
                EnvelopeUrl = string.Empty,
                GraphUrl = string.Empty,
                AdditionalPublishedResults = new List<PublishResults>(),
                //retain for now
                IsFloridaVirtualCampusRequest = this.IsFloridaVirtualCampusRequest,
                FloridaVirtualCampusPublishPublic = this.FloridaVirtualCampusPublishPublic,
                PrivateRegistryRequest = this.PrivateRegistryRequest,
                ExceptionAddendum = string.Empty,

                Messages = new List<RequestMessage>(),
                WasChanged = false,
            };
            return clone;

        }
        public List<string> GetAllMessages()
        {
            List<string> messages = new List<string>();
            if ( Messages == null || Messages.Count == 0 )
                return messages;

            string prefix = string.Empty;
            foreach ( RequestMessage msg in Messages.OrderBy( m => m.IsWarning ) )
            {
                prefix = string.Empty;
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
        public List<string> GetAllErrorMessages()
        {
            List<string> messages = new List<string>();
            if ( Messages == null || Messages.Count == 0 )
                return messages;
            foreach ( RequestMessage msg in Messages.OrderBy( m => m.IsWarning ) )
            {
                string prefix = string.Empty;
                if ( !msg.IsWarning )
                {
                    if ( msg.Message.ToLower().IndexOf( "error" ) == -1 && msg.Message.ToLower().IndexOf( "warning" ) == -1 && msg.Message.ToLower().IndexOf( "note" ) == -1 )
                        prefix = "Error - ";

                    if ( msg.Message.ToLower().IndexOf( "successful" ) == -1 )
                        messages.Add( prefix + msg.Message );
                }

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
            if ( messages == null || messages.Count == 0 ) return;

            foreach ( string msg in messages )
            {
                if ( msg.ToLower().IndexOf( "warning" ) > -1 || msg.ToLower().IndexOf( "note" ) == 0 )
                    AddWarning( msg );
                else
                    if ( msg.ToLower().IndexOf( "successful" ) == -1 )
                {
                    AddError( msg );
                }
            }

        }
    }

    /// <summary>
    /// A helper request class for validating requests to publish to a private registry
    /// </summary>
    public class PrivateRegistryRequest
    {
        public PrivateRegistryRequest( string registry, string apiKey )
        {
            PrivateRegistry = registry ?? string.Empty;
            RequestApiKey = apiKey;
            IsAFloridaRegistryRequest = false;
            IsATexasRegistryRequest = false;
        }
        /// <summary>
        /// set to true if there is a private registry request
        /// </summary>
        public bool IsAPrivateRegistryRequest
        {
            get
            {
                return IsAFloridaRegistryRequest
                    || IsATexasRegistryRequest;
            }
        }
        public bool IsAFloridaRegistryRequest { get; set; }
        public bool IsATexasRegistryRequest { get; set; }

        /// <summary>
        /// Name of the private registry
        /// </summary>
        public string PrivateRegistry { get; set; }

        public string RegistryPrefix { get; set; }
        
        /// <summary>
        /// If true, a copy of the current request will also be published to the public registry
        /// only do if the request was for staging registry and the identifier is Public
        /// May always do for an org
        /// </summary>
        public bool AutoPublishingAllToPublic { get; set; }
        public bool DoingSyncOfResourceToPublic { get; set; }
        /// <summary>
        /// Retaining this just in case.
        /// Generally the org should always be published to public or risk future inconsistencies
        /// </summary>
        public bool AutoPublishingOrganizationToPublic { get; set; }

        /// <summary>
        /// If true, on delete of a resource from a private registry, also delete the related resource from the public registry
        /// </summary>
        public bool AutoDeletePublicResourceOnPrivateDelete { get; set; }
        
        /// <summary>
        /// may have to handle a list
        /// </summary>
        public string AllowedOrgApiKeys { get; set; }

        /// <summary>
        /// ApiKey from config file
        /// </summary>
        public string MainApiKey { get; set; }

        /// <summary>
        /// ApiKey from request
        /// </summary>
        public string RequestApiKey { get; set; }

        /// <summary>
        /// Set to true if request must actively request to sync a private request to the public registry.
		/// the only current method is using a status in Identifier. 
		/// However, why just add a request parameter (ex: AutomaticallySyncPrivateRequestToPublicRegistry)
        /// </summary>
        public bool PublicStatusIsRequired { get; set; }
    }

    public class RequestMessage
    {
        public string Message { get; set; }
        public bool IsWarning { get; set; }
    }
    public class PublishResults
    {
        public string AdditionalEntityIdentifier { get; set; }
        public string RegistryEnvelopeId { get; set; }
        public string EnvelopeUrl { get; set; }
        public string GraphUrl { get; set; }
        public string SerializedInput { get; set; }
        public string Payload { get; set; }

    }
}
