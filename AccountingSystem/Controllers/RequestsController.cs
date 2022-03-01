using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AccountingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AccountingSystem.Models.ViewModels;
using AccountingSystem.Models.Enum;

namespace AccountingSystem.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly DmdCoursDBContext _context;

        public RequestsController(DmdCoursDBContext context)
        {
            _context = context;
        }

        // GET: Requests
        [Authorize(Roles = "Администратор,Директор,Руководитель")]
        public async Task<IActionResult> GetRequests(string? unit, string name, string surname, string sortOrder, string? message)
        {
            ViewData["Message"] = message;
            bool isManager = User.IsInRole("Руководитель");
            bool isDirector = User.IsInRole("Директор");

            var currentUser = GetCurrentUserAsync().Result;

            var requests = from Request in _context.Requests
                            join User 
                            in _context.Users
                            on Request.Sender equals User.Id 
                            select new RequestListViewModel {
                                Id = Request.Id, 
                                Unit = User.UnitNavigation.Name, 
                                Name = User.Name, 
                                Surname = User.Surname,
                                RequestTypeNavigation = Request.RequestTypeNavigation,
                                Status = Request.Status,
                                StatusNavigation = Request.StatusNavigation,
                                CreationDate = Request.CreationDate,
                                StartDate = Request.StartDate,
                                EndDate = Request.EndDate,
                                Users = Request.Users
                            };     
            
            if (isManager)
            {
                requests =  requests
                    .Where(r => r.Unit == currentUser.UnitNavigation.Name 
                    && r.Status == (int)StatusRequest.SendForApprove 
                    && r.Users
                    .Contains(currentUser));
            }

            if (isDirector)
            {
                requests = requests
                    .Where(r => r.Status == (int)StatusRequest.Agreed 
                    || r.Status == (int)StatusRequest.SendForApprove 
                    && r.Users
                    .Contains(currentUser));
            }

            if (unit != null && unit != null)
            {
                requests = requests
                    .Where(u => u.Unit == unit);
            }

            if (!String.IsNullOrEmpty(name))
            {
                requests = requests
                    .Where(u => u.Name.Contains(name));
            }

            if (!String.IsNullOrEmpty(surname))
            {
                requests = requests.Where(u => u.Surname.Contains(surname));
            }

            List<SelectListItem> units = new SelectList(_context.Units, "Name", "Name").ToList();
            units.Insert(0, (new SelectListItem {Text="***Все***", Value=""}));
            ViewData["Units"] = units;
            ViewData["NameSort"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["SurnameSort"] = sortOrder == "surname_asc" ? "surname_desc" : "surname_asc";
            ViewData["StatusSort"] = sortOrder == "status_asc" ? "status_desc" : "status_asc";
            ViewData["UnitSort"] = sortOrder == "unit_asc" ? "unit_desc" : "unit_asc";
            ViewData["DateSort"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["TypeSort"] = sortOrder == "type_asc" ? "type_desc" : "type_asc";
            requests = sortOrder switch
            {
                "name_desc" => requests.OrderByDescending(s => s.Name),
                "name_asc" => requests.OrderBy(s => s.Name),
                "surname_desc" => requests.OrderByDescending(s => s.Surname),
                "surname_asc" => requests.OrderBy(s => s.Surname),
                "Date" => requests.OrderBy(s => s.CreationDate),
                "status_asc" => requests.OrderBy(s => s.Status),
                "status_desc" => requests.OrderByDescending(s => s.Status),
                "unit_desc" => requests.OrderByDescending(s => s.Unit),
                "unit_asc" => requests.OrderBy(s => s.Unit),
                "type_desc" => requests.OrderByDescending(s => s.RequestTypeNavigation.Name),
                "type_asc" => requests.OrderBy(s => s.RequestTypeNavigation.Name),
                _ => requests.OrderByDescending(s => s.CreationDate),
            };
            return View(await requests.ToListAsync());
        }

        //Получить заявки текущего пользователя
        public async Task<IActionResult> GetRequestsForPerson(string sortOrder, string? message)
        {
            ViewData["Message"] = message;
            int userId = GetCurrentUserId();
            var requests = from Request 
                           in _context.Requests
                           join User 
                           in _context.Users
                           on Request.Sender 
                           equals User.Id
                           where User.Id == userId 
                           select new RequestListViewModel { 
                               Id = Request.Id, 
                               Unit = User.UnitNavigation.Name, 
                               Name = User.Name, 
                               Surname = User.Surname, 
                               RequestTypeNavigation = Request.RequestTypeNavigation,
                               Status = Request.Status, 
                               StatusNavigation = Request.StatusNavigation,
                               CreationDate = Request.CreationDate ,
                               StartDate = Request.StartDate,
                               EndDate = Request.EndDate,
                               Users = Request.Users
                           };
            requests.Where(r => r.Status == (int)StatusRequest.New);

            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewData["SurnameSort"] = String.IsNullOrEmpty(sortOrder) ? "surname_desc" : "surname_asc";
            ViewData["StatusSort"] = String.IsNullOrEmpty(sortOrder) ? "status_desc" : "status_asc";
            ViewData["UnitSort"] = String.IsNullOrEmpty(sortOrder) ? "unit_desc" : "unit_asc";
            ViewData["DateSort"] = sortOrder == "Date" ? "date_desc" : "Date";
            requests = sortOrder switch
            {
                "name_desc" => requests.OrderByDescending(s => s.Name),
                "name_asc" => requests.OrderBy(s => s.Name),
                "surname_desc" => requests.OrderByDescending(s => s.Surname),
                "surname_asc" => requests.OrderBy(s => s.Surname),
                "Date" => requests.OrderBy(s => s.CreationDate),
                "status_asc" => requests.OrderBy(s => s.Status),
                "status_desc" => requests.OrderByDescending(s => s.Status),
                "unit_desc" => requests.OrderByDescending(s => s.Unit),
                "unit_asc" => requests.OrderBy(s => s.Unit),
                _ => requests.OrderByDescending(s => s.CreationDate),
            };
            return View(await requests.ToListAsync());
        }

        //Детальная информация по заявке
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFromDB = await _context.Requests
                .Include(r => r.RequestTypeNavigation)
                .Include(r => r.SenderNavigation)
                .Include(r => r.StatusNavigation)
                .Include(r => r.Users)
                .FirstOrDefaultAsync(m => m.Id == id);

            var substituteEmployee = await _context.Users.FirstOrDefaultAsync(u => u.Id == requestFromDB.SubstituteEmployee);

            if(requestFromDB == null)
            {
                return NotFound();
            }
            var request = RequestDetailsViewModel.FromRequest(requestFromDB);
            if(substituteEmployee != null)
            {
                request.SubstituteEmployee = substituteEmployee.Name;
            }

            if (request == null)
            {
                return NotFound();
            }
            return PartialView(request);
        }

        //Создание заявки
        public IActionResult Create()
        {                  
            return View();
        }
        public IActionResult ShowFormCreate(int? id)
        {
            bool isManager = User.IsInRole("Руководитель");
            bool isDirector = User.IsInRole("Директор");
            var currentUser = GetCurrentUserAsync().Result;
            var usersForSubstitute = _context.Users
                .Where(u => u.Unit == currentUser.Unit && currentUser.Id != u.Id)
                .Where(u => u.Roles
                .Any(u => u.Id == (int)Roles.Employee || u.Id == (int)Roles.Manager))
                .AsNoTracking();

            if (isManager)
            {
                var usersForApprove = _context.Users
                .Where(u => u.Unit == currentUser.Unit)
                .Where(u => u.Roles
                .Any(u => u.Id == (int)Roles.Director))
                .AsNoTracking();

                ViewData["UsersForApprove"] = new SelectList(usersForApprove, "Id", "Surname");
            }
            else if (isDirector)
            {
                var usersForApprove = _context.Users
                .Where(u => u.Id == currentUser.Id)
                .AsNoTracking();
                ViewData["UsersForApprove"] = new SelectList(usersForApprove, "Id", "Surname", currentUser.Id);
            }
            else
            {
                var usersForApprove = _context.Users
               .Where(u => u.Unit == currentUser.Unit)
               .Where(u => u.Roles
               .Any(u => u.Id == (int)Roles.Director || u.Id == (int)Roles.Manager))
               .AsNoTracking();
                ViewData["UsersForApprove"] = new SelectList(usersForApprove, "Id", "Surname");
            }

            ViewData["UsersForSubstitute"] = new SelectList(usersForSubstitute, "Id", "Surname");
            ViewData["RequestType"] = new SelectList(_context.RequestTypes, "Id", "Name");

            return id switch
            {
                1 => PartialView("_Vacation"),
                2 => PartialView("_VacationDay"),
                3 => PartialView("_DayOffForWork"),
                4 => PartialView("_DayOff"),
                5 => PartialView("_RemoteWork"),
                _ => RedirectToAction(nameof(ShowFormCreate)),
            };
        }

        //Создание заявки POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreateViewModel request, int approverId)
        {
           /* if (request.RequestType == (int)RequestsTypes.Vacation || request.RequestType == (int)RequestsTypes.VacationDay)
            {
                var requestForCheckIntersection = await _context.Requests
                .Where(r => r.Status == (int)StatusRequest.SendForApprove || r.Status == (int)StatusRequest.Approved)
                .Select(r => new { r.StartDate, r.EndDate })
                .ToListAsync();
                foreach (var requestForCheck in requestForCheckIntersection)
                {
                    if (request.StartDate.IsInRange(requestForCheck.StartDate, request.EndDate) || request.EndDate.IsInRange(requestForCheck.EndDate, request.StartDate))
                    {
                        ModelState.AddModelError(nameof(request.StartDate), "Заявленные даты пересекаются с другими заявками");
                        return View();
                    }
                }
            }*/
            if (request.EndDate == DateTime.MinValue)
            {
                request.EndDate = request.StartDate;
            }
            if (request.StartDate.CompareTo(request.EndDate) > 0)
            {
                ModelState.AddModelError(nameof(request.StartDate), "Дата конца раньше даты начала");
            }
            if (request.StartDate.CompareTo(DateTime.Now) < 0 || request.EndDate.CompareTo(DateTime.Now) < 0)
            {
                ModelState.AddModelError(nameof(request.StartDate), "Дата конца/начала уже прошла");
            }
            if (ModelState.IsValid)
            {
                int userId = GetCurrentUserId();
                var currentUser = GetCurrentUserAsync().Result;

                Request requestToDb = new()
                {
                    Id = request.Id,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Comment = request.Comment,
                    PortabilityDate = request.PortabilityDate,
                    RequestType = request.RequestType ?? 0,
                    SubstituteEmployee = request.SubstituteEmployee,
                };

                requestToDb.Sender = userId;
                var approver = await _context.Users.FindAsync(approverId);
                if(approver == null)
                {
                    return NotFound();
                }
                requestToDb.Users.Add(approver);
                bool isManager = User.IsInRole("Руководитель");
                bool isDirector = User.IsInRole("Директор");


                requestToDb.Status = (int)StatusRequest.New;


                requestToDb.CreationDate = DateTime.Now;
                _context.Add(requestToDb);
                await _context.SaveChangesAsync();
                if (isDirector)
                {

                    await SendForAgree(requestToDb.Id);
                    await Agree(requestToDb.Id);
                    await Approve(requestToDb.Id);
                }
                else if (isManager)
                {

                    await SendForAgree(requestToDb.Id);
                    await Agree(requestToDb.Id);
                }
                return RedirectToAction(nameof(GetRequestsForPerson));
            }
            else
            {
                return View(request);
            }
        }

        //Рассмотреть заявку, кто назначен согласующим
        [Authorize(Roles = "Руководитель,Директор")]
        public async Task<IActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFromDB = await _context.Requests
                .Include(r => r.RequestTypeNavigation)
                .Include(r => r.SenderNavigation)
                .Include(r => r.Users)
                .Include(r => r.StatusNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            var senderUser = await _context.Users.FindAsync(requestFromDB.Sender);
            var usersForSubstitute = _context.Users.Where(u => u.Unit == senderUser.Unit)
                .Where(u => u.Roles
                .Any(u => u.Id == (int)Roles.Employee || u.Id == (int)Roles.Manager))
                .AsNoTracking(); 
            ViewData["UsersForSubstitute"] = new SelectList(usersForSubstitute, "Id", "Surname");

            var substituteEmployee = await _context.Users.FirstOrDefaultAsync(u => u.Id == requestFromDB.SubstituteEmployee);
            var request = RequestDetailsViewModel.FromRequest(requestFromDB);
            if (substituteEmployee != null)
            {
                request.SubstituteEmployee = substituteEmployee.Name;
            }

            if (request == null)
            {
                return NotFound();
            }
            if (request.Status == (int)StatusRequest.SendForApprove)
            {
                return PartialView("ReviewAgree",request);
            }
            else
            {
                return PartialView("ReviewApprove", request);
            }
        }   

        //Отправить на согласование
        public async Task<IActionResult> SendForAgree(int? id) //Отправить на согласование
        {
            var request = await _context.Requests.FindAsync(id);
            var currentUser = await GetCurrentUserAsync();

            if (request.RequestType == (int)RequestsTypes.Vacation && ((request.EndDate - request.StartDate).Days > currentUser.UnusedVacationDays))
            {
                return RedirectToAction(nameof(GetRequestsForPerson), new {message = "Количество заданных дней отпуска превышает доступные"});            
            }
            if (request.Status == (int)StatusRequest.New || request.Status == (int)StatusRequest.NotAgreed)
            {
                request.Status = (int)StatusRequest.SendForApprove;
                request.SendingDate = DateTime.Now;
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetRequestsForPerson));
            }
            return RedirectToAction(nameof(GetRequestsForPerson));
        }

        //Согласовать заявку
        [Authorize(Roles = "Руководитель")]
        public async Task<IActionResult> Agree(int? id) //Согласование заявки
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUserAsync().Result;
            var senderUser = await _context.Users
                .Include(u => u.UnitNavigation)
                .FirstOrDefaultAsync(u => u.Id == request.Sender);
            
            if (request.Status == (int)StatusRequest.SendForApprove && senderUser.Unit == currentUser.Unit)
            {
                request.Status = (int)StatusRequest.Agreed;
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetRequests));
            }
            return NotFound();
        }

        //Отклонить с согласования заявку
        [Authorize(Roles = "Руководитель")]
        public async Task<IActionResult> NotAgree(int? id) //Согласование заявки
        {
            if (id == null)
            {
                return NotFound();
            }
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUserAsync().Result;
            var senderUser = await _context.Users
                .Include(u => u.UnitNavigation)
                .FirstOrDefaultAsync(u => u.Id == request.Sender);

            if (request.Status == (int)StatusRequest.SendForApprove && senderUser.Unit == currentUser.Unit)
            {
                request.Status = (int)StatusRequest.NotAgreed;
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetRequests));
            }
            return NotFound();
        }

        //Не подтвердить заявку
        [Authorize(Roles = "Директор")]
        public async Task<IActionResult> NotApprove(int? id) //Утверждение заявки
        {
            if (id == null)
            {
                return NotFound();
            }
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }
            if (request.Status == (int)StatusRequest.Agreed)
            {
                request.Status = (int)StatusRequest.NotApproved;
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetRequests));
            }
            return NotFound();
        }

        //Подтвердить заявку
        [Authorize(Roles = "Директор")]
        public async Task<IActionResult> Approve(int? id) //Утверждение заявки
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }
            if (request.Status == (int)StatusRequest.Agreed)
            {
                if(request.RequestType == (int)RequestsTypes.Vacation || request.RequestType == (int)RequestsTypes.VacationDay)
                {
                    var sender = await _context.Users.FindAsync(request.Sender);
                    sender.UnusedVacationDays -= (request.EndDate - request.StartDate).Days;
                }
                request.Users.Add(GetCurrentUserAsync().Result);
                request.Status = (int)StatusRequest.Approved;
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetRequests));
            }
            return NotFound();
        }


        //Редактирование заявки GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFromDB = await _context.Requests.Include(r => r.RequestTypeNavigation).Include(r => r.Users).FirstOrDefaultAsync(r => r.Id == id);

            if (requestFromDB == null)
            {
                return NotFound();
            }

            var request = new RequestEditViewModel()
            {
                Id = requestFromDB.Id,
                Comment = requestFromDB.Comment,
                RequestType = requestFromDB.RequestType,
                EndDate = requestFromDB.EndDate,
                StartDate = requestFromDB.StartDate,
                Users = requestFromDB.Users,
                PortabilityDate = requestFromDB.PortabilityDate,
                SubstituteEmployee = requestFromDB.SubstituteEmployee,
                RequestTypeNavigation = requestFromDB.RequestTypeNavigation
            };

            var currentUser = GetCurrentUserAsync().Result;
            var usersForSubstitute = _context.Users
                .Where(u => u.Unit == currentUser.Unit)
                .Where(u => u.Roles
                .Any(u => u.Id == (int)Roles.Employee || u.Id == (int)Roles.Manager))
                .AsNoTracking();
            var usersForApprove =  _context.Users
               .Where(u => u.Unit == currentUser.Unit)
               .Where(u => u.Roles
               .Any(u => u.Id == (int)Roles.Manager || u.Id == (int)Roles.Director))
               .AsNoTracking();

            request.UsersForSubstitute = new SelectList(usersForSubstitute, "Id", "Surname", request.SubstituteEmployee);
            request.UsersForApprove = new SelectList(usersForSubstitute, "Id", "Surname", request.Users.First().Id);
            request.RequestTypes = new SelectList(_context.RequestTypes, "Id", "Name", request.RequestType);

            return PartialView(request);
        }

        //Редактирование заявки POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int approverId, RequestEditViewModel request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }
            if (request.EndDate == DateTime.MinValue)
            {
                request.EndDate = request.StartDate;
            }
            if (request.StartDate.CompareTo(request.EndDate) > 0)
            {
                return RedirectToAction(nameof(GetRequestsForPerson), new { message = "Дата конца раньше даты начала" });
            }
            if (request.StartDate.CompareTo(DateTime.Now) < 0 || request.EndDate.CompareTo(DateTime.Now) < 0)
            {
                return RedirectToAction(nameof(GetRequestsForPerson), new { message = "Дата конца/начала уже прошла" });
            }
            var requestFromDB = await _context.Requests.Include(r => r.Users).FirstOrDefaultAsync(r => r.Id == request.Id);
            if (request.EndDate != DateTime.MinValue && request.StartDate != DateTime.MinValue)
            {
                requestFromDB.EndDate = request.EndDate;
                requestFromDB.StartDate = request.StartDate;
             }
            requestFromDB.Comment = request.Comment;
            requestFromDB.PortabilityDate = request.PortabilityDate;
            requestFromDB.RequestType = request.RequestType;
            requestFromDB.SubstituteEmployee = request.SubstituteEmployee;
            requestFromDB.Users.Clear();
            var approver = await _context.Users.FindAsync(approverId);
            if (approver != null)
            {
                requestFromDB.Users.Add(approver);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GetRequestsForPerson));
        }

        public async Task<IActionResult> EditFromReview(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFromDB = await _context.Requests.Include(r => r.RequestTypeNavigation).Include(r => r.Users).FirstOrDefaultAsync(r => r.Id == id);

            if (requestFromDB == null)
            {
                return NotFound();
            }

            var request = new RequestEditViewModel()
            {
                Id = requestFromDB.Id,
                Comment = requestFromDB.Comment,
                RequestType = requestFromDB.RequestType,
                EndDate = requestFromDB.EndDate,
                StartDate = requestFromDB.StartDate,
                Users = requestFromDB.Users,
                PortabilityDate = requestFromDB.PortabilityDate,
                SubstituteEmployee = requestFromDB.SubstituteEmployee,
                RequestTypeNavigation = requestFromDB.RequestTypeNavigation,
            };
            var currentUser = GetCurrentUserAsync().Result;
            var usersForSubstitute = _context.Users
                .Where(u => u.Unit == currentUser.Unit)
                .Where(u => u.Roles
                .Any(u => u.Id == (int)Roles.Employee || u.Id == (int)Roles.Manager))
                .AsNoTracking();

            request.UsersForSubstitute = new SelectList(usersForSubstitute, "Id", "Surname", request.SubstituteEmployee);

            return PartialView("_EditFromReview", request);
        }

        //Редактирование заявки POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFromReview(int id, int approverId, RequestEditViewModel request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }
            var requestFromDB = await _context.Requests.FindAsync(request.Id);
            if (request.EndDate == DateTime.MinValue)
            {
                request.EndDate = request.StartDate;
            }
            if (request.StartDate.CompareTo(request.EndDate) > 0)
            {
                return RedirectToAction(nameof(GetRequests), new { message = "Дата конца раньше даты начала" });
            }
            if (request.StartDate.CompareTo(DateTime.Now) < 0 || request.EndDate.CompareTo(DateTime.Now) < 0)
            {
                return RedirectToAction(nameof(GetRequests), new { message = "Дата конца/начала уже прошла" });
            }

            if (request.EndDate != DateTime.MinValue && request.StartDate != DateTime.MinValue)
            {
                requestFromDB.EndDate = request.EndDate;
                requestFromDB.StartDate = request.StartDate;
            }
            requestFromDB.Comment = request.Comment;
            requestFromDB.SubstituteEmployee = request.SubstituteEmployee;
            await _context.SaveChangesAsync();

             return RedirectToAction(nameof(GetRequests));
        }

        //Отмена отправки
        public async Task<IActionResult> CancelSend(int? id) //Отменить отправку
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.StatusNavigation)
                .Include(r => r.SenderNavigation)
                .Include(r => r.RequestTypeNavigation)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }
            return PartialView(request);          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSendConfirmed(int id) //Подтвердить отмену заявки
        {
            var request = await _context.Requests.FindAsync(id);
            if (request.Status == (int)StatusRequest.SendForApprove || request.Status == (int)StatusRequest.Agreed)
            {
                request.Status = (int)StatusRequest.New;
                request.SendingDate = null;
                _context.Update(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetRequestsForPerson));
            }
            return RedirectToAction(nameof(GetRequestsForPerson));
        }

        //Отменить заявку
        public async Task<IActionResult> Cancel(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.StatusNavigation)
                .Include(r => r.SenderNavigation)
                .Include(r => r.RequestTypeNavigation)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return PartialView(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id) //Подтвердить отмену заявки
        {
            var request = await _context.Requests.FindAsync(id);
            request.Status = (int)StatusRequest.Canceled;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetRequestsForPerson));
        }

        //Получить ID текущего пользователя
        private int GetCurrentUserId()
        {
            return Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        //Получить текущего пользователя
        private async Task<User> GetCurrentUserAsync()
        {
            var user = await _context.Users.FindAsync(GetCurrentUserId());

            if(user != null)
            {
                return user;
            }
            else
            {
                throw new Exception();
            }
        }

        //Есть ли заявка в БД
        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}