using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiGatewayService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiGatewayBaseUrl;

    public ApiGatewayService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiGatewayBaseUrl = "http://localhost:5111";
    }

    public async Task<string> CallApiGatewayAsync(string endpoint, object payload)
    {
        var uri = new Uri(new Uri(_apiGatewayBaseUrl), endpoint);

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(uri, content);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw new HttpRequestException($"Error calling API Gateway: {response.StatusCode}");
    }
}
