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
    public class TestDecryptNoteViewModel
    {

        //create NoteService mocked object
        private readonly Mock<INoteService> _note_service_mocked;

        //DecryptNoteViewModel object
        private readonly DecryptNoteViewModel decrypt_viewModel;


        //declare constructor
        public TestDecryptNoteViewModel()
        {
            //initialize a new NoteService mocked object each time the class is being called
            _note_service_mocked = new Mock<INoteService>();

            //initialize a new ViewNoteViewModel object is being called
            decrypt_viewModel = new DecryptNoteViewModel(_note_service_mocked.Object);
        }


        //test DecryptNote successfully decrypts note
        [Fact]
        public async Task Test_DecryptNote_Successfully_Decrypt_Notes()
        {
            //Arrange
            
            //declare variables
            var content = "test";
            var title = "test";
            var password = "password";
            var decrypted_text = "decrypted test";

            //assign variable's values into model's properties
            decrypt_viewModel.Password = password;
            decrypt_viewModel.Id = 1;

           

            //set up _note_service_mocked to return the created note when DecryptNote is being called
            _note_service_mocked.Setup(x => x.DecryptNote(1, password)).Returns(Task.FromResult(decrypted_text));

            //Act
            //call DecryptNoteCommand
            await decrypt_viewModel.DecryptNoteCommand.ExecuteAsync(1);

            //Assert

            //verify DecryptNote from NoteService has been called once
            _note_service_mocked.Verify(x => x.DecryptNote(1, password), Times.Once);


            //verify decrypted_text is equals with decrypt_viewmodel's Decrypted_content
            Assert.Equal(decrypted_text, decrypt_viewModel.Decrypted_content);


        }

    }
}
