using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using MagicT.Tests.Extensions;
using MagicT.Tests.Services.Interfaces;

namespace MagicT.Tests.Services.Base;

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
        // Act: Create a new record using the model
        var createdRecord = await MagicServerService.CreateAsync(model);
        
        // Assert
        Assert.That(createdRecord, Is.Not.Null, "The created record should not be null.");

    }


    /// <summary>
    /// Test case for creating multiple instances of the specified model asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordListData))]
    public async Task CreateRangeAsync(List<TModel> models)
    {
        // Act: Create a list of new records using the model
        var createdRecords = await MagicServerService.CreateRangeAsync(models);

        // Assert
        Assert.That(createdRecords.Count, Is.EqualTo(models.Count));
    }

  

    [Test]
    public async Task ReadAsync()
    {
        // Arrange
        await CreateRangeAsync(CreateNewRecordList(30));
              
        var readRecords = await MagicServerService.ReadAsync();

        // Assert
        Assert.That(readRecords, Is.Not.Null, "The returned list of records should not be null.");
        Assert.That(readRecords.Count, Is.GreaterThan(0), "The number of returned records should be greater than zero.");
    }

    public Task StreamReadAllAsync(int batchSize)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Test case for creating a new record asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordData))]
    public async Task UpdateAsync(TModel model)
    {
        // Act
        var createdRecord = await MagicServerService.CreateAsync(model);

        var cloned =  createdRecord.SetRandomProperty();
        // Assert
        var updatedRecord = await MagicServerService.UpdateAsync(cloned);

        // Assert
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(IsSame(createdRecord, updatedRecord), Is.True, "The fields have same primary key");
        Assert.That(createdRecord.Equals(updatedRecord), Is.False, "The created record should not match the modified record.");
    }

    /// <summary>
    /// Test case for updating a range of records asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordListData))]
    public async Task UpdateRangeAsync(List<TModel> models)
    {
        // Act
        var createdRecords = await MagicServerService.CreateRangeAsync(models);

        // Assert
        Assert.That(createdRecords.Count, Is.EqualTo(models.Count), "The number of created records should match the number of input models.");

         
        // Act
        var updateResponse = await MagicServerService.UpdateRangeAsync(createdRecords);

        // Assert
        Assert.That(updateResponse.Count, Is.EqualTo(createdRecords.Count), "The number of updated records should match the number of created records.");

        // Assert
        var fetchedRecords = await MagicServerService.ReadAsync();

        Assert.That(createdRecords.Count, Is.EqualTo(models.Count));

        foreach (var fetchedData in fetchedRecords)
        {
            Assert.That(createdRecords.Any(arg => arg.Equals(fetchedData)), Is.True);
        }
    }

 
    /// <summary>
    /// Test case for deleting a record asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordData))]
    public async Task DeleteAsync(TModel model)
    {
        // Act
        var createdRecord = await MagicServerService.CreateAsync(model);

        // Assert
        Assert.That(createdRecord, Is.Not.Null, "The created record should not be null.");
        Assert.That(model.Equals(createdRecord), Is.True, "The model should be equal to the created record.");

        // Act
        var deletedRecord = await MagicServerService.DeleteAsync(createdRecord);

        // Assert
        Assert.That(deletedRecord, Is.Not.Null, "The deleted record should not be null.");
        Assert.That(model.Equals(deletedRecord), Is.True, "The model should not be equal to the deleted record.");
        Assert.That(createdRecord, Is.EqualTo(deletedRecord), "The created record should be equal to the deleted record.");

        // Additional check to verify the record is actually deleted
        var foundRecords = await MagicServerService.FindByParametersAsync(GetPrimaryKeyAsBytes(deletedRecord));
        Assert.That(foundRecords.Count, Is.EqualTo(0), "No records should be found after deletion.");
    }

    /// <summary>
    /// Test case for deleting a range of records asynchronously.
    /// </summary>
    [Test, TestCaseSource(nameof(NewRecordListData))]
    public async Task DeleteRangeAsync(List<TModel> models)
    {
        // Act: Create a range of records
        var createResponse = await MagicServerService.CreateRangeAsync(models);

        // Assert: Verify that the correct number of records were created
        Assert.That(createResponse.Count, Is.EqualTo(models.Count), "The number of created records should match the number of input models.");

        // Act: Delete the range of created records
        var deleteResponse = await MagicServerService.DeleteRangeAsync(createResponse);

        // Assert: Verify that the correct number of records were deleted
        Assert.That(deleteResponse.Count, Is.EqualTo(createResponse.Count), "The number of deleted records should match the number of created records.");

        // Fetch the records to verify they have been deleted
        var primaryKeyParams = GetPrimaryKeyAsBytes(createResponse.First());
        var fetchedRecords = await MagicServerService.FindByParametersAsync(primaryKeyParams);

        // Assert: Verify that no records are found after deletion
        Assert.That(fetchedRecords.Count, Is.EqualTo(0), "No records should be found after deletion.");

        // Additional assertions for each deleted record
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
        // Arrange: Deserialize the parameter bytes to get the model
        var model = parameterBytes.DeserializeFromBytes<TModel>();

        // Act: Create a new record using the deserialized model
        var createdRecord = await MagicServerService.CreateAsync(model);

        // Assert: Verify that the created record is not null
        Assert.That(createdRecord, Is.Not.Null, "The created record should not be null.");

        // Act: Find records using the given parameters
        var foundRecords = await MagicServerService.FindByParametersAsync(GetPrimaryKeyAsBytes(model));

        // Assert: Verify that the found records contain exactly one record and it matches the created record
        Assert.That(foundRecords.Count, Is.EqualTo(1), "The number of found records should be exactly one.");
        Assert.That(foundRecords.First(), Is.EqualTo(createdRecord), "The found record should match the created record.");
    }


}

