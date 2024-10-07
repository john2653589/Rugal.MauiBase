using System.Runtime.CompilerServices;

namespace Rugal.MauiBase.Core.Model;

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
    public HttpMethodType Method { get; set; }
    public ParamType ParamType { get; set; } = ParamType.None;
    public ApiInfoBase(string Path = null)
    {
        if (string.IsNullOrWhiteSpace(Path))
            throw new Exception($"{nameof(Path)} can not be null or empty.");

        this.Path = Path;
    }
    public ApiInfoBase(HttpMethodType Method, string Path = null) : this(Path)
    {
        this.Method = Method;
    }
}
public class ApiInfo<TResult> : ApiInfoBase
{
    public ApiInfo([CallerMemberName] string Path = null) : base(Path) { }
    public ApiInfo(HttpMethodType Method, [CallerMemberName] string Path = null) : base(Method, Path) { }
}
public class ApiInfo<TParam, TResult> : ApiInfoBase
{
    public ApiInfo([CallerMemberName] string Path = null) : base(Path) { }
    public ApiInfo(HttpMethodType Method, [CallerMemberName] string Path = null) : base(Method, Path) { }
}