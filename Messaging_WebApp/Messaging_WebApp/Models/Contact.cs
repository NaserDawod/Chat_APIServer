using System.ComponentModel.DataAnnotations;

namespace Messaging_WebApp.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public String Contname { get; set; }

        public String UserId { get; set; }
        
        public string Name { get; set; }

        public String Server { get; set; }

        public String? Last { get; set; }

        public String? Lastdate { get; set; }

        public List<Message> Messages { get; set; }
    }
}
