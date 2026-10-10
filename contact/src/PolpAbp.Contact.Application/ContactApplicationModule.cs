using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using PolpAbp.Framework;

namespace PolpAbp.Contact;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(ContactDomainModule),
    typeof(ContactApplicationContractsModule),
    typeof(PolpAbpFrameworkAbpExtensionsModule)
    )]
public class ContactApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<ContactApplicationModule>();
    }
}
