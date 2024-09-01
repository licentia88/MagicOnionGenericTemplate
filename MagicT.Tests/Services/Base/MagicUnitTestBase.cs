// MagicUnitTestBase.cs
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Tests.Extensions;
using MagicT.Tests.Services.Interfaces;

namespace MagicT.Tests.Services.Base;

/// <summary>
/// Base class for unit tests, providing common setup and utility methods for Magic services.
/// </summary>
/// <typeparam name="TService">The type of the service being tested.</typeparam>
/// <typeparam name="TModel">The type of the model being tested.</typeparam>
[TestFixture]
public abstract class MagicUnitTestBase<TService, TModel> : UnitTestBase<TService, TModel>, IMagicTestService<TModel>
    where TService : IMagicService<TService, TModel>
    where TModel : class, new()
{
    /// <summary>
    /// Test case for creating a new record asynchronously.
    /// </summary>
    /// <param name="model">The model to be created.</param>
    [Test, TestCaseSource(nameof(NewRecordData))]
    public async Task CreateAsync(TModel model)
    {
        var createdRecord = await MagicServerService.CreateAsync(model);
        Assert.That(createdRecord, Is.Not.Null, "The created record should not be null.");
        Assert.That(createdRecord, Is.EqualTo(model), "The created record should match the input model.");
    }

    /// <summary>
    /// Test case for creating multiple instances of the specified model asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordListData))]
    public async Task CreateRangeAsync(List<TModel> models)
    {
        var createdRecords = await MagicServerService.CreateRangeAsync(models);
        Assert.That(createdRecords.Count, Is.EqualTo(models.Count), "The number of created records should match the number of input models.");
        for (int i = 0; i < models.Count; i++)
        {
            Assert.That(createdRecords[i], Is.EqualTo(models[i]), $"The created record at index {i} should match the input model.");
        }
    }

    /// <summary>
    /// Test case for reading records asynchronously.
    /// </summary>
    [Test]
    public async Task ReadAsync()
    {
        await CreateRangeAsync(CreateNewRecordList(30));
        var readRecords = await MagicServerService.ReadAsync();
        Assert.That(readRecords, Is.Not.Null, "The returned list of records should not be null.");
        Assert.That(readRecords.Count, Is.GreaterThan(0), "The number of returned records should be greater than zero.");
        Assert.That(readRecords.Count, Is.EqualTo(30), "The number of returned records should be 30.");
    }

    /// <summary>
    /// Test case for updating a record asynchronously.
    /// </summary>
    /// <param name="model">The model to be updated.</param>
    [Test, TestCaseSource(nameof(NewRecordData))]
    public async Task UpdateAsync(TModel model)
    {
        var createdRecord = await MagicServerService.CreateAsync(model);
        var cloned = createdRecord.SetRandomProperty();
        var updatedRecord = await MagicServerService.UpdateAsync(cloned);
        Assert.That(updatedRecord, Is.Not.Null, "The updated record should not be null.");
        Assert.That(IsSame(createdRecord, updatedRecord), Is.True, "The fields have the same primary key.");
        Assert.That(createdRecord.Equals(updatedRecord), Is.False, "The created record should not match the modified record.");
        Assert.That(updatedRecord, Is.EqualTo(cloned), "The updated record should match the modified record.");
    }

    /// <summary>
    /// Test case for updating a range of records asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordListData))]
    public async Task UpdateRangeAsync(List<TModel> models)
    {
        try
        {
            var createdRecords = await MagicServerService.CreateRangeAsync(models);
            Assert.That(createdRecords.Count, Is.EqualTo(models.Count),
                "The number of created records should match the number of input models.");
            var updateResponse = await MagicServerService.UpdateRangeAsync(createdRecords);
            Assert.That(updateResponse.Count, Is.EqualTo(createdRecords.Count),
                "The number of updated records should match the number of created records.");
            var fetchedRecords = await MagicServerService.ReadAsync();
            Assert.That(fetchedRecords.Count, Is.EqualTo(models.Count),
                "The number of fetched records should match the number of input models.");
            foreach (var fetchedData in fetchedRecords)
            {
                var record = createdRecords.FirstOrDefault(x=> x == fetchedData);
                Assert.That(createdRecords.Any(arg => arg.Equals(fetchedData)), Is.True,
                    "Each fetched record should match a created record.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Test case for deleting a record asynchronously.
    /// </summary>
    /// <param name="model">The model to be deleted.</param>
    [Test, TestCaseSource(nameof(NewRecordData))]
    public async Task DeleteAsync(TModel model)
    {
        var createdRecord = await MagicServerService.CreateAsync(model);
        Assert.That(createdRecord, Is.Not.Null, "The created record should not be null.");
        Assert.That(model.Equals(createdRecord), Is.True, "The model should be equal to the created record.");
        var deletedRecord = await MagicServerService.DeleteAsync(createdRecord);
        Assert.That(deletedRecord, Is.Not.Null, "The deleted record should not be null.");
        Assert.That(model.Equals(deletedRecord), Is.True, "The model should be equal to the deleted record.");
        Assert.That(createdRecord, Is.EqualTo(deletedRecord), "The created record should be equal to the deleted record.");
        var foundRecords = await MagicServerService.FindByParametersAsync(GetPrimaryKeyAsBytes(deletedRecord));
        Assert.That(foundRecords.Count, Is.EqualTo(0), "No records should be found after deletion.");
    }

    /// <summary>
    /// Test case for deleting a range of records asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordListData))]
    public async Task DeleteRangeAsync(List<TModel> models)
    {
        var createResponse = await MagicServerService.CreateRangeAsync(models);
        Assert.That(createResponse.Count, Is.EqualTo(models.Count), "The number of created records should match the number of input models.");
        var deleteResponse = await MagicServerService.DeleteRangeAsync(createResponse);
        Assert.That(deleteResponse.Count, Is.EqualTo(createResponse.Count), "The number of deleted records should match the number of created records.");
        var primaryKeyParams = GetPrimaryKeyAsBytes(createResponse.First());
        var fetchedRecords = await MagicServerService.FindByParametersAsync(primaryKeyParams);
        Assert.That(fetchedRecords.Count, Is.EqualTo(0), "No records should be found after deletion.");
        foreach (var model in createResponse)
        {
            var isDeleted = fetchedRecords.All(r => !r.Equals(model));
            Assert.That(isDeleted, Is.True, $"The model with ID {model.GetPrimaryKey()} should not exist in the fetched records after deletion.");
        }
    }

    /// <summary>
    /// Test case for finding records by parameters asynchronously.
    /// </summary>
    /// <param name="parameterBytes">The serialized parameters to find the records by.</param>
    [Test, TestCaseSource(nameof(NewRecordWithParamBytesData))]
    public async Task FindByParametersAsync(byte[] parameterBytes)
    {
        var model = parameterBytes.DeserializeFromBytes<TModel>();
        var createdRecord = await MagicServerService.CreateAsync(model);
        Assert.That(createdRecord, Is.Not.Null, "The created record should not be null.");
        var foundRecords = await MagicServerService.FindByParametersAsync(GetPrimaryKeyAsBytes(model));
        Assert.That(foundRecords.Count, Is.EqualTo(1), "The number of found records should be exactly one.");
        Assert.That(foundRecords.First(), Is.EqualTo(createdRecord), "The found record should match the created record.");
    }

    public Task StreamReadAllAsync(int batchSize)
    {
        throw new NotImplementedException();
    }
}