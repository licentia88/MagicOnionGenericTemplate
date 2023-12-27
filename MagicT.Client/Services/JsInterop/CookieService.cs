using Microsoft.JSInterop;

namespace MagicT.Client.Services.JsInterop;

/// <summary>
/// Represents a service for managing cookies.
/// </summary>
[RegisterScoped]
public sealed class CookieService : ICookieService
{
    private readonly IJSRuntime _jsRuntime;
    private string _expires = "";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="jsRuntime"></param>
    public CookieService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        ExpireDays = 300;
    }

    /// <summary>
    /// Sets a cookie value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="days"></param>
    public async Task SetValue(string key, string value, int? days = null)
    {
        var curExp = (days != null) ? (days > 0 ? DateToUtc(days.Value) : "") : _expires;
        await SetCookie($"{key}={value}; expires={curExp}; path=/");
    }

    /// <summary>
    /// Gets a cookie value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public async Task<string> GetValue(string key, string def = "")
    {
        var cValue = await GetCookie();
        if (string.IsNullOrEmpty(cValue)) return def;

        var vals = cValue.Split(';');
        foreach (var val in vals)
            if (!string.IsNullOrEmpty(val) && val.IndexOf('=') > 0)
                if (val.Substring(0, val.IndexOf('=')).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    return val.Substring(val.IndexOf('=') + 1);
        return def;
    }

    private async Task SetCookie(string value)
    {
        await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
    }

    private async Task<string> GetCookie()
    {
        return await _jsRuntime.InvokeAsync<string>("eval", $"document.cookie");
    }

    /// <summary>
    /// Sets the expiration date of the cookie.
    /// </summary>
    public int ExpireDays
    {
        set => _expires = DateToUtc(value);
    }

    private static string DateToUtc(int days) => DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");
}

