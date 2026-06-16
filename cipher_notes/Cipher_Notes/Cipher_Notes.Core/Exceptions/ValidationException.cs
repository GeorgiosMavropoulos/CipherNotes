using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Exceptions
{
    //validation exception class, to return exception message for all validation logic, e.g empty password for decryption
    public class ValidationException: Exception
    {
        public ValidationException(string message) : base(message) { }

        //+ inner exception for debugging
        public ValidationException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
