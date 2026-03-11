using System.Text.Json;
using UTSCITAS_API.Services.Interfaces;

namespace UTSCitas_API.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public HolidayService(IConfiguration config)
        {
            _config = config;
            _client = new HttpClient();
        }

        public async Task<bool> EsDiaFestivo(DateTime fecha)
        {
            var apiKey = _config["Calendarific:ApiKey"];
            var baseUrl = _config["Calendarific:BaseUrl"];

            var url = $"{baseUrl}/holidays?api_key={apiKey}&country=MX&year={fecha.Year}";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(json);

            var holidays = doc.RootElement
                .GetProperty("response")
                .GetProperty("holidays");

            foreach (var holiday in holidays.EnumerateArray())
            {
                var date = holiday.GetProperty("date")
                    .GetProperty("iso")
                    .GetString();

                if (DateTime.Parse(date).Date == fecha.Date)
                    return true;
            }

            return false;
        }
    }
}