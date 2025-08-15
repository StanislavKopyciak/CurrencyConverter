using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Converter.Models;
using Converter.Services;
using System.Collections.ObjectModel;

namespace Converter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly CurrencyService _service;
        private readonly DataService _dataService;

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

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<CurrencyModel> currenciesForDate = new();

        private Dictionary<string, decimal> rates = new();

        public MainViewModel()
        {
            _service = new CurrencyService();
            _dataService = new DataService();
            _ = LoadRatesAsync();
            _ = AutoInsertUAHRateAsync();
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

        [RelayCommand]
        private async Task LoadCurrenciesByDateAsync()
        {
            var list = await _dataService.GetRateListAsync(DateOnly.FromDateTime(SelectedDate));
            CurrenciesForDate = new ObservableCollection<CurrencyModel>(list);
        }

        private async Task AutoInsertUAHRateAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            bool exists = await _dataService.DateExistsAsync(today, "UAH");
            if (exists)
                return;

            var rates = await _service.GetRatesAsync();

            if (rates.TryGetValue("UAH", out decimal rate))
            {
                var gr = new CurrencyModel
                {
                    Code = "UAH",
                    Name = "Українська гривня",
                    Rate = rate,
                    Date = today
                };

                await _dataService.AddRateListAsync(gr);
            }
        }
    }
}

