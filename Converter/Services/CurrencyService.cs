using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Json;

namespace Converter.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        public CurrencyService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, decimal>> GetRatesAsync(string baseCurrency = "USD")
        {
            var url = $"https://open.er-api.com/v6//latest/{baseCurrency}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse>(url);
            return response?.Rates ?? new Dictionary<string, decimal>();
        }

        private class ApiResponse
        {
            public Dictionary<string, decimal>? Rates { get; set; }
        }
    }
}
