namespace MagicT.Client.Services.JsInterop;

public interface ICookieService
{
    public Task SetValue(string key, string value, int? days = null);
    public Task<string> GetValue(string key, string def = "");
}

