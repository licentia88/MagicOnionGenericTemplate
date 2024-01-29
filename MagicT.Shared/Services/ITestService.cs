using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface ITestService : IMagicService<ITestService, TestModel>
{
    UnaryResult CreateMillionData();
}
