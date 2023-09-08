namespace MagicT.Server.Jwt;


/// <summary>
/// Represents a MagicT authentication token containing user information and roles.
/// </summary>
public sealed class MagicTToken
{
    /// <summary>
    /// Gets or sets the contact identifier associated with the token, which can be an email or phone number.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets an array of role IDs associated with the token.
    /// </summary>
    public int[] Roles { get; set; }


    public string Identifier { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTToken"/> class.
    /// </summary>
    /// <param name="contactIdentifier">The contact identifier associated with the token, which can be an email or phone number.</param>
    /// <param name="roles">An array of role IDs associated with the token.</param>
    public MagicTToken(int id, params int[] roles)
    {
        Id = id;
        //Identifier = identifier;
        Roles = roles;
    }

    public MagicTToken(int id,string identifier, params int[] roles)
    {
        Id = id;
        Identifier = identifier;
        Roles = roles;
    }
}

