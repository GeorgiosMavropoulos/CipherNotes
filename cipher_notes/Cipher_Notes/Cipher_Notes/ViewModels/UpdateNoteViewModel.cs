using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cipher_Notes.ViewModels
{
    public partial class UpdateNoteViewModel:ObservableObject
    {
        //declare variables
        private readonly NoteService note_service;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

        [ObservableProperty]
        private string password;


        //declare a constructor
        public UpdateNoteViewModel(NoteService note_service)
        {
            this.note_service = note_service;
        }

        //method to create the update command
        [RelayCommand]
        public async Task Update(int id)
        {
            //try-catch to handle unexpected errors
            try
            {
                //use the update method from Note Service
                await note_service.UpdateNote(id,Title, Content, Password);
            }
            catch (Exception ex)
            {
                throw; //return the catched error message
            }

        }
    }
}
