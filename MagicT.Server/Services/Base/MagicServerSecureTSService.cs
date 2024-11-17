using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Generator.Equals;
using MagicOnion;
using MagicT.Shared.Services.Base;
using Microsoft.AspNetCore.Components;
using Nito.AsyncEx;

namespace MagicT.Server.Services.Base;

/// <summary>
/// A base service class that provides thread-safe execution of tasks using asynchronous locks, with added security features.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
/// <remarks>
/// <para><b>Important:</b> <typeparamref name="TModel"/> must have the <see cref="EquatableAttribute"/> attribute for the mutex to work. 
/// Otherwise, users should implement their own <see cref="ConcurrentDictionary{TKey, TValue}"/> with <c>int</c> as the key and 
/// <see cref="AsyncLock"/> as the value, and pass the primary key value of the model.</para>
/// </remarks>
[Filters.Authorize]
public abstract class MagicServerSecureTsService<TService, TModel, TContext> : MagicServerSecureService<TService, TModel, TContext>
    where TService : IMagicSecureService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    
    ~MagicServerSecureTsService()
    {
        Dispose();
    }
    /// <summary>
    /// Gets or sets the concurrent dictionary that holds the asynchronous locks for each model.
    /// </summary>
    [Inject]
    protected ConcurrentDictionary<TModel, AsyncLock> ConcurrentLocks { get; set; }

    /// <summary>
    /// Gets or sets the asynchronous lock for the current operation.
    /// </summary>
    private AsyncLock Mutex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MagicServerSecureTsService{TService,TModel,TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected MagicServerSecureTsService(IServiceProvider provider) : base(provider)
    {
        ConcurrentLocks = provider.GetService<ConcurrentDictionary<TModel, AsyncLock>>();
    }

    /// <summary>
    /// Sets the mutex for the specified model.
    /// </summary>
    /// <param name="model">The model for which to set the mutex.</param>
    protected void SetMutex(TModel model)
    {
        Mutex = ConcurrentLocks.GetOrAdd(model, _ => new AsyncLock());
    }

    /// <summary>
    /// Executes the specified asynchronous task with thread-safety.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>The result of the task.</returns>
    /// <exception cref="ReturnStatusException">Thrown when the mutex is not initialized.</exception>
    protected override async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Mutex == null)
            return await base.ExecuteAsync(task, callerMemberName: callerMemberName);

        using (await Mutex.LockAsync())
        {
            return await base.ExecuteAsync(task, callerMemberName: callerMemberName);
        }
    }

    /// <summary>
    /// Executes the specified synchronous task with thread-safety.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>The result of the task.</returns>
    /// <exception cref="ReturnStatusException">Thrown when the mutex is not initialized.</exception>
    protected override async UnaryResult<T> ExecuteAsync<T>(Func<T> task, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Mutex == null)
            return await base.ExecuteAsync(task,callerMemberName: callerMemberName);

        using (await Mutex.LockAsync())
        {
            return await base.ExecuteAsync(task, callerMemberName: callerMemberName);
        }
    }

    /// <summary>
    /// Executes the specified asynchronous task with thread-safety.
    /// </summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ReturnStatusException">Thrown when the mutex is not initialized.</exception>
    protected override async Task ExecuteAsync(Func<Task> task, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Mutex == null)
        {
            await base.ExecuteAsync(task,callerMemberName: callerMemberName);
            return;
        }

        using (await Mutex.LockAsync())
        {
            await base.ExecuteAsync(task, callerMemberName: callerMemberName);
        }
    }

    /// <summary>
    /// Executes the specified synchronous task with thread-safety.
    /// </summary>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <param name="message">An optional message that provides additional context for the execution.</param>
    /// <exception cref="ReturnStatusException">Thrown when the mutex is not initialized.</exception>
    protected override void Execute(Action task, string callerFilePath = default, string callerMemberName = default,
        int callerLineNumber = default, string message = default)
    {
        if (Mutex == null)
        {
            base.Execute(task,callerMemberName: callerMemberName);
            return;
        }

        using (Mutex.Lock())
        {
            base.Execute(task, callerMemberName: callerMemberName);
        }
    }
}