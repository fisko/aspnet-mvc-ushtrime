using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace UniversityApi
{
    public static class WebApiConfig
    {
        // Zëvendëso metodën Register në App_Start/WebApiConfig.cs me këtë
        public static void Register(HttpConfiguration config)
        {
            // Kthe vetëm JSON (hiq formatuesin XML)
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Emrat e vetive në JSON fillojnë me shkronjë të vogël: firstName
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}