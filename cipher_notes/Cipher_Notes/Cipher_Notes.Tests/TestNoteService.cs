using System;
using System.Collections.Generic;
using System.Text;
using Cipher_Notes.Core.Models;
using Moq;
using Cipher_Notes.Core.Services;


using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;

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




    }
}
