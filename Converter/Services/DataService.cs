using System.Data;
using Converter.Models;
using MySql.Data.MySqlClient;

namespace Converter.Services
{
    public class DataService
    {
        private readonly string connectionString;

        public DataService(string connectionString = "server=localhost;user=root;password=root;database=converter")
        {
            this.connectionString = connectionString;
        }

        public async Task<List<CurrencyModel>> GetRateListAsync(DateOnly dateOnly)
        {
            var result = new List<CurrencyModel>();
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT * FROM ratelist WHERE DATE(date) = @date", connection);
            command.Parameters.AddWithValue("@date", dateOnly.ToDateTime(TimeOnly.MinValue));

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new CurrencyModel
                {
                    Code = reader.GetString("code"),
                    Name = reader.GetString("name"),
                    Date = DateOnly.FromDateTime(reader.GetDateTime("date")),
                    Rate = reader.GetDecimal("rate")
                });
            }

            return result;
        }


        public async Task AddRateListAsync(CurrencyModel gr)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("INSERT INTO ratelist (code, name, rate, date) VALUES (@code, @name, @rate, @date)", connection);

            command.Parameters.AddWithValue("@code", gr.Code);
            command.Parameters.AddWithValue("@name", gr.Name);
            command.Parameters.AddWithValue("@rate", gr.Rate);
            command.Parameters.AddWithValue("date", gr.Date.ToDateTime(TimeOnly.MinValue));

            await command.ExecuteReaderAsync();
            await connection.CloseAsync();
        }

        public async Task<bool> DateExistsAsync(DateOnly date, string code)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT COUNT(*) FROM ratelist WHERE date = @date AND code = @code", connection);
            command.Parameters.AddWithValue("@date", date.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@code", code);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }
    }
}
