using System;
namespace PolpAbp.MultiTenancy.Shared
{
    public enum TenantPictureRoleEnum
    {
        UnClassified = 0,
        // Serve as the light theme logo
        PrimayLogo = 1001,
        // Serve as the mini logo in the light theme
        SecondaryLogo = 1002,

        DarkLogo = 1010,
        DarkMiniLogo = 1011
    }
}

