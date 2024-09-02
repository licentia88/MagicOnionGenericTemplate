using MagicOnion;
using MagicT.Redis.Models;

namespace MagicT.Shared.Services;

public interface IClientBlockService : IService<IClientBlockService>
{
    UnaryResult<ClientData> AddSoftBlock(ClientData clientData);
    UnaryResult<ClientData> AddHardBlock(ClientData clientData);
    UnaryResult RemovePermanentBlock(ClientData clientData);
    UnaryResult<List<ClientData>> ReadClients();
}