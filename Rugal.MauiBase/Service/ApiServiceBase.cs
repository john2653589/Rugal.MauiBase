namespace Rugal.MauiBase.Service;

public abstract class ApiServiceBase
{
    public abstract string BasePath { get; }
    public virtual HttpMethodType DefaultMethod { get; } = HttpMethodType.Get;
}