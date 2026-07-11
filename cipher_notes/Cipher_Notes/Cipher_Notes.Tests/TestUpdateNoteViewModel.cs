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

    }
}
