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

    }
}
