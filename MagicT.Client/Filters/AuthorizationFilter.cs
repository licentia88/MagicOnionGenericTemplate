﻿using System.Security.Authentication;
using Benutomo;
using Humanizer;
using MagicOnion.Client;
using MagicT.Client.Extensions;
using MagicT.Client.Managers;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models.ServiceModels;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
/// Filter for adding token to gRPC client requests.
/// </summary>
[AutomaticDisposeImpl]
public partial class AuthorizationFilter : IClientFilter, IFilterHelper,IDisposable
{
    private KeyExchangeData GlobalData { get; }
    
    [EnableAutomaticDispose]
    private StorageManager StorageManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationFilter"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthorizationFilter(IServiceProvider provider)
    {
        StorageManager = provider.GetService<StorageManager>();
        GlobalData = provider.GetService<KeyExchangeData>();
    }

    ~AuthorizationFilter()
    {
        Dispose(false);
    }
    /// <summary>
    /// Adds the authentication token to the request headers and sends the request.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="next">The next step in the filter pipeline.</param>
    /// <returns>The response context.</returns>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        var header = await CreateHeaderAsync();
        context.CallOptions.Headers.AddOrUpdateItem(header.Key, header.Data);
        return await next(context);
    }

    /// <summary>
    /// Creates the authentication header data.
    /// </summary>
    /// <returns>A tuple containing the header key and data.</returns>
    /// <exception cref="AuthenticationException">Thrown when login data or token is not found.</exception>
    public async ValueTask<(string Key, byte[] Data)> CreateHeaderAsync()
    {
        var loginData = await StorageManager.GetLoginDataAsync() ?? throw new AuthenticationException("Failed to Sign in");
        var token = await StorageManager.GetTokenAsync() ?? throw new AuthenticationException("Security Token not found");

        var contactIdentifier = loginData.Identifier;
        var authData = new AuthenticationData(token, contactIdentifier);
        var cryptedAuthData = CryptoHelper.EncryptData(authData, GlobalData.SharedBytes);
        var cryptedAuthBin = cryptedAuthData.SerializeToBytes();

        return (nameof(BinType.CryptedAuthBin).Kebaberize(), cryptedAuthBin);
    }
}