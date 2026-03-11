
namespace UTSCITAS_API.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<bool> EsDiaFestivo(DateTime fecha);
    }
}
