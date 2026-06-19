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

        Task<List<SecureNotes>> GetSecureNotes();
        Task<SecureNotes?> GetById(int id);
        Task Update(SecureNotes note);
        Task Delete(int id);
        Task InitAsync();
    }
}
