using System.Runtime.CompilerServices;

namespace Rugal.MauiBase.Service;

public enum HttpMethodType
{
    None,
    Get,
    Post,
}
public enum ParamType
{
    None,
    Body,
    Form,
}

public abstract class ApiInfoBase
{
    public string Path { get; set; }
    public string Store { get; set; }
    public HttpMethodType Method { get; set; }
    public ParamType ParamType { get; set; } = ParamType.None;
}
public class ApiInfo<TApiResult> : ApiInfoBase
{
    public ApiInfo([CallerMemberName] string Path = null)
    {
        if (string.IsNullOrWhiteSpace(Path))
            throw new Exception($"{nameof(Path)} can not be null or empty.");

        this.Path = Path;
    }
    public ApiInfo(HttpMethodType Method, [CallerMemberName] string Path = null) : this(Path)
    {
        this.Method = Method;
    }
    public ApiInfo(HttpMethodType Method, ParamType ParamType, [CallerMemberName] string Path = null) : this(Method, Path)
    {
        this.ParamType = ParamType;
    }
}
public class ApiInfo<TApiParam, TApiResult> : ApiInfo<TApiResult>
{
    public ApiInfo([CallerMemberName] string Path = null) : base(Path)
    {

    }
}
