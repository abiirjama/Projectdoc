using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentDocs.Data;
using StudentDocs.Models;

namespace StudentDocs.Pages
{
    // Handles deleting a document
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Inject database context and hosting environment
        public DeleteModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Document ID
        [BindProperty]
        public int Id { get; set; }

        // Document info for confirmation display
        public string FileName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // Error message if something goes wrong
        public string? ErrorMessage { get; set; }

        // Load document details for confirmation
        public IActionResult OnGet(int id)
        {
            var doc = _context.Documents.Find(id);
            if (doc == null)
            {
                ErrorMessage = "Document not found.";
                return Page();
            }

            Id = doc.Id;
            FileName = doc.FileName;
            Category = doc.Category;

            return Page();
        }

        // Delete document after confirmation
        public IActionResult OnPost()
        {
            var doc = _context.Documents.Find(Id);
            if (doc == null)
            {
                ErrorMessage = "Document not found.";
                return Page();
            }

            // Delete physical file if it exists
            var filePath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Remove record from database
            _context.Documents.Remove(doc);
            _context.SaveChanges();

            // Redirect back to documents list
            return RedirectToPage("/Documents");
        }
    }
}
