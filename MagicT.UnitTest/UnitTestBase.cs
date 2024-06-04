using Grpc.Core;
using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.UnitTest;

public class UnitTestBase<TModel, TService> : IMagicService<TService, TModel> where TService : IService<TService>
{
    public virtual UnaryResult<TModel> CreateAsync(TModel model)
    {
        throw new NotImplementedException();
    }

    public virtual UnaryResult<TModel> DeleteAsync(TModel model)
    {
        throw new NotImplementedException();
    }

    public virtual UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameterBytes)
    {
        throw new NotImplementedException();
    }

    public virtual UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        throw new NotImplementedException();
    }

    public virtual UnaryResult<List<TModel>> ReadAsync()
    {
        throw new NotImplementedException();
    }

    public virtual Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        throw new NotImplementedException();
    }

    public virtual UnaryResult<TModel> UpdateAsync(TModel model)
    {
        throw new NotImplementedException();
    }

    public virtual TService WithCancellationToken(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public virtual TService WithDeadline(DateTime deadline)
    {
        throw new NotImplementedException();
    }

    public virtual TService WithHeaders(Metadata headers)
    {
        throw new NotImplementedException();
    }

    public virtual TService WithHost(string host)
    {
        throw new NotImplementedException();
    }

    public virtual TService WithOptions(CallOptions option)
    {
        throw new NotImplementedException();
    }
}
