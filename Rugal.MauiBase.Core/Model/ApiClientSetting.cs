namespace Rugal.MauiBase.Core.Model;

public class ApiClientSetting
{
    public event Action<HttpResponseMessage> OnRetry;
    public event Action<Exception> OnRetryThrow;
    public event Action<Exception> OnThrow;
    public event Action<HttpResponseMessage> OnError;

    public event Action OnCalling;
    public event Action<HttpResponseMessage, object> OnSuccess;
    public event Action OnComplete;

    private string _Domain;
    public string Domain
    {
        get => _Domain;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            _Domain = value.TrimEnd('/') + '/';
        }
    }
    public string Token { get; set; }
    public string FormBodyKey { get; set; } = "Body";
    public int RetryCount { get; set; } = 1;
    public int RetryDelay { get; set; } = 500;
    public bool IsRetry { get; set; }
    public bool IsRetryThrow { get; set; }
    public List<int> RetryCodes { get; set; }
    public void Retry(HttpResponseMessage ApiResponse)
    {
        OnRetry?.Invoke(ApiResponse);
    }
    public void RetryThrow(Exception ApiResponse)
    {
        OnRetryThrow?.Invoke(ApiResponse);
    }
    public void Throw(Exception Ex)
    {
        OnThrow?.Invoke(Ex);
    }
    public void Error(HttpResponseMessage ApiResponse)
    {
        OnError?.Invoke(ApiResponse);
    }
    public void Calling()
    {
        OnCalling?.Invoke();
    }
    public void Success(HttpResponseMessage ApiResponse, object Result)
    {
        OnSuccess?.Invoke(ApiResponse, Result);
    }
    public void Complete()
    {
        OnComplete?.Invoke();
    }
}