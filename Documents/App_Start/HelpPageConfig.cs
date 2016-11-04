using Documents.Areas.HelpPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Documents
{
    /// <summary>
    /// Help page configuration
    /// </summary>
    public static class HelpPageConfig
    {
        /// <summary>
        /// Register help provider
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.SetDocumentationProvider(new XmlDocumentationProvider(
                HttpContext.Current.Server.MapPath("~/App_Data/XmlDocument.xml")));
        }
    }
}
