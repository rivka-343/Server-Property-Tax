using PropertyTax.Core.Models;

namespace PropertyTax.DTO
{

    public class DocDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string S3Url { get; set; }
        public DocumentType Type { get; set; } // הוספת סוג המסמך

    }
}
