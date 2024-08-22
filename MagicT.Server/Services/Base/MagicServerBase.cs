using System.Runtime.CompilerServices;
using Benutomo;
using Coravel.Queuing.Interfaces;
using EntityFramework.Exceptions.Common;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Redis;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Server.Models;
using MagicT.Shared.Managers;
using Microsoft.EntityFrameworkCore.Storage;
using Nito.AsyncEx;

namespace MagicT.Server.Services.Base;

[AutomaticDisposeImpl]
public abstract partial class MagicServerBase<TService> : ServiceBase<TService> ,IDisposable,IAsyncDisposable where TService : IService<TService>
{
    protected readonly IQueue Queue;
    
    [EnableAutomaticDispose]
    protected MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    [EnableAutomaticDispose] private CancellationTokenManager CancellationTokenManager { get; set; }

    private MagicTToken Token => Context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    private int CurrentUserId => Token?.Id ?? 0;
    
    protected byte[] SharedKey => MagicTRedisDatabase.ReadAs<UsersCredentials>(CurrentUserId.ToString()).SharedKey;

    [EnableAutomaticDispose]
    protected IDbContextTransaction Transaction;

    private LogManager<TService> LogManager { get; set; }

    private AsyncSemaphore Semaphore { get; set; }


    protected MagicServerBase(IServiceProvider provider)
    {
        Queue = provider.GetService<IQueue>();

        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();

        CancellationTokenManager = provider.GetService<CancellationTokenManager>();

        LogManager = provider.GetService<LogManager<TService>>();

        Semaphore = provider.GetService<AsyncSemaphore>();

    }

    protected virtual async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task,
        [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default) 
    {
        try
        {
            await Semaphore.WaitAsync();
            var result = await task().ConfigureAwait(false);

            if (Transaction is not null)
                await Transaction.CommitAsync();

     
            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);

            Semaphore.Release();
            return result;
        }
        catch (UniqueConstraintException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A unique constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A unique constraint violation occurred: {ex.Message}");
        }
        catch (ReferenceConstraintException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A reference constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A reference constraint violation occurred: {ex.Message}");
        }
        catch (CannotInsertNullException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A not null constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A not null constraint violation occurred: {ex.Message}");

        }
        catch (MaxLengthExceededException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A max length constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A max length constraint violation occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, ex.Message,callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }

    }

    protected virtual  UnaryResult<T> ExecuteAsync<T>(Func<T> task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            Semaphore.Wait();
            var result =  task();

            if (Transaction is not null)
                Transaction.Commit();

            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);

            Semaphore.Release();
            return UnaryResult.FromResult(result);
        }
        catch (UniqueConstraintException ex)
        {
            if (Transaction is not null)
                 Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A unique constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A unique constraint violation occurred: {ex.Message}");
        }
        catch (ReferenceConstraintException ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A reference constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A reference constraint violation occurred: {ex.Message}");
        }
        catch (CannotInsertNullException ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A not null constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A not null constraint violation occurred: {ex.Message}");

        }
        catch (MaxLengthExceededException ex)
        {
            if (Transaction is not null)
                  Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A max length constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A max length constraint violation occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                  Transaction.Rollback();

            LogManager.LogError(CurrentUserId, ex.Message, callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
    }

    /// <summary>
    ///     Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerLineNumber"></param>
    protected virtual void Execute(Action task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        try
        {
            task();

            if (Transaction is not null)
                 Transaction.Commit();

            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);
        }
        catch (UniqueConstraintException ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A unique constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A unique constraint violation occurred: {ex.Message}");
        }
        catch (ReferenceConstraintException ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A reference constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A reference constraint violation occurred: {ex.Message}");
        }
        catch (CannotInsertNullException ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A not null constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A not null constraint violation occurred: {ex.Message}");

        }
        catch (MaxLengthExceededException ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, $"A max length constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A max length constraint violation occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                Transaction.Rollback();

            LogManager.LogError(CurrentUserId, ex.Message, callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
    }

    /// <summary>
    /// Executes an asynchronous task without returning a response.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <param name="callerFilePath"></param>
    /// <param name="callerMemberName"></param>
    /// <param name="callerLineNumber"></param>
    protected virtual async Task ExecuteAsync(Func<Task> task, [CallerFilePath] string callerFilePath = default,
        [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {

        try
        {
            await task().ConfigureAwait(false);

            if (Transaction is not null)
                await Transaction.CommitAsync();

            LogManager.LogMessage(CurrentUserId,"",callerFilePath,callerMemberName);

        }
        catch (UniqueConstraintException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A unique constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A unique constraint violation occurred: {ex.Message}");
        }
        catch (ReferenceConstraintException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A reference constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A reference constraint violation occurred: {ex.Message}");
        }
        catch (CannotInsertNullException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A not null constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A not null constraint violation occurred: {ex.Message}");

        }
        catch (MaxLengthExceededException ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, $"A max length constraint violation occurred: {ex.Message}", callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, $"A max length constraint violation occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();

            LogManager.LogError(CurrentUserId, ex.Message, callerFilePath, callerMemberName, callerLineNumber);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }

    }

 
}
