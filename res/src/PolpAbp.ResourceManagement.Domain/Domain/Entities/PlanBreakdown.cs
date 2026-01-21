using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace PolpAbp.ResourceManagement.Domain.Entities
{
    public class PlanBreakdown : AuditedEntity<Guid>
    {
        public Guid PlanId { get; set; }

        public Guid ResourceId { get; set; }

        public long LimitAcrossTenant { get; set; }
        public long LimitPerUser { get; set; }

        protected PlanBreakdown() : base() { }

        public PlanBreakdown(Guid id) : base(id) { }

    }
}
