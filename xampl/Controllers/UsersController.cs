using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using xampl.Models.Documents;
using xampl.Services.RepositoryService;
using xampl.Utils;
using xampl.ViewModels;

namespace xampl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController(
        IRepository<DocumentsContext> documentsRepository,
        IMapper mapper,
        ILogger<UsersController> logger
    ) : Controller
    {
        private readonly IRepository<DocumentsContext> _documentsRepository = documentsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UsersController> _logger = logger;

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            //TODO: introduce Search;
            ToastUtils.BindData(ViewBag, TempData);
            var totalCount = await _documentsRepository
                .GetAllAsQueryable<User>()
                .CountAsync();
            var userVMs = await _documentsRepository
                .GetAllAsQueryable<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => _mapper.Map<UserVM>(x))
                .ToListAsync();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(userVMs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _documentsRepository
                .GetAllAsQueryable<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();

            return View(_mapper.Map<UserVM>(user));
        }

        public async Task<IActionResult> Create()
        {
            await AttachAvailableRolesToViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                await _documentsRepository.CreateAsync(_mapper.Map<User>(userVM));
                return RedirectToAction(nameof(Index));
            }
            await AttachAvailableRolesToViewData();
            return View(userVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _documentsRepository
                .GetAllAsQueryable<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();

            await AttachAvailableRolesToViewData();
            return View(_mapper.Map<UserVM>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserVM userVM, [FromForm] List<int> SelectedRoleIds)
        {
            if (id != userVM.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _documentsRepository
                        .GetAllAsQueryable<User>()
                        .Include(u => u.UserRoles)
                        .FirstOrDefaultAsync(u => u.Id == id);
                    if (userToUpdate is null) return NotFound();

                    userVM.UserRoles = [..
                        SelectedRoleIds
                        .Distinct()
                        .Select(roleId => new UserRole
                        {
                            UserId = userVM.Id,
                            RoleId = roleId
                        })
                    ];
                    await _documentsRepository.UpdateUserWithRoles(_mapper.Map<User>(userVM));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(userVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await AttachAvailableRolesToViewData();
            return View(userVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _documentsRepository.GetAllAsQueryable<User>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user is not null)
            {
                var userRoles = _documentsRepository.GetAllAsQueryable<UserRole>().Where(x => x.UserId == id);
                await _documentsRepository.DeleteManyAsync(userRoles);
                await _documentsRepository.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(int id)
        {
            return await _documentsRepository.GetAllAsQueryable<User>()
                .AnyAsync(x => x.Id == id);
        }

        private async Task AttachAvailableRolesToViewData()
        {
            var availableRoles = await _documentsRepository.GetAllAsQueryable<Role>().ToListAsync() ?? new List<Role>();
            ViewData["AvailableRoles"] = availableRoles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Title
            }).ToList();
        }
    }
}
