﻿using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

[RegisterScoped]
public sealed class SecureTestService : MagicClientSecureService<ISecureTestService, TestModel>, ISecureTestService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public SecureTestService(IServiceProvider provider)
        : base(provider)
    {
    }

    public UnaryResult<string> EncryptedString(EncryptedData<string> data)
    {
        return Client.EncryptedString(data);
    }
}
