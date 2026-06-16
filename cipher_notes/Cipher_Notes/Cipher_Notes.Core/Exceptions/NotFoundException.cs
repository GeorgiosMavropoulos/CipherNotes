using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Exceptions
{
    public class NotFoundException:Exception
    {

        public NotFoundException(string message):base(message) { }

        //+ inner exception for debugging
        public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
