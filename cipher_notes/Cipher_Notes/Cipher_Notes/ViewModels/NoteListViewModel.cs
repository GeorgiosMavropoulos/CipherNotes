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
    public partial class NoteListViewModel: ObservableObject
    {
        //declaring variables
       
        private readonly NoteService note_service;

        public ObservableCollection<SecureNotes> Notes { get; } = new ObservableCollection<SecureNotes>();

        //declare constructor
        public NoteListViewModel(NoteService note_service)
        {
            this.note_service = note_service;

        }

        //create load notes method
        [RelayCommand]
        public async Task LoadNotes()
        {
            //try-catch function to return error message if sth went wrong
            try
            {
                //using GetAllNotes function from NoteService to retrieve all notes
                var notes = await note_service.GetAllNotes();

                Notes.Clear();//clear previous notes

                //add notes to the list
                foreach (var note in notes)
                {
                    Notes.Add(note);    //add note to the list
                }

            }
            catch (Exception ex) 
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            
        }

        //create delete note method
        [RelayCommand]
        public async Task DeleteNote(int id)
        {
            //try-catch function to return error message if sth went wrong
            try
            {
                //retrieve note by its id
                var note = await note_service.GetNoteById(id);


                

                //return an error message if note does not exist
                if(note != null)
                {
                    var note_to_remove = Notes.FirstOrDefault(x => x.Id == id); //remove the first element with this id

                    await note_service.Delete(id);//delete from DB

                    //using Delete method from SecureNotes service
                    Notes.Remove(note_to_remove); //update UI also
                }
               
            }
            catch (Exception ex) 
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }

            

        }
    }
}
