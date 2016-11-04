using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Documents
{
    /// <summary>
    /// Web api config
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Configure web api
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {           
            UnityConfig.RegisterComponents(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
