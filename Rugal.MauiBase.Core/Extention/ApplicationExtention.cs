using Rugal.MauiBase.Core.ApplicationBase;
using Rugal.MauiBase.Core.Extention;

namespace Rugal.MauiBase.Core.Extention;

public static class ApplicationExtention
{
    public static IServiceProvider GetProvider(this Application App)
    {
        if(App is null)
            throw new ArgumentNullException(nameof(App));

        if (App is ProviderApp ProviderApp)
            return ProviderApp.Provider;

        if (App.Handler?.MauiContext is null)
            throw new Exception("Service provider can only be accessed after OnHandlerChanged() or if the Application is inherited from ProviderApp.");

        return App.Handler.MauiContext.Services;
    }
    public static TService GetService<TService>(this Application App)
    {
        return App.GetProvider().GetService<TService>();
    }
    public static TService GetRequiredService<TService>(this Application App)
    {
        return App.GetProvider().GetRequiredService<TService>();
    }
}