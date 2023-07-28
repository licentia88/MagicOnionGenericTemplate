using Magic.Server.Jwt;
using Magic.Server.Services.Base;
using Magic.Shared.Services;
using MagicOnion;

namespace Magic.Server.Services;

public class TokenService : MagicBase<ITokenService, byte[]>, ITokenService
{
    public FastJwtTokenService FastJwtTokenService { get; set; }

    //public Lazy<List<PERMISSIONS>> Permissions { get; set; }
    public TokenService(IServiceProvider provider) : base(provider)
    {
        FastJwtTokenService = provider.GetService<FastJwtTokenService>();

        //Permissions = provider.GetService<Lazy<List<PERMISSIONS>>>();
    }


    public UnaryResult<byte[]> Request(int id, string password)
    {
        //Permissions.Value.Add(new PERMISSIONS { PER_ROWID = 1 });
        return new UnaryResult<byte[]>(FastJwtTokenService.CreateToken(id, 1));
    }
}

