using Microsoft.Extensions.Logging;

namespace fairdao.portal
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
                new fairdao.extensions.shared.Extender()).Result;

        }
    }
}
