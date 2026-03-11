using UTSCITAS_API.Models;
namespace UTSCITAS_API.Services.Interfaces
{
    public interface IProfesionalService
    {
        Task<List<Profesional>> ObtenerProfesionales();
    }
}