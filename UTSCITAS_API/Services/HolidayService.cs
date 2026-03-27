using System.Text.Json;
using UTSCITAS_API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace UTSCitas_API.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private readonly ILogger<HolidayService> _logger;

        public HolidayService(IConfiguration config, HttpClient client, ILogger<HolidayService> logger)
        {
            _config = config;
            _client = client;
            _logger = logger;
        }

        public async Task<bool> EsDiaFestivo(DateTime fecha)
        {
            var apiKey = _config["Calendarific:ApiKey"] ?? Environment.GetEnvironmentVariable("CALENDARIFIC_APIKEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("Calendarific API key not configured");
                return false;
            }

            var url = $"/holidays?api_key={apiKey}&country=MX&year={fecha.Year}";

            try
            {
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Calendarific returned non-success status {Status}", response.StatusCode);
                    return false;
                }

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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando Calendarific");
            }

            return false;
        }
    }
}
