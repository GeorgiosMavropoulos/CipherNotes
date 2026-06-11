using Cipher_Notes.Services;
using Cipher_Notes.ViewModels;
using Cipher_Notes.Views;
using Microsoft.Extensions.Logging;

namespace Cipher_Notes
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
                });

            builder.Services.AddTransient<NoteListPage>();
            builder.Services.AddTransient<NoteListViewModel>();

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
