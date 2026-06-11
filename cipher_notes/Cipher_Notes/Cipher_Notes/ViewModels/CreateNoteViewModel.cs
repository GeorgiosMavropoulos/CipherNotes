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
        private NoteService note_service;


        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

        [ObservableProperty]
        private string password;

        //declare constructor
        public CreateNoteViewModel(NoteService note_service)
        {
            this.note_service = note_service; 
        }

        //create not function
        [RelayCommand]
        public async Task CreateNote()
        {
            //try-catch to handle unexpected errors
            try
            {
                await note_service.CreateNote(Title, Content, Password);
            }
            catch (Exception ex)
            {
                //display an error alert in UI
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");

            }

        }
    }
}
