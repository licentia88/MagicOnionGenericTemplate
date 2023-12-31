//using Majorsoft.Blazor.Extensions.BrowserStorage;
//using Majorsoft.Blazor.Extensions.BrowserStorage;

namespace MagicT.Client.Filters;

public interface IFilterHelper
{
    ValueTask<(string Key, byte[] Data)> CreateHeaderAsync();
}
