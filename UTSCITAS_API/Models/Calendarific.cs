namespace UTSCITAS_API.Models;

public class CalendarificResponse
{
    public CalendarificMeta? Meta { get; set; }
    public CalendarificData? Response { get; set; }
}

public class CalendarificMeta
{
    public int Code { get; set; }
}

public class CalendarificData
{
    public List<Holiday>? Holidays { get; set; }
}

public class Holiday
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public HolidayDate? Date { get; set; }
    public List<string>? Type { get; set; }
    public string? Locations { get; set; }
    public string? States { get; set; }
}

public class HolidayDate
{
    public string? Iso { get; set; }
    public HolidayDatetime? Datetime { get; set; }
}

public class HolidayDatetime
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}
