using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolpAbp.ResourceManagement.Domain.Entities;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace PolpAbp.ResourceManagement.EntityFrameworkCore;

public static class ResourceManagementDbContextModelCreatingExtensions
{
    public static void ConfigureResourceManagement(
        this ModelBuilder builder,
        Action<EntityTypeBuilder<ResourceUsageLog>> usageLogBuilder = null)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Resource>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.Resource,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            //Properties
            b.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(ResourceManagementDomainConsts.MaxResourceNameLength);

            b.Property(q => q.Description)
            .HasMaxLength(ResourceManagementDomainConsts.MaxResourceDescLength);

            b.Property(q => q.Category)
            .HasMaxLength(ResourceManagementDomainConsts.MaxResourceCategoryLength);

            // Indices
            b.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<ResourceUsageLog>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.ResourceUsageLog,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            //Properties
            b.HasOne(x => x.Resource).WithMany()
            .HasForeignKey(p => p.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.Intension).HasMaxLength(ResourceManagementDomainConsts.MaxGenenalStringLength);
            b.Property(x => x.ExemptionReason).HasMaxLength(ResourceManagementDomainConsts.MaxGenenalStringLength);
            b.Property(x => x.Destination).HasMaxLength(ResourceManagementDomainConsts.MaxGenenalStringLength);

            if (usageLogBuilder != null)
            {
                usageLogBuilder(b);
            }
        });

        builder.Entity<ResourceMonthlyUsage>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.ResourceMonthlyUsage,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            //Properties
            b.HasOne(x => x.Resource).WithMany()
            .HasForeignKey(p => p.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);
            // Indices
            b.HasIndex(x => new { x.TenantId, x.ResourceId, x.Year, x.Month })
            .IsUnique();
        });

        builder.Entity<ResourceYearlyUsage>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.ResourceYearlyUsage,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            //Properties
            b.HasOne(x => x.Resource).WithMany()
            .HasForeignKey(p => p.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);

            // Indices
            b.HasIndex(x => new { x.TenantId, x.ResourceId, x.Year })
            .IsUnique();
        });

        builder.Entity<Plan>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.Plan,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();
            //Properties
            b.Property(q => q.Name)
                       .IsRequired()
                       .HasMaxLength(ResourceManagementDomainConsts.MaxPlanNameLength);

            b.Property(q => q.Family)
            .IsRequired()
            .HasMaxLength(ResourceManagementDomainConsts.MaxPlanFamilyLength);

            b.Property(q => q.Description)
            .HasMaxLength(ResourceManagementDomainConsts.MaxPlanDescLength);

            // Indices 
            b.HasIndex(x => new { x.Name, x.Family }).IsUnique();
        });

        builder.Entity<PlanBreakdown>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.PlanBreakdown,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();

            // Navigators
            b.HasOne<Resource>()
            .WithMany().HasForeignKey(p => p.ResourceId);

            // Plan has been configured above.
            b.HasOne<Plan>()
            .WithMany(y => y.Breakdowns)
            .HasForeignKey(p => p.PlanId);
        });

        builder.Entity<PlanCategoryQuota>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.PLanCategoryQuota,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(q => q.Category)
          .HasMaxLength(ResourceManagementDomainConsts.MaxResourceCategoryLength);

            // Plan has been configured above.
            b.HasOne<Plan>()
            .WithMany(y => y.CategoryQuotas)
            .HasForeignKey(p => p.PlanId);
        });

        builder.Entity<TenantSubscription>(b =>
        {
            //Configure table & schema name
            b.ToTable(ResourceManagementDbProperties.DbTablePrefix
                + ResourceManagementDbProperties.TableNames.TenantSubscription,
                ResourceManagementDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(q => q.ExternalId)
            .HasMaxLength(ResourceManagementDomainConsts.MaxGenenalStringLength);

            // Navigators
            b.HasOne(x => x.Plan)
            .WithMany()
            .HasForeignKey(p => p.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
        });

    }
}
