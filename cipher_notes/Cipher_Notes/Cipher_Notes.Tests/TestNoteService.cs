using System;
using System.Collections.Generic;
using System.Text;
using Cipher_Notes.Core.Models;
using Moq;
using Cipher_Notes.Core.Services;

using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;

namespace Cipher_Notes.Tests
{
    //unit tests for NotService
    public class TestNoteService
    {

        //declare variables
        private readonly NoteService _noteService;//create NoteService object
        private readonly Mock<IDatabaseService> mocked_db;// create a mocked DatabaseService object
        private readonly Mock<IEncryptionService> mocked_encryption;//crreate a mocked encryption's service object


        //create a constructor and create a new object each time the test runs to nullify previous data for each test
        public TestNoteService()
        {
            mocked_db = new Mock<IDatabaseService>(); // initialize object mocked_db
            mocked_encryption = new Mock<IEncryptionService>(); // initialize object mocked_encryption

            _noteService = new NoteService( //initialize note's service object and attach Objects of interface's instances
                mocked_db.Object,
                mocked_encryption.Object
          );

        }


        }
}
