using System;

namespace StudentDocs.Models
{
    // Represents a document uploaded by a student
    public class Document
    {
        // Primary key
        public int Id { get; set; }

        // Original file name
        public string FileName { get; set; } = string.Empty;

        // Document category (e.g. Transcript, ID, Certificate)
        public string Category { get; set; } = string.Empty;

        // File size in bytes
        public long FileSize { get; set; }

        // Relative path to the stored file
        public string FilePath { get; set; } = string.Empty;

        // Date and time when the file was uploaded
        public DateTime UploadDate { get; set; }
    }
}
