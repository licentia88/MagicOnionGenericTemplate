using System.Collections.Concurrent;
using Benutomo;
using EFCore.BulkExtensions;
using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Managers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;


public sealed partial class TestService : AuditDatabaseService<ITestService, TestModel, MagicTContext>, ITestService
{
    // public LogManager<TestModel> TestModelLog { get; set; }
    public KeyExchangeData GlobalData { get; set; }

    public TestService(IServiceProvider provider) : base(provider)
    {
        GlobalData = provider.GetService<KeyExchangeData>();
        // TestModelLog = provider.GetService<LogManager<TestModel>>();
        
    }

    ~TestService()
    {
        Dispose(false);
    }
    
    public override UnaryResult<List<TestModel>> ReadAsync()
    {
        var response = Db.TestModel.AsNoTracking().ToList();

        return UnaryResult.FromResult(response);
    }

    public override UnaryResult<TestModel> CreateAsync(TestModel model)
    {
        model.CheckData = new Random().Next().ToString();
        return base.CreateAsync(model);
    }

 
    public async UnaryResult CreateMillionsData()
    {

        var dataBag = new ConcurrentBag<TestModel>();

        Parallel.For(0, 1000000, i =>
        {
            var newModel = GenFu.GenFu.New<TestModel>();
            newModel.Id = 0;
            dataBag.Add(newModel);
        });

        var dataList = dataBag.ToList();

        // Batch SaveChanges to improve performance
        for (int i = 0; i < dataList.Count; i += 10000)
        {
            // Db.BulkInsertAsync()
            await Db.BulkInsertAsync(dataList.Skip(i).Take(10000));
            // Db.SaveChanges();
            Db.ChangeTracker.Clear(); // Clear tracked entities to reduce memory usage
        }

        //return UnaryResult.CompletedResult;
    }

    //public async override Task<ServerStreamingResult<List<TestModel>>> StreamReadAllAsync(int batchSize)
    //{
    //    var stream = GetServerStreamingContext<List<TestModel>>();

    //    var queryData = QueryManager.BuildQuery<TestModel>();

    //    await foreach(var reader in Db.Manager().StreamReaderAsync(queryData.query+" ORDER BY 1", queryData.parameters))
    //    {

    //        await stream.WriteAsync(reader.ToTestModel());
          
    //    }
 

    //    return stream.Result();
    //}
}
