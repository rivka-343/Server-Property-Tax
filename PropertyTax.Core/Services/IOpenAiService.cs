using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{
    public interface IOpenAiService
    {
       Task<string> GetChatResponse(string userMessage,string request);
       Task<string> Run();

        Task<IEnumerable<string>> ListModelsAsync();

    }
}
