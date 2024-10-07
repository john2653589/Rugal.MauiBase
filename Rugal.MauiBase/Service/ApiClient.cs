namespace Rugal.MauiBase.Service;
public class ApiClient
{
    protected readonly HttpClient Client;
    protected readonly ApiClientSetting Setting;
    public ApiClient(HttpClient Client, ApiClientSetting Setting)
    {
        this.Client = Client;
        this.Setting = Setting;
    }
    public ApiClient<TService> AsService<TService>() where TService : ApiServiceBase, new()
    {
        return new ApiClient<TService>(Client, Setting);
    }
    protected void BaseApiCall<TApiParam, TApiResult>(
        ApiInfoBase Info,
        ApiOptionBase<TApiParam, TApiResult> Option = null)
    {

    }
}
public class ApiClient<TService> : ApiClient where TService : ApiServiceBase, new()
{
    public TService Service { get; private set; }
    public ApiClient(HttpClient Client, ApiClientSetting Setting) : base(Client, Setting)
    {
        Service = new TService();
    }
    public ApiClient<TService> ApiCall<TApiResult>(
        Func<TService, ApiInfo<TApiResult>> ApiInfoFunc,
        Action<ApiOption<TApiResult>> OptionFunc = null)
    {
        InitApiSet(ApiInfoFunc);
        BaseApiCall(ApiInfo, Option);
        return this;
    }
    public ApiClient<TService> ApiCall<TApiParam, TApiResult>(
       Func<TService, ApiInfo<TApiParam, TApiResult>> ApiInfoFunc,
       Action<ApiOption<TApiParam, TApiResult>> OptionFunc = null) where TApiParam : class
    {
        var ApiInfo = SetInfo(ApiInfoFunc.Invoke(Service));

        var Option = new ApiOption<TApiParam, TApiResult>();
        OptionFunc(Option);
        BaseApiCall(ApiInfo, Option);
        return this;
    }
    public ApiClient<TService> ApiCallParam<TApiParam, TApiResult>(
        Func<TService, ApiInfo<TApiParam, TApiResult>> ApiInfoFunc,
        TApiParam Param,
        Action<ApiOption<TApiParam, TApiResult>> OptionFunc = null) where TApiParam : class
    {
        var ApiInfo = SetInfo(ApiInfoFunc.Invoke(Service));

        var Option = new ApiOption<TApiParam, TApiResult>();
        Option ??= new ApiOption<TApiParam, TApiResult>();
        SetParam(Param, ApiInfo, Option);
        BaseApiCall(ApiInfo, Option);
        return this;
    }

    protected void SetParam<TApiParam, TApiResult>(TApiParam Param, ApiInfoBase ApiInfo, ApiOptionBase<TApiParam, TApiResult> Option)
        where TApiParam : class
    {
        if (ApiInfo.Method == HttpMethodType.Get)
            Option.WithQuery(Param);
        else
            Option.WithParam(Param);
    }
    protected ApiInfo<TApiResult> SetInfo<TApiResult>(ApiInfo<TApiResult> Info)
    {
        if (Info.Method == HttpMethodType.None)
            Info.Method = Service.DefaultMethod;

        return Info;
    }

    protected void InitApiSet<TApiResult>(
        Func<TService, ApiInfo<TApiResult>> ApiInfoFunc, Action<ApiOption<TApiResult>> OptionFunc
        )
    {
        var ApiInfo = SetInfo(ApiInfoFunc.Invoke(Service));
        var Option = new ApiOption<TApiResult>();
        OptionFunc(Option);
    }

}
