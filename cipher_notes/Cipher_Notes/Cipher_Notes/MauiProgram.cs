using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Cipher_Notes.Services;
using Cipher_Notes.Core.ViewModels;
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

           
           

           
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<IDatabaseService>(sp =>
    new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "cipher_notes.db")));//define path
            builder.Services.AddTransient<IEncryptionService, EncryptionService>();
            builder.Services.AddTransient<INoteService, NoteService>();
            builder.Services.AddTransient<IDialogService, DialogService>();
            builder.Services.AddTransient<CreateNoteViewModel>();
            builder.Services.AddTransient<DecryptNoteViewModel>();
            builder.Services.AddTransient<NoteListViewModel>();
            builder.Services.AddTransient<UpdateNoteViewModel>();
            builder.Services.AddTransient<ViewNoteViewModel>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
