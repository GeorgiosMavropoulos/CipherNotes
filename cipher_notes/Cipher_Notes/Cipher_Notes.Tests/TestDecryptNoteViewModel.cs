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


        //test

    }
}
