using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using xampl.Models.Documents;
using xampl.Services.Repository;
using xampl.ViewModels;

namespace xampl.Controllers
{
    public class DocumentsController(
        DocumentsContext context,
        IRepository<DocumentsContext> documentsRepository,
        IMapper mapper,
        ILogger<DocumentsController> logger
    ) : Controller
    {
        private readonly DocumentsContext _context = context;
        private readonly IRepository<DocumentsContext> _documentsRepository = documentsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DocumentsController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            var documentsContext = _context.Documents.Include(d => d.CreatedByNavigation);
            var documentVMs = await documentsContext.Select(d => _mapper.Map<DocumentVM>(d)).ToListAsync();
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
            if (ModelState.IsValid)
            {
                var document = _mapper.Map<Document>(documentVM);
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(documentVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var document = await _context.Documents.FindAsync(id);
            if (document == null) return NotFound();
            var documentVM = _mapper.Map<DocumentVM>(document);
            return View(documentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentVM documentVM)
        {
            if (id != documentVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var document = _mapper.Map<Document>(documentVM);
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(documentVM);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
