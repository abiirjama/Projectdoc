using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentDocs.Data;
using StudentDocs.Models;

namespace StudentDocs.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DeleteModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

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

        public IActionResult OnPost()
        {
            var doc = _context.Documents.Find(Id);
            if (doc == null)
            {
                ErrorMessage = "Document not found.";
                return Page();
            }

            // Dosya fiziksel olarak da silinsin istersek:
            var filePath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Documents.Remove(doc);
            _context.SaveChanges();

            return RedirectToPage("/Documents");
        }
    }
}
