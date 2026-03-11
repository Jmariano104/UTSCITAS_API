using UTSCITAS_API.Models;

namespace UTSCITAS_API.Services.Interfaces
{
    public interface ICitaService
    {
        Task<bool> CrearCita(Cita cita);
    }
}
