using System;
using System.Collections.Generic;
using System.Text;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.ViewModels;
using Cipher_Notes.Core.Services;
using Moq;
using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Xunit.Sdk;


namespace Cipher_Notes.Tests
{

    //tests for UpdateNoteViewModel
    public class TestUpdateNoteViewModel
    {
        //create an UpdateNoteViewModel's object and a NoteService's mocked object
        private readonly Mock<INoteService> note_service_mocked;
        private readonly UpdateNoteViewModel update_note_view_model;

        //delegate a constructor
        public TestUpdateNoteViewModel()
        {
            //initialize each object each time the test run
            note_service_mocked = new Mock<INoteService>();
            update_note_view_model = new UpdateNoteViewModel(note_service_mocked.Object);
        }


        ///------------------------------------------------Test LoadNote----------------------------------////////////////

        //test LoadNote successfully returns a note
        [Fact]
        public async Task Test_LoadNote_Successfully_Returns_The_Requested_Note()
        {
            //Arrange

            //declare variables
            var content = "test";
            var title = "title";

            //create a new note
            var note = new SecureNotes
            {
                Id = 1,
                Title = title,
                Encrypted_content = content,
                Salt= "salt",
                IV = "iv"
            };
           

            //set up through note_service_mocked object the GetNoteById method to return the created note
            note_service_mocked.Setup(x => x.GetNoteById(1)).ReturnsAsync(note);

            //Act
            //execute LoadNoteCommand from view model
           await update_note_view_model.LoadNoteCommand.ExecuteAsync(1);

            //Assert
            //verify GetNoteById has been called once
            note_service_mocked.Verify(x=> x.GetNoteById(1),Times.Once());

        

           
        }

        //test LoadNote returns general exception
        [Fact]
        public async Task Test_LoadNote_Returns_General_Exception()
        {
            //Arrange
            //declare a variable expected_exception message 
            string expected_exception_message = "Failed to get note";

            //set up GetNoteById from NoteService to return a general exception using note_service_moced object
            note_service_mocked.Setup(x => x.GetNoteById(1)).Throws(new Exception("Failed to get note"));

            //Act
            //execute LoadNoteCommand. Delegate it into a variable to store the result. Assert it will return an Exception
            var exception = await Assert.ThrowsAsync<Exception>(() => update_note_view_model.LoadNoteCommand.ExecuteAsync(1));


            //Assert
            //verify exception's message is equal to expected_exception_message 
            Assert.Equal(expected_exception_message, exception.Message);
        }


        ///--------------------------------Test Decrypt--------------------------------------------///

        //test Decrypt method successfully decrypts the note
        [Fact]
        public async Task Test_Decrypt_Successfully_Decrypts_Note()
        {
            //Arrange
            //declare variables
            var password = "123";
            var decrypted_content = "decrypted";
            
            var id = 1;
         

            //Assign note's id into model's Id property
            update_note_view_model.Id = id;
            //assign password into model's password property
            update_note_view_model.Password = password;

            //set up DecryptNote from NoteService using the mocked object to return the note
            note_service_mocked.Setup(x => x.DecryptNote(id, password)).Returns(Task.FromResult(decrypted_content));


            //Act
            //execute DecryptCommand to decrypt text
            await update_note_view_model.DecryptCommand.ExecuteAsync(id);

            //Assert
            //verify DecryptNote from NoteService has been called
            note_service_mocked.Verify(x=> x.DecryptNote(id, password), Times.Once());

            //verify decrypted_content's value is equal to update_note_view_model.DecryptedContent value
            Assert.Equal(decrypted_content, update_note_view_model.DecryptedContent);
        }


        //test Decrypt returns an exception. The other exceptions have been tested via NoteService
        [Fact]
        public async Task Test_Decrypt_Return_Exception()
        {
            //Arrange

            //declare variables
            
            string password = "pass";
            int id = 1;
            string expected_exception_message = "Unexpected error during decryption";

            //
            //Assign note's id into model's Id property
            update_note_view_model.Id = id;
            //assign password into model's password property
            update_note_view_model.Password = password;

            //set up DecryptNote from NoteService to retun the general exception
            note_service_mocked.Setup(x => x.DecryptNote(id, password)).Throws(new Exception("Unexpected error during decryption"));

            //Act
            //create a variable called exception and delagete the DecryptCommand. This variable will store the returned exception's message.
            //Assert it will return the Exception
            var exception = await Assert.ThrowsAsync<Exception>(()=> update_note_view_model.DecryptCommand.ExecuteAsync(id));

            //Assert
            //verify exception's message is equal to expected_exception_message
            Assert.Equal(expected_exception_message, exception.Message);
        }




        ///---------------------------------Testing Update---------------------------------------------------------///

        //test Update successfully updates a note
        [Fact]
        public async Task Test_Update_Successfully_Updates_Note()
        {
            //Arrange
            //declare variables
            int id = 1;
            string title = "title";
            string password = "pass";
            string content = "content";
            
            //create a new note object
            var note = new SecureNotes
            {
                Id = id,
                Title = title,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

           
            
            //set up note service's object to return the completed task when UpdateNote is being called
            note_service_mocked.Setup(x=>x.UpdateNote(id, title, content, password)).Returns(Task.CompletedTask);

            //Assign variable's values to model's properties
            update_note_view_model.Title = title;

            update_note_view_model.DecryptedContent = content;
            update_note_view_model.Id = id;
            update_note_view_model.Password = password;

            //Act
            //execute UpdateCommand from view model
            await update_note_view_model.UpdateCommand.ExecuteAsync(id);

            //Assert 

            //verify that UpdateNote has been called once
            note_service_mocked.Verify(x => x.UpdateNote(id, title, content, password), Times.Once);
        }

        //test that Update method returns NotFoundException if id does not exist
        [Fact]
        public async Task Test_Update_Returns_NotFoundException()
        {
            //Arrange
            //declare a variable to store the expected exception message
            var expected_exception_message = "Note not found";

            //declare variables
            var title = "title";
            var content = "content";
            var pass = "pass";
            var id = 2;

            //set up UpdateNote from NoteService to return NotFoundException when is being called
            note_service_mocked.Setup(x => x.UpdateNote(id, title, content, pass)).Throws(new NotFoundException("Note not found"));

            //assign value's to model's properties
            update_note_view_model.Title = title;
            update_note_view_model.DecryptedContent = content;
            update_note_view_model.Id = id;
            update_note_view_model.Password = pass;

            //Act
            //execute UpdateCommand and delegate the result into a variable. Assert it returns a NotFoundException
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => update_note_view_model.UpdateCommand.ExecuteAsync(2));

            //Assert
            //verify that exception message is equals to expected_exception_message
            Assert.Equal(expected_exception_message, exception.Message);
        }

    }
}
