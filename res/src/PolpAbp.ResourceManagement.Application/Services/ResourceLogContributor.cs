using PolpAbp.ResourceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace PolpAbp.ResourceManagement.Services
{
    public abstract class ResourceLogContributor : IResourceLogContributor
    {
        public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

        public abstract string ResourceName { get; }

        protected readonly IRepository<ResourceUsageLog, Guid> UsageLogRepository;
        protected readonly IRepository<ResourceMonthlyUsage, Guid> MonthlyUsageRepository;
        protected readonly IRepository<ResourceYearlyUsage, Guid> YearlyUsageRepository;

        protected IRepository<Resource, Guid> ResourceRepository => LazyServiceProvider.LazyGetRequiredService<IRepository<Resource, Guid>>();
        protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();


        public ResourceLogContributor(IRepository<ResourceUsageLog, Guid> usageLogRepo,
                IRepository<ResourceMonthlyUsage, Guid> monthlyUsageRepo,
                IRepository<ResourceYearlyUsage, Guid> yearlyUsageRepo)
        {
            UsageLogRepository = usageLogRepo;
            MonthlyUsageRepository = monthlyUsageRepo;
            YearlyUsageRepository = yearlyUsageRepo;
        }

        public virtual async Task StoreAsync(ResourceLogInfo resourceLogInfo, CancellationToken cancellationToken, bool autoSave = false)
        {
            if (resourceLogInfo.ResourceName != ResourceName)
            {
                throw new Exception("Resource not matched");
            }

            var resourceEntry = await ResourceRepository.GetAsync(a => a.Name == ResourceName, cancellationToken: cancellationToken);

            var entity = new ResourceUsageLog(GuidGenerator.Create())
            {
                ResourceId = resourceEntry.Id,
                Usage = resourceLogInfo.Usage,
                UserId = resourceLogInfo.UserId,
                TenantId = resourceLogInfo.TenantId,
                Intension = resourceLogInfo.Intension,
                IsExempt = resourceLogInfo.IsExempt,
                ExemptionReason = resourceLogInfo.ExemptionReason,
                Destination = resourceLogInfo.Destination,
                CreationTime = resourceLogInfo.HappenedOn
            };

            await UsageLogRepository.InsertAsync(entity, autoSave, cancellationToken: cancellationToken);
        }

        public virtual async Task<long> CountCurrentUsageAsync(Guid? userId, DateTime StartedOn, DateTime? EndedOn, CancellationToken cancellationToken)
        {
            var resourceEntry = await ResourceRepository.GetAsync(a => a.Name == ResourceName, cancellationToken: cancellationToken);

            var query = await UsageLogRepository.GetQueryableAsync();
            var amount = query
                .Where(x => x.ResourceId == resourceEntry.Id && !x.IsExempt && x.CreationTime >= StartedOn)
                .WhereIf(userId.HasValue, w => w.UserId == userId.Value)
                .WhereIf(EndedOn.HasValue, y => y.CreationTime < EndedOn)
                .Select(z => z.Usage)
                .Sum();

            return amount;
        }

        public virtual async Task<long> GetMonthlyUsageAsync(int year, int month, CancellationToken cancellationToken)
        {
            var resourceEntry = await ResourceRepository.GetAsync(a => a.Name == ResourceName, cancellationToken: cancellationToken);

            var query = await MonthlyUsageRepository.GetQueryableAsync();
            var entry = query
                .Where(x => x.ResourceId == resourceEntry.Id && x.Year == year && x.Month == month)
                .FirstOrDefault();

            var sum = 0L;
            if (entry != null)
            {
                sum = entry.Usage;
            }
            else
            {
                var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1);
                sum = await CountCurrentUsageAsync(null, startDate, endDate, cancellationToken);
            }
            return sum;
        }

        public virtual async Task<long> GetYearlyUsageAsync(int year, CancellationToken cancellationToken)
        {
            var resourceEntry = await ResourceRepository.GetAsync(a => a.Name == ResourceName, cancellationToken: cancellationToken);

            var query = await YearlyUsageRepository.GetQueryableAsync();
            var entry = query
                .Where(x => x.ResourceId == resourceEntry.Id && x.Year == year)
            .FirstOrDefault();

            var sum = 0L;
            if (entry != null)
            {
                sum = entry.Usage;
            }
            else
            {
                // Find out the
                for (var m = 1; m <= 12; m++)
                {
                    sum += await GetMonthlyUsageAsync(year, m, cancellationToken);
                }
            }
            return sum;
        }
    }
}
