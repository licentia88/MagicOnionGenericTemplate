using System.Runtime.InteropServices;

namespace MagicT.Server.Helpers;

/// <summary>
/// Provides helper methods for determining the current operating system platform.
/// </summary>
public static class PlatFormHelper
{
    /// <summary>
    /// Determines if the current operating system is Windows.
    /// </summary>
    /// <returns>\c true if the current operating system is Windows; otherwise, \c false.</returns>
    public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Determines if the current operating system is macOS.
    /// </summary>
    /// <returns>\c true if the current operating system is macOS; otherwise, \c false.</returns>
    public static bool IsMacOs() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    /// <summary>
    /// Determines if the current operating system is Linux.
    /// </summary>
    /// <returns>\c true if the current operating system is Linux; otherwise, \c false.</returns>
    public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}
 