using UTSCITAS_API.Models;

namespace UTSCITAS_API.Services.Interfaces

{
    public interface IUsuarioService
    {
        Task CrearUsuario(Usuario usuario);
    }
}