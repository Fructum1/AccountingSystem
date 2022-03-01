using AccountingSystem.Models;
using AccountingSystem.Models.Enum;
using AccountingSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly DmdCoursDBContext _context;

        public CalendarController(DmdCoursDBContext context)
        {
            _context = context;
        }
        public JsonResult GetRequests(int? id)
        {
            bool isManager = User.IsInRole("Руководитель");
            bool isDirector = User.IsInRole("Директор");
            bool isAdministrator = User.IsInRole("Администратор");
            if (!isManager && !isDirector && !isAdministrator)
            {
                if (id != null)
                {
                    var requests = from Request in _context.Requests
                                   where Request.RequestType == id && Request.Status == (int)StatusRequest.Approved && Request.Sender == GetCurrentUserId()
                                   select new RequestCalendarViewModel
                                   {
                                       Id = Request.Id,
                                       Title = Request.RequestTypeNavigation.Name,
                                       Start = Request.StartDate.ToString("yyyy-MM-dd"),
                                       End = Request.EndDate.ToString("yyyy-MM-dd"),
                                       Status = Request.StatusNavigation.Name,
                                   };
                    return Json(requests);
                }
                else
                {
                    var requests = from Request in _context.Requests
                                   where Request.Sender == GetCurrentUserId()
                                   select new RequestCalendarViewModel
                                   {
                                       Id = Request.Id,
                                       Title = Request.RequestTypeNavigation.Name,
                                       Start = Request.StartDate.ToString("yyyy-MM-dd"),
                                       End = Request.EndDate.ToString("yyyy-MM-dd"),
                                       Status = Request.StatusNavigation.Name,
                                   };
                    return Json(requests);
                }
            }
            else
            {
                if (id != null)
                {
                    var requests = from Request in _context.Requests
                                   where Request.RequestType == id && Request.Status == (int)StatusRequest.Approved
                                   select new RequestCalendarViewModel
                                   {
                                       Id = Request.Id,
                                       Title = Request.RequestTypeNavigation.Name,
                                       Start = Request.StartDate.ToString("yyyy-MM-dd"),
                                       End = Request.EndDate.ToString("yyyy-MM-dd"),
                                       Status = Request.StatusNavigation.Name,
                                   };
                    return Json(requests);
                }
                else
                {
                    var requests = from Request in _context.Requests

                                   select new RequestCalendarViewModel
                                   {
                                       Id = Request.Id,
                                       Title = Request.RequestTypeNavigation.Name,
                                       Start = Request.StartDate.ToString("yyyy-MM-dd"),
                                       End = Request.EndDate.ToString("yyyy-MM-dd"),
                                       Status = Request.StatusNavigation.Name,
                                   };
                    return Json(requests);
                }
            }
        }
        public IActionResult Index()
        {
            ViewData["RequestType"] = new SelectList(_context.RequestTypes, "Id", "Name");
            return View();
            
        }
        private int GetCurrentUserId()
        {
            return Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        private async Task<User> GetCurrentUserAsync()
        {
            var user = await _context.Users.FindAsync(GetCurrentUserId());

            if (user != null)
            {
                return user;
            }
            else
            {
                throw new Exception();
            }
        }
    }
  
}
