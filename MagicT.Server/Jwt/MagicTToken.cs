namespace MagicT.Server.Jwt;

/// <summary>
/// Represents a MagicT authentication token containing user information and roles.
/// </summary>
public sealed class MagicTToken
{
    /// <summary>
    /// Gets or sets the user ID associated with the token.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets an array of role IDs associated with the token.
    /// </summary>
    public int[] Roles { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTToken"/> class.
    /// </summary>
    /// <param name="userId">The user ID associated with the token.</param>
    /// <param name="roles">An array of role IDs associated with the token.</param>
    public MagicTToken(int userId, params int[] roles)
    {
        UserId = userId;
        Roles = roles;
    }
}
