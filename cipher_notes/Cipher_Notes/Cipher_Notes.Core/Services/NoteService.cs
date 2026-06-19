using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Interfaces;
namespace Cipher_Notes.Core.Services
{
    public class NoteService : INoteService
    {
        private readonly IDatabaseService databaseService;
        private readonly IEncryptionService encryptionService;

        public NoteService(IDatabaseService db, IEncryptionService crypto)
        {
            databaseService = db;
            encryptionService = crypto;
        }

        //create note method
        public async Task CreateNote(string title, string content, string password)
        {
            try
            {
                //return an error message if content or title is missing

                ValidateInputs(content, title);

                //encrypt note's content
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

            //get all notes method
            public async Task<List<SecureNotes>> GetAllNotes()
            {

            try
            {
                //retrieve all notes from db!
                return await databaseService.GetSecureNotes();
                

            }
            catch (Exception ex) 
            {
                throw new Exception($"Failed to get notes", ex);
            }
              
            }

            //get note by id
            public async Task<SecureNotes?> GetNoteById(int id)
                {
                try
                {
                   //create a new object to retrieve the desired note
                   var note = await databaseService.GetById(id);

                   //return an error message if note does not exist
                   if(note == null)
                   {
                    throw new NotFoundException("Note not found");
                   }

                  //return the note
                   return note;
                   }
                catch (Exception ex) //return a general exception error
                {
                throw new Exception($"Failed to get note", ex);
                }
                

        }


            //decrypt note method
            public async Task<string> DecryptNote(int id, string password)
            {
            try
            {
                //retrieve selected note
                var note = await databaseService.GetById(id);

                //return an error message if note does not exists
                if (note == null)
                {
                    throw new NotFoundException("Note not found");
                }

                    //if note exists continue decryption
                    return encryptionService.DecryptContent
                    (
                        note.Encrypted_content,
                        password,
                        note.Salt,
                        note.IV

                    );

            }
            catch (InvalidPasswordException ex) //return an error message if password is wrong
            {
                throw new InvalidPasswordException("Wrong password",ex);
            }

            catch (FormatException e)
            {
                throw new ValidationException("Decryption error", e);
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

                //reuturn an error message if existingNote is null
                if(existingNote == null)
                {
                    throw new NotFoundException("Note does not exist");
                }


                //validate password
               
                    var decrypted_note = encryptionService.DecryptContent
                   (
                       existingNote.Encrypted_content,
                       password,
                       existingNote.Salt,
                       existingNote.IV
                   );




                    //return an error message if content or title is missing
                    ValidateInputs(content, title);

                    //encrypt updated content
                    var (cipher, salt, iv) = encryptionService.EncryptNote(content, password);

                    //update note
                    existingNote.Title = title;
                    existingNote.Encrypted_content = cipher;
                    existingNote.Salt = salt;
                    existingNote.IV = iv;
                    existingNote.Updated_at = DateTime.Now;

                    //send query in the DB
                    await databaseService.Update(existingNote);

                }
                  
                catch(ValidationException) //return an error message if use has not completed all the inputs in the form
                {
                    throw;
                }
                catch (InvalidPasswordException) //return an error message if password is wrong
                {
                throw;
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
                    throw new NotFoundException("Note not found");
                }
           
                //delete note if exists
                await databaseService.Delete(id);

            }
            catch(NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error.Deletion failed", ex);
            }
               
            
           

        }

        //method to return an error message if content or title is missing
        private void ValidateInputs(string content, string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                throw new ValidationException("Title is empty");
                
            }
            else if(string.IsNullOrWhiteSpace(content))
            {
                throw new ValidationException("Content is empty");
            }
           
        }
        }
    }
