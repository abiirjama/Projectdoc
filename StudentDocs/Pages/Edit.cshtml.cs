using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentDocs.Data;
using StudentDocs.Models;

namespace StudentDocs.Pages
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Document ID
        [BindProperty]
        public int Id { get; set; }

        // Editable category
        [BindProperty]
        public string Category { get; set; } = string.Empty;

        // Optional new file
        [BindProperty]
        public IFormFile? NewFile { get; set; }

        // Current file info (for display)
        public string? CurrentFileName { get; set; }
        public string? CurrentFilePath { get; set; }

        // Messages
        public string? ErrorMessage { get; set; }
        public string? Message { get; set; }

        // Load existing document
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return RedirectToPage("/Documents");

            Id = doc.Id;
            Category = doc.Category;

            // Set current file info
            CurrentFileName = doc.FileName;
            CurrentFilePath = doc.FilePath;

            return Page();
        }

        // Save changes
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Category))
            {
                ErrorMessage = "Category is required.";
                return Page();
            }

            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == Id);
            if (doc == null)
                return RedirectToPage("/Documents");

            doc.Category = Category;

            // Replace file if a new one is uploaded
            if (NewFile != null && NewFile.Length > 0)
            {
                // Validate file size (max 5 MB)
                const long maxSize = 5 * 1024 * 1024;
                if (NewFile.Length > maxSize)
                {
                    ErrorMessage = "File is too large. Max 5 MB.";
                    return Page();
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".docx", ".png", ".jpg", ".jpeg" };
                var ext = Path.GetExtension(NewFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    ErrorMessage = "Only PDF, DOCX, PNG, JPG files are allowed.";
                    return Page();
                }

                // Delete old file if it exists
                if (!string.IsNullOrWhiteSpace(doc.FilePath))
                {
                    var oldPhysicalPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhysicalPath))
                    {
                        System.IO.File.Delete(oldPhysicalPath);
                    }
                }

                // Save new file
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = Path.GetFileName(NewFile.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
                var newPhysicalPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(newPhysicalPath, FileMode.Create))
                {
                    await NewFile.CopyToAsync(stream);
                }

                // Update document info
                doc.FileName = safeFileName;
                doc.FileSize = NewFile.Length;
                doc.FilePath = "/uploads/" + uniqueFileName;
                doc.UploadDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // Update displayed file info
            CurrentFileName = doc.FileName;
            CurrentFilePath = doc.FilePath;

            Message = "Document updated successfully!";
            return Page();
        }
    }
}
