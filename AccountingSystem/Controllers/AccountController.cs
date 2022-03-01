using AccountingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;
using System.Security.Claims;

namespace AccountingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly DmdCoursDBContext _context;

        public AccountController(DmdCoursDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null && Crypto.VerifyHashedPassword(user.Password, model.Password))
                {
                    if(user.Email == "Admin")
                    {
                        foreach(var role in _context.Roles)
                        {
                            user.Roles.Add(role);
                            
                        }
                    }
                    await _context.SaveChangesAsync();
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("GetRequestsForPerson", "Requests");

                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var roles = _context.Roles.Where(u => u.Users.Contains(user)).Select(y => y.Name);
            // создаем один claim
            var id = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            }, "ApplicationCookie");
            foreach (var r in roles)
            {
                id.AddClaim(new Claim(ClaimTypes.Role, r));
            }
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}

