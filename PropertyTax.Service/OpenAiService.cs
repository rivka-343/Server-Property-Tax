using Microsoft.Extensions.Configuration;
using PropertyTax.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace PropertyTax.Servise
{
  public class OpenAiService: IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            
          //  _apiKey = configuration["OpenAI:ApiKey"];
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        }

        public async Task<string> GetChatResponse(string userMessage)
        {
            var payload = new
            {
               // model = "gpt-3.5-turbo",  // או כל מודל שיש לך גישה אליו
                model = "gpt-4o-mini",  // או כל מודל שיש לך גישה אליו
                messages = new[]
            {
                new { role = "user", content = userMessage }
            }
            };
            var json = JsonSerializer.Serialize(payload);
            var resp = await _httpClient.PostAsync("chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json"));

            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            // נניח שה־response JSON הוא במבנה { choices: [ { message: { content: "..." } } ] }
            var message = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();
            return message ?? "";
        
        //var requestBody = new
        //{
        //    model = "gpt-3.5-turbo",
        //    messages = new[]
        //    {
        //        new { role = "system", content = "אתה עוזר AI." },
        //        new { role = "user", content = userMessage }
        //    },
        //    max_tokens = 100
        //};

        //var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        //HttpResponseMessage response = await _httpClient.PostAsync(ApiUrl, requestContent);
        ////string responseBody = await response.Content.ReadAsStringAsync();
        ////using JsonDocument doc = JsonDocument.Parse(responseBody);
        ////return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        //return await response.Content.ReadAsStringAsync();
    }

        public async Task<IEnumerable<string>> ListModelsAsync()
        {
            var resp = await _httpClient.GetAsync("models");
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new ApplicationException($"list-models failed ({resp.StatusCode}): {body}");

            using var doc = JsonDocument.Parse(body);
            return doc.RootElement
                      .GetProperty("data")
                      .EnumerateArray()
                      .Select(e => e.GetProperty("id").GetString()!)
                      .ToList();
        }
    }
}
