namespace Messaging_WebApp.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public String Created { get; set; }

        public bool Sent { get; set; }

        public int ContactId { get; set; }
    }
}
