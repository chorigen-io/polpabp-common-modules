using Microsoft.Extensions.Options;
using PolpAbp.ResourceManagement.Core;
using PolpAbp.ResourceManagement.Domain.Entities;
using PolpAbp.ResourceManagement.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace PolpAbp.ResourceManagement.Services
{
    [RemoteService(false)]
    public class SubscriptionPlanService : ResourceManagementAppService, ISubscriptionPlanService
    {
        private readonly IRepository<TenantSubscription, Guid> _subscriptionRepository;
        private readonly IRepository<Resource, Guid> _resourceRepository;
        private readonly ResourceMgmtOptions _options;

        public SubscriptionPlanService(IRepository<TenantSubscription, Guid> subscriptionRepository,
            IRepository<Resource, Guid> resourceRepository,
            IOptions<ResourceMgmtOptions> options)
        {
            _subscriptionRepository = subscriptionRepository;
            _resourceRepository = resourceRepository;
            _options = options.Value;
        }

        public async Task<List<SubscriptionPlanOutputDto>> LoadEffectivePlansAsync(DateTime referenceTime, bool withBreakdown, CancellationToken cancellationToken)
        {
            // Next 
            var query = await _subscriptionRepository.WithDetailsAsync();

            var entries = query
                .Where(a => a.EffectiveOn < referenceTime && (!a.TerminatedOn.HasValue || a.TerminatedOn.Value > referenceTime))
                .ToList();

            var ret = entries.Select(elem =>
            {
                var y = ObjectMapper.Map<TenantSubscription, SubscriptionPlanOutputDto>(elem);
                // Plan detail
                y.Name = elem.Plan.Name;
                y.Family = elem.Plan.Family;
                y.Description = elem.Plan.Description;
                y.BillingCycleId = elem.Plan.BillingCycleId;

                // Inferred properties
                var u = ComputeBillingDateRange(elem.BillingCycleOn, elem.Plan.BillingCycle, referenceTime);
                y.CurrentBillingStartDate = u.Item1;
                y.CurrentBillingEndDate = u.Item2;


                if (withBreakdown)
                {
                    // Breakdows 
                    y.Breakdowns = elem.Plan.Breakdowns.Select(l =>
                    {
                        var m = ObjectMapper.Map<PlanBreakdown, PlanBreakdownOutputDto>(l);
                        return m;
                    }).ToList();

                    y.CategoryQuotas = elem.Plan.CategoryQuotas.Select(l =>
                    {
                        var m = ObjectMapper.Map<PlanCategoryQuota, PlanCategoryQuotaOutputDto>(l);
                        return m;
                    }).ToList();
                }

                return y;

            }).ToList();

            if (withBreakdown)
            {
                var resourceIds = new List<Guid>();
                foreach (var x in ret)
                {
                    foreach (var y in x.Breakdowns)
                    {
                        resourceIds.Add(y.ResourceId);
                    }
                }
                resourceIds = resourceIds.Distinct().ToList();
                var resources = await _resourceRepository.GetListAsync(a => resourceIds.Contains(a.Id), cancellationToken: cancellationToken);
                foreach (var x in ret)
                {
                    foreach (var y in x.Breakdowns)
                    {
                        var z = resources.FirstOrDefault(w => w.Id == y.ResourceId);
                        if (z != null)
                        {
                            y.Name = z.Name;
                            y.Description = z.Description;
                            y.Category = z.Category;
                        }
                    }
                }
            }

            return ret;
        }

        public async Task CancelSubscriptionsAsync(DateTime cancelledOn, CancellationToken cancellationToken)
        {
            // Find out the current subscriptions 
            var entries = await _subscriptionRepository.GetListAsync();
            await _subscriptionRepository.DeleteManyAsync(entries, cancellationToken: cancellationToken);
        }

        public async Task UpdateSubscriptionsAsync(List<SubscriptionPlanInputDto> input, CancellationToken cancellationToken)
        {
            // Find out the current subscriptions 
            var entries = await _subscriptionRepository.GetListAsync();

            // Create new subscriptions 
            foreach (var newPlan in input)
            {
                var newSub = ObjectMapper.Map<SubscriptionPlanInputDto, TenantSubscription>(newPlan);
                await _subscriptionRepository.InsertAsync(newSub, cancellationToken: cancellationToken);
            }

            // Delete old one          
            await _subscriptionRepository.DeleteManyAsync(entries, cancellationToken: cancellationToken);
        }

        public async Task<long> GetQuotaByResourceNameAsync(string resourceName, bool isTenantLevel, CancellationToken cancellationToken)
        {
            // Require the resource id 
            var resourceEntry = await _resourceRepository.GetAsync(a => a.Name == resourceName, cancellationToken: cancellationToken);

            // Next 
            var query = await _subscriptionRepository.WithDetailsAsync();

            var now = DateTime.UtcNow;
            var entries = query
                .Where(a => a.EffectiveOn < now && (!a.TerminatedOn.HasValue || a.TerminatedOn.Value > now) && a.Plan.Breakdowns.Any(b => b.ResourceId == resourceEntry.Id))
                .ToList();

            if (entries.Count > 0)
            {
                if (isTenantLevel)
                {
                    // Aggreategate 
                    var outerCount = entries.Aggregate(0L, (sum, elem) =>
                    {
                        // Resources 
                        var details = elem.Plan.Breakdowns.Where(c => c.ResourceId == resourceEntry.Id);
                        var innerCount = details.Aggregate(0L, (s2, e2) =>
                        {
                            return s2 + e2.LimitAcrossTenant;
                        });

                        return sum + innerCount * elem.Quantity;
                    });

                    return outerCount;
                }
                else
                {
                    // Aggreategate 
                    var outerCount = entries.Aggregate(0L, (sum, elem) =>
                    {
                        // Resources 
                        var details = elem.Plan.Breakdowns.Where(c => c.ResourceId == resourceEntry.Id);
                        var innerCount = details.Aggregate(0L, (s2, e2) =>
                        {
                            return s2 + e2.LimitPerUser;
                        });

                        return sum + innerCount;
                    });

                    return outerCount;
                }
            }

            // Read the configuration 
            var anyCfg = _options.FreeUsageLimit.FirstOrDefault(x => x.ResourceName == resourceName);
            if (anyCfg != null)
            {
                return isTenantLevel ? anyCfg.LimitAcrossTenant : anyCfg.LimitPerUser;
            }

            return 0;
        }

        public async Task<long> GetQuotaByCategoryNameAsync(string categoryName, bool isTenantLevel, CancellationToken cancellationToken)
        {
            // Next 
            var query = await _subscriptionRepository.WithDetailsAsync();

            var now = DateTime.UtcNow;
            var entries = query
                .Where(a => a.EffectiveOn < now && (!a.TerminatedOn.HasValue || a.TerminatedOn.Value > now) && a.Plan.CategoryQuotas.Any(b => b.Category == categoryName))
                .ToList();

            if (entries.Count > 0)
            {
                if (isTenantLevel)
                {
                    // Aggreategate 
                    var outerCount = entries.Aggregate(0L, (sum, elem) =>
                    {
                        // Resources 
                        var details = elem.Plan.CategoryQuotas.Where(c => c.Category == categoryName);
                        var innerCount = details.Aggregate(0L, (s2, e2) =>
                        {
                            return s2 + e2.LimitAcrossTenant;
                        });

                        return sum + innerCount * elem.Quantity;
                    });

                    return outerCount;
                }
                else
                {
                    // Aggreategate 
                    var outerCount = entries.Aggregate(0L, (sum, elem) =>
                    {
                        // Resources 
                        var details = elem.Plan.CategoryQuotas.Where(c => c.Category == categoryName);
                        var innerCount = details.Aggregate(0L, (s2, e2) =>
                        {
                            return s2 + e2.LimitPerUser;
                        });

                        return sum + innerCount;
                    });

                    return outerCount;
                }
            }

            return 0;
        }

        public async Task<Tuple<DateTime, DateTime?>> GetCurrentBillingPeriodAsync(string resourceName, CancellationToken cancellationToken)
        {
            var resourceEntry = await _resourceRepository.GetAsync(a => a.Name == resourceName, cancellationToken: cancellationToken);

            // Next 
            var query = await _subscriptionRepository.WithDetailsAsync();
            var now = DateTime.UtcNow;
            var entry = query
                .Where(a => a.EffectiveOn < now && (!a.TerminatedOn.HasValue || a.TerminatedOn.Value > now) && a.Plan.Breakdowns.Any(b => b.ResourceId == resourceEntry.Id))
                .FirstOrDefault();

            if (entry != null)
            {
                return ComputeBillingDateRange(entry.BillingCycleOn, entry.Plan.BillingCycle, now);
            }

            // Fall back to the configured monthly plan.
            // Get the calendar month 
            var startingMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = startingMonth.AddMonths(1);

            return Tuple.Create<DateTime, DateTime?>(startingMonth, endMonth);
        }

        private Tuple<DateTime, DateTime?> ComputeBillingDateRange(DateTime cycleOn, BillingCycleEnum cycle, DateTime referenceTime)
        {
            var maxLoop = 0;
            // a < x < b
            var rangeStart = cycleOn;
            while (maxLoop < 365 * 100)
            {
                var rangeEnd = GetRangeEndDate(cycle, rangeStart);
                // Case 1:
                if (rangeEnd == null)
                {
                    return Tuple.Create<DateTime, DateTime?>(rangeStart, null);
                }
                // Case 2:
                if (referenceTime >= rangeStart && referenceTime < rangeEnd)
                {
                    return Tuple.Create(rangeStart, rangeEnd);
                }

                rangeStart = rangeEnd!.Value;
                maxLoop++;
            }

            throw new Exception("Wrong cycle");
        }


        private DateTime? GetRangeEndDate(BillingCycleEnum cycle, DateTime startDate)
        {
            switch (cycle)
            {
                case BillingCycleEnum.Day:
                    return startDate.AddDays(1);
                case BillingCycleEnum.Month:
                    return startDate.AddMonths(1);
                case BillingCycleEnum.Year:
                    return startDate.AddYears(1);
                case BillingCycleEnum.Forever:
                    return null;
            }

            return null;
        }
    }
}
