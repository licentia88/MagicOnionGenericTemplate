using System.Text.Json;
using Coravel.Queuing.Interfaces;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Server.Database;
using MagicT.Server.Exceptions;
using MagicT.Server.Extensions;
using MagicT.Server.Invocables;
using MagicT.Server.Jwt;
using MagicT.Server.ZoneTree;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;

namespace MagicT.Server.Services.Base;

 
public abstract class MagicServerBase<TService,TModel> : ServiceBase<TService> where TService : IService<TService>
{
    protected readonly IQueue Queue;

    private ILogger<TModel> Logger { get; set; }

    protected DbExceptionHandler DbExceptionHandler { get; set; }

    /// <summary>
    /// ZoneTree Database Manager
    /// </summary>
    public ZoneDbManager ZoneDbManager { get; set; }

   

    /// <summary>
    /// Gets shared key from service context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    //protected byte[] SharedKey(ServiceContext context) => ZoneDbManager.UsersZoneDb.FindBy(x => x.UserId == CurrentUserId(context))
    //                                                       .FirstOrDefault().Value.SharedKey;


    //protected MagicTToken Token(ServiceContext context) => context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    public MagicTToken Token(ServiceContext context) => context.GetItemAs<MagicTToken>(nameof(MagicTToken));

    public byte[] GetSharedKey(ServiceContext context)
        => ZoneDbManager.UsersZoneDb
            .FindBy(x => x.UserId == Token(context).Id)
            .LastOrDefault().Value.SharedKey;

    protected int CurrentUserId(ServiceContext context) => Token(context).Id;

   

    public MagicServerBase(IServiceProvider provider)
    {
        Queue = provider.GetService<IQueue>();

        ZoneDbManager = provider.GetService<ZoneDbManager>();

        Logger = provider.GetService<ILogger<TModel>>();

        DbExceptionHandler = provider.GetService<DbExceptionHandler>();
    }

    /// <summary>
    ///     Executes an asynchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task)
    {
        return ExecuteWithoutResponseAsync(task, null);
    }

    public async UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task, IDbContextTransaction transaction)
    {
        try
        {
            var result = await task().ConfigureAwait(false);

            if (transaction is not null)
                await transaction?.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (transaction is not null)
                await transaction?.RollbackAsync();


            AuditFailed();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }

    }

    /// <summary>
    ///     Executes a synchronous task without returning a response.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="task">The synchronous task to execute.</param>
    /// <returns>A <see cref="UnaryResult{T}" /> containing the result of the task.</returns>
    public UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task)
    {
        return ExecuteWithoutResponse(task, null);
    }

    public UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task, IDbContextTransaction transaction)
    {
        try
        {
            var result = task();

            if (transaction is not null)
                transaction?.Commit();

            return UnaryResult.FromResult(result);
        }
        catch (Exception ex)
        {
            if (transaction is not null)
                transaction?.Rollback();


            AuditFailed();

            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

 
    /// <summary>
    ///     Executes an action.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    public void Execute(Action task)
    {
        try
        {
            task();
        }
        catch (Exception ex)
        {
            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
    }

    /// <summary>
    /// Executes an asynchronous task without returning a response.
    /// </summary>
    /// <param name="task">The asynchronous task to execute.</param>
    public async Task ExecuteAsync(Func<Task> task)
    {

        try
        {
            await task().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new ReturnStatusException(StatusCode.Cancelled, HandleException(ex));
        }
         
    }




    private string HandleException(Exception ex)
    {
        //Logger.Log(LogLevel.Error, ex.Message);
        return DbExceptionHandler.HandleException(ex);
    }


    #region Helper Methods


    protected void AuditQueries()
       => Queue.QueueInvocableWithPayload<AuditQueryInvocable<MagicTContext>, AuditQueryPayload>(
          new AuditQueryPayload(CurrentUserId(Context), Context.ServiceType.Name, Context.MethodInfo.Name, Context.CallContext.Method, GetRequestParameter()));


    protected void AuditFailed()
        => Queue.QueueInvocableWithPayload<AuditFailedInvocable<MagicTContext>, AuditFailedPayload>(
            new AuditFailedPayload(CurrentUserId(Context), Context.ServiceType.Name, Context.MethodInfo.Name, Context.CallContext.Method, GetRequestParameter()));


    public string GetRequestParameter()
    {
        var data = Context.GetRawRequest();

        if (Context.MethodInfo.IsEncryptedData())
        {
            data = CryptoHelper.DecryptData((EncryptedData<TModel>)data, GetSharedKey(Context));
        }

        if (Context.MethodInfo.IsByteArray())
        {
            data = ((byte[])data).UnPickleFromBytes<KeyValuePair<string, object>[]>();
        }

        return JsonSerializer.Serialize(data);
    }
    #endregion


}
