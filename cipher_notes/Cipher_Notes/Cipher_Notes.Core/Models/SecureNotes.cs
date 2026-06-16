using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Cipher_Notes.Core.Models
{
    [Table("SecureNotes")]
    public class SecureNotes
    {
        //declaring properties with public setter and getter properties!

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public String Title { get; set; } = string.Empty;

        [NotNull]
        public String Encrypted_content { get; set; } = string.Empty;

        [NotNull]
        public String Salt { get; set; } = string.Empty;

        [NotNull]
        public String IV { get; set; } = string.Empty;

    
        public DateTime Created_at { get; set; } 

        
        public DateTime? Updated_at { get; set; }


    }
}
