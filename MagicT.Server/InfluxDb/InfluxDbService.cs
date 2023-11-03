using InfluxDB.Client;

namespace MagicT.Server.InfluxDb;

public class InfluxDbService<TModel>: IInfluxDbService<TModel>
{
	private InfluxDBClient DbClient { get; set; }
	 
	public InfluxDbService(IServiceProvider provider)
	{
		DbClient = provider.GetService<InfluxDBClient>();
    }

	public void Write(Action<WriteApi> action)
	{
		using WriteApi write = DbClient.GetWriteApi();
		action(write);
	}

    public void WriteAsync(Action<WriteApiAsync> action)
    {
        WriteApiAsync write = DbClient.GetWriteApiAsync();

        action(write);
    }

    public async Task<TModel> QueryAsync(Func<QueryApi, Task<TModel>> action)
	{
		QueryApi query =  DbClient.GetQueryApi();

		return await action(query);
	}
}

