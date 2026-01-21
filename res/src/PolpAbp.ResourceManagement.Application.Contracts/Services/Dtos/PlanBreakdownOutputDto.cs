using System;

namespace PolpAbp.ResourceManagement.Services.Dtos
{
    public class PlanBreakdownOutputDto
    {
        public Guid ResourceId { get; set; }
        // From resource
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public long LimitAcrossTenant { get; set; }
        public long LimitPerUser { get; set; }
    }
}
