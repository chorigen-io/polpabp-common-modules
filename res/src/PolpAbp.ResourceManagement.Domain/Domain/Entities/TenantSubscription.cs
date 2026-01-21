using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace PolpAbp.ResourceManagement.Domain.Entities
{
    public class TenantSubscription : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid PlanId { get; set; }

        public int Quantity { get; set; }

        public DateTime EffectiveOn { get; set; }
        [Obsolete("Replace the use of it with the use of TerminatedOn", true)]
        public bool IsTerminated { get; set; }
        public DateTime? TerminatedOn { get; set; }

        public DateTime BillingCycleOn { get; set; }

        // Can be used for the external identifier Id.
        public string ExternalId { get; set; }

        // Navigation
        public virtual Plan Plan { get; set; }

        protected TenantSubscription() : base() { }

        public TenantSubscription(Guid id) : base(id) { }
    }
}
