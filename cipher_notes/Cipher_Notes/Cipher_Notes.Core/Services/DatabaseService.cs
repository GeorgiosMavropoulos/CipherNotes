
using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;

using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cipher_Notes.Core.Services
{
    public class DatabaseService:IDatabaseService
    {
        //declaring variables!

        private const string db = "cipher_notes.db"; //declaring db's name!

        private SQLiteAsyncConnection? _connection; //declaring a private readonly variable for the connection object!

        private bool initialized;

        private readonly SemaphoreSlim _initLock = new(1, 1);//created this variable in order to thread-safe init!

        //connection method
        public async Task InitAsync()
        {
            if (initialized) return;

            await _initLock.WaitAsync();

            try
            {
                if (initialized) return;

                if (_connection == null)
                    throw new NotFoundException("Database connection not initialized");

                await _connection.CreateTableAsync<SecureNotes>();

                
                initialized = true;
            }
            finally
            {
                _initLock.Release(); //release thread in order not to collide if 2 methods have been called simultaneoulsy
            }
        }

        //declare constructor!
        public DatabaseService(string dbPath)
        {
           
            _connection = new SQLiteAsyncConnection(dbPath); //initialize the connection
        }

        //create this method in order to terminate connection after each db process.For instance, when a note is being created, connection should be closed.
        //Otherwise it's difficult for intergration tests to work, because connection is still open
        public async Task CloseAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection = null;
                initialized = false;
            }
        }

        //create crud operations for the DB

        //get all notes function
        public async Task<List<SecureNotes>> GetSecureNotes()
        {

            //call the db initialization method if connection has not been initialized
            if (!initialized)
                await InitAsync();

            //try catch method to handle db errors
            try
            {
                //return all notes displaying the newrest first 
                return await _connection.Table<SecureNotes>().OrderByDescending(n => n.Created_at).ToListAsync();

            }catch(Exception ex)
            {
                throw new DatabaseException("Failed to retrive all notes", ex);
            }
            
        }

        //get a specific note by id function
        public async Task<SecureNotes?> GetById(int id)
        {
            //call the db initialization method if connection has not been initialized
            if (!initialized)
                await InitAsync();

            //try catch method to handle db errors
            try
            {
                //return the asked note
                return await _connection.Table<SecureNotes>().Where(x => x.Id == id).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw new DatabaseException("Failed to retrieve note", ex);
            }

        }

        //create a new note function
        public async Task Create(SecureNotes securenote)
        {
            //call the db initialization method if connection has not been initialized
            if (!initialized)
                await InitAsync();

            //try-catch method to handle unexpected errors
            try
            {
                await _connection.InsertAsync(securenote);//insert the new object's details into the Db

            }
            catch (Exception ex) //return a db exception
            {
                throw new DatabaseException("Failed to save note in the Database", ex);
            }
           
            
        }

        //update a note function
        public async Task Update(SecureNotes securenote)
        {

            //return an exception if note is null
            if(securenote == null)
            {
                throw new ValidationException("Note does not exist");
            }
            //call the db initialization method if connection has not been initialized
            if (!initialized)
                await InitAsync();

            //try-catch method to handle unexpected errors
            try
            {
                int rows = await _connection.UpdateAsync(securenote);//update the new object's details into the Db

                if (rows == 0) //return an error message if note does not exist or not updated
                    throw new  NotFoundException("Note not found or not updated");

            }
            catch(ValidationException)
            {
                throw; //throw validation exxception if note is null
            }
            catch(NotFoundException)
            {
                throw; //throw NotFoundException if note is null
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Failed to update note", ex);
            }

        }

        //delete note function
        public async Task Delete(int id)
        {
            //call the db initialization method if connection has not been initialized
            if (!initialized)
                await InitAsync();

            //try-catch method to handle unexpected errors
            try
            {
                //create the row variables in order to know whether deletion succeeded or not
                int rows = await _connection.DeleteAsync<SecureNotes>(id);//update the new object's details into the Db

               //return an error message if id does not exist
               if(rows == 0)
                {
                    throw new NotFoundException("Note does not exist");
                }

            }
            catch (Exception ex)
            {
                throw new DatabaseException("Failed to delete note", ex);
            }
        }



    }
}
