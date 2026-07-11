using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Cipher_Notes.Core.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Xunit.Sdk;

namespace Cipher_Notes.Tests
{

    //tests for ViewNoteViewModel.
    //Tests include LoadNote happy path and general exception. Same applies to Decrypt. Other exceptions have been tested in NoteService.
    public class Test_ViewNoteViewModel
    {
        //create NoteService mocked object
        private readonly Mock<INoteService> _note_service_mocked;

        //ViewNoteViewModel object
        private readonly ViewNoteViewModel _viewModel;

        //declare constructor
        public Test_ViewNoteViewModel()
        {
            //initialize a new NoteService mocked object each time the class is being called
            _note_service_mocked = new Mock<INoteService>();

            //initialize a new ViewNoteViewModel object is being called
            _viewModel = new ViewNoteViewModel(_note_service_mocked.Object);
        }

        /////////////////////////--------------Test LoadNote-------------------------------/////////////////

        //test LoadNote successfully returns the requested note
        [Fact]
        public async Task Test_LoadNote_Successfully_Returns_The_Requested_Note()
        {
            //Arrange

            //declare variables
            //the following variables have been declared in order to firstly create a note
            var title = "Title";
            var content = "content";

            //create a new secure note object
            var note = new SecureNotes
            {
                Id = 1,
                Title = title,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

            //set up using NoteService mocked object CreateNote method to return the above note when it's being called
            _note_service_mocked.Setup(x => x.GetNoteById(1)).ReturnsAsync(note);

            //Act
            //Use LoadNoteCommand from ViewNoteViewModel to request the note.
             await _viewModel.LoadNoteCommand.ExecuteAsync(1);

            //Assert
            //verify that the NoteService's method GetById has been called once
            _note_service_mocked.Verify(x => x.GetNoteById(1), Times.Once);

            //Assert that _viewModel.Title is equals to the declared title  from this method
            Assert.Equal(title, _viewModel.Note.Title);


        }


        //test LoadNote successfully returns general exception
        [Fact]
        public async Task Test_LoadNote_Successfully_Returns_General_Exception()
        {
            //Arrange
            //declare variables
            var expected_exception_message = "Failed to get note";

            //create a SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Title = "title",
                Encrypted_content = "title",
                Salt = "salt",
                IV = "iv"
            };

            //set up mocked_note_service to return the exception when the method GetNoteById is being called.
            //LoadNote uses this method from NoteService to retrieve notes
            _note_service_mocked.Setup(x => x.GetNoteById(1)).Throws(new Exception("Failed to get note"));

            //Act
            //call the LoadNoteCommand from ViewModel. Assert command returns the Exception.
            //Delegate this method into a variable in order to store the exception message.
            var exception = await Assert.ThrowsAsync<Exception>(() => _viewModel.LoadNoteCommand.ExecuteAsync(1));

            //Assert
            //verify the returned exception from LoadNoteCommand is equals to expected_exception_message
            Assert.Equal(expected_exception_message, exception.Message);

        }

        /////////////////////////--------------Test Decrypt-------------------------------/////////////////
        [Fact]
        public async Task Test_Decrypt_Success_Decrypts_Note()
        {
            //Arrange

            //declare variables
            var content = "title";
            var decrypted_content = "decrypted_text";
            var password = "password";

            //assign the declared values into view model's properties
            _viewModel.Id = 1;
            _viewModel.Password = password;

         //set up GetNoteById from NoteService to return the created note when it's being called. Use mocked_encryption_service for this purpose.
         _note_service_mocked.Setup(x=> x.DecryptNote(1, password)).Returns((Task.FromResult(decrypted_content)));

            //Act
            //using DecryptCommand from ViewModel decrypted the created note
            await _viewModel.DecryptCommand.ExecuteAsync(null);

            //Assert
            //Verify decrypted content is equals to the value of declared content
            Assert.Equal(_viewModel.DecryptedContent, decrypted_content);

        }

        //test Decrypt returns general exception with success
        [Fact]
        public async Task Test_Decrypt_Returns_General_Exception_With_Success()
        {
            //Arrange
            //declare variables
            var expected_exception_message = "Unexpected error during decryption";

            _viewModel.Id = 1; //define id
            _viewModel.Password = "password"; //define password

            //set up DecryptNote from NoteService to return the general exception
            _note_service_mocked.Setup(x => x.DecryptNote(1, "password")).Throws(new Exception("Unexpected error during decryption"));

            //Acct
            //use DecryptCommand and assert it'll will return the exception
            var exception = await Assert.ThrowsAsync<Exception>(() => _viewModel.DecryptCommand.ExecuteAsync(null));

            //Assert
            //Verify exception's message is equals to expected_exception_message
            Assert.Equal(expected_exception_message, exception.Message);
        }
    }
}
