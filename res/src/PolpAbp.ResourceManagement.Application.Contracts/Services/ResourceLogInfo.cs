using System;

namespace PolpAbp.ResourceManagement.Services
{
    public class ResourceLogInfo
    {
        public string ResourceName { get; set; }

        public Guid? UserId { get; set; }
        public Guid TenantId { get; set; }

        public string Intension { get; set; }
        public string Destination { get; set; }
        public bool IsExempt { get; set; }
        public string ExemptionReason { get; set; }

        public int Usage { get; set; }

        public DateTime HappenedOn { get; set; }
    }
}
