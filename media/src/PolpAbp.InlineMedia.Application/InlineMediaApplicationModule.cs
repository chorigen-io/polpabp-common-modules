using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace PolpAbp.InlineMedia;

[DependsOn(
    typeof(InlineMediaDomainModule),
    typeof(InlineMediaApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(PolpAbp.Framework.PolpAbpFrameworkAbpExtensionsModule)
    )]
public class InlineMediaApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<InlineMediaApplicationModule>();
    }
}
