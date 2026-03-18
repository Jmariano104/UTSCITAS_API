using Microsoft.EntityFrameworkCore;
using UTSCITAS_API.Models;

namespace UTSCITAS_API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Profesional> Profesionales { get; set; }
        public DbSet<Cita> Citas { get; set; }
    }
}