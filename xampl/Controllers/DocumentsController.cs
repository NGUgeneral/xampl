using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
            var documentVMs = await _documentsRepository.GetAllAsQueryable<Document>()
                .Select(d => _mapper.Map<DocumentVM>(d))
                .ToListAsync();
            return View(documentVMs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var document = await _documentsRepository.GetDocumentById((int)id);
            if (document == null) return NotFound();

            var documentVM = _mapper.Map<DocumentVM>(document);

            return View(documentVM);
        }

        public IActionResult Create()
        {
            var documentVM = new DocumentVM();
            //documentVM.CreatedBy = Request.User.GetUserId();
            return View(documentVM);
        }

        [HttpPost]
        public IActionResult AddNote(DocumentVM documentVM)
        {
            documentVM.DocumentNotes.Add(new DocumentNoteVM());
            return View(nameof(Create), documentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentVM documentVM)
        {
            //TODO: wrap it all in try block and move into utils;
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _documentsRepository.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Email == userEmail);
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
                foreach (var note in documentVM.DocumentNotes)
                {
                    //TODO: move this to utils;
                    note.Position = (short)documentVM.DocumentNotes.IndexOf(note);
                }
                documentVM.CreatedBy = user.Id;
                documentVM.LastUpdatedBy = user.Id;
                var document = _mapper.Map<Document>(documentVM);
                await _documentsRepository.CreateAsync(document);
                return RedirectToAction(nameof(Index));
            }

            return View(documentVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var document = await _documentsRepository.GetDocumentById((int)id);
            if (document == null) return NotFound();
            var documentVM = _mapper.Map<DocumentVM>(document);
            return View(documentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentVM documentVM)
        {
            //TODO: wrap it all in try block and move into utils;
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _documentsRepository.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Email == userEmail);
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
                foreach (var note in documentVM.DocumentNotes)
                {
                    //TODO: move this to utils;
                    note.Position = (short)documentVM.DocumentNotes.IndexOf(note);
                }
                documentVM.LastUpdatedBy = user.Id;
                var document = _mapper.Map<Document>(documentVM);
                try
                {
                    await _documentsRepository.UpdateAsync(document);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DocumentExists(document.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(documentVM);
        }

        // POST: Documents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (User.Identity is null) return Unauthorized();
            var document = await _documentsRepository.GetAllAsQueryable<Document>()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (document is not null)
            {
                await _documentsRepository.DeleteAsync(document);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DocumentExists(int id)
        {
            return await _documentsRepository.GetAllAsQueryable<Document>()
                .AnyAsync(x => x.Id == id);
        }
    }
}
