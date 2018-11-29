using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
    }
}
