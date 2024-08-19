using AppLanches.Services;
using Microsoft.Extensions.Logging;

namespace AppLanches
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.UseMauiApp<App>();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ApiService>();

            return builder.Build();
        }
    }
}
