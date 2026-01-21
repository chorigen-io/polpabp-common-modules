using System;
using System.Threading;
using System.Threading.Tasks;

namespace PolpAbp.ResourceManagement.Services
{
    public interface IResourceUsageQueryService
    {
        Task<ResourceUsageCacheItem> GetUsageByResourceNameAsync(string resourceName, string cacheKey,
            int expiredInMins = 10, bool forceReload = false, CancellationToken cancellationToken = default);

        Task<ResourceUsageCacheItem> GetUsageByCategoryNameAsync(string categoryName,
                Func<string, string> cacheKeyFunc,
                int expiredInMins = 10, bool forceReload = false,
                CancellationToken cancellationToken = default);
        Task<string> GetCategoryAsync(string resourceName, CancellationToken cancellationToken = default);
    }
}
