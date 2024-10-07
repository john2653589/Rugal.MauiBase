using Rugal.MauiBase.Core.Model;

namespace Rugal.MauiBase.Core.Service;

public abstract class ApiServiceBase
{
    public abstract string BasePath { get; }
    public virtual HttpMethodType DefaultMethod { get; } = HttpMethodType.Get;
}