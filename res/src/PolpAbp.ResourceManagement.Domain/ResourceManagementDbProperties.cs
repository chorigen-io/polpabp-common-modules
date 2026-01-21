namespace PolpAbp.ResourceManagement;

public static class ResourceManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = "PolpAbpResMgmt";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "PolpAbpResourceManagement";

    public static class TableNames
    {
        public const string Resource = "Resources";
        public const string ResourceUsageLog = "ResourceUsageLogs";
        public const string ResourceMonthlyUsage = "ResourceMonthlyUsages";
        public const string ResourceYearlyUsage = "ResourceYearlyUsages";

        public const string Plan = "Plans";
        public const string PlanBreakdown = "PlanBreakdowns";
        public const string PLanCategoryQuota = "PlanCategoryQuotas";
        public const string TenantSubscription = "TenantSubscriptions";
    }
}
