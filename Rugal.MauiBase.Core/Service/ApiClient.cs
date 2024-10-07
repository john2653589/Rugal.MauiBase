using Rugal.MauiBase.Core.Model;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Rugal.MauiBase.Core.Service;
public class ApiClient
{
    public readonly HttpClient Client;
    public readonly ApiClientSetting Setting;

    public event Action<object, ApiSet, HttpResponseMessage> OnSuccess;
    public ApiClient(HttpClient Client, ApiClientSetting Setting)
    {
        this.Client = Client;
        this.Setting = Setting;
    }
    public ApiClient<TService> AsService<TService>() where TService : ApiServiceBase, new()
    {
        var CreateClient = new ApiClient<TService>(this);
        return CreateClient;
    }
    public async void ApiCall<TResult>(ApiSet CallApiSet)
    {
        var Info = CallApiSet.Info;
        var Option = CallApiSet.Option;
        if (CallApiSet.Info.Method == 0)
            throw new Exception("Method can not be 'None'");

        using var SendRequest = GenerateRequest(CallApiSet);

        var InSafeNext = true;
        var SendCount = 0;

        Setting.Calling();
        Option.Calling();

        while (InSafeNext)
        {
            SendCount++;
            InSafeNext = SendCount <= Setting.RetryCount;
            try
            {
                var ApiResponse = await Client.SendAsync(SendRequest);
                if (ApiResponse.IsSuccessStatusCode)
                {
                    object ApiResult;
                    if (typeof(TResult) == typeof(string))
                        ApiResult = await ApiResponse.Content.ReadAsStringAsync();
                    else
                        ApiResult = await ApiResponse.Content.ReadFromJsonAsync<TResult>();

                    Setting.Success(ApiResponse, ApiResult);
                    Option.Success(ApiResult);

                    OnSuccess?.Invoke(ApiResult, CallApiSet, ApiResponse);

                    Setting.Complete();
                    Option.Complete();
                    return;
                }

                if (Setting.IsRetry && InSafeNext)
                {
                    Setting.Retry(ApiResponse);
                    continue;
                }

                Setting.Error(ApiResponse);
                Option.Error(ApiResponse);
                break;
            }
            catch (Exception ex)
            {
                if (Setting.IsRetryThrow && InSafeNext)
                {
                    Setting.RetryThrow(ex);
                    continue;
                }
                Setting.Throw(ex);
                Option.Throw(ex);
                break;
            }
        }

        Setting.Complete();
        Option.Complete();
    }
    public ApiClient WithToken(string Token)
    {
        Setting.Token = Token;
        return this;
    }
    protected string GenerateUrl(ApiSet Set)
    {
        var Paths = new List<string>();
        if (!Regex.IsMatch(Set.BasePath, "^http", RegexOptions.IgnoreCase))
            Paths.Add(Setting.Domain);
        Paths.Add(Set.BasePath);
        Paths.Add(Set.Info.Path);

        var ClearPaths = Paths
            .Select(Item => Item.TrimStart('/').TrimEnd('/'))
            .ToArray();

        var Url = string.Join('/', ClearPaths).TrimEnd('?');
        var QueryString = Set.Option.GenerateQueryString();
        if (QueryString is not null)
            Url += $"?{QueryString}";

        return Url;
    }
    protected HttpRequestMessage GenerateRequest(ApiSet Set)
    {
        var Url = GenerateUrl(Set);
        var SendRequest = new HttpRequestMessage(new HttpMethod(Set.Info.Method.ToString()), Url);
        if (!string.IsNullOrWhiteSpace(Setting.Token))
            SendRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Setting.Token);

        if (Set.Info.Method == HttpMethodType.Get)
            return SendRequest;

        if (Set.Info.ParamType == ParamType.None || Set.Info.ParamType == ParamType.Body)
        {
            var JsonString = JsonSerializer.Serialize(Set.Option.Param ?? new { });
            SendRequest.Content = new StringContent(JsonString, Encoding.UTF8, "application/json");
            return SendRequest;
        }

        var FormDataContent = new MultipartFormDataContent();
        foreach (var File in Set.Option.Files)
        {
            if (!File.HasFile)
                throw new Exception("File buffer is null, empty, or file path not found");

            HttpContent Content;
            var FileName = File.UploadFileName ?? $"File_{DateTime.Now:yyyyMMddHHmmssff}";
            if (File.Type == FormFileType.FilePath)
            {
                var GetInfo = new FileInfo(File.FilePath);
                FileName = GetInfo.Name;
                Content = new StreamContent(GetInfo.OpenRead());
            }
            else if (File.Type == FormFileType.Buffer)
                Content = new ByteArrayContent(File.Buffer);
            else if (File.Type == FormFileType.Stream)
                Content = new StreamContent(File.Stream);
            else
                throw new Exception("FormFile type cannot be accessed");

            Content.Headers.ContentType = new MediaTypeHeaderValue(File.ContentType);
            FormDataContent.Add(Content, File.Key, FileName);
        }

        if (Set.Option.Param is not null)
        {
            var JsonString = JsonSerializer.Serialize(Set.Option.Param);
            var BodyContent = new StringContent(JsonString, Encoding.UTF8, "application/json");
            FormDataContent.Add(BodyContent, Setting.FormBodyKey);
        }
        SendRequest.Content = FormDataContent;
        return SendRequest;
    }
}
public class ApiClient<TService> where TService : ApiServiceBase, new()
{
    protected ApiClientSetting Setting => Api.Setting;
    protected ApiClient Api;
    public TService Service { get; private set; } = new TService();
    public ApiClient(ApiClient Api)
    {
        this.Api = Api;
    }
    public ApiClient<TService> WithToken(string Token)
    {
        Api.WithToken(Token);
        return this;
    }
    public ApiClient<TService> ApiCall<TResult>(
        Func<TService, ApiInfo<TResult>> ApiInfoFunc,
        Action<ApiOption<TResult>> OptionFunc = null)
    {
        var ApiSet = CreateApiSet(ApiInfoFunc, OptionFunc);
        Api.ApiCall<TResult>(ApiSet);
        return this;
    }
    public ApiClient<TService> ApiCall<TParam, TResult>(
       Func<TService, ApiInfo<TParam, TResult>> ApiInfoFunc,
       Action<ApiOption<TParam, TResult>> OptionFunc = null)
        where TParam : class
        where TResult : class
    {
        var ApiSet = CreateApiSet(ApiInfoFunc, OptionFunc, null);
        Api.ApiCall<TResult>(ApiSet);
        return this;
    }
    public ApiClient<TService> ApiCall_Param<TParam, TResult>(
        Func<TService, ApiInfo<TParam, TResult>> ApiInfoFunc, TParam Param,
        Action<ApiOption<TParam, TResult>> OptionFunc = null)
        where TParam : class
        where TResult : class
    {
        var ApiSet = CreateApiSet(ApiInfoFunc, OptionFunc, Param);
        Api.ApiCall<TResult>(ApiSet);
        return this;
    }
    public ApiClient<TService> ApiCall_Form<TParam, TResult>(
        Func<TService, ApiInfo<TParam, TResult>> ApiInfoFunc,
        Action<ApiOption<TParam, TResult>> OptionFunc = null
        )
        where TParam : class
    {
        ApiCall_FormParam(ApiInfoFunc, null, OptionFunc);
        return this;
    }
    public ApiClient<TService> ApiCall_Form<TResult>(
        Func<TService, ApiInfo<TResult>> ApiInfoFunc,
        Action<ApiOption<TResult>> OptionFunc = null)
    {
        var ApiSet = CreateApiSet(ApiInfoFunc, OptionFunc);
        ApiSet.Info.ParamType = ParamType.Form;
        Api.ApiCall<TResult>(ApiSet);
        return this;
    }
    public ApiClient<TService> ApiCall_FormParam<TParam, TResult>(
        Func<TService, ApiInfo<TParam, TResult>> ApiInfoFunc, TParam Param,
        Action<ApiOption<TParam, TResult>> OptionFunc = null
        )
        where TParam : class
    {
        var ApiSet = CreateApiSet(ApiInfoFunc, OptionFunc, Param);
        ApiSet.Info.ParamType = ParamType.Form;
        Api.ApiCall<TResult>(ApiSet);
        return this;
    }
    protected ApiSet CreateApiSet<TParam, TResult>(
        Func<TService, ApiInfo<TParam, TResult>> ApiInfoFunc,
        Action<ApiOption<TParam, TResult>> OptionFunc,
        TParam Param = null)
        where TParam : class
    {
        return BaseCreateApiSet<TParam, TResult, ApiOption<TParam, TResult>>(ApiInfoFunc, OptionFunc, Param);
    }
    protected ApiSet CreateApiSet<TResult>(
       Func<TService, ApiInfo<TResult>> ApiInfoFunc,
       Action<ApiOption<TResult>> OptionFunc)
    {
        return BaseCreateApiSet<object, TResult, ApiOption<TResult>>(ApiInfoFunc, OptionFunc, null);
    }
    protected ApiSet BaseCreateApiSet<TParam, TResult, TOption>(
        Func<TService, ApiInfoBase> ApiInfoFunc,
        Action<TOption> OptionFunc,
        TParam Param = null)
        where TParam : class
        where TOption : ApiOptionBase<TParam, TResult, TOption>, new()
    {
        var ApiInfo = ApiInfoFunc.Invoke(Service);
        if (ApiInfo.Method == HttpMethodType.None)
            ApiInfo.Method = Service.DefaultMethod;

        var Option = new TOption();
        if (Param is not null)
        {
            if (ApiInfo.Method == HttpMethodType.Get)
                Option.WithQuery(Param);
            else
                Option.WithParam(Param);
        }
        OptionFunc?.Invoke(Option);

        if (ApiInfo.Method == HttpMethodType.Get && !Option.HasQuery && Option.HasParam)
            Option.WithQuery(Option.Param)
                .ClearParam();

        var SetResult = new ApiSet()
        {
            BasePath = Service.BasePath,
            Option = new ApiOption()
                .WithOption<TParam, TResult, TOption>(Option),
            Info = ApiInfo,
        };

        return SetResult;
    }
}