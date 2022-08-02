using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Messaging_WebApp.Data;
using Messaging_WebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using Messaging_WebApp.Services;
using Messaging_WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Messaging_WebApp.Controllers
{

        public class Temp
    {
        public String? UserID { get; set; }
        public String Name { get; set; }

        public String server { get; set; }

    }
    public class Inv
    {
        public String from { get; set; }
        public String to { get; set; }

        public String server { get; set; }

    }

    public class Temp2
    {
        public String Content { get; set; }

    }

    public class Transf
    {
        public String from { get; set; }
        public String to { get; set; }
        public String content { get; set; }

    }
    public class Temp_user
    {
        public String username { get; set; }
        public String? name { get; set; }
        public String password { get; set; }

    }

    public class Device
    {
        public string username { get; set; }
        
        public string token { get; set; }
        
    }

    [ApiController]
    [Route("api/[controller]")]
    public class Contacts2Controller : Controller
    {
        public IUserService _service;
        readonly IHubContext<MyHub> _hub;
        public IConfiguration _configuration;

        public Contacts2Controller(IUserService service, IHubContext<MyHub> hub, IConfiguration configuration)
        {
            _service = service;
            _hub = hub;
            _configuration = configuration;
        }

        [HttpPost]
        
        [Route("login")]
        public async Task<IActionResult> Login([Bind("username,password")] Temp_user temp)
        {
            if (ModelState.IsValid)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var check_user = _service.checkUser(temp.username, temp.password);
#pragma warning restore CS8604 // Possible null reference argument.
                if (check_user != null)
                {
                    //Signin(check_user.First());
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, DateTime.UtcNow.ToString()),
                        new Claim("UserID", temp.username),
                    };
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                    var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["JWTParams:Issuer"],
                        _configuration["JWTParams:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: mac);
                    var tok = new JwtSecurityTokenHandler().WriteToken(token);
                    temp t = new temp() { token = tok };
                    return Json(tok);

                }
                return Json(null);

            }
            return Json(null);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([Bind("username,name,password")] Temp_user temp)
         {
            if (ModelState.IsValid)
            {
                var check_user = _service.getUser(temp.username);
                if (check_user != null) { return Json(null); }
                var Contacts = new List<Contact>();
                User user = new User();
                user.Username = temp.username;
                user.Name = temp.name;
                user.Password = temp.password;
                user.Contacts = Contacts;
                await _service.addUser(user);
                return Json("ok");
            }
            return Json(null);
        }

        public String encode(string authorization)
        {
            var stream = authorization.Remove(0, 7);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            var name = tokenS.Claims.ElementAt(3).Value;
            return name;
        }

        [Authorize]
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser([FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(authorization))
            {
                var name = encode(authorization);
                var user = _service.getUser(name);
                if (user == null) { return NotFound(); }
                return Json(user);
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetContacts([FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(authorization))
            {
                var name = encode(authorization);
                var contacts = _service.getContacts(name);
                if (contacts == null) { return NotFound(); }
                return Json(contacts);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        //[Route("{Username}/contacts")]
        public async Task<IActionResult> AddContact([Bind("UserID,Name,server")] Temp temp, [FromHeader] string authorization)
        {
            if (ModelState.IsValid)
            {
                String name = encode(authorization);
                var contact = await _service.addContact(name, temp.UserID, temp.Name, temp.server);
                if (contact == null) { return Json("bad"); }
                return Json("added");
            }
            return Json("bad");
        }

        [Authorize]
        [HttpGet]
        [Route("{ContID}")]
        public async Task<IActionResult> GetContact(string ContID, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID))
            {
                String name = encode(authorization);
                var contact = _service.getContact(name, ContID);
                if (contact == null) { return NotFound(); }
                return Json(contact);
            }
            return NotFound();
        }

        //[Authorize]
        [HttpPut]
        [Route("{ContID}")]
        public async Task<IActionResult> UpdateContact(string ContID, [Bind("Name,server")] Temp temp, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID) && !string.IsNullOrEmpty(temp.Name) &&
                !string.IsNullOrEmpty(temp.server))
            {
                String name = encode(authorization);
                var contact = await _service.editContact(name, ContID, temp.Name, temp.server);
                if (contact == null) { return NotFound(); }
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete]
        [Route("{ContID}")]
        public async Task<IActionResult> DeleteContact(string ContID, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID))
            {
                String name = encode(authorization);
                var remove = await _service.removeContact(name, ContID);
                if (remove == null) { return NotFound(); }
                return Ok();
            }
            return NotFound();
        }


        [Authorize]
        [HttpGet]
        [Route("{ContID}/messages")]
        public async Task<IActionResult> GetMessages(string ContID, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID))
            {
                String name = encode(authorization);
                var messages = _service.getMessages(name, ContID);
                if (messages == null) { return NotFound(); }
                return Json(messages);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [Route("{ContID}/messages")]
        public async Task<IActionResult> AddMessage(String ContID, [Bind("Content")] Temp2 temp, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(temp.Content))
            {
                String name = encode(authorization);
                Message mess = new Message()
                {
                    Content = temp.Content,
                    Sent = true,
                    Created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };
                var check = await _service.addMessage(name, ContID, mess);
                if (check == null) { return NotFound(); }
                return Json("sent");
            }
            return Json("bad");
        }

        [Authorize]
        [HttpGet]
        [Route("{ContID}/messages/{msgID}")]
        public async Task<IActionResult> GetMessage(string ContID, int msgID, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID))
            {
                String name = encode(authorization);
                var msg = _service.getMessage(name, ContID, msgID);
                if (msg == null) { return NotFound(); }
                return Json(msg);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPut]
        [Route("{ContID}/messages/{msgID}")]
        public async Task<IActionResult> EditMessage(string ContID, int msgID, [Bind("Content")] Temp2 temp, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID) && ModelState.IsValid)
            {
                String name = encode(authorization);
                var mess = await _service.editMessage(name, ContID, msgID, temp.Content);
                if (mess == null) { return NotFound(); }
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete]
        [Route("{ContID}/messages/{msgID}")]
        public async Task<IActionResult> DeleteMessage(string ContID, int msgID, [FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(ContID) && ModelState.IsValid)
            {
                String name = encode(authorization);
                var mess = _service.getMessage(name, ContID, msgID);
                if (mess == null) { return NotFound(); }
                await _service.removeMessage(name, ContID, mess);
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Route("invitations")]
        public async Task<IActionResult> Invite([Bind("from,to,server")] Inv invite)
        {
            if (ModelState.IsValid)
            {
                var contact = await _service.Invite(invite.from, invite.to, invite.server);
                if (contact == null) { return NotFound(); }
                return Json("added");
            }
            return Json("bad");
        }

        [HttpPost]
        [Route("transfer")]
        public async Task<IActionResult> Transfer([Bind("from,to,content")] Transf message)
        {
            if (ModelState.IsValid)
            {
                var msg = await _service.Transfer(message.from, message.to, message.content);
                if (msg == null) {
                 return BadRequest(); 
                }
                string username = message.to;
                if (AndroidUsers.map.ContainsKey(username))
                { // send notalfication
                    AndroidUsers.sendNotifcation(message.from, message.to, message.content);
                }
                await _hub.Clients.All.SendAsync("ChangeReceived", message.to, message.content, message.from, msg.Id);
                return Json("added");
            }
            return Json("bad");
        }
        
        [HttpPost]
        [Route("map")]
        public async Task<IActionResult> addAndroid([Bind("username,token")] Device device)
        {
            if (ModelState.IsValid)
            {
                AndroidUsers.addAndroidUser(device.username, device.token);
                return Json("bad");
            }
            return Json("bad");
        }
    }
}



