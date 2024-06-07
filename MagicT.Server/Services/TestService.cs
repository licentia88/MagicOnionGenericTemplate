using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

[AutomaticDisposeImpl]
public sealed partial class TestService : MagicServerService<ITestService, TestModel, MagicTContext>, ITestService, IDisposable, IAsyncDisposable
{
    public KeyExchangeData globalData { get; set; }

    public TestService(IServiceProvider provider) : base(provider)
    {
        globalData = provider.GetService<KeyExchangeData>();
    }

    public override UnaryResult<TestModel> CreateAsync(TestModel model)
    {
        model.CheckData = new Random().Next().ToString();
        return base.CreateAsync(model);
    }

    public override UnaryResult<TestModel> UpdateAsync(TestModel model)
    {
        model.CheckData = new Random().Next().ToString();
        return base.UpdateAsync(model);
    }

    public async UnaryResult CreateMillionsData()
    {

        var dataList = new List<TestModel>();

        // var loopResult = Parallel.For(0, 1000000, (int arg1, ParallelLoopState arg2) =>
        // {
        //     var newModel = GenFu.GenFu.New<TestModel>();
        //     newModel.Id = 0;
        //     dataList.Add(newModel);
        // });
        for (int i = 0; i < 1000000; i++)
        {
            var newModel = GenFu.GenFu.New<TestModel>();
            //newModel.Id = 0;
            dataList.Add(newModel);
        }
        
       
        Db.TestModel.AddRange(dataList);
        Db.SaveChanges();
        //return UnaryResult.CompletedResult;
    }

    public async override Task<ServerStreamingResult<List<TestModel>>> StreamReadAllAsync(int batchSize)
    {
        var stream = GetServerStreamingContext<List<TestModel>>();

        var queryData = QueryManager.BuildQuery<TestModel>();

        await foreach(var reader in Db.Manager().StreamReaderAsync(queryData.query+" ORDER BY 1", queryData.parameters))
        {

            await stream.WriteAsync(reader.ToTestModel());
          
        }
 

        return stream.Result();
    }
}
