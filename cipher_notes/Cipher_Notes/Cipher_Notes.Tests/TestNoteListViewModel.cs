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

        //test LoadNotes successfully returns Exception if sth goes wrong
        [Fact]
        public async Task Test_LoadNotes_Returns_General_Exception()
        {
            //Arrange
            //declare a variable to store the expected exception message
            var expected_exception_message = "Failed to get notes";

            //set up GetAllNotes from NoteService to return the exception
            note_service_mocked.Setup(x => x.GetAllNotes()).Throws(new Exception("Failed to get notes"));


            //Act
            //delegate LoadNotesCommand in a variable to store the exception message. Assert it throws the Exception
            var exception = await Assert.ThrowsAsync<Exception>(() => note_list_view_model.LoadNotesCommand.ExecuteAsync(null));

            //Assert
            //verify exception message is equal to expected_exception_message
            Assert.Equal(expected_exception_message, exception.Message);
        }



        ///////////////////////////////////////////////////////Test DeleteNote//////////////////////////////////////////////////////////

        //test DeleteNote deletes note with success (Happy Path)
        [Fact]
        public async Task Test_DeleteNote_Deletes_Note_With_Success()
        {
            //Arrange

            //create a new note
            var note = new SecureNotes
            {
                Id = 1,
                Title = "Title",
                Salt = "salt",
                IV = "Iv"
            };

            //set up GetNoteById to return the created note
            note_service_mocked.Setup(x => x.GetNoteById(1)).ReturnsAsync(note);

            //Act
            //call DeleteNoteCommand
            await note_list_view_model.DeleteNoteCommand.ExecuteAsync(1);

            //Assert
            //verify Delete method from NoteService has been called at least once
            note_service_mocked.Verify(x => x.Delete(1), Times.Once);


        }


        //test that DeleteNote returns a general exception. Other exception have been tested through NoteService and DB service tests.
        [Fact]
        public async Task Test_DeleteNote_Returns_Exception()
        {
            //Arrange
            var expected_exception_message = "Error.Deletion failed";

            note_list_view_model.Id = 1; // assign an id value to Id property of the view model

            //set up GetNotById to return a new object
            note_service_mocked.Setup(x => x.GetNoteById(1)).ReturnsAsync(new SecureNotes { Id = 1 });

            //set up Delete from NoteService to return a general Exception
            note_service_mocked.Setup(x => x.Delete(1)).Throws(new Exception("Error.Deletion failed"));


            //Act
            //delegate DeleteNoteCommand in a variable to store the returned exception. Additionally, assert it returns an exception
            var exception = await Assert.ThrowsAsync<Exception>(() => note_list_view_model.DeleteNote(1));

            //Assert

            //verify exception's message is equals to expected_exception_message
            Assert.Equal(expected_exception_message, exception.Message);
        }

        //test DeleteNote successfully removes the deleted note from the list in order for the UI to get updated
        [Fact]
        public async Task Test_DeleteNote_Removes_Deleted_Notes_From_The_List()
        {
            //Arrange

            //create a new note
            var note = new SecureNotes { Id = 1, Title = "title", Salt = "salt", IV = "iv" };

            //add note into ViewModel's collection        

            note_list_view_model.Notes.Add(note);

            //set up GetNoteById to return the created note
            note_service_mocked.Setup(x => x.GetNoteById(1)).ReturnsAsync(note);

            //set up Delete from note_service_mocked to return completed task
            note_service_mocked.Setup(x => x.Delete(1)).Returns(Task.CompletedTask);

            //Act
            //execute DeleteNoteCommand
            await note_list_view_model.DeleteNoteCommand.ExecuteAsync(1);

            //Assert

            //verify that note collection does not contain the note with id= 1
            Assert.DoesNotContain(note_list_view_model.Notes, x => x.Id == 1);

            //verify that Note list's count has been decreased by 1
            Assert.Equal(0, note_list_view_model.Notes.Count);
        }


        //--------------------------------------------------Test FindNoteByTitle------------------------------///////////

        //test FindNoteByTitle successfully returns the requested note
        [Fact]
        public async Task Test_FindNoteByTitle_Successfully_Returns_Title()
        {
            
            //Arrange
            //Declare variables
            var title = "Title";

            //create a new note
            var note = new SecureNotes
            {
                Id = 1,
                Title = title,
                Encrypted_content = "test",
                Salt = "test",
                IV = "test"
            };

            //add the new note in the collection
            note_list_view_model.Notes.Add(note);

            //Act
            //call FindNoteByTitle method
            var result = note_list_view_model.FindNoteByTitle(title);

            //Assert
            //verify that result is not null
            Assert.NotNull(result);

            //verify that result.Title is equals to the declared title
            Assert.Equal(title, result.Title);  



        }
    }
}
