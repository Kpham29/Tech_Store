using System.Text;
using System.Text.Json;

namespace Ecommerce.Web.Helper;

public class ApiHelperService : IApiHelperService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiHelperService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("ApiClient");
        _jsonOptions = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        return default;
    }

    public async Task<TResponse> PostAsync<TResponse, TRequest>(string endpoint, TRequest payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(endpoint, httpContent);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }
        return default;
    }

    public async Task<TResponse> PutAsync<TResponse, TRequest>(string endpoint, TRequest payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync(endpoint, httpContent);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }
        return default;
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _client.DeleteAsync(endpoint);
        return response.IsSuccessStatusCode;
    }
}