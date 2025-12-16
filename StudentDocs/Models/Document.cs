using System;

namespace StudentDocs.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
    }
}
