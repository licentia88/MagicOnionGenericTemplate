using Benutomo;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Jwt;

namespace MagicT.Server.Managers;

/// <summary>
/// Manages token operations including processing JWT tokens.
/// </summary>
[AutomaticDisposeImpl]
public partial class TokenManager : IDisposable, IAsyncDisposable
{
    [EnableAutomaticDispose]
    private MagicTTokenService MagicTTokenService { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public TokenManager(IServiceProvider provider)
    {
        MagicTTokenService = provider.GetService<MagicTTokenService>();
    }

    /// <summary>
    /// Processes the JWT token from the request headers.
    /// </summary>
    /// <param name="token">The JWT token as a byte array.</param>
    /// <returns>The decoded MagicT token.</returns>
    /// <exception cref="ReturnStatusException">Thrown when the token is null or not found.</exception>
    public MagicTToken Process(byte[] token)
    {
        if (token is null)
        {
            throw new ReturnStatusException(StatusCode.NotFound, "Security Token not found");
        }

        return DecodeToken(token);
    }

    /// <summary>
    /// Decodes the JWT token.
    /// </summary>
    /// <param name="token">The JWT token as a byte array.</param>
    /// <returns>The decoded MagicT token.</returns>
    private MagicTToken DecodeToken(byte[] token)
    {
        return MagicTTokenService.DecodeToken(token);
    }
}