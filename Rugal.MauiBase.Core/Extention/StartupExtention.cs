using Microsoft.Extensions.Configuration;
using Rugal.ConfigFiller.Maui.Extention;
using Rugal.MauiBase.Core.Model;
using Rugal.MauiBase.Core.Service;

namespace Rugal.MauiBase.Core.Extention;

public static class StartupExtention
{
    public static MauiAppBuilder UseMauiBaseCore(this MauiAppBuilder Builder)
    {
        Builder.Services.AddSingleton<HttpClient>();
        Builder.Services.AddTransient<ApiClient>();
        Builder.UseConfigFiller();
        Builder.Services.AddSingleton(new ApiClientSetting()
        {
            Domain = Builder.Configuration["ApiClient:Domain"],
        });
        return Builder;
    }
}