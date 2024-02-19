using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GithubHookTransformer.Services.HttpCallerService;

public class HttpCallerService : IHttpCallerService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpCallerService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task PostPayloadAsync(string url, string jsonPayload)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, content);

            // Ensure success status code.
            response.EnsureSuccessStatusCode();

            // Optionally, read and return the response body.
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response received successfully.");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
}
