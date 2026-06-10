using Cipher_Notes.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Cipher_Notes.Services
{
    public class NoteService
    {
        private readonly DatabaseService databaseService;
        private readonly EncryptionService encryptionService;

        public NoteService(DatabaseService db, EncryptionService crypto)
        {
            databaseService = db;
            encryptionService = crypto;
        }

        //create note method
        public async Task CreateNote(string title, string content, string password)
        {
            try
            {
                var (cipher, salt, iv) = encryptionService.EncryptNote(content, password);

                var note = new SecureNotes
                {
                    Title = title,
                    Encrypted_content = cipher,
                    Salt = salt,
                    IV = iv,
                    Created_at = DateTime.Now
                };

                await databaseService.Create(note);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


            //update note method
            public async Task UpdateNote(int id,string title, string content, string password)
             {

            //try catch to handle unexpected errors
            try
            {
                // first decrypt content

                //loading note
                var existingNote = await databaseService.GetById(id);


                //validate password
                try
                {
                    var decrypted_note = encryptionService.DecryptContent
                   (
                       existingNote.Encrypted_content,
                       password,
                       existingNote.Salt,
                       existingNote.IV
                   );

                }
                catch 
                {
                    throw new Exception("Incorrect password");
                }

                //encrypt updated content
                var (cipher, salt, iv) = encryptionService.EncryptNote(content,password);

                //update note
                existingNote.Title = title;
                existingNote.Encrypted_content = cipher;
                existingNote.Salt = salt;
                existingNote.IV = iv;
                existingNote.Updated_at = DateTime.Now;       
                
                //send query in the DB
                await databaseService.Update(existingNote);

            }
            catch (Exception ex) 
            {
                //return error message
                throw new Exception(ex.Message, ex);
            
            }
                   




             }

           //delete note method
           public async Task Delete(int id)
            {

            //try catch method to handle unexpected errors
            try
            {
                //loading note
                var retrieve_Note = await databaseService.GetById(id);

                //return an error message if note does not exist
                if(retrieve_Note == null)
                {
                    throw new Exception("Note not found");
                }
                //delete note if exists
                await databaseService.Delete(id);



            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException("Error.Deletion failed", ex);
            }
               
            
            



        }
        }
    }
