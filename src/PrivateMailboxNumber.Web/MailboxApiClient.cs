using System.Net.Http.Json;

namespace PrivateMailboxNumber.Web;

public class MailboxApiClient
{
    private readonly HttpClient _httpClient;

    public MailboxApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<int>> GetMailboxNumberAsync(string locationNumber, string accountId)
    {
        var url = $"?locationNumber={Uri.EscapeDataString(locationNumber)}&accountId={Uri.EscapeDataString(accountId)}";
        var response = await _httpClient.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<MailboxResponse>();
            return new ApiResponse<int>
            {
                Value = result?.MailboxNumber ?? 0,
                RequestUrl = url,
                StatusCode = response.StatusCode,
                ResponseBody = responseBody
            };
        }

        throw new HttpRequestException($"Error: {response.StatusCode} - {responseBody}");
    }

    public async Task<ApiResponse<string>> GetAccountIdAsync(string locationNumber, int mailboxNumber)
    {
        var url = $"?locationNumber={Uri.EscapeDataString(locationNumber)}&mailboxNumber={mailboxNumber}";
        var response = await _httpClient.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AccountResponse>();
            return new ApiResponse<string>
            {
                Value = result?.AccountId ?? string.Empty,
                RequestUrl = url,
                StatusCode = response.StatusCode,
                ResponseBody = responseBody
            };
        }

        throw new HttpRequestException($"Error: {response.StatusCode} - {responseBody}");
    }

    private record MailboxResponse(int MailboxNumber);
    private record AccountResponse(string AccountId);
}

public class ApiResponse<T>
{
    public T Value { get; set; } = default!;
    public string RequestUrl { get; set; } = string.Empty;
    public System.Net.HttpStatusCode StatusCode { get; set; }
    public string ResponseBody { get; set; } = string.Empty;
} 