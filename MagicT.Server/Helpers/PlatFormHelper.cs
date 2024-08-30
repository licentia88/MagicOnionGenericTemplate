namespace MagicT.Server.Helpers;

public static class PlatFormHelper
{
    public static bool IsWindows() => global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.Windows);

    public static bool IsMacOS() => global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.OSX);

    public static bool IsLinux() => global::System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(global::System.Runtime.InteropServices.OSPlatform.Linux);
}


 