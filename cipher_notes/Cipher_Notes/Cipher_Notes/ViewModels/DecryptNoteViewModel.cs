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
    public partial class DecryptNoteViewModel : ObservableObject
    {
        //declare variables
        private readonly NoteService note_service;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string decrypted_content;


        //declare constructor
        public DecryptNoteViewModel(NoteService note_service)
        {
            this.note_service = note_service;
        }

        //decrypt note method
        [RelayCommand]
        public async Task DecryptNote(int id)
        {
            //try catch method to return error message in UI
            try
            {
                //if all goes well use the decrypt method from NoteService
                decrypted_content = await note_service.DecryptNote(id, password);   

                //use on property changed to update UI immediately
                OnPropertyChanged(nameof(decrypted_content));
                
              

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK"); //return an error message if sth goes wrong
              
            }

        }
    }
}
