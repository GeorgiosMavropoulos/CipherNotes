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
                
                //return an error message if content/password or title is missing
                ValidateInputs(content, title, password);

               

                var note = new SecureNotes //create the note object and apply the new title
                {
                    Title = title,
                    
                    Created_at = DateTime.Now
                };

                //encrypt note
                await ApplyEncryption(note, content, password);

                await databaseService.Create(note);
            }
            catch(ValidationException) //catch all validation exceptions
            {
                throw;
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
                    catch(NotFoundException)
                    {
                        throw; //return the caught exception
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
                var note = await GetNoteById(id);

               
                    //if note exists continue decryption
                    return encryptionService.DecryptContent
                    (
                        note.Encrypted_content,
                        password,
                        note.Salt,
                        note.IV

                    );

            }
            catch(ValidationException)
            {
                throw new ValidationException("Password is missing");//return an error message if password is missing
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

                //return an error message if content/password or title is missing
                ValidateInputs(content, title, password);
                //loading note
                var note = await GetNoteById(id);

                // first decrypt content
                //validate password

                var decrypted_note = await DecryptNote(id, password); //call the method to decrypt note

                    //update note's title
                    note.Title = title;

                    //encrypt note
                    await ApplyEncryption(note, content, password);
                    note.Updated_at = DateTime.Now;
                              

                //send query in the DB to update note's details
                await databaseService.Update(note);

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
                   
            //method to apply encryption
            public async Task ApplyEncryption(SecureNotes note, string content, string password)
            {
                //encrypt update content
                var (cipher, salt, iv) = encryptionService.EncryptNote(content, password);

               note.Encrypted_content = cipher;
               note.Salt = salt;
               note.IV = iv;
            }
        
             

           //delete note method
           public async Task Delete(int id)
            {

            //try catch method to handle unexpected errors
            try
            {
                //loading note
                var note = await GetNoteById(id);

           
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
        private void ValidateInputs(string content, string title,string password)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                throw new ValidationException("Title is empty");
                
            }
            else if(string.IsNullOrWhiteSpace(content))
            {
                throw new ValidationException("Content is empty");
            }
            else if(string.IsNullOrEmpty(password))
            {
                throw new ValidationException("Password is missing");
            }
           
        }
        }
    }
