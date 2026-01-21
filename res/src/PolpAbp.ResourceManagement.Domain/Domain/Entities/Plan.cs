using PolpAbp.ResourceManagement.Core;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace PolpAbp.ResourceManagement.Domain.Entities
{
    public class Plan : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string Description { get; set; }

        public int BillingCycleId { get; set; }

        public BillingCycleEnum BillingCycle => (BillingCycleEnum)BillingCycleId;

        public virtual List<PlanBreakdown> Breakdowns { get; set; }
        public virtual List<PlanCategoryQuota> CategoryQuotas { get; set; }

        protected Plan() : base()
        {
            Breakdowns = new List<PlanBreakdown>();
            CategoryQuotas = new List<PlanCategoryQuota>();
        }

        public Plan(Guid id) : base(id)
        {
            Breakdowns = new List<PlanBreakdown>();
            CategoryQuotas = new List<PlanCategoryQuota>();
        }
    }
}
