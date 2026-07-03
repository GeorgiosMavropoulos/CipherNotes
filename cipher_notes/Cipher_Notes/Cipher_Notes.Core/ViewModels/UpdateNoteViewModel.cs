using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Cipher_Notes.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cipher_Notes.Core.ViewModels
{
    public partial class UpdateNoteViewModel:ObservableObject
    {
        //declare variables
        private readonly INoteService note_service;

        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string decryptedContent = string.Empty; //declare this variable in order to store  the decrypted content decryption method returns


        [ObservableProperty]
        private SecureNotes? note;

        //declare a constructor
        public UpdateNoteViewModel(INoteService note_service)
        {
            this.note_service = note_service;
        }


        //method to load note
        [RelayCommand]
        public async Task<SecureNotes?> LoadNote(int id)
        {

            //retrieve note
            return await note_service.GetNoteById(id);
        }

        //method to decrypt note
        [RelayCommand]
        public async Task Decrypt()
        {
            //decrypt through NoteService's decryption method
            DecryptedContent = await note_service.DecryptNote(Id, Password);    
        }
        //method to create the update command
        [RelayCommand]
        public async Task Update(int id)
        {
            //try-catch to handle unexpected errors
            try
            {
                //use the update method from Note Service
                await note_service.UpdateNote(id, Title, DecryptedContent, Password);
            }
            catch (Exception ex)
            {
                throw; //return the catched error message
            }

        }
    }
}
