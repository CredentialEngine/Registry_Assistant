using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;

namespace RegistryAPI.Controllers
{
    public class BaseController : ApiController
    {

		protected string FormatObsoleteEndpoint(string controller, string method)
		{
			string domain = "https://" + Request.RequestUri.Authority;
			string oldpath = "https://" + Request.RequestUri.AbsolutePath;
			string message = string.Format( "The requested endpoint ({0}) is now obsolete. Your request was redirected to the /{1}/{2}V2 endpoint. Please use the latter endpoint for future requests.", oldpath, controller, method) ;

			return message;
		}
		//

		public class JsonResponseMessage : HttpResponseMessage
		{
			public JsonResponseMessage( object data, bool valid, string status, object extra )
			{
				Content = new StringContent( JsonConvert.SerializeObject( new { data = data, valid = valid, status = status, extra = extra } ), Encoding.UTF8, "application/json" );
			}
		}
		//

		public JsonResponseMessage JsonResponse( object data, bool valid, string status, object extra = null )
		{
			return new JsonResponseMessage( data, valid, status, extra );
		}
		//
    }

}
