using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Webbchat.App_Start {
	public class Startup {
		public void Konfigurera(IAppBuilder app) {
			app.UseCookieAuthentication(new CookieAuthenticationOptions {
				AuthenticationType = "ApplicationCookie",
				LoginPath = new PathString("/auth/login")
			});
		}
	}
}