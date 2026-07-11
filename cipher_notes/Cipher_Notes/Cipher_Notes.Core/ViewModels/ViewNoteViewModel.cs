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
    //ViewNote's view model for ViewNote page
    public partial class ViewNoteViewModel : ObservableObject
    {
        //declaring variables

        private readonly INoteService note_service;//create a new object from INoteService interface

        //declare properties
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string decryptedContent;

        [ObservableProperty]
        private SecureNotes? note;
        public ObservableCollection<SecureNotes> Notes { get; } = new ObservableCollection<SecureNotes>();

        //declare constructor
        public ViewNoteViewModel(INoteService note_service)
        {
            this.note_service = note_service;

        }

        //method to load note
        [RelayCommand]
        public async Task<SecureNotes?> LoadNote(int id)
        {

            //retrieve note
            Note =  await note_service.GetNoteById(id); // save the returned data into note property
            return Note; //return note
        }


        //method to decrypt note
        [RelayCommand]
        public async Task Decrypt()
        {
            //decrypt through NoteService's decryption method
            DecryptedContent = await note_service.DecryptNote(Id, Password);
        }

    }
}
