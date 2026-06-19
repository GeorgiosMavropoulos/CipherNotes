using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Interfaces
{
    //INTERFACE class for encryption service. 
    public interface IEncryptionService
    {
        //interface for encryptnote method
        (string CipherText, string Salt, string IV) EncryptNote(string content, string password);

        //interface for decryptcontent method
        string DecryptContent(string encrypted_content, string password, string salt, string iv);

       

    }
}
