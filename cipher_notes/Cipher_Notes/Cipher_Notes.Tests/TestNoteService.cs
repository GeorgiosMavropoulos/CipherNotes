using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

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

        //-------------Test CreateNote----------------------------//

        //test that CreateNote  successfully creates a note
        [Fact]
        public async Task Test_CreateNote_ValidInput_Calls_Create()
        {
            //Arrange
            //encrypt a text using mock encryption with EncryptNote method. Method should return the encrypted text, salt and iv for decryption
            mocked_encryption.Setup(x => x.EncryptNote("content", "pass")).Returns(("cipherText", "salt", "IV"));

            //Act 
            //call the Create()method to create the note from NoteService
            await _noteService.CreateNote("title", "content", "pass");

            //Assert
            //verify that database.Create method from NoteService has been called once
            mocked_db.Verify(x => x.Create(It.IsAny<SecureNotes>()), Times.Once);



        }

        //test that CreateNote returns validation exception with the correct message when user does not insert title
        [Fact]
        public async Task Test_CreateNote_Returns_ValidationException_When_Title_Missing()
        {
            //Arrange 
            var title = string.Empty;
            var password = "1234";
            var content = "test";

            //Act and Assert
            //call the CreateNote method using _noteService object. Assert that ValidationException will be returned
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _noteService.CreateNote(title, content, password));

            //Assert
            //Assert that error message is equals to Title is empty
            Assert.Equal("Title is empty", exception.Message);
        }

        //test that CreateNote returns validation exception with the correct message when user does not insert content
        [Fact]
        public async Task Test_CreateNote_Returns_ValidationException_When_Content_Missing()
        {
            //Arrange 
            var title = "test";
            var password = "1234";
            var content = string.Empty;

            //Act and Assert
            //call the CreateNote method using _noteService object. Assert that ValidationException will be returned
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _noteService.CreateNote(title, content, password));

            //Assert
            //Assert that the error message is equals to 'Content is empty'
            Assert.Equal("Content is empty", exception.Message);
        }

        //test that CreateNote returns a validation exception iwth the correct error message (Password is missing) if password is missing
        [Fact]
        public async Task Test_CreateNote_Returns_ValidationException_When_Password_Missing()
        {
            //Arrange 
            var title = "test";
            var password = string.Empty;
            var content = "test";

            //Act and Assert
            //call the CreateNote method using _noteService object. Assert that ValidationException will be returned
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _noteService.CreateNote(title, content, password));

            //Assert
            //Assert that the error message is equals to 'Password is missing'
            Assert.Equal("Password is missing", exception.Message);

        }

        ///----------------- Test GetAllNotes method --------------------//

        //test that GetAllNotes returns a list with a note. This method creates a mocked note, and it must return a single item inside a list
        [Fact]
        public async Task Test_GetAllNotes_Returns_A_List_With_A_Single_Note()
        {
            //
            //Arrange
            //add a single note called title inside the db with a mocked object
            mocked_db.Setup(x => x.GetSecureNotes()).ReturnsAsync(new List<SecureNotes> { new SecureNotes { Title = "title" } });

            //Act

            //call GetAllNotes method to return the note
            var result = await _noteService.GetAllNotes();

            //Assert
            //verify that get all notes returns the note with title as 'title
            Assert.Single(result);//assert that a single item is being returned
            Assert.Equal("title", result[0].Title);//verify that note in the position zero inside the list has been returned with a title :title

            //assert that it quered the DB
            //verify that database.Create method from NoteService has been called once
            mocked_db.Verify(x => x.GetSecureNotes(), Times.Once);

        }

        //test that GetAllNotes returns am empty list if db is empty.
        [Fact]
        public async Task Test_GetAllNotes_Returns_An_Empty_List_If_DB_Is_Empty()
        {
            //Arrange
            //use the mocked db object, but leave it empty.
            mocked_db.Setup(x => x.GetSecureNotes()).ReturnsAsync(new List<SecureNotes>());

            //Act
            //call GetAllNotes method
            var result = await _noteService.GetAllNotes();

            //Assert that the list has been returned
            Assert.NotNull(result);
            //Assert that result is empty
            Assert.Empty(result);
        }

        //test that the NoteService allows the exception of GetAllNotes to be returned
        [Fact]
        public async Task Test_GetAllNotes_Returns_The_Expected_Exception()
        {
            //Arrange
            // use the mocked db object to return the exceptions
            mocked_db.Setup(x => x.GetSecureNotes()).ThrowsAsync(new Exception("Db error"));

            //Act 
            var service = new NoteService(mocked_db.Object, mocked_encryption.Object);
            //call the method to return the exception
            var ex = await Assert.ThrowsAsync<Exception>(() => service.GetAllNotes());

            //Assert that the the exception message is equals to 'Failed to get notes'
            Assert.Equal("Failed to get notes", ex.Message);
        }



        ///----------------- Test GetANoteById method --------------------//


        //test that GetNoteById successfully retrieves the requested note
        [Fact]
        public async Task Test_GetNoteById_Returnes_The_Requested_Note()
        {
            //Arrange
            //create a mocked note
            var expectedNote = new SecureNotes { Id = 1, Title = "title" };
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(expectedNote);// when method will be called it should return this note

            //Act
            //retrieve note
            var note = await _noteService.GetNoteById(1);

            //Assert
            //validate that the list is not null
            Assert.NotNull(note);

            //verify that title matches with the expected note
            Assert.Equal(expectedNote.Title, note.Title);

            //verify that the id matches with the expected note
            Assert.Equal(expectedNote.Id, note.Id);

            //verify that the db has been called
            mocked_db.Verify(x => x.GetById(1), Times.Once());
        }


        //test that GetNoteById returns NotFoundException("Note not found") if note does not exists
        [Fact]
        public async Task Test_GetNoteById_Returns_NotFoundException_With_The_Appropriate_Message_When_Note_Does_Not_Exist()
        {
            //Arrange
            //mock a db object
            mocked_db.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((SecureNotes?)null); //create a null object

            //Act
            //try to retrieve note
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _noteService.GetNoteById(1));


            //Assert
            ////verify that the proper exception will be returned
            Assert.Equal("Note not found", ex.Message);

            //verify that the db has been called
            mocked_db.Verify(x => x.GetById(1), Times.Once());


        }

        //test that the NoteService allows the exception of GetNoteById to be returned
        [Fact]
        public async Task Test_GetNoteById_Returns_The_Expected_General_Exception()
        {
            //Arrange
            //create a mocked db object in order to create a note
            mocked_db.Setup(x => x.GetById(It.IsAny<int>())).ThrowsAsync(new Exception("DB error"));

            //Act
            var service = new NoteService(mocked_db.Object, mocked_encryption.Object);

            //call the method to return the exception
            var exception = await Assert.ThrowsAsync<Exception>(() => service.GetNoteById(1));

            //Assert
            //verify that the expected message will be: Failed to get note
            Assert.Equal("Failed to get note", exception.Message);

            //verify the DB has been called
            mocked_db.Verify(x => x.GetById(1), Times.Once());
        }

        //----------------------- TEST DecryptNote method-----------------------------------------//

        //test that DecryptNote method successfully decrypts the note. 
        [Fact]
        public async Task Test_DecryptNote_Successfully_Decrypts_Note()
        {
            //Arrange            
            //create a new SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "cipher",
                Salt = "salt",
                IV = "iv"

            };

            //create the mocked db object and return note
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //set up the mocked decryption. This mocked encryption returns "note" if anyone calls the DecryptContent method
            mocked_encryption.Setup(x => x.DecryptContent("cipher", "pass", "salt", "iv")).Returns("note");

            //act
            //call the real decrypt content method
            var decrypted_content = await _noteService.DecryptNote(1, "pass");

            //assert that decrypted content = "note"
            Assert.Equal("note", decrypted_content);

            //verify that DB has been called at least once

            mocked_db.Verify(x => x.GetById(1), Times.Once());

            //verify that DecryptContent has been called

            mocked_encryption.Verify(x => x.DecryptContent("cipher", "pass", "salt", "iv"), Times.Once());

        }

        //test that DecryptMethod will return validationexception if password is empty
        [Fact]
        public async Task Test_DecryptNote_Will_Return_ValidationException_If_Password_Is_Empty()
        {
            //Arrange
            //declare an empty variable called pass
            var pass = string.Empty;
            var expectedException = "Password is missing";
            //create a new SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "cipher",
                Salt = "salt",
                IV = "iv"

            };

            //create the mocked db object and return note
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //set up the mocked decryption. This mocked encryption returns "note" if anyone calls the DecryptContent method
            mocked_encryption.Setup(x => x.DecryptContent("cipher", pass, "salt", "iv")).Returns("note");

            //Act
            //Call DecryptNote with nullified pass
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _noteService.DecryptNote(1, pass));


            //Assert
            //Assert the ValidationException message is 'Password is missing'
            Assert.Equal(expectedException, ex.Message);

        }

        //test that DecryptContent will return InvalidPasswordException (Wrong Password) if password is wrong
        [Fact]
        public async Task Test_DecryptNote_Returns_InvalidPasswordException_If_Password_Is_Wrong()
        {
            //Arrange
            //declare variables

            var pass = "wrong pass";
            var expected_ex_message = "Wrong Password";

            //create secure note object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "cipher",
                Salt = "salt",
                IV = "iv"
            };

            //create the mocked db object and return the note
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //create the mocked encryption object and set it up to return the InvalidPasswordException
            //if sb calls DecryptContent. Add the correct password
            mocked_encryption.Setup(x => x.DecryptContent("cipher", pass, "salt", "iv")).Throws(new InvalidPasswordException("Wrong Password"));

            //Act
            //call DecryptContent and decrypt with a falsed password
            //declare it as exception to compare later
            //Assert the expected exception
            var ex = await Assert.ThrowsAsync<InvalidPasswordException>(() => _noteService.DecryptNote(1, pass));

            //Assert
            //verify the exception message is equals to expected_ex_message (Wrong Password)
            Assert.Equal(expected_ex_message, ex.Message);
        }

        //test that DecryptContent successfully returns NotFoundException
        [Fact]
        public async Task Test_DecryptContent_Returns_NotFoundException()
        {
            //Arrange
            //Create secure notes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "cipher",
                Salt = "salt",
                IV = "iv"
            };

            //create the mocked db object and return the note
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);


            //create the mocked encryption onject and set it up to throw a NotFoundException
            mocked_encryption.Setup(x => x.DecryptContent("cipher", "pass", "salt", "iv")).Throws(new NotFoundException("Note not found"));

            //Act 
            //call decryptContent method and use an invali id
            //Assert the expected exception
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _noteService.DecryptNote(5, "pass"));

            //Assert
            //verify exception message is equals to: Note does not exists
            Assert.Equal("Note not found", ex.Message);
        }

        //test DecryptContent returns CryptographicException
        [Fact]
        public async Task Test_DecryptNote_Returns_CryptographicException()
        {
            //Arrange
            //create  SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "cipher",
                Salt = "salt",
                IV = "iv"
            };

            //create the mocked db object
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //create the mocked encryption object with ItIsAny paremeters
            mocked_encryption.Setup(x => x.DecryptContent
            (
             It.IsAny<string>(),
              It.IsAny<string>(),
               It.IsAny<string>(),
                It.IsAny<string>()
            )).Throws(new CryptographicException("Decryption failed"));


            //Act
            //call the DecryptContent method and manipulate it to return CryptographicException
            var ex = await Assert.ThrowsAsync<CryptographicException>(() => _noteService.DecryptNote(1, "wrongpassword"));

            //Assert
            //assert that exception contains the following exception message: Decryption error
            Assert.Contains("Decryption error", ex.Message);
        }



        //test that DecryptNote returns Exception
        [Fact]
        public async Task Test_DecryptNote_Returns_Exception()
        {
            //Arrange
            var expected_message = "Unexpected error during decryption";
            //create SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "cipher",
                Salt = "salt",
                IV = "iv"
            };

            //set up the mocked db object
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //set up the mocked encryption to throw the exception
            mocked_encryption.Setup(x => x.DecryptContent(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Unexpected error during decryption"));

            //Act
            //call the DecryptNote method and manipulate it to throw the Exception msg
            var ex = await Assert.ThrowsAsync<Exception>(() => _noteService.DecryptNote(1, "pass"));

            //Assert
            //verify exception contains the expected exception message
            Assert.Equal(expected_message, ex.Message);
        }




        /// -------------------UpdateNote NoteService---------------------------------------///


        //test that update note updates the note successfully
        [Fact]
        public async Task Test_UpdateNote_updates_note_successfully()
        {
            //Arrange
            //declare variables
            var original_note = "original";

            var updated_note = "updated_note";
            var updated_title = "updated_title";
            var pass = "1234";

            //create a secure notes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = original_note,
                Salt = "salt",
                IV = "iv"

            };

            //created a mocked db object and return note
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);


            //call the decrypt content method
            mocked_encryption.Setup(x => x.DecryptContent(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Returns("original decrypted content");

            //encrypt via mocked_encryption the object
            mocked_encryption.Setup(x => x.EncryptNote(It.IsAny<string>(), It.IsAny<string>())).Returns(("cipherText", "salt", "IV"));

            //Act
            //call UpdateNote and save data into a new var
            await _noteService.UpdateNote(1, updated_title, updated_note, pass);


            //Assert    
            //Assert that the method in the DB has been called once
            mocked_db.Verify(x => x.Update(It.IsAny<SecureNotes>()), Times.Once());
        }

        //test UpdateNote successfully returns ValidationException if content is missing
        [Fact]
        public async Task Test_UpdateNote_Returns_ValidationException_if_Content_Is_Missing()
        {
            //Arrange
            //declare variables
            var content = string.Empty;
            var title = "title";
            var pass = "pass";
            var exception_message = "Content is empty";

            //create a secure Notes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked db object
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //create a mocked encryption object to decrypt content
            mocked_encryption.Setup
                (x => x.DecryptContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("Decrypted note");

            //encrypt updated note via encrypt mocked object
            mocked_encryption.Setup(x => x.EncryptNote(content, pass)).Throws(new ValidationException("Content is empty"));


            //Act
            // call UpdateNote method and return the exception's result in a variable
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _noteService.UpdateNote(1, title, content, pass));

            //Assert 
            //verify that exception message is 'Title is empty'
            Assert.Equal(exception_message, ex.Message);

        }

        //test UpdateNote successfully returns ValidationException if title is missing
        [Fact]
        public async Task Test_UpdateNote_Returns_ValidationException_if_Title_Is_Missing()
        {
            //Arrange
            //declare variables
            var content = "string";
            var title = string.Empty;
            var pass = "pass";
            var exception_message = "Title is empty";

            //create a secure Notes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked db object
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //create a mocked encryption object to decrypt content
            mocked_encryption.Setup
                (x => x.DecryptContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("Decrypted note");

            //encrypt updated note via encrypt mocked object
            mocked_encryption.Setup(x => x.EncryptNote(content, pass)).Throws(new ValidationException("Title is empty"));


            //Act
            // call UpdateNote method and return the exception's result in a variable
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _noteService.UpdateNote(1, title, content, pass));

            //Assert 
            //verify that exception message is 'Title is empty'
            Assert.Equal(exception_message, ex.Message);

        }

        //test UpdateNote successfully returns ValidationException if password is missing
        [Fact]
        public async Task Test_UpdateNote_Returns_ValidationException_if_Password_Is_Missing()
        {
            //Arrange
            //declare variables
            var content = "string";
            var title = "title";
            var pass = string.Empty;
            var exception_message = "Password is missing";

            //create a secure Notes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked db object
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //create a mocked encryption object to decrypt content
            mocked_encryption.Setup
                (x => x.DecryptContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("Decrypted note");

            //encrypt updated note via encrypt mocked object
            mocked_encryption.Setup(x => x.EncryptNote(content, pass)).Throws(new ValidationException("Password is missing"));


            //Act
            // call UpdateNote method and return the exception's result in a variable
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _noteService.UpdateNote(1, title, content, pass));

            //Assert 
            //verify that exception message is 'Title is empty'
            Assert.Equal(exception_message, ex.Message);

        }

        //test that UpdateNote returns InvalidPasswordException if password is wrong
        [Fact]
        public async Task Test_UpdateNote_Returns_InvalidPasswordException_if_Password_Is_Wrong()
        {
            //Arrange
            //Arrange
            //declare variables
            var content = "string";
            var title = "title";
            var pass = "pass";
            var wrong_pass = "wrong_pass";
            var exception_message = "Wrong password";

            //create a secure Notes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };


            //create a mocked db object
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //create a mocked encryption object to decrypt content
            mocked_encryption.Setup
           (x => x.DecryptContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
           .Throws(new InvalidPasswordException("Wrong password"));

            //Act
            //return an InvalidPasswordException by adding a wrong pass
            var ex = await Assert.ThrowsAsync<InvalidPasswordException>(() => _noteService.UpdateNote(1, title, content, wrong_pass));

            //Assert
            //verify that exception message is equals to 'Wrong password'
            Assert.Equal(exception_message, ex.Message);
        }

        //test UpdateNote returns NotFoundException if requested note does not exist
        [Fact]
        public async Task Test_UpdateNote_Returns_NotFoundException_If_Note_Does_Not_Exist()
        {
            //Arrange

            //declare variables
            var expected_message = "Note not found";


            //Act
            //call the UpdateNot method using a non existent note
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _noteService.UpdateNote(2, "title", "content", "pass"));

            //Assert that Exception message is equals to expected_message
            Assert.Equal(expected_message, ex.Message);
        }


        //test UpdateNote returns CryptographicException if Decryption fail
        [Fact]
        public async Task Test_UpdateNote_Returns_CryptographicException_If_Decryption_Fail()
        {
            //Arrange
            //declare variables
            var excepted_ex_message = "Decryption error";

            //create SecureNotes object

            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "test",
                Salt = "salt",
                IV = "iv"
            };

            //set up db to return the fake object when note with id 1 is being requested
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //set up mocked_encryption object to return the CryptographicException via DecryptNote method
            mocked_encryption.Setup(x => x.DecryptContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
             .Throws(new CryptographicException(excepted_ex_message));

            //Act
            //call update note and store the exception result's value in a variable/ Manipulate it to return the exception
            var ex = await Assert.ThrowsAsync<CryptographicException>
                (() => _noteService.UpdateNote(1, "title", "content", "pass"));


            //Assert
            //verify exception messages is:Cryptographic process failed
            Assert.Equal(excepted_ex_message, ex.Message);
        }


        //test UpdateNote returns CryptographicException if Encryption fail
        [Fact]
        public async Task Test_UpdateNote_Returns_CryptographicException_If_Encryption_Fail()
        {
            //Arrange
            //declare variables
            var excepted_ex_message = "Encryption failed";

            //create SecureNotes object

            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "test",
                Salt = "salt",
                IV = "iv"
            };

            //set up db to return the fake object when note with id 1 is being requested
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);

            //set up mocked_encryption object to return the CryptographicException via DecryptNote method
            mocked_encryption.Setup(x => x.DecryptContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("decrypted content");


            //encrypt via mocked_encryption object
            mocked_encryption.Setup(x => x.EncryptNote(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new CryptographicException(excepted_ex_message));

            //Act
            //call update note and store the exception result's value in a variable/ Manipulate it to return the exception
            var ex = await Assert.ThrowsAsync<CryptographicException>
                (() => _noteService.UpdateNote(1, "title", "content", "pass"));


            //Assert
            //verify exception messages is:Cryptographic process failed
            Assert.Equal(excepted_ex_message, ex.Message);
        }



        //-----------------------------ApplyEncryption-------------------------------------------//

        //test ApplyEncryption successfully encrypts note--
        [Fact]
        public async Task Test_ApplyEncryption_Successfully_Encrypts_Note()
        {
            //Arrange
            //Declare variables
            var content = "test";
            var pass = "pass";

            //create SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };


            //create mocked_encryption
            mocked_encryption.Setup(x => x.EncryptNote(content, pass)).Returns(("newCipher", "newSalt", "newIV"));

            //Act
            //encryptnote
            await _noteService.ApplyEncryption(note, content, pass);


            //Assert
            //verify EncryptNote has been called once
            mocked_encryption.Verify(x => x.EncryptNote(content, pass), Times.Once());

            //verify that newCipher is equals to note.Encrypted_content
            Assert.Equal("newCipher", note.Encrypted_content);

            //verify newSalt is equals to note.Salt
            Assert.Equal("newSalt", note.Salt);

            //verify newIV is equals to note.IV
            Assert.Equal("newIV", note.IV);
        }

        //test ApplyEncryption returns ValidationException if password is missing:Message should be Password is missing
        [Fact]
        public async Task Test_ApplyEncryption_Returns_ValidationException_If_Password_Is_Empty()
        {
            //Arrange

            //declare variables
            var content = "test";
            var password = string.Empty;
            var expected_exception_message = "Password is missing";

            //create SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked encryption object and manipulate it to return ValidationException
            mocked_encryption.Setup(x => x.EncryptNote(content, password)).Throws(new ValidationException("Password is missing"));

            //Act
            //call ApplyEncryption and wrap it into an exception variable
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _noteService.ApplyEncryption(note, content, password));

            //Assert
            //verify exception message is equals to expected_exception_message(Password is missing)
            Assert.Equal(expected_exception_message, ex.Message);
        }

        //test ApplyEncryption returns ValidationException if content is missing:Message should be Content is missing
        [Fact]
        public async Task Test_ApplyEncryption_Returns_ValidationException_If_Content_Is_Empty()
        {
            //Arrange

            //declare variables
            var content = string.Empty;
            var password = "test";
            var expected_exception_message = "Content is missing";

            //create SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked encryption object and manipulate it to return ValidationException
            mocked_encryption.Setup(x => x.EncryptNote(content, password)).Throws(new ValidationException("Content is missing"));

            //Act
            //call ApplyEncryption and wrap it into an exception variable
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _noteService.ApplyEncryption(note, content, password));

            //Assert
            //verify exception message is equals to expected_exception_message(Content is missing)
            Assert.Equal(expected_exception_message, ex.Message);
        }

        //test ApplyEncryption returns NotFoundException if content is missing:Message should be Content is missing
        [Fact]
        public async Task Test_ApplyEncryption_Returns_NotFoundException_If_Note_Is_Empty()
        {
            //Arrange

            //declare variables
            var content = string.Empty;
            var password = "test";
            var expected_exception_message = "Note does not exist";



            //create a null SecureNotes object
            SecureNotes? note = null;

            //create a mocked encryption object and manipulate it to return NotFoundException
            mocked_encryption.Setup(x => x.EncryptNote(content, password)).Throws(new NotFoundException("Note does not exists"));

            //Act
            //call ApplyEncryption and wrap it into an exception variable
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _noteService.ApplyEncryption(note, content, password));

            //Assert
            //verify exception message is equals to expected_exception_message(Note does not exist)
            Assert.Equal(expected_exception_message, ex.Message);
        }

        //test that AppleyEncryption returns successfully CryptographicException
        [Fact]
        public async Task Test_that_AppleyEncryption_Returns_Successfully_CryptographicException()
        {
            //Arrange

            //declare variables
            var content = "test";
            var password = "test";
            var expected_exception_message = "Encryption failed";

            //create SecureNotes object
            //create SecureNotes object
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = content,
                Salt = "salt",
                IV = "iv"
            };


            //create a mocked encryption object and manipulate it to return NotFoundException
            mocked_encryption.Setup(x => x.EncryptNote(content, password)).Throws(new CryptographicException("Encryption failed"));

            //Act
            var ex = await Assert.ThrowsAsync<CryptographicException>(() => _noteService.ApplyEncryption(note, content, password));

            //Assert
            //verify exception message is equals to expected_exception_message(Encryption failed)
            Assert.Equal(expected_exception_message, ex.Message);
        }



        //-----------------------------------------------------------------Delete-----------------------------/////

        //test that Delete from NoteService successfully deletes notes
        [Fact]
        public async Task Test_Delete_Sucessfully_Deletes_Notes()
        {
            //Arrange

            //create a new note
            var note = new SecureNotes
            {
                Id = 1,
                Encrypted_content = "test",
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked_db object to test deletion
            mocked_db.Setup(x => x.GetById(1)).ReturnsAsync(note);  //set up mocked_db to return the note

            //Act
            //Call the Delete from NoteService to delete the mocked note
            await _noteService.Delete(1);

            //Assert
            //verify that Delete method from db service has been called  once to make sure method works
            mocked_db.Verify(x => x.Delete(1), Times.Once());
        }

        //Test Delete returns NotFoundException if note does not exist
        [Fact]
        public async Task Test_Delete_Returns_NotFoundException_If_Note_Does_Not_Exist()
        {
            //Arrange
            var id = 1;
            var exception_message = "Note not found";

            //create a new note
            var note = new SecureNotes
            {
                Id = 2,
                Encrypted_content = "test",
                Salt = "salt",
                IV = "iv"
            };

            //create a mocked_db object to test deletion
            mocked_db.Setup(x => x.GetById(1)).Throws(new NotFoundException("Note not found"));  //set up mocked_db to return the note

            //Act
            //declare a variable and as a value assign Delete method from NoteService. Manipulate the variable to return NotFoundException
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _noteService.Delete(id));


            //Assert
            //Verify exception_message is equals to NotFoundException's message
            Assert.Equal(exception_message, ex.Message);
        
        }

    }
}
