using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Converter.Models;
using Converter.Services;


namespace Converter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly CurrencyService _service;

        [ObservableProperty]
        private string fromCurrency = "USD";

        [ObservableProperty]
        private string toCurrency = "EUR";

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private decimal convertedAmount;

        [ObservableProperty]
        private List<string> currencyCodes = new();

        private Dictionary<string, decimal> rates = new();

        public MainViewModel()
        {
            _service = new CurrencyService();
            _ = LoadRatesAsync();
        }


        private async Task LoadRatesAsync()
        {
            rates = await _service.GetRatesAsync();
            CurrencyCodes = rates.Keys.ToList();
        }

        [RelayCommand]
        private void Convert()
        {
            if (rates.ContainsKey(FromCurrency) && rates.ContainsKey(ToCurrency))
            {
                decimal fromRate = rates[FromCurrency];
                decimal toRate = rates[ToCurrency];

                ConvertedAmount = Amount / fromRate * toRate;
            }
        }
    }
}



