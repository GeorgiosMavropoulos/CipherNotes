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
    public class TestCreateNoteViewModel
    {
        //create a mocked noteservice object
        private readonly Mock<INoteService> mocked_note_service;

        //create a readonly CreateNoteViewModel's object
        private readonly CreateNoteViewModel create_note_view_model;

        //declare constructor 
        public TestCreateNoteViewModel()
        {
            //set up a new note service mocked object to be created each time the test runs
            mocked_note_service = new Mock<INoteService>();

            //set up a new view model's object to be created each time the test runs and pass a parameter note's services object
            create_note_view_model = new CreateNoteViewModel(mocked_note_service.Object);
        }

        //Test create note successfully creates a note
        [Fact]
        public async Task Test_CreateNote_Successfully_Creates_Notes()
        {
            //Arrange  

            //assign to object's variables title, content, pass values.
            create_note_view_model.Title = "Title";
            create_note_view_model.Content = "content";
            create_note_view_model.Password = "password";

            //set up service to return completes.task when CreateNote method will be called
            mocked_note_service.Setup(x => x.CreateNote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);


            //Act 
            //call CreateNote command
            await create_note_view_model.CreateNoteCommand.ExecuteAsync(null);

            //Assert
            //verify the method will return the values assigned above
            mocked_note_service.Verify(x => x.CreateNote("Title", "content", "password"),Times.Once); //verify method has been called once
        }


        //Test create note successfully returns exception
        [Fact]
        public async Task Test_CreateNote_Returns_General_Exception()
        {
            //Arrange

            //declare a variable to store the expected message
            var expected_message = "Error, cannot create the note";

            //set up the CreateNote method via mocked_service object to return the exception
            mocked_note_service.Setup(x => x.CreateNote
            (It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Error, cannot create the note"));


            //Act
            //call CreateNote via CreateNote command using the view model's object. Delegate this method into a variable to store the exception
            var ex = await Assert.ThrowsAsync<Exception>(
                () => create_note_view_model.CreateNoteCommand.ExecuteAsync(null));
        }
    }
}
