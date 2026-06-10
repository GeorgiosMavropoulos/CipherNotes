using Cipher_Notes.Models;
using Cipher_Notes.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cipher_Notes.ViewModels
{
    //since I use the CommunityToolkit I do not have to manually add iCommands and the toolkit creates them by itself

    public partial class CreateNoteViewModel:ObservableObject
    {
        //declare variable properties
        private SecureNotes secure_note;

        public ObservableCollection<SecureNotes> Notes { get; } = new ObservableCollection<SecureNotes>();

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

        [ObservableProperty]
        private string password;

        //declare constructor
        public CreateNoteViewModel(SecureNotes secure_note)
        {
            this.secure_note = secure_note; 
        }

        //create not function
        public async Task CreateNote(string title, string content, string password)
        {
            //try-catch to handle unexpected errors
            try
            {

            }
            catch (Exception ex)
            {

            }

        }
    }
}
