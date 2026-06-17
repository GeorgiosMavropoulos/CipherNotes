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


        //-----TESTS FOR ENCRYPTION SERVICE---//

        //test encryption and decryption without errors
        [Fact]
        public void EncryptNote_ValidInput_ReturnsEncryptedData()
        {
            //Arrange
            var content = "This is to test encryption";
            
            var password = "password";

            //ACT
            var (cipherText, salt, iv) = _encryptionService.EncryptNote(content, password);

            //Assert
            Assert.NotNull(cipherText); //test that cipher text is not null
            Assert.NotNull(salt);// test that salt is not null
            Assert.NotNull(iv); //test that iv is not null
            Assert.NotNull(password); //test that password is not null
            Assert.NotEqual(cipherText, content);//test that cipher text and content is not equal. This means that content has been encrypted


        }

        //test that the proper error message will return if pass is null
        [Fact]
        public void Test_EmptyPassword_Throws_ValidationException()
        {
            //Arange 
            var content = "pass is null";
           
            var password = string.Empty;

            //Act
           
            //act that asserts whether the exception message will return or not
            var ex = Assert.Throws<ValidationException> (() => _encryptionService.EncryptNote(content, password));



            //Assert
            Assert.Equal("Password cannot be empty", ex.Message);
        }

        // test that when 2 notes or more are being encrypted, the method returns different cipher texts even if content is the same
        [Fact]
        public void Test_Encrypt_Note_Randomness()
        {
            //Arrange
            var content_1 = "text";
            var content_2 = "text";
            var password_1 = "1234";
            var password_2 = "1234";

            //Act
            //create 2 encryption objects and encrypt 2 different texts
            var text_1 = _encryptionService.EncryptNote(content_1, password_1); //text 1
            var text_2 = _encryptionService.EncryptNote(content_2, password_2); //text 2

            //Asert
            Assert.NotEqual(text_1.Salt, text_2.Salt); //verify that text's 1 salt is not equal to the second's one
            Assert.NotEqual(text_1.IV, text_2.IV); //verify that text's 1 initialization vector is not equal to the second's one
            Assert.NotEqual(text_1.CipherText, text_2.CipherText);
        }

        //-----TESTS FOR DECRYPTION SERVICE---//

        //this test validates that the encrypted content EncryptNote method encrypts, will be decrypted successfully. 
        //I validate that the original content before encryption is identical to the decrypted content
        [Fact]
        public void Test_Decrypted_Content_Will_Be_Equals_To_The_Original_Content_Before_Being_Encrypted()
        {
            //Arrange
            var content = "text";
            var password = "1234";
            var decrypted_content = "";
            var decrypt_pass = "1234";

            //Act
            //Firstly content is being encrypted
            var (cipher,salt,iv) = _encryptionService.EncryptNote(content, password);

            //then content is being decrypted
            decrypted_content = _encryptionService.DecryptContent(cipher, decrypt_pass, salt, iv);


            //Assert
            //validate that content is equals to decrypted content
            Assert.Equal(content, decrypted_content);
        }

        //test that DecryptNote method will return a ValidationException error message (Password cannot be empty) if password is null
        [Fact]
        public void Test_Decryption_Will_Return_Validation_Exception_If_Password_Null()
        {
            //Arrange
            var content = "text";
            var password = "1234";
            var decrypted_content = "";
            var decrypt_pass = string.Empty;

            //Act
            //Firstly content is being encrypted
            var (cipher, salt, iv) = _encryptionService.EncryptNote(content, password);

            //then content is being decrypted
            var ex = Assert.Throws<ValidationException>(() => _encryptionService.DecryptContent(cipher, decrypt_pass, salt, iv));

            //Assert
            //validate that the proper exception message will be returned
            //Assert
            Assert.Equal("Password cannot be empty", ex.Message);

        }

        //test that Decrypt note method will return an invalid password exception if password is invalid
        [Fact]
        public void Test_Decryption_Will_Return_InvalidPasswordException_If_Password_Is_Wrong()
        {
            //Arrange
            var content = "text";
            var password = "1234"; //declare encryption's password

            var decrypted_content = string.Empty;
            var decrypt_pass = "wrong password"; //declare a falsed password for decryption's process

            //Act
            //Firstly content is being encrypted
            var (cipher, salt, iv) = _encryptionService.EncryptNote(content, password);

            //then content is being decrypted
            var ex = Assert.Throws<InvalidPasswordException>(()=> _encryptionService.DecryptContent(cipher, decrypt_pass, salt, iv));

            //Assert
            //error message must be equals to 'Wrong password'
            Assert.Equal("Wrong password", ex.Message);
        }


        //test that DeriveKey method will return the same output if salt and password are equal. If not, encrypted content won't be able to get decrypted 
        //even using the same pass
        [Fact]
        public void Test_DeriveKey_Will_Return_The_Same_Key_If_The_Same_Pass_And_Salt_Are_Being_Provided()
        {
            
            //Arrange
            var password = "1234";
            var salt = _encryptionService.GenerateSalt(); //generate salt

            //Act
            //create two keys from the same salt and password
            var key_1 = _encryptionService.DeriveKey(password, salt);
            var key_2 = _encryptionService.DeriveKey(password, salt);

            //Assert
            //verify that boths keys are equal
            Assert.Equal(key_1, key_2);


        }

        //test that DeriveKey method will return a ValidationException error if a custom written salt has been used as a salt
        [Fact]
        public void Test_DeriveKey_Will_Return_Validation_Exception_If_A_Custom_Written_Salt_Has_Been_Used()
        {
            //Arrange
            var password = "1234";
            var salt = "this is an invalid salt, lol";

            //Act
            //Create a variable using EncryptionService's method DeriveKey. I will add the custom written salt
            var exception = Assert.Throws<ValidationException>(()=>_encryptionService.DeriveKey(password, salt));

            //Assert
            //ValidationException should return the message: Invalid salt format
            Assert.Equal("Invalid salt format", exception.Message);
        }


        //test that GenerateSalt returns a 16 char sequence
        [Fact]
        public void Test_GenerateSalt_Returns_A_16_Char_Sequence()
        {
            //Arrange
            byte[] decoded_salt;

            //Act
            //use the salt variable to call the GenerateSalt method
            var salt = _encryptionService.GenerateSalt();

            //Assert
            //verify that salt is not null
            Assert.NotNull(salt);

            //decode salt and store it in decrypted_salt variable
            decoded_salt = Convert.FromBase64String(salt);

            //Assert that decoded_salt's length is equals to 16 
            Assert.Equal(16, decoded_salt.Length);
        }

    }
}
