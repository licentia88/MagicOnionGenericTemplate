namespace MagicT.Server.Jwt;

public class MagicTToken
{

    public int UserId { get; set; }

    public int[] Roles { get; set; }

    public MagicTToken(int userId, params int[] roles)
    {
        UserId = userId;
        Roles = roles;
    }


}

