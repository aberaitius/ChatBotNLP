using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NlpBackend.Models;

[ApiController]
[Route("[controller]")]
public class NlpController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public NlpController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] NlpRequest request)
    {
        var client = _httpClientFactory.CreateClient();

        var requestBody = new
        {
            inputs = request.Message
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var apiToken = _configuration["HuggingFaceApi:Token"];
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

        var response = await client.PostAsync("https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english", content);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Error calling Hugging Face API");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);

        var sentiment = responseObject[0].label.ToString();

        var nlpResponse = new NlpResponse
        {
            Response = sentiment
        };

        return Ok(nlpResponse);
    }
}
