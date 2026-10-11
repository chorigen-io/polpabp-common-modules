using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using PolpAbp.Framework;

namespace PolpAbp.Directory;

[DependsOn(
    typeof(AbpMapperlyModule),
    typeof(AbpDddApplicationModule),
    typeof(DirectoryDomainModule),
    typeof(DirectoryApplicationContractsModule),
    typeof(PolpAbpFrameworkAbpExtensionsModule)
    )]
public class DirectoryApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<DirectoryApplicationModule>();
    }
}
