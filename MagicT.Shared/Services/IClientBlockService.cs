using MagicOnion;
using MagicT.Shared.Models.ViewModels;

namespace MagicT.Shared.Services;

public interface IClientBlockService : IService<IClientBlockService>
{
    UnaryResult<ClientData> AddSoftBlock(ClientData clientData);
    UnaryResult<ClientData> AddPermanentBlock(ClientData clientData);
    UnaryResult<ClientData> RemovePermanentBlock(ClientData clientData);
    UnaryResult<List<ClientData>> ReadClients();
}