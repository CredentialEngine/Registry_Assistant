using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistryAPI.Controllers
{
	public class HelperController : Controller
	{
		public JsonResult GetClass( string className )
		{
			try
			{
				var target = Type.GetType( "RA.Models.Input." + className + ",RA.Models" );
				var result = new Dictionary<string, object>();
				foreach( var property in target.GetProperties() )
				{
					if( property.PropertyType.Name == "List`1" ) //Lists
					{
						var listType = property.PropertyType.GenericTypeArguments.FirstOrDefault().Name;
						result.Add( property.Name, new PropertyData() { Type = listType, Multi = true } );
					}
					else if(property.PropertyType.Name == "Nullable`1" ) //Nullable booleans
					{
						var nullType = property.PropertyType.GenericTypeArguments.FirstOrDefault().Name;
						result.Add( property.Name, new PropertyData() { Type = nullType, Multi = false } );
					}
					else
					{
						result.Add( property.Name, new PropertyData() { Type = property.PropertyType.Name } );
					}
				}
				return JsonResponse( result, true, "", null );
			}
			catch
			{
				return JsonResponse( null, false, "Error: Unable to find class", null );
			}
		}
		//

		public class PropertyData
		{
			public string Type { get; set; }
			public bool Multi { get; set; }
		}

		public JsonResult JsonResponse( object data, bool valid, string status, object extra )
		{
			return new JsonResult() { Data = new { Data = data, Valid = valid, Status = status, Extra = extra }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}
		//
    }
}