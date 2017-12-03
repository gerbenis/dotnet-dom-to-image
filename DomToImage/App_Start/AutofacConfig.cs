using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using DomToImage.ServiceContracts;
using DomToImage.Services;

namespace DomToImage
{
    public static class AutofacConfig
    {
        public static void RegisterAutofacContainer(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<PhantomJsClient>().As<IPhantomJsClient>().SingleInstance();
            builder.RegisterType<ImageExportService>().As<IImageExportService>().SingleInstance();
            
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}