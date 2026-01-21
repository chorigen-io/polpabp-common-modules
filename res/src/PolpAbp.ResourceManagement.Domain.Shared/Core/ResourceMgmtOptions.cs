using System;
using System.Collections.Generic;

namespace PolpAbp.ResourceManagement.Core
{
    public class ResourceMgmtOptions
    {
        public bool IsEnabled { get; set; }

        public int Threshold { get; set; }

        public Dictionary<string, Type> Contributors { get; }

        public List<ResourceUsageLimit> FreeUsageLimit { get; set; }

        public ResourceMgmtOptions()
        {
            IsEnabled = true;
            Contributors = new Dictionary<string, Type>();
            FreeUsageLimit = new List<ResourceUsageLimit>();
        }
    }
}
