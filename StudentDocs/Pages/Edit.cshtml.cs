using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentDocs.Data;

namespace StudentDocs.Pages
{
    // Handles editing an existing document
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Inject database context and hosting environment
        public EditModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Document ID
        [BindProperty]
        public int Id { get; set; }

        // Editable document category
        [BindProperty]
        public string Category { get; set; } = string.Empty;

        // Optional replacement file
        [BindProperty]
        public IFormFile? NewFile { get; set; }

        // Current file info (shown on the page)
        public string? CurrentFileName { get; set; }
        public string? CurrentFilePath { get; set; }

        // Status messages
        public string? ErrorMessage { get; set; }
        public string? Message { get; set; }

        // Load document data
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return RedirectToPage("/Documents");

            Id = doc.Id;
            Category = doc.Category;
            CurrentFileName = doc.FileName;
            CurrentFilePath = doc.FilePath;

            return Page();
        }

        // Save updated data
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate category
            if (string.IsNullOrWhiteSpace(Category))
            {
                ErrorMessage = "Category is required.";
                return Page();
            }

            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == Id);
            if (doc == null)
                return RedirectToPage("/Documents");

            doc.Category = Category;

            // Replace file if a new one was uploaded (any file type allowed)
            if (NewFile != null && NewFile.Length > 0)
            {
                // Validate file size (max 10 MB)
                const long maxSize = 10 * 1024 * 1024;
                if (NewFile.Length > maxSize)
                {
                    ErrorMessage = "File is too large. Max 10 MB.";
                    return Page();
                }

                // Delete old file if it exists
                if (!string.IsNullOrWhiteSpace(doc.FilePath))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, doc.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Save new file
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = Path.GetFileName(NewFile.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
                var newPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await NewFile.CopyToAsync(stream);
                }

                // Update document fields
                doc.FileName = safeFileName;
                doc.FileSize = NewFile.Length;
                doc.FilePath = "/uploads/" + uniqueFileName;
                doc.UploadDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // Refresh displayed file info
            CurrentFileName = doc.FileName;
            CurrentFilePath = doc.FilePath;

            Message = "Document updated successfully!";
            return Page();
        }
    }
}
