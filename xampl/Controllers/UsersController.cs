using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // GET: Users
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

        // GET: Users/Details/5
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

        // GET: Users/Create
        public IActionResult Create()
        {
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
            return View(userVM);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _documentsRepository.GetAllAsQueryable<User>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            
            return View(_mapper.Map<UserVM>(user));
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserVM userVM)
        {
            if (id != userVM.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _documentsRepository.UpdateAsync(_mapper.Map<User>(userVM));
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
            return View(userVM);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            //TODO: delete roles as well, in a single transaction;
            var user = await _documentsRepository.GetAllAsQueryable<User>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user is not null)
            {
                await _documentsRepository.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(int id)
        {
            return await _documentsRepository.GetAllAsQueryable<User>()
                .AnyAsync(x => x.Id == id);
        }
    }
}
