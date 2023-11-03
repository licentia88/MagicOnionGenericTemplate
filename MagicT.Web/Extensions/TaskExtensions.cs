namespace MagicT.Web.Extensions;

public static class TaskExtensions
{
    public static async Task<T> OnComplete<T>(this Task<T> task, Func<T, Task> func)
    {
        var data = await task;
        await func.Invoke(data);
        return data;
    }

    public static async Task<T> OnComplete<T>(this Task<T> result, Action<T> action)
    {
        var data = await result;
        action.Invoke(data);
        return data;
    }

    public static async Task<T> OnComplete<T, TArg>(this Task<T> task, Action<T, TArg> action, TArg arg)
    {
        var data = await task;
        action.Invoke(data, arg);
        return data;
    }

    public static async Task<T> OnComplete<T>(this Task<T> result, Action action)
    {
        var data = await result;
        action.Invoke();
        return data;
    }

    public static async Task OnComplete(this Task task, Func<Task> func)
    {
        await task;
        await func.Invoke();
    }

    public static async Task OnComplete(this Task task, Action action)
    {
        await task;
        action.Invoke();
    }

    public static async Task OnComplete<T>(this Task task, Action<T> action, T arg)
    {
        await task;
        action.Invoke(arg);
    }
}