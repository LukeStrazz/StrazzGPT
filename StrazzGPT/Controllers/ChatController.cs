using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StrazzGPT.Models;

namespace StrazzGPT.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ChatController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Chat()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetChatResponse([FromBody] ChatRequest request)
        {
            var client = _clientFactory.CreateClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            requestMessage.Headers.Add("Authorization", $"Bearer {apiKey}");
            requestMessage.Content = new StringContent(
        JsonConvert.SerializeObject(new
        {
            model = "gpt-3.5-turbo",
            messages = new[] { new { role = "user", content = request.Prompt } }
        }),
            System.Text.Encoding.UTF8,
            "application/json"
        );

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                var reply = jsonResponse["choices"][0]["message"]["content"].ToString();
                return Json(new { message = reply });  // Wrap the string in a JSON object
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, "Error from API: " + errorContent);
            }
        }


    }
}
