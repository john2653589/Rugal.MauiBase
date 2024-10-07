namespace Rugal.MauiBase.Extention;

public static class ApplicationExtention
{
    public static IServiceProvider GetProvider(this Application App)
    {
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
