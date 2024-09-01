using MagicOnion;
using MagicT.Shared.Services;
using MagicT.Shared.Models.ViewModels;
using MagicT.Redis.Services;
using MagicOnion.Server;

namespace MagicT.Server.Services;

/// <summary>
/// Service for managing client blocks.
/// </summary>
// ReSharper disable once UnusedType.Global
public class ClientBlockService : ServiceBase<IClientBlockService>, IClientBlockService
{
    private readonly ClientBlockerService _clientBlockerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientBlockService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public ClientBlockService(IServiceProvider provider)
    {
        _clientBlockerService = provider.GetService<ClientBlockerService>();
    }

    /// <summary>
    /// Adds a soft block to a client.
    /// </summary>
    /// <param name="clientData">The client data.</param>
    /// <returns>The client data with the block applied.</returns>
    public UnaryResult<ClientData> AddSoftBlock(ClientData clientData)
    {
        _clientBlockerService.AddSoftBlock(clientData.Id);
        return UnaryResult.FromResult(clientData);
    }

    /// <summary>
    /// Adds a permanent block to a client.
    /// </summary>
    /// <param name="clientData">The client data.</param>
    /// <returns>The client data with the block applied.</returns>
    public UnaryResult<ClientData> AddPermanentBlock(ClientData clientData)
    {
        _clientBlockerService.AddHardBlock(clientData.Id);
        return UnaryResult.FromResult(clientData);
    }

    /// <summary>
    /// Removes a permanent block from a client.
    /// </summary>
    /// <param name="clientData">The client data.</param>
    /// <returns>The client data with the block removed.</returns>
    public UnaryResult<ClientData> RemovePermanentBlock(ClientData clientData)
    {
        _clientBlockerService.RemoveBlock(clientData.Id);
        return UnaryResult.FromResult(clientData);
    }

    /// <summary>
    /// Reads the list of clients.
    /// </summary>
    /// <returns>A list of client data.</returns>
    public UnaryResult<List<ClientData>> ReadClients()
    {
        var clientIds = _clientBlockerService.ReadClients();
        var clients = clientIds.Select(clientId => new ClientData { Id = clientId.Id, Ip = clientId.Ip, Status = clientId.BlockType }).ToList();

        return UnaryResult.FromResult(clients);
    }
}