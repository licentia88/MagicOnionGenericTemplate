// UnitTestBase.cs
using Clave.Expressionify;
using MagicT.Server.Database;
using MagicT.Server.Helpers;
using MagicT.Server.Services.Base;
using MagicT.Shared.Extensions;
using MagicT.Shared.Formatters;
using MagicT.Shared.Managers;
using MagicT.Shared.Services.Base;
using MemoryPack;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MagicT.Tests.Services.Base;

/// <summary>
/// Base class for unit tests, providing common setup and utility methods.
/// </summary>
/// <typeparam name="TService">The type of the service being tested.</typeparam>
/// <typeparam name="TModel">The type of the model being tested.</typeparam>
public abstract class UnitTestBase<TService, TModel> where TService : IMagicService<TService, TModel>
    where TModel : class, new()
{
    protected MagicTestService<TService, TModel, MagicTContext> MagicServerService { get; set; }

    /// <summary>
    /// Provides test data for a new record.
    /// </summary>
    public static IEnumerable<TestCaseData> NewRecordData => new[] { new TestCaseData(CreateNewRecord()) };

    /// <summary>
    /// Provides test data for new record with parameter bytes.
    /// </summary>
    public static IEnumerable<TestCaseData> NewRecordWithParamBytesData => new[] { new TestCaseData(CreateNewRecord().SerializeToBytes()) };

    /// <summary>
    /// Provides test data for a list of new records.
    /// </summary>
    public static IEnumerable<TestCaseData> NewRecordListData => new[] { new TestCaseData(CreateNewRecordList(30)) };

    private MockContext _mockContext;
    private Mock<LogManager<TService>> _mockLogManager;
    // private Mock<QueryBuilder> _mockQueryManager;
    private Mock<IServiceProvider> _mockServiceProvider;

    static UnitTestBase()
    {
        MemoryPackFormatterProvider.Register(new UnsafeObjectFormatter());
    }

    [SetUp]
    public void Initialize()
    {
        MemoryPackFormatterProvider.Register(new UnsafeObjectFormatter());

        var options = new DbContextOptionsBuilder<MagicTContext>()
           .UseSqlServer("Server=localhost;Database=MockContext;User Id=sa;Password=LucidNala88!;TrustServerCertificate=true")
           .Options;

        _mockContext = new MockContext(options);
        _mockContext.Database.EnsureCreated();

        _mockLogManager = new Mock<LogManager<TService>>();
        // _mockQueryManager = new Mock<QueryBuilder>();
        _mockServiceProvider = new Mock<IServiceProvider>();

        _mockServiceProvider.Setup(x => x.GetService(typeof(MagicTContext))).Returns(_mockContext);
        _mockServiceProvider.Setup(x => x.GetService(typeof(LogManager<TService>))).Returns(_mockLogManager.Object);
        // _mockServiceProvider.Setup(x => x.GetService(typeof(QueryBuilder))).Returns(_mockQueryManager.Object);

        MagicServerService = new MagicTestService<TService, TModel, MagicTContext>(_mockServiceProvider.Object);
    }

    [TearDown]
    public void CleanUp()
    {
        _mockContext.Database.EnsureDeleted();
        _mockContext.Dispose();
    }

    #region Static Helpers

    /// <summary>
    /// Creates a new instance of the model with default values.
    /// </summary>
    protected static TModel CreateNewRecord()
    {
        var model = GenFu.GenFu.New<TModel>();
        var primaryKey = Shared.Extensions.ModelExtensions.GetPrimaryKey<TModel>();
        model.SetPropertyValue(primaryKey, null);
        return model;
    }

    /// <summary>
    /// Creates a list of new instances of the model with default values.
    /// </summary>
    protected static List<TModel> CreateNewRecordList(int itemCount)
    {
        var modelList = GenFu.GenFu.ListOf<TModel>(itemCount);
        var primaryKey = Shared.Extensions.ModelExtensions.GetPrimaryKey<TModel>();
        modelList.ForEach(x => x.SetPropertyValue(primaryKey, null));
        return modelList;
    }

    /// <summary>
    /// Gets the parameter bytes for the primary key of the model.
    /// </summary>
    protected static byte[] GetPrimaryKeyAsBytes(TModel model)
    {
        var primaryKey = model.GetPrimaryKey();
        var primaryKeyValue = model.GetPropertyValue(primaryKey);

        KeyValuePair<string, object>[] parameters = new KeyValuePair<string, object>[]
        {
            new KeyValuePair<string, object>(primaryKey, primaryKeyValue)
        };
        byte[] paramBytes = default;

        if (parameters.Any())
            paramBytes = parameters.SerializeToBytes();

        return paramBytes;
    }

    /// <summary>
    /// Checks if the original and updated models have the same primary key.
    /// </summary>
    [Expressionify]
    protected bool IsSame(TModel original, TModel updated)
    {
        var key = Shared.Extensions.ModelExtensions.GetPrimaryKey<TModel>();
        return original.GetPropertyValue(key).ToString() == updated.GetPropertyValue(key).ToString();
    }

    #endregion
}