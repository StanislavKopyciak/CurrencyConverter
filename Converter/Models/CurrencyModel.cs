
namespace Converter.Models
{
    public class CurrencyModel
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public decimal Rate { get; set; }
        public DateOnly Date {  get; set; }
    }
}
