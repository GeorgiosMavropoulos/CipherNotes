using System;
using System.Collections.Generic;
using System.Text;
using Cipher_Notes.Core.Interfaces;
using Microsoft.Maui.Controls;

namespace Cipher_Notes.Services
{
    public class DialogService:IDialogService
    {
        //method to display the alrt messages
        public async Task ShowAlert(string title, string message, string button)
        {
            await App.Current.MainPage.DisplayAlertAsync(title, message, button);
        }
    }
}
