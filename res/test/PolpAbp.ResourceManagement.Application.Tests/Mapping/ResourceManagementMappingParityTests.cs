using System;
using PolpAbp.ResourceManagement.Domain.Entities;
using PolpAbp.ResourceManagement.Services.Dtos;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace PolpAbp.ResourceManagement.Mapping
{
    /// <summary>
    /// Mapping-parity tests (T026): one fact per pair of the former
    /// ResourceManagementApplicationAutoMapperProfile. Each fact encodes the
    /// AutoMapper semantics as explicit expected values: ignored members and members
    /// missing from the source stay default on a new object. AutoMapper copied the
    /// [Obsolete(error: true)] TenantSubscription.IsTerminated by reflection in both
    /// directions, so the tests read and write it by reflection too.
    /// </summary>
    public class ResourceManagementMappingParityTests : ResourceManagementApplicationTestBase
    {
        private readonly IObjectMapper _objectMapper;

        public ResourceManagementMappingParityTests()
        {
            _objectMapper = GetRequiredService<IObjectMapper<ResourceManagementApplicationModule>>();
        }

        // CreateMap<TenantSubscription, SubscriptionPlanOutputDto>()
        //   Ignore: Name, Family, Description, BillingCycleId, Breakdowns, CategoryQuotas,
        //   CurrentBillingStartDate, CurrentBillingEndDate; IgnoreSourceMissingProperties
        [Fact]
        public void TenantSubscription_To_SubscriptionPlanOutputDto()
        {
            var planId = Guid.NewGuid();
            var effectiveOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var terminatedOn = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            var billingCycleOn = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var subscription = new TenantSubscription(Guid.NewGuid())
            {
                TenantId = Guid.NewGuid(),
                PlanId = planId,
                Quantity = 5,
                EffectiveOn = effectiveOn,
                TerminatedOn = terminatedOn,
                BillingCycleOn = billingCycleOn,
                ExternalId = "ext-1",
                Plan = new Plan(Guid.NewGuid()) { Name = "Plan name" }
            };
            SetIsTerminated(subscription, true);

            var dto = _objectMapper.Map<TenantSubscription, SubscriptionPlanOutputDto>(subscription);

            dto.EffectiveOn.ShouldBe(effectiveOn);
            dto.IsTerminated.ShouldBeTrue();
            dto.TerminatedOn.ShouldBe(terminatedOn);
            dto.BillingCycleOn.ShouldBe(billingCycleOn);
            dto.Quantity.ShouldBe(5);
            dto.PlanId.ShouldBe(planId);
            dto.ExternalId.ShouldBe("ext-1");
            // Ignored: defaults of a new SubscriptionPlanOutputDto
            dto.Name.ShouldBeNull();
            dto.Family.ShouldBeNull();
            dto.Description.ShouldBeNull();
            dto.BillingCycleId.ShouldBe(0);
            dto.Breakdowns.ShouldNotBeNull();
            dto.Breakdowns.ShouldBeEmpty();
            dto.CategoryQuotas.ShouldNotBeNull();
            dto.CategoryQuotas.ShouldBeEmpty();
            dto.CurrentBillingStartDate.ShouldBe(default);
            dto.CurrentBillingEndDate.ShouldBeNull();
        }

        // CreateMap<SubscriptionPlanInputDto, TenantSubscription>().IgnoreSourceMissingProperties()
        [Fact]
        public void SubscriptionPlanInputDto_To_TenantSubscription()
        {
            var planId = Guid.NewGuid();
            var input = new SubscriptionPlanInputDto
            {
                EffectiveOn = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                IsTerminated = true,
                TerminatedOn = new DateTime(2026, 11, 30, 0, 0, 0, DateTimeKind.Utc),
                BillingCycleOn = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                Quantity = 3,
                PlanId = planId,
                ExternalId = "ext-2"
            };

            var subscription = _objectMapper.Map<SubscriptionPlanInputDto, TenantSubscription>(input);

            subscription.EffectiveOn.ShouldBe(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            GetIsTerminated(subscription).ShouldBeTrue();
            subscription.TerminatedOn.ShouldBe(new DateTime(2026, 11, 30, 0, 0, 0, DateTimeKind.Utc));
            subscription.BillingCycleOn.ShouldBe(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
            subscription.Quantity.ShouldBe(3);
            subscription.PlanId.ShouldBe(planId);
            subscription.ExternalId.ShouldBe("ext-2");
            // Not in the source: defaults of a new TenantSubscription
            subscription.Id.ShouldBe(Guid.Empty);
            subscription.TenantId.ShouldBeNull();
            subscription.Plan.ShouldBeNull();
            subscription.CreationTime.ShouldBe(default);
            subscription.IsDeleted.ShouldBeFalse();
            subscription.ExtraProperties.ShouldBeEmpty();
        }

        // Same pair, in-place form: members missing from the source keep their values.
        [Fact]
        public void SubscriptionPlanInputDto_To_Existing_TenantSubscription_Preserves_Unmapped()
        {
            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var subscription = new TenantSubscription(id)
            {
                TenantId = tenantId,
                Quantity = 1,
                ConcurrencyStamp = "stamp-1"
            };
            subscription.SetProperty("Keep", "kept");
            SetIsTerminated(subscription, true);
            var input = new SubscriptionPlanInputDto { Quantity = 8, IsTerminated = false };

            _objectMapper.Map(input, subscription);

            subscription.Quantity.ShouldBe(8);
            GetIsTerminated(subscription).ShouldBeFalse();
            subscription.Id.ShouldBe(id);
            subscription.TenantId.ShouldBe(tenantId);
            subscription.ConcurrencyStamp.ShouldBe("stamp-1");
            subscription.ExtraProperties.Count.ShouldBe(1);
            subscription.ExtraProperties["Keep"].ShouldBe("kept");
        }

        // CreateMap<PlanBreakdown, PlanBreakdownOutputDto>()
        //   Ignore: Name, Description, Category
        [Fact]
        public void PlanBreakdown_To_PlanBreakdownOutputDto()
        {
            var resourceId = Guid.NewGuid();
            var breakdown = new PlanBreakdown(Guid.NewGuid())
            {
                PlanId = Guid.NewGuid(),
                ResourceId = resourceId,
                LimitAcrossTenant = 1000,
                LimitPerUser = 10
            };

            var dto = _objectMapper.Map<PlanBreakdown, PlanBreakdownOutputDto>(breakdown);

            dto.ResourceId.ShouldBe(resourceId);
            dto.LimitAcrossTenant.ShouldBe(1000);
            dto.LimitPerUser.ShouldBe(10);
            dto.Name.ShouldBeNull();
            dto.Description.ShouldBeNull();
            dto.Category.ShouldBeNull();
        }

        // CreateMap<PlanCategoryQuota, PlanCategoryQuotaOutputDto>() -- by-name copy.
        [Fact]
        public void PlanCategoryQuota_To_PlanCategoryQuotaOutputDto()
        {
            var quota = new PlanCategoryQuota(Guid.NewGuid())
            {
                PlanId = Guid.NewGuid(),
                Category = "sms",
                LimitAcrossTenant = 500,
                LimitPerUser = 5
            };

            var dto = _objectMapper.Map<PlanCategoryQuota, PlanCategoryQuotaOutputDto>(quota);

            dto.Category.ShouldBe("sms");
            dto.LimitAcrossTenant.ShouldBe(500);
            dto.LimitPerUser.ShouldBe(5);
        }

        private static bool GetIsTerminated(TenantSubscription subscription)
        {
            return (bool)typeof(TenantSubscription).GetProperty("IsTerminated").GetValue(subscription);
        }

        private static void SetIsTerminated(TenantSubscription subscription, bool value)
        {
            typeof(TenantSubscription).GetProperty("IsTerminated").SetValue(subscription, value);
        }
    }
}
