using Cipher_Notes.Core.Models;
using Cipher_Notes.Core.Services;

using Cipher_Notes.Core.Exceptions;
namespace Cipher_Notes.Tests
{
    public class TestEncryptionService
    {
        //create objects and declare variables
        EncryptionService _encryptionService;


        //declare a constructor to reset _encryptionService data 
        public TestEncryptionService() => _encryptionService = new EncryptionService();

        //test encryption and decryption without errors
        [Fact]
        public void Test_Encryption_Without_Errors()
        {
            //Arrange
            var content = "This is to test encryption";
            var title = "Test title";
            var password = "password";

            //ACT
            var (cipherText, salt, iv) = _encryptionService.EncryptNote(content, password);

            //Assert
            Assert.NotNull(cipherText); //test that cipher text is not null
            Assert.NotNull(title); //test that title is not null
            Assert.NotNull(password); //test that password is not null
            Assert.NotEqual(cipherText, content);//test that cipher text and content is not equal. This means that content has been encrypted


        }

        //test that the proper error message will return if cipherText, title, or pass is null
        [Fact]
        public void Test_EmptyPassword_Throws_ValidationException()
        {
            //Arange 
            var content = "pass is null";
            var title = "title";
            var password = string.Empty;

            //Act
           
            //act that asserts whether the exception message will return or not
            var ex = Assert.Throws<ValidationException> (() => _encryptionService.EncryptNote(content, password));



            //Assert
            Assert.Equal("Password cannot be empty", ex.Message);
        }
    }
}
