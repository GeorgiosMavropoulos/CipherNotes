using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Moq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

namespace Cipher_Notes.Tests
{
    //unit tests for DatabseService
    public class TestDatabaseService : IAsyncLifetime, IDisposable
    {
        //declare variables
        private readonly DatabaseService dbService; //create a new db service object
        private readonly string testDbPath; //create a dbpath variable

        //declare constructor
        public TestDatabaseService()
        {


            //create a unique temporary db object for each test
            testDbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");//use Guid to get a new different db object each time.

            //initialize a new db object for each test and assign the temp path
            dbService = new DatabaseService(testDbPath);


        }



        //create the Dispose() method to delete temp db path after each test
        public async Task DisposeAsync()
        {
            //close sqlite connection
            await dbService.CloseAsync();

            if (File.Exists(testDbPath))
            {
                File.Delete(testDbPath);//delete file
            }
        }
        public void Dispose() { }//empty dispose is added in order IDisposable to be implemented in class

        public Task InitializeAsync() => Task.CompletedTask;//initialize AsyncLifeTime for each test

        //-------------Test Create----------------------------//
        [Fact]
        public async Task Test_Create_Successfully_Creates_Note_and_GetById_Successfully_Returns_The_Created_Note()
        {
            //Arrange

            //declare variables
            var title = "Title";
            var encrypted_text = "test";
            var salt = "Salt";
            var iv = "Iv";

            //create a new SecureNotes object
            var note = new SecureNotes
            {
                Title = title,
                Encrypted_content = encrypted_text,
                Salt = salt,
                IV = iv,
                Created_at = DateTime.Now
            };

            //Act
            //insert object using Create method from DatabaseService
            await dbService.Create(note);

            //declare a variable and return the object inside this var.Use GetById to retrieve note
            var result = await dbService.GetById(note.Id);


            //Assert
            //verifry that result is not null
            Assert.NotNull(result);
            //verify that stored object's title is equal to declared title
            Assert.Equal(title, result.Title);

            //verify that stored object's salt and iv are equal to the declared ones
            Assert.Equal(salt, result.Salt);
            Assert.Equal(iv, result.IV);
        }


        //test that Create method returns DatabaseException
        [Fact]
        public async Task Test_Create_Method_Returns_DatabaseException()
        {
            //Arrange
            //declare variables
            var exception_message = "Failed to save note in the Database";

            //create the mocked db object
            var mocked_db = new Mock<IDatabaseService>();

            //create note
            var note = new SecureNotes
            {
                Title = "title",
                Encrypted_content = "text",
                Salt = "salt",
                IV = "iv",
                Created_at = DateTime.Now

            };

            //set up the DB to return DatabaseException when the mocked db object is being called
            mocked_db.Setup(x => x.Create(note)).ThrowsAsync(new DatabaseException(exception_message, new Exception("Sql query failed")));
            //Act
            //close connection so as Create will fail            
            await dbService.CloseAsync();

            //call the Create method and return the exception inside a variable
            var ex = await Assert.ThrowsAsync<DatabaseException>(() => mocked_db.Object.Create(note));



            //Assert
            //verify exception messagei s equals to the expected one
            Assert.Equal(exception_message, ex.Message);
        }


        //-------------------------------------------------Test Update-----------------------------------------------------//

        //Intergration test.Test that Update successfully updates notes
        [Fact]
        public async Task Test_Update_Successfully_Updates_Notes()
        {
            //Arrange
            //declare variables
            var original_tile = "title";
            var original_content = "content";
            var updated_title = "updated_title";
            var updated_content = "updated_content";
            var salt = "salt";
            var iv = "iv";

            //create SecureNotes object 
            var note = new SecureNotes
            {
                Title = original_tile,
                Encrypted_content = original_content,
                Salt = salt,
                IV = iv
            };

            //Create the note
            await dbService.Create(note);



            //Act
            //update the note
            note.Title = updated_title;
            note.Encrypted_content = updated_content;
            note.Updated_at = DateTime.Now;//add update time

            //call the method to update the note
            await dbService.Update(note);

            //call GetById method to retrieve note
            var result = await dbService.GetById(note.Id);

            //Assert
            //verify result is not null
            Assert.NotNull(result);

            //verify updated note's title is equal to updated_title
            Assert.Equal(updated_title, result.Title);

            //verify updated note's content is equal to updated_content
            Assert.Equal(updated_content, result.Encrypted_content);


        }

        //test Update method returns ValidationException if note is null. Exception message should be:Note does not exist
        [Fact]
        public async Task Test_Update_Returns_ValidationException_If_Note_Is_Null()
        {
            //Arrange

            //declare variables
            var exception_message = "Note does not exist";
            //Create empty note
            SecureNotes note = null;


            //Act
            //call Update method and return validation exception
            var ex = await Assert.ThrowsAsync<ValidationException>(()=>dbService.Update(note));

            //Assert
            //verify exception_message is equals to ex.Message
            Assert.Equal(exception_message, ex.Message);

        }

        //test Update returns NotFoundException if db query return 0 rows had changes.
        [Fact]
        public async Task Test_Update_Returns_NotFoundException_If_DB_Query_Return_0_Rows_Changed()
        {
            //Arrange
            //declare variables
            var expected_exception = "Something went wrong.Note not found or not updated";

            //Create SecureNotes object and assign id = 99999. This id does not exists
            var note = new SecureNotes
            {
                Id = 99999,
                Title = "Test",
                Encrypted_content = "test",
                Salt = "Test",
                IV = "Test",
                Updated_at = DateTime.Now,
            };

            //Act delegate Update method into a variable. Force Update method to return NotFoundException 
            var ex = await Assert.ThrowsAsync<NotFoundException>(()=> dbService.Update(note));

            //Assert
            //verify returned excception message is equals to expected_exception
            Assert.Equal(expected_exception, ex.Message);

        }
    }
}
