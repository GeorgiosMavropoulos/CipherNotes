using Cipher_Notes.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Interfaces
{
    //NoteService Interface in order to seperate UI from Service so as the services do not know which UI uses those interfaces
    public interface INoteService
    {

        //Add all tasks into the interface 
        Task CreateNote(string title, string content, string password); //create note interface
        Task<List<SecureNotes>> GetAllNotes(); //get all notes interface

        Task<SecureNotes?> GetNoteById(int id);//get note by id interface

        Task<string> DecryptNote(int id, string password);//DecryptNote Interface

        Task UpdateNote(int id, string title, string content, string password); //update note interface

        Task Delete(int id); //Delete interface
    }
}
