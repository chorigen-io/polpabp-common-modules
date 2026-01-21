using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace PolpAbp.ResourceManagement.Domain.Entities
{
    public class ResourceMonthlyUsage : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid ResourceId { get; set; }
        public Guid? TenantId { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public long Usage { get; set; }

        // Resource navigation
        public virtual Resource Resource { get; set; }

        protected ResourceMonthlyUsage() : base() { }

        public ResourceMonthlyUsage(Guid id) : base(id) { }
    }
}
