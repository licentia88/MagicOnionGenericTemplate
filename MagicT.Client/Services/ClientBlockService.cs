using MagicT.Shared.Services;
using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models.ViewModels;

namespace MagicT.Client.Services;

/// <summary>
/// Handler for client block services.
/// </summary>
// ReSharper disable once UnusedType.Global
[RegisterScoped]
public class ClientBlockServiceHandler : MagicClientServiceBase<IClientBlockService>, IClientBlockService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBlockServiceHandler"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public ClientBlockServiceHandler(IServiceProvider provider) : base(provider)
    {
    }

    /// <summary>
    /// Adds a soft block to a client.
    /// </summary>
    /// <param name="clientData">The client data.</param>
    /// <returns>The client data with the block applied.</returns>
    public UnaryResult<ClientData> AddSoftBlock(ClientData clientData)
    {
        return Client.AddSoftBlock(clientData);
    }

    /// <summary>
    /// Adds a permanent block to a client.
    /// </summary>
    /// <param name="clientData">The client data.</param>
    /// <returns>The client data with the block applied.</returns>
    public UnaryResult<ClientData> AddPermanentBlock(ClientData clientData)
    {
        return Client.AddPermanentBlock(clientData);
    }

    /// <summary>
    /// Removes a permanent block from a client.
    /// </summary>
    /// <param name="clientData">The client data.</param>
    /// <returns>The client data with the block removed.</returns>
    public UnaryResult<ClientData> RemovePermanentBlock(ClientData clientData)
    {
        return Client.RemovePermanentBlock(clientData);
    }

    /// <summary>
    /// Reads the list of clients.
    /// </summary>
    /// <returns>A list of client data.</returns>
    public UnaryResult<List<ClientData>> ReadClients()
    {
        return Client.ReadClients();
    }
}