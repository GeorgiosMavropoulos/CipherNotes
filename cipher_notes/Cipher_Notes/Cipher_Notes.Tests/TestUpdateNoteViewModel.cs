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
    }
}
