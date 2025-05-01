using PropertyTax.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;               // הוסף
//using UglyToad.PdfPig.Document;
namespace PropertyTax.Service
{
    public class PdfParser : IPdfParser
    {
        public Task<string> ExtractTextAsync(Stream pdfStream)
        {
            using var doc = PdfDocument.Open(pdfStream);
            var sb = new StringBuilder();
            foreach (var page in doc.GetPages())
                sb.AppendLine(page.Text);
            return Task.FromResult(sb.ToString());
        }
    }
}
