using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{
    public interface IPdfParser
    {
        public Task<string> ExtractTextAsync(Stream pdfStream);

    }
}
