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


    }
}
