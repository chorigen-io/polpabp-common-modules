using Microsoft.Extensions.Options;
using PolpAbp.ResourceManagement.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PolpAbp.ResourceManagement.Services
{
    public class ResourceLogProvider : IResourceLogProvider, ITransientDependency
    {
        public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

        protected ResourceMgmtOptions Options;

        public ResourceLogProvider(IOptions<ResourceMgmtOptions> options)
        {
            Options = options.Value;
        }

        public async Task StoreAsync(ResourceLogInfo resourceLogInfo, CancellationToken cancellationToken, bool autoSave = false)
        {
            if (Options.IsEnabled)
            {
                // todo: Exception handling
                var contributor = Options.Contributors.First(a => a.Key == resourceLogInfo.ResourceName);
                var instance = LazyServiceProvider.LazyGetRequiredService(contributor.Value) as IResourceLogContributor;

                await instance.StoreAsync(resourceLogInfo, cancellationToken, autoSave);
            }
        }

        public virtual async Task<long> CountCurrentUsageAsync(string resourceName, Guid? userId, DateTime StartedOn, DateTime? EndedOn, CancellationToken cancellationToken)
        {
            var contributor = Options.Contributors.FirstOrDefault(a => a.Key == resourceName);
            if (contributor.Key == null)
            {
                // No contributor meters this resource: it counts as zero usage.
                return 0;
            }
            var instance = LazyServiceProvider.LazyGetRequiredService(contributor.Value) as IResourceLogContributor;

            return await instance.CountCurrentUsageAsync(userId, StartedOn, EndedOn, cancellationToken);
        }

        public virtual async Task<long> GetMonthlyUsageAsync(string resourceName, int year, int month, CancellationToken cancellationToken)
        {
            var contributor = Options.Contributors.FirstOrDefault(a => a.Key == resourceName);
            if (contributor.Key == null)
            {
                // No contributor meters this resource: it counts as zero usage.
                return 0;
            }
            var instance = LazyServiceProvider.LazyGetRequiredService(contributor.Value) as IResourceLogContributor;

            return await instance.GetMonthlyUsageAsync(year, month, cancellationToken);
        }

        public virtual async Task<long> GetYearlyUsageAsync(string resourceName, int year, CancellationToken cancellationToken)
        {
            var contributor = Options.Contributors.FirstOrDefault(a => a.Key == resourceName);
            if (contributor.Key == null)
            {
                // No contributor meters this resource: it counts as zero usage.
                return 0;
            }
            var instance = LazyServiceProvider.LazyGetRequiredService(contributor.Value) as IResourceLogContributor;

            return await instance.GetYearlyUsageAsync(year, cancellationToken);
        }
    }
}
