using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentDocs.Data;
using StudentDocs.Models;

namespace StudentDocs.Pages
{
    // Handles uploading a new document (any file type allowed)
    public class UploadModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Inject database context and hosting environment
        public UploadModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Uploaded file from the form
        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        // Document category
        [BindProperty]
        public string Category { get; set; } = string.Empty;

        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if a file was selected
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                ErrorMessage = "Please select a file.";
                return Page();
            }

            // Check if category is provided
            if (string.IsNullOrWhiteSpace(Category))
            {
                ErrorMessage = "Please enter a category.";
                return Page();
            }

            // Validate file size (max 10 MB) - adjust if needed
            const long maxSize = 10 * 1024 * 1024;
            if (UploadedFile.Length > maxSize)
            {
                ErrorMessage = "File is too large. Max 10 MB.";
                return Page();
            }

            // Create uploads folder if it does not exist
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            // Generate a unique file name (prevents overwriting)
            var safeFileName = Path.GetFileName(UploadedFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to wwwroot/uploads
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await UploadedFile.CopyToAsync(stream);
            }

            // Save document info to database
            var doc = new Document
            {
                FileName = safeFileName,
                Category = Category,
                FileSize = UploadedFile.Length,
                FilePath = "/uploads/" + uniqueFileName,
                UploadDate = DateTime.Now
            };

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            // Success message
            Message = "File uploaded successfully!";
            ModelState.Clear();
            Category = string.Empty;

            return Page();
        }
    }
}
