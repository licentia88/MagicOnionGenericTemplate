using System.Runtime.InteropServices;

namespace MagicT.Shared.Types;

/// <summary>
/// A fixed-size string structure with a maximum length of 32 characters.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FixedString32
{
    private const int MaxLength = 32;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxLength)]
    private char[] _chars;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedString32"/> struct.
    /// </summary>
    /// <param name="value">The string value to initialize with.</param>
    public FixedString32(string value)
    {
        _chars = new char[MaxLength];
        if (value == null) return;
        int length = Math.Min(value.Length, MaxLength);
        value.CopyTo(0, _chars, 0, length);
    }

    /// <summary>
    /// Returns the string representation of the fixed-size string.
    /// </summary>
    /// <returns>A string that represents the fixed-size string.</returns>
    public override string ToString()
    {
        return new string(_chars).TrimEnd('\0');
    }
}

// /// <summary>
// /// A structure representing odds with associated sports information.
// /// </summary>
// public struct Odds2
// {
//     public FixedString32 OddsId;
//     public FixedString32 SportsKey;
//     public FixedString32 SportsTitle;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="Odds2"/> struct.
//     /// </summary>
//     /// <param name="oddsId">The odds identifier.</param>
//     /// <param name="sportsKey">The sports key.</param>
//     /// <param name="sportsTitle">The sports title.</param>
//     public Odds2(string oddsId, string sportsKey, string sportsTitle)
//     {
//         OddsId = new FixedString32(oddsId);
//         SportsKey = new FixedString32(sportsKey);
//         SportsTitle = new FixedString32(sportsTitle);
//     }
// }
//
// /// <summary>
// /// A multi-dimensional array structure for <see cref="Odds2"/>.
// /// </summary>
// [MultiArray(typeof(Odds2))]
// [MultiArrayList]
// public readonly partial struct Odds2MultiArray
// {
// }