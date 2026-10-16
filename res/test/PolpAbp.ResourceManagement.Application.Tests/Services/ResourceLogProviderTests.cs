using PolpAbp.ResourceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace PolpAbp.ResourceManagement.Services
{
    public class ResourceLogProviderTests : ResourceManagementApplicationTestBase
    {
        private IResourceLogProvider _resourceLogProvider;
        private ICurrentTenant _currentTenant;
        private IRepository<ResourceMonthlyUsage> _monthlyUsageRepository;
        private IRepository<ResourceYearlyUsage> _yearlyUsageRepository;
        private IRepository<Resource> _resourceRepository;

        public ResourceLogProviderTests()
        {

            _resourceLogProvider = GetRequiredService<IResourceLogProvider>();
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _monthlyUsageRepository = GetRequiredService<IRepository<ResourceMonthlyUsage>>();
            _yearlyUsageRepository = GetRequiredService<IRepository<ResourceYearlyUsage>>();
            _resourceRepository = GetRequiredService<IRepository<Resource>>();
        }

        [Fact]
        public async Task CanStoreAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var info = new ResourceLogInfo()
                    {
                        ResourceName = ResourceManagementTestConsts.SmsResourceName,
                        TenantId = ResourceManagementTestConsts.TenantId,
                        UserId = ResourceManagementTestConsts.AdminId,
                        HappenedOn = DateTime.UtcNow,
                        Usage = 1
                    };
                    await _resourceLogProvider.StoreAsync(info, default);

                    Assert.True(true);
                });
            }
        }

        [Fact]
        public async Task CanCountCurrentUsageAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var info = new ResourceLogInfo()
                    {
                        ResourceName = ResourceManagementTestConsts.SmsResourceName,
                        TenantId = ResourceManagementTestConsts.TenantId,
                        UserId = ResourceManagementTestConsts.AdminId,
                        HappenedOn = DateTime.UtcNow,
                        Usage = 1
                    };
                    await _resourceLogProvider.StoreAsync(info, default);
                });

                await WithUnitOfWorkAsync(async () =>
                {

                    var a = await _resourceLogProvider.CountCurrentUsageAsync(
                        ResourceManagementTestConsts.SmsResourceName,
                        null,
                        DateTime.UtcNow.Subtract(new TimeSpan(48, 0, 0)),
                            null, 
                            default);

                    Assert.Equal(1, a);
                });
            }
        }

        [Fact]
        public async Task CanGetMonthlyUsageAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var resource = await _resourceRepository
                    .GetAsync(x => x.Name == ResourceManagementTestConsts.SmsResourceName);
                    await _monthlyUsageRepository.InsertAsync(
                        new ResourceMonthlyUsage(Guid.NewGuid())
                        {
                            ResourceId = resource.Id,
                            Year = 2023,
                            Month = 1,
                            Usage = 55
                        });
                });

                await WithUnitOfWorkAsync(async () =>
                {

                    var b = await _resourceLogProvider
                    .GetMonthlyUsageAsync(ResourceManagementTestConsts.SmsResourceName,
                        2023, 1, default);

                    Assert.Equal(55, b);
                });
            }
        }


        [Fact]
        public async Task CanGetYearlyUsageAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var resource = await _resourceRepository
                    .GetAsync(x => x.Name == ResourceManagementTestConsts.SmsResourceName);

                    await _yearlyUsageRepository.InsertAsync(
                        new ResourceYearlyUsage(Guid.NewGuid())
                        {
                            ResourceId = resource.Id,
                            Year = 2023,
                            Usage = 2023
                        });
                });

                await WithUnitOfWorkAsync(async () =>
                {

                    var b = await _resourceLogProvider
                    .GetYearlyUsageAsync(ResourceManagementTestConsts.SmsResourceName,
                        2023, default);

                    Assert.Equal(2023, b);
                });
            }
        }

        [Fact]
        public async Task CanGetUsageInChainWayAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var info = new ResourceLogInfo()
                    {
                        ResourceName = ResourceManagementTestConsts.SmsResourceName,
                        TenantId = ResourceManagementTestConsts.TenantId,
                        UserId = ResourceManagementTestConsts.AdminId,
                        HappenedOn = DateTime.UtcNow,
                        Usage = 100
                    };
                    await _resourceLogProvider.StoreAsync(info, default);
                });

                await WithUnitOfWorkAsync(async () =>
                {

                    var b = await _resourceLogProvider
                    .GetYearlyUsageAsync(ResourceManagementTestConsts.SmsResourceName,
                        DateTime.UtcNow.Year, default);

                    Assert.Equal(100, b);
                });
            }
        }

        // A seeded resource with no registered contributor counts as zero usage (#2).
        [Fact]
        public async Task CountCurrentUsageAsync_IsZero_ForUnregisteredResourceAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var a = await _resourceLogProvider.CountCurrentUsageAsync(
                        ResourceManagementTestConsts.FormPublication,
                        null,
                        DateTime.UtcNow.Subtract(new TimeSpan(48, 0, 0)),
                        null,
                        default);

                    Assert.Equal(0, a);
                });
            }
        }

        [Fact]
        public async Task GetMonthlyUsageAsync_IsZero_ForUnregisteredResourceAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var b = await _resourceLogProvider
                    .GetMonthlyUsageAsync(ResourceManagementTestConsts.FormPublication,
                        2023, 1, default);

                    Assert.Equal(0, b);
                });
            }
        }

        [Fact]
        public async Task GetYearlyUsageAsync_IsZero_ForUnregisteredResourceAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var b = await _resourceLogProvider
                    .GetYearlyUsageAsync(ResourceManagementTestConsts.FormPublication,
                        2023, default);

                    Assert.Equal(0, b);
                });
            }
        }

        // Storing usage that nobody meters stays a programming error (#2).
        [Fact]
        public async Task StoreAsync_Throws_ForUnregisteredResourceAsync()
        {
            using (_currentTenant.Change(ResourceManagementTestConsts.TenantId))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var info = new ResourceLogInfo()
                    {
                        ResourceName = ResourceManagementTestConsts.FormPublication,
                        TenantId = ResourceManagementTestConsts.TenantId,
                        UserId = ResourceManagementTestConsts.AdminId,
                        HappenedOn = DateTime.UtcNow,
                        Usage = 1
                    };

                    await Assert.ThrowsAsync<InvalidOperationException>(
                        () => _resourceLogProvider.StoreAsync(info, default));
                });
            }
        }

    }
}
