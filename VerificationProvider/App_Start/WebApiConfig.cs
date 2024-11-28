using System.Web.Http;
using System.Web.Http.Cors;

namespace VerificationProvider
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			config.EnableCors(new EnableCorsAttribute(
				origins: "http://localhost:5173",
				headers: "*",
				methods: "*"
			));

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
