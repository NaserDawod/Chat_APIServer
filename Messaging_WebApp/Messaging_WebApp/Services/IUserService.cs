using Messaging_WebApp.Models;

namespace Messaging_WebApp.Services
{
    public interface IUserService
    {

        public User checkUser(string username, string password);

        public Task<String> addUser(User user);
        public User getUser(string username);

        public List<Contact> getContacts(string userName);
        public Contact getContact(string userName, string contId);
        public List<Message> getMessages(string userName, string contId);
        public Message getMessage(string userName, string contId, int msgId);

        public Task<Contact> addContact(string userName, string contId, string name, string server);

        public Task<User> removeContact(string userName, string contId);

        public Task<Contact> editContact(string userName, string contId, string name, string server);

        public Task<Message> addMessage(string userName, string contId, Message message);

        public Task<Contact> removeMessage(string userName, string contId, Message message);

        public Task<Message> editMessage(string userName, string contId, int msgID, string content);

        public Task<Contact> Invite(string from, string to, string server);

        public Task<Message> Transfer(string from, string to, string content);
    }
}
