using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Services
{
    public class NoteService
    {
        //declaring variables
        private DatabaseService databaseService;

        private EncryptionService encryptionService;

        //declare constructor
        public NoteService()
        {

        }

        //create note method
        public void CreateNote(string title, string content, string password)
        {
            //try-catch method to handle unexpected errors
            try
            {
                
            }catch(Exception ex)
            {
                throw new Exception("Cannot create the note", ex);
            }
        }
    }
}
