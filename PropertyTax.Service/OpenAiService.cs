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
using Umbraco.Web.Media;
using System.Drawing.Imaging;
using System.Net.Http.Headers;

namespace PropertyTax.Servise
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IS3Service _IS3Service;

        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

        public OpenAiService(HttpClient httpClient, IConfiguration configuration, IS3Service iS3Service)
        {
            _httpClient = httpClient;
            //  _apiKey = configuration["OpenAI:ApiKey"];
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            _IS3Service = iS3Service;
        }
        public async Task<string> GetChatResponse(string fileUrl,string request)
        {

            var payload = new
            {
                model = "gpt-4o-mini",
                messages = new object[]
                {
                new
                {
                  role = "user",
                 content = new object[]
                {
                    new { type = "text", text = request},
                    new
                    {
                        type = "image_url",
                        image_url = new
                        {
                            url =fileUrl
                            //$"data:image/jpeg;base64,{imageBase64}"
                         }
                     }
                }
                }
            },
                max_tokens = 1000
            };

            var json = JsonSerializer.Serialize(payload);
            var resp = await _httpClient.PostAsync("chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json"));

            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var message = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return message ?? "";
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

        public async Task<string> RunAssistantOnThreadAsync(string threadId, string assistantId)
        {
            var client = new HttpClient();
            var requestBody = new
            {
                assistant_id = assistantId
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseString).GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return result;
        }
        //public async Task<string> CreateThreadWithFileAsync(string fileId)
        //{

        //    var client = new HttpClient();
        //    var requestBody = new
        //    {
        //        model = "gpt-4o-mini",
        //        messages = new object[]
        //        {
        //    new
        //    {
        //        role = "user",
        //        content = "סכם לי את הקובץ המצורף.",
        //        file_ids = new[] { fileId }
        //    }
        //}
        //    };
        //    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        //    var response = await client.PostAsync("https://api.openai.com/v1/threads", jsonContent);
        //    response.EnsureSuccessStatusCode();

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var threadId = JsonSerializer.Deserialize<JsonElement>(responseString).GetProperty("id").GetString();

        //    return threadId;
        //}
        //public async Task<string> UploadFileToOpenAiAsync(string filePath)
        //{
        //    var client = new HttpClient();
        //    var requestContent = new MultipartFormDataContent();

        //    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
        //    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        //    requestContent.Add(fileContent, "file", Path.GetFileName(filePath));
        //    requestContent.Add(new StringContent("assistants"), "purpose");

        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_API_KEY");

        //    var response = await client.PostAsync("https://api.openai.com/v1/chat/completions/files", requestContent);
        //    response.EnsureSuccessStatusCode();

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var fileId = JsonSerializer.Deserialize<JsonElement>(responseString).GetProperty("id").GetString();

        //    return fileId;
        //}
        //public async Task<string> CreateThreadWithFileAsync(string fileId)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        //    // 1. יצירת thread ריק
        //    var threadResponse = await client.PostAsync("https://api.openai.com/v1/threads", null);
        //    threadResponse.EnsureSuccessStatusCode();

        //    var threadJson = await threadResponse.Content.ReadAsStringAsync();
        //    var threadId = JsonSerializer.Deserialize<JsonElement>(threadJson).GetProperty("id").GetString();

        //    // 2. הוספת הודעה עם הקובץ
        //    var messageBody = new
        //    {
        //        role = "user",
        //        content = "סכם לי את הקובץ המצורף.",
        //        file_ids = new[] { fileId }
        //    };
        //    var messageContent = new StringContent(JsonSerializer.Serialize(messageBody), Encoding.UTF8, "application/json");
        //    var messageResponse = await client.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", messageContent);
        //    messageResponse.EnsureSuccessStatusCode();

        //    return threadId;
        //}
        public async Task<string> CreateThreadWithFileAsync(string fileId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                messages = new object[]
                {
            new
            {
                role = "user",
                content = new object[]
                {
                    new { type = "text", text = "סכם לי את הקובץ המצורף." },
                    new { type = "file_id", file_id = fileId }
                }
            }
                }
            };


            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/threads", jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var threadId = JsonDocument.Parse(responseString).RootElement.GetProperty("id").GetString();

            return threadId!;
        }

        public async Task<string> UploadFileToOpenAiAsync(string fileUrl)
        {
            var client = new HttpClient();
            var requestContent = new MultipartFormDataContent();

            // הורדת הקובץ מ-S3
            var fileContent = await client.GetByteArrayAsync(fileUrl);
            var byteArrayContent = new ByteArrayContent(fileContent);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            requestContent.Add(byteArrayContent, "file", "file.pdf"); // שם הקובץ בהעלאה

            requestContent.Add(new StringContent("assistants"), "purpose");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await client.PostAsync("https://api.openai.com/v1/files", requestContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var fileId = JsonSerializer.Deserialize<JsonElement>(responseString).GetProperty("id").GetString();

            return fileId;
        }

        public async Task<string> Run()
        {
            // 1. העלאת קובץ ל-OpenAI
            string fileUrl = "https://propertytax-documents.s3.us-east-1.amazonaws.com/uploads/user_95/fba9cc3f-b3bd-403d-b948-42e97e4044c5_tz.pdf?X-Amz-Expires=1800&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAWMFUO5W7ILAUC6XJ%2F20250504%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250504T000220Z&X-Amz-SignedHeaders=host&X-Amz-Signature=f1a8a9a632443993c45369a75f089d7a9219a14adaf5140c2f45d8f7966c3f42";
            //string fileUrl = await _IS3Service.GetDownloadUrlAsync("uploads/user_95/fba9cc3f-b3bd-403d-b948-42e97e4044c5_tz.pdf");
            string fileId = await UploadFileToOpenAiAsync(fileUrl);

            // 2. יצירת Thread עם הקובץ
            string threadId = await CreateThreadWithFileAsync(fileId);

            // 3. הרצת הבקשה ולקיחת התשובה
            string result = await RunAssistantOnThreadAsync(threadId, "asst_Q089GAXZC1HW2cFrbsUcNKx5");

            //Console.WriteLine(result);
            return result;
        }
    }
}
//public async Task<string> GetChatResponse(string imageUrl)
//{
//    var payload = new
//    {
//        // model = "gpt-3.5-turbo",  // או כל מודל שיש לך גישה אליו
//        model = "gpt-4o-mini",  // או כל מודל שיש לך גישה אליו
//                                //     messages = new[]
//                                //     {new { role = "user", content = userMessage }}
//                                //};
//        messages = new object[]
//{
//    new
//    {
//        role = "user",
//        content = new object[]
//        {
//            new { type = "text", text = "מה אתה רואה בתמונה?" },
//            new
//            {
//                type = "image_url",
//                image_url = new { url = imageUrl }
//            }
//        }
//    }
//},
//        max_tokens = 1000
//    };
//    var json = JsonSerializer.Serialize(payload);
//    var resp = await _httpClient.PostAsync("chat/completions",
//        new StringContent(json, Encoding.UTF8, "application/json"));

//    resp.EnsureSuccessStatusCode();
//    using var stream = await resp.Content.ReadAsStreamAsync();
//    using var doc = await JsonDocument.ParseAsync(stream);

//    // נניח שה־response JSON הוא במבנה { choices: [ { message: { content: "..." } } ] }
//    var message = doc.RootElement
//                     .GetProperty("choices")[0]
//                     .GetProperty("message")
//                     .GetProperty("content")
//                     .GetString();
//    return message ?? "";



//    //var requestBody = new
//    //{
//    //    model = "gpt-3.5-turbo",
//    //    messages = new[]
//    //    {
//    //        new { role = "system", content = "אתה עוזר AI." },
//    //        new { role = "user", content = userMessage }
//    //    },
//    //    max_tokens = 100
//    //};

//    //var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
//    //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
//    //HttpResponseMessage response = await _httpClient.PostAsync(ApiUrl, requestContent);
//    ////string responseBody = await response.Content.ReadAsStringAsync();
//    ////using JsonDocument doc = JsonDocument.Parse(responseBody);
//    ////return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
//    //return await response.Content.ReadAsStringAsync();
//}
//public async Task<bool> IsPdfUrlAsync(string url)
//{
//    using var request = new HttpRequestMessage(HttpMethod.Head, url);
//    using var response = await _httpClient.SendAsync(request);

//    if (!response.IsSuccessStatusCode)
//        return false;

//    if (response.Content.Headers.ContentType?.MediaType == "application/pdf")
//        return true;

//    return false;
//}
//string imageBase64;
//bool isPdf = await IsPdfUrlAsync(fileUrl);
//if (isPdf)
//{
//    // בדיקת האם הקובץ הוא PDF
//    //if (fileUrl.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
//  // הורדת ה-PDF
//    var pdfBytes = await _httpClient.GetByteArrayAsync(fileUrl);
//    var tempPdfPath = Path.GetTempFileName() + ".pdf";
//    await System.IO.File.WriteAllBytesAsync(tempPdfPath, pdfBytes);

//    // המרת עמוד ראשון בתור תמונה
//    using var document = PdfiumViewer.PdfDocument.Load(tempPdfPath);
//    using var image = document.Render(0, 300, 300, true);

//    using var ms = new MemoryStream();
//    image.Save(ms, ImageFormat.Jpeg);
//    imageBase64 = Convert.ToBase64String(ms.ToArray());

//    System.IO.File.Delete(tempPdfPath);
//}
//else
//{
//    // תמונה רגילה – הורדה והמרה ל-base64
//    var imageBytes = await _httpClient.GetByteArrayAsync(fileUrl);
//    imageBase64 = Convert.ToBase64String(imageBytes);
//}

// הכנה לשליחה ל-OpenAI עם base64
