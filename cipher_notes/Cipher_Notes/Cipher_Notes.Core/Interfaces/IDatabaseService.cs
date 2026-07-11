using Cipher_Notes.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Interfaces
{
    //created an iDatabaseService in order to use this both for mock tests and real DB functionality
    public interface IDatabaseService
    {

        //create some Tasks for all DatabaseService's methods
        Task Create(SecureNotes securenote);// creating a task for note creation

        Task<List<SecureNotes>> GetSecureNotes();//create a task for retrieving all notes
        Task<SecureNotes?> GetById(int id); //task to retrieve a specific note
        Task Update(SecureNotes note);//task to update notes
        Task Delete(int id);//deletion task
        
        //task to initialize connection
        Task InitAsync();

        Task CloseAsync();//task to close connection
    }
}
