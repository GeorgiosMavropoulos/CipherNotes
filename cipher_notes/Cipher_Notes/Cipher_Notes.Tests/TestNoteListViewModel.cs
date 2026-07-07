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
    }
}
