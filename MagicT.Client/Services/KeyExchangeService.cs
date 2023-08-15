using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// Diffie-Hellman key exchange service
/// </summary>
public sealed class KeyExchangeService : MagicTClientServiceBase<IKeyExchangeService, byte[]>, IKeyExchangeService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="filters"></param>
    public KeyExchangeService(IServiceProvider provider) : base(provider, new KeyExchangeFilter(provider))
    {
    }

    /// <summary>
    /// Makes a request to get a public Key from the server
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public UnaryResult<byte[]> RequestServerPublicKeyAsync()
    {
        return Client.RequestServerPublicKeyAsync();
    }
}