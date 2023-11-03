using InfluxDB.Client;

namespace MagicT.Server.InfluxDb;

public interface IInfluxDbService<TModel>
{
	public void Write(Action<WriteApi> action);

    public void WriteAsync(Action<WriteApiAsync> action);

    public Task<TModel> QueryAsync(Func<QueryApi, Task<TModel>> action);

}

