
using Cipher_Notes.Exceptions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Cipher_Notes.Services
{
    public class EncryptionService
    {
        //declaring variables
        private const int key_size = 256;
        private const int iterations = 10000;

 

        //declare constructor
        public EncryptionService() 
        {
        
        }

        //encrypt note method
        public (string CipherText, string Salt, string IV) EncryptNote(string content, string password)
        {
            string salt = GenerateSalt(); //generate salt using GenerateSalt() method

            string iv = GenerateIV(); //generate IV using GenerateIV method

           //derive key
           byte[] key = DeriveKey(password, salt);

            string cipherText = Encrypt(content,key, iv);

            //return encrypted string
            return (cipherText, salt, iv);


            
        }

        //method to ecnrypt the text
        public string Encrypt(string content, byte[] key, string iv)
        {
            //try catch method to handle errors
            try
            {
                //use AES encryption
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = Convert.FromBase64String(iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                //use encryptor
                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                //use crypto stream writer
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                //use stream writer
                using (var writer = new StreamWriter(cs))
                {
                    //encrypt and save content
                    writer.Write(content);
                }

                //convert encrypted bytes to string
                return Convert.ToBase64String(ms.ToArray());

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Content encryption failed", e);
            }

        }

        //decrypt note method
        public String DecryptContent(string encrypted_content, string password, string salt, string iv)
        {
            //derive key from password + salt
            byte[] key = DeriveKey(password, salt);
           
            return Decrypt(encrypted_content, key, iv); //return decrypted content
        }

        //method to create decryption process
        public string Decrypt(string encrypted_content, byte[] key, string iv)
        {
            //try-catch method to handle unexpected errors
            try
            {

                //convert iv + cipher text from Base64
                byte[] ivBytes = Convert.FromBase64String(iv);
                byte[] cipherBytes = Convert.FromBase64String(encrypted_content);

                //set up AES
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                //decryptor
                using var decryptor = aes.CreateDecryptor();

                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

                //use reader, to read content
                using var reader = new StreamReader(cs);

                //read plaintext
                string plain_text = reader.ReadToEnd();

                return plain_text; //return plain text to user
            }
            catch (CryptographicException ex)
            {
                 throw new InvalidPasswordException(ex);
            }
        }

        //derive key method
        public byte[] DeriveKey(string password, string salt)
        {
            try
            {
                var saltBytes = Convert.FromBase64String(salt);

                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    saltBytes,
                    iterations,
                    HashAlgorithmName.SHA256
                );

                return pbkdf2.GetBytes(key_size / 8); // 256 bits = 32 bytes

            } catch (Exception e) 
            {
                throw new Exception("Key derivation failed", e);
            }
           

        }

        //generate salt method
        public String GenerateSalt() 
        {
            try
            {
                byte[] salt = new byte[16]; //128-bit salt
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt); //generate a random salt
                }

                return Convert.ToBase64String(salt); //convert salt to string

            } catch (CryptographicException e) 
            {
                throw new InvalidOperationException("Salt generation failed", e);
            }
           
        }

        //generate initialization vector method
        public String GenerateIV() 
        {
            byte[] iv = new byte[16]; //AES ENCRYPTION bytes
            
            //using random number generator to generate iv
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv); 
            }
            return Convert.ToBase64String(iv);
        
        }
    }
}
