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
    //since I use the CommunityToolkit I do not have to manually add iCommands and the toolkit creates them by itself

    public partial class CreateNoteViewModel:ObservableObject
    {
        //declare variable properties
        private INoteService note_service;


        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string content;

        [ObservableProperty]
        private string password;

        //declare constructor
        public CreateNoteViewModel(INoteService note_service)
        {
            this.note_service = note_service; 
        }

        //create not function
        [RelayCommand]
        public async Task CreateNote()
        {
           
            //call create note method from NoteService to create the note
                await note_service.CreateNote(Title, Content, Password);
          
          

        }
    }
}
