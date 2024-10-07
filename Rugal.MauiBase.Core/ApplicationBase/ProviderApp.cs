namespace Rugal.MauiBase.Core.ApplicationBase;

public class ProviderApp : Application
{
    public IServiceProvider Provider { get; private set; }
    public ProviderApp(IServiceProvider Provider)
    {
        this.Provider = Provider;
    }
}