using Rugal.MauiBase.Service;

namespace Rugal.MauiBase.Extention;

public static class StartupExtention
{
    public static MauiAppBuilder UseRugalsMauiBase(this MauiAppBuilder Builder)
    {
        Builder.Services.AddSingleton<HttpClient>();
        Builder.Services.AddSingleton<ApiClient>();
        return Builder; 
    }
}