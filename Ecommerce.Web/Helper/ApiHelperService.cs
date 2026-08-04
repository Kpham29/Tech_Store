using System.Text;
using System.Text.Json;

namespace Ecommerce.Web.Helper;

public class ApiHelperService : IApiHelperService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiHelperService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _client = clientFactory.CreateClient("ApiClient");
        _jsonOptions = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
        _httpContextAccessor = httpContextAccessor;
    }
    
    private void AttachToken()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwtToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        AttachToken();
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
        AttachToken();
        var json = JsonSerializer.Serialize(payload);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(endpoint, httpContent);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception(errorContent);
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

    public async Task<TResponse> PutMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent payload)
    {
        AttachToken();
        var response = await _client.PutAsync(endpoint, payload);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception(errorContent);
    }
}