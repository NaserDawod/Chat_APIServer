using Messaging_WebApp.Data;
using Messaging_WebApp.Models;

namespace Messaging_WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly Messaging_WebAppContext _context;
        public static List<User> Users;
        public UserService(Messaging_WebAppContext context) {
            _context = context;
            Users = _context.User.ToList();
            var contacts = _context.Contact.ToList();
            var messages = _context.Message.ToList();
            foreach (var user in Users)
            {
                user.Contacts = contacts.Where(x => x.UserId == user.Username).ToList();
                foreach (var contact in user.Contacts)
                {
                    contact.Messages = messages.Where(x => x.ContactId == contact.Id).ToList();
                }
            }
        }

        public User getUser(string username) {
            var user = _context.User.Where(x => x.Username == username);
            if (!user.Any()) {
                return null;
            }
            return user.First();
        }

        public User checkUser(string username, string password) {
            var user = _context.User.Where(x => x.Username == username && x.Password == password);
            if (user == null) {
                return null;
            }
            return user.First();
        }

        public async Task<String> addUser(User user) {
            user.Contacts = new List<Contact>();
            Users.Add(user);
            _context.Add(user);
            await _context.SaveChangesAsync();
            return "done";
        }

        public List<Contact> getContacts(string userName) {
            var user = Users.Find(x => x.Username == userName);
            if (user == null) {
                return null;
            }
            return user.Contacts;
        }

        public Contact getContact(string userName, string contId) {
            var user = getUser(userName);
            if (user == null) {
                return null;
            }
            var contact = user.Contacts.Find(x => x.Contname == contId);
            if (contact == null)
            {
                return null;
            }
            return contact;
        }

        public List<Message> getMessages(string userName, string contId) {
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            return contact.Messages;
        }

        public Message getMessage(string userName, string contId, int msgId) {
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            var msgs = getMessages(userName, contId);
            var msg = contact.Messages.Find(x => x.Id == msgId);
            if (msg == null){
                return null;
            }
            return msg;
        }

        public async Task<Contact> addContact(string userName, string contId, string name, string server) {
            var user = getUser(userName);
            if (user == null) {
                return null;
            }
            Contact contact = new Contact()
            {
                UserId = userName,
                Name = name,
                Server = server,
                Contname = contId,
                Last = null,
                Lastdate = null
            };
            user.Contacts.Add(contact);
            _context.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<User> removeContact(string userName, string contId) {
            var user = getUser(userName);
            if (user == null) {
                return null;
            }
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            user.Contacts.Remove(contact);
            _context.Remove(contact);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<Contact> editContact(string userName, string contId, string name, string server) {
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            contact.Name = name;
            contact.Server = server;
            _context.Contact.Update(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<Message> addMessage(string userName, string contId, Message message) {
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            contact.Messages.Add(message);
            contact.Last = message.Content.Trim();
            contact.Lastdate = message.Created;
            _context.Add(message);
            _context.Update(contact);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Contact> removeMessage(string userName, string contId, Message message) {
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            contact.Messages.Remove(message);
            _context.Remove(message);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<Message> editMessage(string userName, string contId, int msgID, string content) {
            var contact = getContact(userName, contId);
            if (contact == null) {
                return null;
            }
            var msg = getMessage(userName, contId, msgID);
            msg.Content = content;
            _context.Update(msg);
            await _context.SaveChangesAsync();
            return msg;
        }

        public async Task<Contact> Invite(string from, string to, string server)
        {
            var user = Users.Find(x => x.Username == to);
            if (user == null) { return null; }
            Contact contact = new Contact() { Contname = from, UserId = to, Name = from, Server = server, Last = null, Lastdate = null };
            user.Contacts.Add(contact);
            _context.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<Message> Transfer(string from, string to, string content)
        {
            var user = Users.Find(x => x.Username == to);
            if (user == null) { return null; }
            var cont = user.Contacts.Find(x => x.Contname == from);
            if (cont == null) { return null; }
            Message msg = new Message() { Content = content, Sent = false, Created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") };
            cont.Messages.Add(msg);
            cont.Last = content.Trim();
            cont.Lastdate = msg.Created;
            _context.Add(msg);
            _context.Update(cont);
            await _context.SaveChangesAsync();
            return msg;
        }
    }
}
