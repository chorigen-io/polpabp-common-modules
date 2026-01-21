using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace PolpAbp.ResourceManagement;

[DependsOn(
    typeof(ResourceManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class ResourceManagementApplicationContractsModule : AbpModule
{

}
