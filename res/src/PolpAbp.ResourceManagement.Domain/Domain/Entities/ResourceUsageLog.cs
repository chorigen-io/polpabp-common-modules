using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace PolpAbp.ResourceManagement.Domain.Entities
{
    public class ResourceUsageLog : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid ResourceId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? UserId { get; set; }
        public string Intension { get; set; }
        public string Destination { get; set; }
        public bool IsExempt { get; set; }
        public string ExemptionReason { get; set; }
        public int Usage { get; set; }

        // Resource navigation
        public virtual Resource Resource { get; set; }

        protected ResourceUsageLog() : base() { }

        public ResourceUsageLog(Guid id) : base(id) { }
    }
}
