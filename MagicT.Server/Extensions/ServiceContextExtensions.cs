using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
// ReSharper disable UnusedMember.Global

namespace MagicT.Server.Extensions;

/// <summary>
/// Provides extension methods for the MagicOnion framework, specifically targeting the ServiceContext type.
/// </summary>
public static class ServiceContextExtensions
{
    /// <summary>
    /// Gets the client name from the request headers in the ServiceContext.
    /// </summary>
    /// <param name="context">The ServiceContext instance.</param>
    /// <returns>The client name extracted from the request headers.</returns>
    public static string GetClientName(this ServiceContext context) => context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == "client")?.Value;

    /// <summary>
    /// Gets an item from the context as the specified type.
    /// </summary>
    /// <typeparam name="TReturn">The type to return.</typeparam>
    /// <param name="context">The ServiceContext instance.</param>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>The item as the specified type, or null if not found.</returns>
    public static TReturn GetItemAs<TReturn>(this ServiceContext context, string key) where TReturn : class
    {
        if (context is null || !context.Items.TryGetValue(key, out var item)) return default;

        return item as TReturn;
    }

    /// <summary>
    /// Gets an item from the request headers as the specified type.
    /// </summary>
    /// <typeparam name="TReturn">The type to return.</typeparam>
    /// <param name="context">The ServiceContext instance.</param>
    /// <param name="key">The key of the header to retrieve.</param>
    /// <returns>The header value as the specified type, or null if not found.</returns>
    public static TReturn GetItemFromHeaderAs<TReturn>(this ServiceContext context, string key) where TReturn : class => context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == key)?.ValueBytes as TReturn;

    /// <summary>
    /// Adds the given item to the context.
    /// </summary>
    /// <param name="context">The ServiceContext instance.</param>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    public static void AddItem(this ServiceContext context, string key, object value)
    {
        if (value is not null)
        {
            context.Items.AddOrUpdate(key, _ => value, (_, _) => value);
            return;
        }

        var item = context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == key);

        if (item is null)
        {
#if DEBUG
            throw new ReturnStatusException(StatusCode.NotFound, $"{key} not found");
#else
            return;
#endif
        }

        context.Items.AddOrUpdate(key, _ => item.IsBinary ? item.ValueBytes : item.Value,
            (_, _) => item.IsBinary ? item.ValueBytes : item.Value);
    }

    /// <summary>
    /// Adds an item to the context from the request header.
    /// </summary>
    /// <param name="context">The ServiceContext instance.</param>
    /// <param name="key">The key of the header to add.</param>
    public static void AddItem(this ServiceContext context, string key)
    {
        context.AddItem(key, null);
    }
}