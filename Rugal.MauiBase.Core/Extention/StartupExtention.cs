using Rugal.ConfigFiller.Maui.Extention;
using Rugal.DotNetLib.Core.ValueParse;
using Rugal.MauiBase.Core.Model;
using Rugal.MauiBase.Core.Service;

namespace Rugal.MauiBase.Core.Extention;

public static class StartupExtention
{
    public static MauiAppBuilder UseMauiBaseCore(this MauiAppBuilder Builder)
    {
        Builder.Services
            .AddDotNetLib_ValueParse()
            .AddSingleton<HttpClient>()
            .AddTransient<ApiClient>();

        Builder.UseConfigFiller();
        Builder.Services.AddSingleton(new ApiClientSetting()
        {
            Domain = Builder.Configuration["ApiClient:Domain"],
        });
        return Builder;
    }
}