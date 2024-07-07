using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NlpBackend.Models;

namespace NlpBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ChatController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            _logger.LogInformation("Received request: {Message}", request.Message);

            var client = _httpClientFactory.CreateClient();

            try
            {
                // Prepare request body
                var requestBody = new
                {
                    text = request.Message
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                // Sentiment Analysis
                var apiToken = _configuration["HuggingFaceApi:Token"];
                var sentimentModelUrl = _configuration["HuggingFaceApi:SentimentModelUrl"];
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

                _logger.LogInformation("Calling sentiment analysis API at {SentimentModelUrl}", sentimentModelUrl);
                var sentimentResponse = await client.PostAsync(sentimentModelUrl, content);
                if (!sentimentResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error calling Hugging Face API for sentiment analysis: {StatusCode}", sentimentResponse.StatusCode);
                    return StatusCode((int)sentimentResponse.StatusCode, "Error calling Hugging Face API for sentiment analysis");
                }

                var sentimentResponseString = await sentimentResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Sentiment response: {Response}", sentimentResponseString);
                var sentimentArray = JArray.Parse(sentimentResponseString);
                var sentimentLabel = sentimentArray[0]["label"]?.ToString() ?? "Unknown";

                // Intent Recognition
                var intentModelUrl = _configuration["HuggingFaceApi:IntentModelUrl"];
                _logger.LogInformation("Calling intent recognition API at {IntentModelUrl}", intentModelUrl);
                var intentResponse = await client.PostAsync(intentModelUrl, content);
                if (!intentResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error calling Hugging Face API for intent recognition: {StatusCode}, Response: {ResponseContent}", intentResponse.StatusCode, await intentResponse.Content.ReadAsStringAsync());
                    return StatusCode((int)intentResponse.StatusCode, "Error calling Hugging Face API for intent recognition");
                }

                var intentResponseString = await intentResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Intent response: {Response}", intentResponseString);
                var intentArray = JArray.Parse(intentResponseString);
                var intentLabel = intentArray[0]["label"]?.ToString() ?? "Unknown";

                // Generate response based on intent and sentiment
                string generatedResponse;
                if (intentLabel == "greeting")
                {
                    if (sentimentLabel == "POSITIVE")
                    {
                        generatedResponse = "Hello! I'm glad to hear that you're doing well. How can I assist you today?";
                    }
                    else
                    {
                        generatedResponse = "Hello! I'm sorry to hear that. Is there anything I can help you with?";
                    }
                }
                else if (intentLabel == "farewell")
                {
                    generatedResponse = "Goodbye! Have a great day!";
                }
                else
                {
                    generatedResponse = "I'm here to help! What do you need assistance with?";
                }

                var chatResponse = new ChatResponse
                {
                    Response = generatedResponse,
                    Sentiment = sentimentLabel
                };

                _logger.LogInformation("Generated response: {Response}", chatResponse.Response);

                return Ok(chatResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
