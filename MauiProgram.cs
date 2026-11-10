using Microsoft.Extensions.Logging;
using MBUNAY_S6.Servicios;
using MBUNAY_S6.Vistas;

namespace MBUNAY_S6
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<EstudianteApiService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<Estudiante>();

            return builder.Build();
        }
    }
}
