using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Exceptions
{
    //this class will be used to create invalid password exception message
    public class InvalidPasswordException : Exception
    {
        
        //create the constructor to store the message
        public InvalidPasswordException(string message = "Wrong password") : base(message)
        {

        }
        public InvalidPasswordException(string message, Exception innerException)
    : base(message, innerException)
        {
        }

        public InvalidPasswordException(Exception innerException)
            : base("Wrong password", innerException)
        {
        }

    }
}
