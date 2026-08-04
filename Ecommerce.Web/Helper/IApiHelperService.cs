namespace Ecommerce.Web.Helper;

public interface IApiHelperService
{
    Task<T> GetAsync<T>(string endpoint);
    Task<TResponse> PostAsync<TResponse, TRequest>(string endpoint, TRequest payload);
    Task<TResponse> PutAsync<TResponse, TRequest>(string endpoint, TRequest payload);
    Task<bool> DeleteAsync(string endpoint);
    Task<TResponse> PutMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent payload);
}