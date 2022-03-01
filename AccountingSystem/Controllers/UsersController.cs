using AccountingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;
using System.Security.Claims;
using AccountingSystem.Models.ViewModels;

namespace DmdSystem.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly DmdCoursDBContext _context;

        public UsersController(DmdCoursDBContext context)
        {
            _context = context;
        }

        //Получить информацию о пользователе GET
        public async Task<IActionResult> GetUser(int? id)
        {
            var usersForList = _context.Users.AsNoTracking();
            bool isAdminOrDirector = User.IsInRole("Администратор") || User.IsInRole("Директор");
            bool isManager = User.IsInRole("Руководитель");

            if (isAdminOrDirector)
            {
                ViewData["Users"] = new SelectList(usersForList, "Id", "Surname");
            }
            else if (isManager)
            {
                ViewData["Users"] = new SelectList(usersForList.Where(u => u.Roles.Any(u => u.Id == (int)AccountingSystem.Models.Enum.Roles.Employee)), "Id", "Surname");
            }
            else
            {
                ViewData["Users"] = new SelectList(usersForList.Where(u => u.Id == GetCurrentUserId()), "Id", "Surname");
            }

            if (id == null)
            {
                int currentUserId = GetCurrentUserId();
                var userFromDB = await _context.Users
                .Include(u => u.PositionNavigation)
                .Include(u => u.Roles)
                .Include(u => u.UnitNavigation)
                .FirstOrDefaultAsync(m => m.Id == currentUserId);

                if(userFromDB == null)
                {
                    return NotFound();
                }
                var user = UserInfoViewModel.FromUser(userFromDB);
                user.DaysOff = _context.Requests
                    .Where(r => r.Sender == currentUserId && r.Status == 3 && r.RequestType == 3)
                    .Count();
                user.UnusedVacationDaysCurrentYear -= _context.Requests
                    .Where(r => r.Sender == currentUserId && r.RequestType == 4 && r.Status == 5)
                    .Count();
                user.RequestsGrouping = (from request in _context.Requests
                         where request.Sender == GetCurrentUserId()
                         group request by request.StatusNavigation.Name
                                   into requestGrouping
                         select new RequestsByStatusViewModel
                         {
                             Name = requestGrouping.Key,
                             Count = requestGrouping.Count()
                         }).ToList();

                return View(user);
            }
            else
            {
                int currentUserId = GetCurrentUserId();
                var userFromDB = await _context.Users
                .Include(u => u.PositionNavigation)
                .Include(u => u.Roles)
                .Include(u => u.UnitNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

                if(userFromDB == null)
                {
                    return NotFound();
                }
                var user = UserInfoViewModel.FromUser(userFromDB);
                user.DaysOff = _context.Requests
                    .Where(r => r.Sender == id && r.Status == 3 && r.RequestType == 3)
                    .Count();
                user.UnusedVacationDaysCurrentYear -= _context.Requests
                    .Where(r => r.Sender == id && r.RequestType == 4 && r.Status == 5 && r.CreationDate.Year == DateTime.Now.Year)
                    .Count();

                user.RequestsGrouping = (from request in _context.Requests
                                   where request.Sender == id
                                   group request by request.StatusNavigation.Name
                                   into requestGrouping
                                   select new RequestsByStatusViewModel
                                   {
                                       Name = requestGrouping.Key,
                                       Count = requestGrouping.Count()
                                   }).ToList();
                return View(user);
            }
        }

        //Создание пользователя GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name");
            PopulateAssignedRolesData();
            ViewData["Unit"] = new SelectList(_context.Units, "Id", "Name");
            return View();
        }

        //Создание пользователя POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel user, string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                SetUserRoles(selectedRoles, user);
                user.Password = Crypto.HashPassword(user.Password);
                User userToDB = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Patronymic = user.Patronymic,
                    Password = user.Password,
                    Unit = user.Unit,
                    Roles = user.Roles,
                    Position = user.Position,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };
                _context.Add(userToDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetUser", "Users");
            }

            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name");
            PopulateAssignedRolesData();
            ViewData["Unit"] = new SelectList(_context.Units, "Id", "Name");
            return View();   
        }

        //Редактирование пользователя GET
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            bool isAdministrator = User.IsInRole("Администратор");
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            PopulateAssignedRolesData(user);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name", user.Position);
            ViewData["Unit"] = new SelectList(_context.Units, "Id", "Name", user.Unit);

            if (isAdministrator)
            {
                return PartialView("EditForAdministrator",user);
            }
            else
            {
                return PartialView(user);
            }
        }

        //Редактирование пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, string[] selectedRoles)
        {
            bool isAdministrator = User.IsInRole("Администратор");
            if (id != user.Id)
            {
                return NotFound();
            }
            if (isAdministrator)
            {
                try
                {
                    _context.Update(user);
                    UpdateUserRoles(selectedRoles, user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetUser));               
            }
            else if (!isAdministrator)
            {
                try
                {
                    _context.Entry(user).Property(x => x.Email).IsModified = true;
                    _context.Entry(user).Property(x => x.PhoneNumber).IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) 
                {
                    if (!UserExists(user.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetUser));
            }
            PopulateAssignedRolesData(user);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Name", user.Position);
            ViewData["Unit"] = new SelectList(_context.Units, "Id", "Name", user.Unit);
            return View(user);
        }

        //Установить роли пользователю
        private void SetUserRoles(string[] selectedRoles, UserCreateViewModel userToUpdate)
        {
            var selectedRolesHS = new HashSet<string>(selectedRoles);
            foreach (var role in _context.Roles)
            {
                if (selectedRolesHS.Contains(role.Id.ToString()))
                {
                        userToUpdate.Roles.Add(role);
                }
            }
        }

        //Обновить роли пользователя
        private void UpdateUserRoles(string[] selectedRoles, User userToUpdate)
        {
            if (selectedRoles == null)
            {
                userToUpdate.Roles = new List<AccountingSystem.Models.Role>();
                return;
            }

            var roles = _context.Users.Where(e => e.Id == userToUpdate.Id).Include(e => e.Roles).First();
            var selectedRolesHS = new HashSet<string>(selectedRoles);
            var userRoles = new HashSet<int>(_context.Roles.Where(u => u.Users.Contains(userToUpdate)).Select(y => y.Id));

            foreach (var role in _context.Roles)
            {
                if (selectedRolesHS.Contains(role.Id.ToString()))
                {
                    if (!userRoles.Contains(role.Id))
                    {
                        userToUpdate.Roles.Add(role);
                    }
                }
                else
                {
                    if (userRoles.Contains(role.Id))
                    {

                        roles.Roles.Remove(role);
                    }
                }
            }
        }

        //Создать список ролей
        private void PopulateAssignedRolesData(User? user = null)
        {
            var allRoles = _context.Roles.AsNoTracking();
            HashSet<int>? userRoles = null;

            if(user != null)
            {
                userRoles = new HashSet<int>(_context.Roles.Where(u => u.Users.Contains(user)).Select(y => y.Id));
            }

            var viewModel = new List<AssignedRolesData>();

            foreach (var role in allRoles)
            {
                viewModel.Add(new AssignedRolesData
                {
                    RoleId = role.Id,
                    Title = role.Name,
                    Assigned = user is not null && userRoles.Contains(role.Id)
                });;
            }

            ViewBag.Role = viewModel;
        }

        //Удаление GET
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.PositionNavigation)
                .Include(u => u.Roles)
                .Include(u => u.UnitNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return PartialView(user);
        }

        //Подтверждение удаления
        [Authorize(Roles = "Администратор")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.PositionNavigation)
                .Include(u => u.Roles)
                .Include(u => u.UnitNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            var roles = await _context.Users
                .Where(e => e.Id == id)
                .Include(e => e.Roles)
                .FirstOrDefaultAsync();

            var senderRequests = _context.Requests
                .Include(r => r.Users)
                .Where(r => r.Sender == user.Id);
            var substituteEmployee = _context.Requests
                .Where(r => r.SubstituteEmployee == user.Id);

            var approverRequests = _context.Requests.Include(r => r.Users).Where(r => r.SubstituteEmployee == user.Id);

            if (user == null || roles == null)
            {
                return NotFound();
            }
            if(substituteEmployee != null)
            {
                foreach(var request in substituteEmployee)
                {
                    request.SubstituteEmployee = null;
                }
            }
            if(senderRequests != null)
            {
                foreach(var request in senderRequests)
                {
                    request.Users.Clear();
                    _context.Requests.Remove(request);
                }
            }
            if (approverRequests != null)
            {
                foreach(var request in approverRequests)
                {
                    request.Users.Remove(user);
                }
            }          

            roles.Roles.Clear();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetUser));
        }

        //Получить ID текущего пользователя
        private int GetCurrentUserId()
        {
            return Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        public JsonResult CheckEmail(string email)
        {
            if (_context.Users.Any(e => e.Email == email))
                return Json(false);
            return Json(true);
        }

        //Проверить есть ли пользователь в БД
        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
    }
}