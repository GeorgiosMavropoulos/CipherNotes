using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Cipher_Notes.Tests
{
    //unit tests for NotService
    public class TestNoteService
    {

        //declare variables
        private readonly NoteService _noteService;//create NoteService object
        private readonly Mock<IDatabaseService> mocked_db;// create a mocked DatabaseService object
        private readonly Mock<IEncryptionService> mocked_encryption;//crreate a mocked encryption's service object


        //create a constructor and create a new object each time the test runs to nullify previous data for each test
        public TestNoteService()
        {
            mocked_db = new Mock<IDatabaseService>(); // initialize object mocked_db
            mocked_encryption = new Mock<IEncryptionService>(); // initialize object mocked_encryption

            _noteService = new NoteService( //initialize note's service object and attach Objects of interface's instances
                mocked_db.Object,
                mocked_encryption.Object
          );

        }


        //test that CreateNote  successfully creates a note
        [Fact]
        public async Task Test_CreateNote_ValidInput_Calls_Create()
        {
            //Arrange
            //encrypt a text using mock encryption with EncryptNote method. Method should return the encrypted text, salt and iv for decryption
            mocked_encryption.Setup(x => x.EncryptNote("content", "pass")).Returns(("cipherText", "salt", "IV"));

            //Act 
            //call the Create()method to create the note from NoteService
            await _noteService.CreateNote("title", "content", "pass");

            //Assert
            //verify that database.Create method from NoteService has been called once
            mocked_db.Verify(x => x.Create(It.IsAny<SecureNotes>()),Times.Once);



        }

        //test that CreateNote returns validation exception with the correct message when user does not insert title
        [Fact]
        public async Task Test_CreateNote_Returns_ValidationException_When_Title_Missing()
        {
            //Arrange 
            var title = string.Empty;
            var password = "1234";
            var content = "test";

            //Act and Assert
            //call the CreateNote method using _noteService object. Assert that ValidationException will be returned
            var exception = await Assert.ThrowsAsync<ValidationException>(()=> _noteService.CreateNote(title, content, password));

            //Assert
            //Assert that error message is equals to Title is empty
            Assert.Equal("Title is empty", exception.Message);
        }

        //test that CreateNote returns validation exception with the correct message when user does not insert content
        [Fact]
        public async Task Test_CreateNote_Returns_ValidationException_When_Content_Missing() 
        {
            //Arrange 
            var title = "test";
            var password = "1234";
            var content = string.Empty;

            //Act and Assert
            //call the CreateNote method using _noteService object. Assert that ValidationException will be returned
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _noteService.CreateNote(title, content, password));

            //Assert
            //Assert that the error message is equals to 'Content is empty'
            Assert.Equal("Content is empty", exception.Message);
        }

        //test that CreateNote returns a validation exception iwth the correct error message (Password is missing) if password is missing
        [Fact]
        public async Task Test_CreateNote_Returns_ValidationException_When_Password_Missing()
        {
            //Arrange 
            var title = "test";
            var password = string.Empty;
            var content = "test";

            //Act and Assert
            //call the CreateNote method using _noteService object. Assert that ValidationException will be returned
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _noteService.CreateNote(title, content, password));

            //Assert
            //Assert that the error message is equals to 'Password is missing'
            Assert.Equal("Password is missing", exception.Message);

        }

        //test that GetAllNotes returns a list with a note. This method creates a mocked note, and it must return a single item inside a list
        [Fact]
        public async Task Test_GetAllNotes_Returns_A_List_With_A_Single_Note()
        {
            //
            //Arrange
            //add a single note called title inside the db with a mocked object
            mocked_db.Setup(x => x.GetSecureNotes()).ReturnsAsync(new List<SecureNotes> {new SecureNotes { Title = "title"} });

           //Act
            
            //call GetAllNotes method to return the note
            var result = await _noteService.GetAllNotes();

            //Assert
            //verify that get all notes returns the note with title as 'title
            Assert.Single(result);//assert that a single item is being returned
            Assert.Equal("title", result[0].Title);//verify that note in the position zero inside the list has been returned with a title :title

            //assert that it quered the DB
            //verify that database.Create method from NoteService has been called once
            mocked_db.Verify(x => x.GetSecureNotes(), Times.Once);

        }

        //test that GetAllNotes returns am empty list if db is empty.
        [Fact]
        public async Task Test_GetAllNotes_Returns_An_Empty_List_If_DB_Is_Empty()
        {
            //Arrange
            //use the mocked db object, but leave it empty.
            mocked_db.Setup(x => x.GetSecureNotes()).ReturnsAsync(new List<SecureNotes>());

            //Act
            //call GetAllNotes method
            var result = await _noteService.GetAllNotes();

            //Assert that the list has been returned
            Assert.NotNull(result);
            //Assert that result is empty
            Assert.Empty(result);   
        }

        //test that the NoteService allows the exception of GetAllNotes to be returned
        [Fact]
        public async Task Test_GetAllNotes_Returns_The_Expected_Exception()
        {
            //Arrange
            // use the mocked db object to return the exceptions
            mocked_db.Setup(x => x.GetSecureNotes()).ThrowsAsync(new Exception("Db error"));

            //Act 
            var service = new NoteService(mocked_db.Object,mocked_encryption.Object);
            //call the method to return the exception
            var ex = await Assert.ThrowsAsync<Exception>(()=> service.GetAllNotes());

            //Assert that the the exception message is equals to 'Failed to get notes'
            Assert.Equal("Failed to get notes", ex.Message);
        }



    }
}
