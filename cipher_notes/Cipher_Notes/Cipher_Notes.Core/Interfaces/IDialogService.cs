using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Interfaces
{
    public interface IDialogService
    {
        //interface for the method to display UI messages
        Task ShowAlert(string title, string content, string button);
    }
}
