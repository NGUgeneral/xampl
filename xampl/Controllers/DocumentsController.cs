using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using xampl.Models.Documents;
using xampl.Services.RepositoryService;
using xampl.Utils;
using xampl.ViewModels;

namespace xampl.Controllers
{
    public class DocumentsController(
        IRepository<DocumentsContext> documentsRepository,
        IMapper mapper,
        ILogger<DocumentsController> logger
    ) : Controller
    {
        private readonly IRepository<DocumentsContext> _documentsRepository = documentsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DocumentsController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            ToastUtils.BindData(ViewBag, TempData);
            var documentsQuery = _documentsRepository
                .GetAllAsQueryable<Document>();
            var currentUser = await _documentsRepository
                .GetAllAsQueryable<User>()
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (currentUser is not null)
            {
                var personalDocumentsQuery = _documentsRepository
                    .GetAllAsQueryable<Document>()
                    .Where(x => x.CreatedBy == currentUser.Id && !x.IsPublic);
                documentsQuery = personalDocumentsQuery.Concat(documentsQuery);
            }
            var documentVMs = await documentsQuery
                .Select(x => _mapper.Map<DocumentVM>(x))
                .ToListAsync();
            return View(documentVMs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var document = await _documentsRepository
                .GetAllAsQueryable<Document>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (document == null) return NotFound();

            var documentVM = _mapper.Map<DocumentVM>(document);
            return View(documentVM);
        }

        [Authorize]
        public IActionResult Create()
        {
            var documentVM = new DocumentVM();
            return View(documentVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentVM documentVM)
        {
            //TODO: wrap it all in try block and move into utils;
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _documentsRepository
                .GetAllAsQueryable<User>()
                .FirstOrDefaultAsync(x => x.Email == userEmail);
            if (user is null)
            {
                user = new User
                {
                    CreatedAt = DateTime.UtcNow,
                    Email = userEmail,
                    FirstName = User.FindFirstValue(ClaimTypes.GivenName),
                    LastName = User.FindFirstValue(ClaimTypes.Surname),
                    Source = User.FindFirstValue(User.Identity?.AuthenticationType ?? "unknown")
                };
                await _documentsRepository.CreateAsync(user);
            }

            if (ModelState.IsValid)
            {
                documentVM.CreatedBy = user.Id;
                documentVM.LastUpdatedBy = user.Id;
                documentVM.CreatedAt = DateTime.UtcNow;
                documentVM.LastUpdatedAt = DateTime.UtcNow;
                var document = _mapper.Map<Document>(documentVM);
                await _documentsRepository.CreateAsync(document);
                return RedirectToAction(nameof(Index));
            }

            return View(documentVM);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var document = await _documentsRepository
                .GetAllAsQueryable<Document>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (document == null) return NotFound();
            var documentVM = _mapper.Map<DocumentVM>(document);
            return View(documentVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentVM documentVM)
        {
            //TODO: wrap it all in try block and move into utils;
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _documentsRepository
                .GetAllAsQueryable<User>()
                .FirstOrDefaultAsync(x => x.Email == userEmail);
            //TODO: this is repeated in several places. Generalize;
            if (user is null)
            {
                user = new User
                {
                    CreatedAt = DateTime.UtcNow,
                    Email = userEmail,
                    FirstName = User.FindFirstValue(ClaimTypes.GivenName),
                    LastName = User.FindFirstValue(ClaimTypes.Surname),
                    Source = User.FindFirstValue(User.Identity?.AuthenticationType ?? "unknown")
                };
                await _documentsRepository.CreateAsync(user);
            }

            if (id != documentVM.Id) return NotFound();

            if (ModelState.IsValid)
            {
                documentVM.LastUpdatedBy = user.Id;
                documentVM.LastUpdatedAt = DateTime.UtcNow;
                var document = _mapper.Map<Document>(documentVM);
                try
                {
                    await _documentsRepository.UpdateAsync(document);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{exMessage}", ex.Message);
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(documentVM);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var document = await _documentsRepository
                .GetAllAsQueryable<Document>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (document == null) return NotFound();
            var documentVM = _mapper.Map<DocumentVM>(document);
            return View(documentVM);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _documentsRepository
                .GetAllAsQueryable<Document>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (document is not null)
            {
                await _documentsRepository.DeleteAsync(document);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
