using System.Text.Json;
using UTSCITAS_API.Models;

namespace UTSCITAS_API.Services;

public class CalendarificService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://calendarific.com/api/v2";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CalendarificService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Calendarific:ApiKey"]!;
    }

    public async Task<IEnumerable<Holiday>> ObtenerFeriadosAsync(string country, int year, int? month = null)
    {
        var url = $"{BaseUrl}/holidays?api_key={_apiKey}&country={country}&year={year}";
        if (month.HasValue)
            url += $"&month={month.Value}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CalendarificResponse>(json, JsonOptions);

        return result?.Response?.Holidays ?? Enumerable.Empty<Holiday>();
    }

    public async Task<IEnumerable<Holiday>> FeriadosPorMesAsync(string country, int year, int month)
        => await ObtenerFeriadosAsync(country, year, month);

    // Verifica si una fecha es feriado en el país dado
    public async Task<bool> EsFeriadoAsync(string country, DateTime fecha)
    {
        var feriados = await ObtenerFeriadosAsync(country, fecha.Year, fecha.Month);
        return feriados.Any(f =>
            f.Date?.Datetime?.Day == fecha.Day &&
            f.Date?.Datetime?.Month == fecha.Month &&
            f.Date?.Datetime?.Year == fecha.Year);
    }
}
