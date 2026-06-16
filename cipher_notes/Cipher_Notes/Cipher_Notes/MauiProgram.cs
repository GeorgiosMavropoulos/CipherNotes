using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Cipher_Notes.ViewModels;

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

           
            builder.Services.AddTransient<NoteListViewModel>();

           
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<DatabaseService>(sp =>
    new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "cipher_notes.db")));//define path
            builder.Services.AddTransient<EncryptionService>();
            builder.Services.AddTransient<NoteService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
