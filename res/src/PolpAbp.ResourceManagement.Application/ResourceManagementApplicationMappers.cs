using System;
using System.Reflection;
using PolpAbp.ResourceManagement.Domain.Entities;
using PolpAbp.ResourceManagement.Services.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace PolpAbp.ResourceManagement;

/* Mapperly mappers (formerly ResourceManagementApplicationAutoMapperProfile).
 * One [Mapper] class per type pair; ABP registers each one conventionally
 * and IObjectMapper.Map<,> resolves it.
 * Former IgnoreSourceMissingProperties() = RequiredMappingStrategy.Target plus an
 * explicit [MapperIgnoreTarget] for every settable target member with no source. */

/* TenantSubscription.IsTerminated is [Obsolete(error: true)], so generated code cannot
 * touch it (CS0619). AutoMapper copied it by reflection in both directions; the two
 * TenantSubscription mappers ignore it in the generated code and copy it in AfterMap,
 * which ABP's object mapper runs after every Map (new and in-place). */
internal static class TenantSubscriptionIsTerminated
{
    public const string Name = "IsTerminated";

    private static readonly PropertyInfo Property =
        typeof(TenantSubscription).GetProperty(Name, BindingFlags.Public | BindingFlags.Instance)!;

    public static bool Get(TenantSubscription entity) => (bool)Property.GetValue(entity)!;

    public static void Set(TenantSubscription entity, bool value) => Property.SetValue(entity, value);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantSubscriptionToSubscriptionPlanOutputDtoMapper : MapperBase<TenantSubscription, SubscriptionPlanOutputDto>
{
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Name))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Family))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Description))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.BillingCycleId))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Breakdowns))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.CategoryQuotas))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.CurrentBillingStartDate))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.CurrentBillingEndDate))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.IsTerminated))]
    [MapperIgnoreSource(TenantSubscriptionIsTerminated.Name)]
    public override partial SubscriptionPlanOutputDto Map(TenantSubscription source);

    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Name))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Family))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Description))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.BillingCycleId))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.Breakdowns))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.CategoryQuotas))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.CurrentBillingStartDate))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.CurrentBillingEndDate))]
    [MapperIgnoreTarget(nameof(SubscriptionPlanOutputDto.IsTerminated))]
    [MapperIgnoreSource(TenantSubscriptionIsTerminated.Name)]
    public override partial void Map(TenantSubscription source, SubscriptionPlanOutputDto destination);

    public override void AfterMap(TenantSubscription source, SubscriptionPlanOutputDto destination)
    {
        destination.IsTerminated = TenantSubscriptionIsTerminated.Get(source);
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SubscriptionPlanInputDtoToTenantSubscriptionMapper : MapperBase<SubscriptionPlanInputDto, TenantSubscription>
{
    [MapperIgnoreTarget(nameof(TenantSubscription.TenantId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.Plan))]
    [MapperIgnoreTarget(nameof(TenantSubscription.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(TenantSubscription.ExtraProperties))]
    [MapperIgnoreTarget(nameof(TenantSubscription.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantSubscription.CreatorId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantSubscription.LastModifierId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantSubscription.DeleterId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.DeletionTime))]
    [MapperIgnoreTarget(TenantSubscriptionIsTerminated.Name)]
    [MapperIgnoreSource(nameof(SubscriptionPlanInputDto.IsTerminated))]
    public override partial TenantSubscription Map(SubscriptionPlanInputDto source);

    [MapperIgnoreTarget(nameof(TenantSubscription.TenantId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.Plan))]
    [MapperIgnoreTarget(nameof(TenantSubscription.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(TenantSubscription.ExtraProperties))]
    [MapperIgnoreTarget(nameof(TenantSubscription.CreationTime))]
    [MapperIgnoreTarget(nameof(TenantSubscription.CreatorId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.LastModificationTime))]
    [MapperIgnoreTarget(nameof(TenantSubscription.LastModifierId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.IsDeleted))]
    [MapperIgnoreTarget(nameof(TenantSubscription.DeleterId))]
    [MapperIgnoreTarget(nameof(TenantSubscription.DeletionTime))]
    [MapperIgnoreTarget(TenantSubscriptionIsTerminated.Name)]
    [MapperIgnoreSource(nameof(SubscriptionPlanInputDto.IsTerminated))]
    public override partial void Map(SubscriptionPlanInputDto source, TenantSubscription destination);

    // TenantSubscription's parameterless constructor is protected (AutoMapper used it
    // through reflection); the public one with an empty id builds the same object.
    public override void AfterMap(SubscriptionPlanInputDto source, TenantSubscription destination)
    {
        TenantSubscriptionIsTerminated.Set(destination, source.IsTerminated);
    }

    [ObjectFactory]
    private TenantSubscription CreateTenantSubscription() => new TenantSubscription(Guid.Empty);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PlanBreakdownToPlanBreakdownOutputDtoMapper : MapperBase<PlanBreakdown, PlanBreakdownOutputDto>
{
    [MapperIgnoreTarget(nameof(PlanBreakdownOutputDto.Name))]
    [MapperIgnoreTarget(nameof(PlanBreakdownOutputDto.Description))]
    [MapperIgnoreTarget(nameof(PlanBreakdownOutputDto.Category))]
    public override partial PlanBreakdownOutputDto Map(PlanBreakdown source);

    [MapperIgnoreTarget(nameof(PlanBreakdownOutputDto.Name))]
    [MapperIgnoreTarget(nameof(PlanBreakdownOutputDto.Description))]
    [MapperIgnoreTarget(nameof(PlanBreakdownOutputDto.Category))]
    public override partial void Map(PlanBreakdown source, PlanBreakdownOutputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PlanCategoryQuotaToPlanCategoryQuotaOutputDtoMapper : MapperBase<PlanCategoryQuota, PlanCategoryQuotaOutputDto>
{
    public override partial PlanCategoryQuotaOutputDto Map(PlanCategoryQuota source);

    public override partial void Map(PlanCategoryQuota source, PlanCategoryQuotaOutputDto destination);
}
