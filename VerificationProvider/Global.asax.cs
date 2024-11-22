using System;
using System.Net.Http;
using Autofac;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.WebApi;
using VerificationProvider.Interfaces;
using VerificationProvider.Services;

namespace VerificationProvider
{
	public class WebApiApplication : HttpApplication
	{
		private const int TimeoutDurationInSeconds = 30;
		protected void Application_Start()
		{
			var builder = new ContainerBuilder();

			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

			builder.RegisterType<PassCodeService>()
				.As<IPassCodeService>()
				.SingleInstance();

			builder.Register(c => MemoryCache.Default)
				.AsSelf()
				.SingleInstance();

			builder.Register(x =>
			{
				var client = new HttpClient
				{
					BaseAddress = new Uri("https://rika-solutions-email-provider.azurewebsites.net/"),
					Timeout = TimeSpan.FromSeconds(TimeoutDurationInSeconds)
				};

				return client;
			}).Named<HttpClient>("EmailProvider")
				.SingleInstance();

			builder.Register(x =>
			{
				var client = new HttpClient
				{
					BaseAddress = new Uri("https://rika-tokenservice-agbebvf3drayfqf6.swedencentral-01.azurewebsites.net/"),
					Timeout = TimeSpan.FromSeconds(TimeoutDurationInSeconds)
				};
				return client;
			}).Named<HttpClient>("TokenProvider")
				.SingleInstance();

			builder.RegisterType<HttpClientService>()
				.AsSelf()
				.SingleInstance();

			var container = builder.Build();

			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}
	}
}
