using Cipher_Notes.Core.Exceptions;
using Cipher_Notes.Core.Interfaces;
using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;
using Moq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

namespace Cipher_Notes.Tests
{
    //unit tests for DatabseService
    public class TestDatabaseService: IDisposable
    {
        //declare variables
        private readonly DatabaseService dbService; //create a new db service object
        private readonly string testDbPath; //create a dbpath variable

        //declare constructor
        public TestDatabaseService()
        {
            //create a unique temporary db object for each test
            testDbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");//use Guid to get a new different db object each time.
           
            //initialize a new db object for each test and assign the temp path
            dbService = new DatabaseService(testDbPath);     
            

         }

        //create the Dispose() method to delete temp db path after each test
        public void Dispose()
        {
            if(File.Exists(testDbPath))
            {
                File.Delete(testDbPath);//delete file
            }
        }

        //-------------Test Create----------------------------//
        [Fact]
        public async Task Test_Create_Successfully_Creates_Note_and_GetById_Successfully_Returns_The_Created_Note()
        {
            //Arrange

            //declare variables
            var title = "Title";
            var encrypted_text = "test";
            var salt = "Salt";
            var iv = "Iv";

            //create a new SecureNotes object
            var note = new SecureNotes
            {
                Title = title,
                Encrypted_content = encrypted_text,
                Salt = salt,
                IV = iv,
                Created_at = DateTime.Now
            };

            //Act
            //insert object using Create method from DatabaseService
            await dbService.Create(note);

            //declare a variable and return the object inside this var.Use GetById to retrieve note
            var result = await dbService.GetById(note.Id);


            //Assert
            //verifry that result is not null
            Assert.NotNull(result);
            //verify that stored object's title is equal to declared title
            Assert.Equal(title, result.Title);

            //verify that stored object's salt and iv are equal to the declared ones
            Assert.Equal(salt, result.Salt);
            Assert.Equal(iv, result.IV);
        }


    }
}
