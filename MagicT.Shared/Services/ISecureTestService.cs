using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface ISecureTestService : ISecureMagicService<ISecureTestService, TestModel>
{
    UnaryResult<string> EncryptedString(EncryptedData<string> data);
}
