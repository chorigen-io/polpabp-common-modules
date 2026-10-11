using Microsoft.Extensions.DependencyInjection;
using PolpAbp.Framework;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace PolpAbp.ResourceManagement;

[DependsOn(
    typeof(ResourceManagementDomainModule),
    typeof(ResourceManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpCachingModule),
    typeof(PolpAbpFrameworkAbpExtensionsModule)

    )]
public class ResourceManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<ResourceManagementApplicationModule>();
    }
}
