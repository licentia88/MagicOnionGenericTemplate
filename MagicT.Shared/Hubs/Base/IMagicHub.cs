using MagicOnion;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Shared.Hubs.Base;

public interface IMagicHub<THub, TReceiver, TModel>: IStreamingHub<THub, TReceiver>
{
    Task ConnectAsync();

    Task<RESPONSE_RESULT<TModel>> CreateAsync(TModel model);

    Task<RESPONSE_RESULT<List<TModel>>> ReadAsync();

    Task StreamReadAsync(int batchSize);

    Task<RESPONSE_RESULT<TModel>> UpdateAsync(TModel model);

    Task<RESPONSE_RESULT<TModel>> DeleteAsync(TModel model);

    Task CollectionChanged();
}