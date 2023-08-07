using MagicOnion;
using MagicT.Server.Jwt;
using MagicT.Server.Services.Base;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class TokenService : MagicBase<ITokenService, byte[]>, ITokenService
{
    // public FastJwtTokenService FastJwtTokenService { get; set; }

    //public Lazy<List<PERMISSIONS>> Permissions { get; set; }
    public TokenService(IServiceProvider provider) : base(provider)
    {
        MagicTTokenService = provider.GetService<MagicTTokenService>();

        //Permissions = provider.GetService<Lazy<List<PERMISSIONS>>>();
    }


    public UnaryResult<byte[]> Request(int id, string password)
    {
        //Permissions.Value.Add(new PERMISSIONS { PER_ROWID = 1 });
        return new UnaryResult<byte[]>(MagicTTokenService.CreateToken(id,1));
    }
}
