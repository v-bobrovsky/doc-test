using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Documents.Core;

[assembly: OwinStartupAttribute(typeof(Documents.OwinStartup))]
namespace Documents
{
    /// <summary>
    /// Owin configuration
    /// </summary>
    public class OwinStartup
    {
        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        /// <summary>
        /// Configuration auth
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<ServiceUserManager>(ServiceUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}