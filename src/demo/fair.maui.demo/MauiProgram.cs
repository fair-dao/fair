using Microsoft.Extensions.Logging;

namespace fair.maui.demo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            return builder
                .UseMauiApp<App>()
              .ConfigureFonts(fonts =>
              {
                  fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              }).RunFairHost(
                new fair.extensions.main.Extender()).Result;

        }
    }
}
