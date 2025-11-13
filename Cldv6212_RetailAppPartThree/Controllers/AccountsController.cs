using Cldv6212_RetailAppPartThree.Data;
using Cldv6212_RetailAppPartThree.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cldv6212_RetailAppPartThree.Controllers
{
    public class AccountsController : Controller
    {
        private readonly Cldv6212_RetailAppPartThreeContext _context;

        public AccountsController(Cldv6212_RetailAppPartThreeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);


            bool exists = await _context.User.AnyAsync(u => u.Email == vm.Email);
            if (exists)
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(vm);
            }


            var (hash, salt) = Password.HashPassword(vm.Password);
            var user = new User
            {
                Username = vm.Username,
                Email = vm.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                CreatedAt = DateTime.UtcNow,
                Role = "Customer"
            };


            _context.User.Add(user);
            await _context.SaveChangesAsync();



            await SignInUser(user, isPersistent: false);
            HttpContext.Session.SetString("DisplayName", user.Username);
            TempData["Success"] = "Registration successful! You are now logged in.";
            return RedirectToAction("UserIndex", "Products");
        }

        private async Task SignInUser(User user, bool isPersistent)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);



            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            bool exists = _context.User.Any(u => u.Email == vm.Email);
            if (!exists)
            {
                ModelState.AddModelError("", "Email is not registered");
                return View(vm);
            }

            var user = _context.User.SingleOrDefault(u => u.Email == vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(vm);
            }



            bool isPasswordValid = Password.Verify(vm.Password, user.PasswordHash, user.PasswordSalt);


            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid password.");
                return View(vm);
            }

            if (!string.Equals(user.Role, vm.Role))
            {
                TempData["Error"] = "Access Denied! Please choose your correct role.";
                return View(vm);
            }
            SignInUser(user, isPersistent: false);


            TempData["Success"] = "Successfully logged in!";

            if (user.Role == "Admin")
            {
                return RedirectToAction("AdminIndex", "Products");
            }
            else
            {
                return RedirectToAction("UserIndex", "Products");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Successfully logged out!";
            return RedirectToAction("UserIndex", "Products");
        }
    }
}


