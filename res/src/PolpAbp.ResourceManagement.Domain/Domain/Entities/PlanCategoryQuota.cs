using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace PolpAbp.ResourceManagement.Domain.Entities
{
    public class PlanCategoryQuota : AuditedEntity<Guid>
    {
        public Guid PlanId { get; set; }

        public string Category { get; set; }

        public long LimitAcrossTenant { get; set; }
        public long LimitPerUser { get; set; }

        protected PlanCategoryQuota() : base() { }

        public PlanCategoryQuota(Guid id) : base(id) { }

    }
}
