using System;
using System.Collections.Generic;
using System.Text;

namespace Cipher_Notes.Core.Exceptions
{
    //exception class for the Database Service. This class is responsible to return via its exception object all error messages from the DB!
    public class DatabaseException: Exception
    {

        //declare the constructor which holds the message's paremeters!
        public DatabaseException(string message,Exception innerException) : base(message, innerException) { }
    }
}
