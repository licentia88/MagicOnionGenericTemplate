using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface ITestService : ISecureMagicService<ITestService, TestModel>
{
    UnaryResult CreateMillionsData();
}
