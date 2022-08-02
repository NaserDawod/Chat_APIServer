using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Messaging_WebApp.Data;
using Messaging_WebApp.Models;
using System.Text;

namespace Messaging_WebApp.Controllers
{
    public class SignController : Controller
    {
        private readonly Messaging_WebAppContext _context;
        public IConfiguration _configuration;

        public SignController(Messaging_WebAppContext context, IConfiguration config)
        {
            _context = context;
            _configuration = config;
        }

        // GET: Users
        //[Authorize]
        // public async Task<IActionResult> Index()
        // {
        //     return _context.User != null ?
        //                 View(await _context.User.ToListAsync()) :
        //                 Problem("Entity set 'whatsapp_demoContext.User'  is null.");
        // }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] String Username, String Password)
        {
            if (ModelState.IsValid)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var check_user = _context.User.Where(x => x.Username == Username && x.Password == Password);
#pragma warning restore CS8604 // Possible null reference argument.
                if (check_user.Any())
                {
                    //Signin(check_user.First());
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, DateTime.UtcNow.ToString()),
                        new Claim("UserID", Username),
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
                    return RedirectToAction("MainChat", "Chat", t);
                }
                else
                {
                    ViewBag.error = "Wrong Username or Password";
                }

            }
            return View();
        }

        // GET: Users/Create
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,Name,Password")] String Username, String Nickname, String Password)
        {
            if (ModelState.IsValid)
            {
                var check_user = from u in _context.User
                                 where u.Username == Username
                                 select u;
                var Contacts = new List<Contact>();
                User user = new User();
                user.Username = Username;
                user.Name = Nickname;
                user.Password = Password;
                user.Contacts = Contacts;
                if (check_user.Count() > 0)
                {
                    ViewBag.error = "this username is already rigesterd";
                }
                else
                {
                    //Signin(user);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login), "Sign");
                }
            }
            return View();
        }

        private bool UserExists(string id)
        {
            return (_context.User?.Any(e => e.Username == id)).GetValueOrDefault();
        }

        public void Logout()
        {
            HttpContext.SignOutAsync();
        }

        private async void Signin(User account)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, account.Username)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {

            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
