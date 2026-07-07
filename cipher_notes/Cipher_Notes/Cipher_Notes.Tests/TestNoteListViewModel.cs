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
    public class TestNoteListViewModel
    {

        //create a NoteListViewModel object and a NoteService's mocked object
        private readonly Mock<INoteService> note_service_mocked;

        private readonly NoteListViewModel note_list_view_model;

        //declare constructor
        public TestNoteListViewModel()
        {
            //initialize a new note_service_mocked object and a note_list_view_model object each time a test run
            note_service_mocked = new Mock<INoteService>();
            note_list_view_model = new NoteListViewModel(note_service_mocked.Object);
        }


        //test load notes successfully returns existing notes
        [Fact]
        public async Task Test_LoadNotes_Successfully_Loads_Existing_Notes()
        {
            //Arrange
            //create 2 notes
            var note1 = new SecureNotes
            {
                Id = 1,
                Title = "Note 1",
                Encrypted_content = "content1",
                Salt = "salt1",
                IV = "iv1"
            };

            var note2 = new SecureNotes
            {
                Id = 2,
                Title = "Note 2",
                Encrypted_content = "content2",
                Salt = "salt2",
                IV = "iv2"
            };

            var note_list = new List<SecureNotes> { note1, note2 };

            //set up GetAllNotes from NoteService to return the list from above when GetAllNotes is being called
            note_service_mocked.Setup(x => x.GetAllNotes()).ReturnsAsync(note_list);

            //Act
            //call LoadNotes from ViewModel using LoadNotesCommand
            await note_list_view_model.LoadNotesCommand.ExecuteAsync(null);

            //Assert
            
            //verify GetAllNotes has been called once
            note_service_mocked.Verify(x => x.GetAllNotes(), Times.Once());

            //verify that 2 notes have been returned
            Assert.Equal(2, note_list.Count);

            //verify that note1's title is equals to the returned note's title
            Assert.Equal(note1.Title, note_list[0].Title);

            //verify that note2's title is equals to the returned note's title
            Assert.Equal(note2.Title, note_list[1].Title);

            
        }
    }
}
