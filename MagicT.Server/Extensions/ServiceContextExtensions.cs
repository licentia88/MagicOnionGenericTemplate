using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;

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


    public static TReturn GetItemAs<TReturn>(this ServiceContext context, string key) where TReturn : class
    {
        if (context is null || !context.Items.ContainsKey(key)) return default;

        return context.Items[key] as TReturn;
    }

    public static TReturn GetItemFromHeaderAs<TReturn>(this ServiceContext context, string key) where TReturn : class => context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == key)?.ValueBytes as TReturn;


    /// <summary>
    /// Add given item to context
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    // ReSharper disable once MemberCanBePrivate.Global
    public static void AddItem( this ServiceContext context,  string key, object value)
    {
        if (value is not null)
        {
            context.Items.AddOrUpdate(key, _ => value, (_, _) => value);
            return;
        }
        
        var item = context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == key);

        if (item is null)
        {
//-:cnd
#if DEBUG
            throw new ReturnStatusException(StatusCode.NotFound, $"{key} not found");
#else
   return; 
#endif
//+:cnd
        }

        context.Items.AddOrUpdate(key, _ => item.IsBinary? item.ValueBytes : item.Value, 
            (_, _) => item.IsBinary? item.ValueBytes : item.Value);
    }
    
    /// <summary>
    ///  Add item to context from request header
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    public static void AddItem(this ServiceContext context, string key)
    {
        context.AddItem(key, null);
        
    }
    
}