using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentDocs.Data;
using StudentDocs.Models;

namespace StudentDocs.Pages
{
    public class DocumentsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DocumentsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Document> Documents { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Documents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                query = query.Where(d =>
                    d.FileName.Contains(Q) || d.Category.Contains(Q));
            }

            Documents = await query
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }
    }
}
