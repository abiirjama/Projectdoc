using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentDocs.Data;
using StudentDocs.Models;

namespace StudentDocs.Pages
{
    // Handles listing and searching documents
    public class DocumentsModel : PageModel
    {
        private readonly AppDbContext _context;

        // Inject database context
        public DocumentsModel(AppDbContext context)
        {
            _context = context;
        }

        // List of documents to display
        public List<Document> Documents { get; set; } = new();

        // Search query (from query string)
        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        // Load documents and apply search filter
        public async Task OnGetAsync()
        {
            var query = _context.Documents.AsQueryable();

            // Filter by file name or category
            if (!string.IsNullOrWhiteSpace(Q))
            {
                query = query.Where(d =>
                    d.FileName.Contains(Q) || d.Category.Contains(Q));
            }

            // Order by upload date (newest first)
            Documents = await query
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }
    }
}
