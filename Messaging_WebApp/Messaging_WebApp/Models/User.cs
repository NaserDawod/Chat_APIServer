using System.ComponentModel.DataAnnotations;

namespace Messaging_WebApp.Models
{
    public class User
    {
        [Key]
        public String Username { get; set; }

        public String Name { get; set; }

        [DataType(DataType.Password)]
        public String Password { get; set; }

        public List<Contact> Contacts { get; set; }
    }
}
