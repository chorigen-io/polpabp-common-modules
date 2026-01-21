using Microsoft.Extensions.Caching.Distributed;
using PolpAbp.ResourceManagement.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace PolpAbp.ResourceManagement.Services
{
    [RemoteService(false)]
    public class ResourceUsageQueryService : IResourceUsageQueryService, ITransientDependency
    {
        private readonly IResourceLogProvider _resourceLogProvider;
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IDistributedCache<ResourceUsageCacheItem> _cache;
        private readonly IRepository<Resource, Guid> _resourceRepository;

        public ResourceUsageQueryService(IResourceLogProvider resourceLogProvider,
            ISubscriptionPlanService subscriptionPlanService,
            IDistributedCache<ResourceUsageCacheItem> cache,
            IRepository<Resource, Guid> resourceRepository)
        {
            _resourceLogProvider = resourceLogProvider;
            _subscriptionPlanService = subscriptionPlanService;
            _cache = cache;
            _resourceRepository = resourceRepository;
        }

        public async Task<string> GetCategoryAsync(string resourceName,
            CancellationToken cancellationToken = default)
        {
            var resourceEntry = await _resourceRepository.GetAsync(a => a.Name == resourceName, cancellationToken: cancellationToken);
            return resourceEntry.Category;
        }

        public async Task<ResourceUsageCacheItem> GetUsageByResourceNameAsync(string resourceName,
            string cacheKey,
            int expiredInMins = 10, bool forceReload = false,
            CancellationToken cancellationToken = default)
        {
            if (forceReload)
            {
                await _cache.RemoveAsync(cacheKey, token: cancellationToken);
            }

            return await _cache.GetOrAddAsync(
              cacheKey,
              async () =>
              {
                  var quota = await _subscriptionPlanService.GetQuotaByResourceNameAsync(resourceName, true, cancellationToken: cancellationToken);
                  // Get the usage 
                  var curBillingCycle = await _subscriptionPlanService.GetCurrentBillingPeriodAsync(resourceName, cancellationToken: cancellationToken);
                  var usage = await _resourceLogProvider.CountCurrentUsageAsync(resourceName, null, curBillingCycle.Item1, curBillingCycle.Item2, cancellationToken: cancellationToken);

                  return new ResourceUsageCacheItem
                  {
                      Usage = usage,
                      Quota = quota
                  };
              },
              () => new DistributedCacheEntryOptions
              {
                  SlidingExpiration = new TimeSpan(0, expiredInMins, 0)
              },
              token: cancellationToken
          );
        }

        public async Task<ResourceUsageCacheItem> GetUsageByCategoryNameAsync(string categoryName,
                Func<string, string> cacheKeyFunc,
                int expiredInMins = 10, bool forceReload = false,
                CancellationToken cancellationToken = default)
        {
            var resourceEntries = await _resourceRepository.
                GetListAsync(a => a.Category == categoryName, cancellationToken: cancellationToken);

            var ret = new ResourceUsageCacheItem()
            {
                Quota = 0,
                Usage = 0
            };

            foreach (var elem in resourceEntries)
            {
                var cacheKey = cacheKeyFunc(elem.Name);
                var a = await GetUsageByResourceNameAsync(elem.Name, cacheKey, expiredInMins, forceReload, cancellationToken);
                ret.Quota += a.Quota;
                ret.Usage += a.Usage;
            }

            var categoryQuota = await _subscriptionPlanService.GetQuotaByCategoryNameAsync(categoryName, true, cancellationToken);
            if (categoryQuota > 0)
            {
                ret.Quota = categoryQuota;
            }

            return ret;
        }
    }
}
