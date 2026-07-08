using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Cipher_Notes.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Cipher_Notes.Core.Exceptions;

namespace Cipher_Notes.Core.ViewModels
{
    //since I use the CommunityToolkit I do not have to manually add iCommands and the toolkit creates them by itself
    public partial class NoteListViewModel: ObservableObject
    {
        //declaring variables
       
        private readonly INoteService note_service;

        public ObservableCollection<SecureNotes> Notes { get; } = new ObservableCollection<SecureNotes>();

        //declare constructor
        public NoteListViewModel(INoteService note_service)
        {
            this.note_service = note_service;

        }

        [ObservableProperty] //search query bound for UI search bar searching
        private string searchQuery = string.Empty;

        [ObservableProperty] //create an id property to make DeleteNote unit test work
        private int id;


        //when search query changes, this method informs automatically the query
        partial void OnSearchQueryChanged(string value)
        {
            OnPropertyChanged(nameof(FilteredNotes));
        }


        //filtering for search bar queries
        public IEnumerable<SecureNotes> FilteredNotes =>
            Notes.Where(n => n.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        //create load notes method
        [RelayCommand]
        public async Task LoadNotes()
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

        //create delete note method
        [RelayCommand]
        public async Task DeleteNote(int id)
        {
           
            
            //try-catch to handle null note exception
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
            catch (NotFoundException)
            {
                throw;
            }


        }

        //search query implementation. Searching if query matches and navigating directly
        public SecureNotes? FindNoteByTitle(string query)
        {

            var note = Notes.FirstOrDefault(n =>
                !string.IsNullOrWhiteSpace(n.Title) &&
                n.Title.Contains(query, StringComparison.OrdinalIgnoreCase));

            //throw a NotFoundException if note is null
            if(note == null)
            {
                throw new NotFoundException("Note does not exist");
            }

            return note;
        }
    }
}
