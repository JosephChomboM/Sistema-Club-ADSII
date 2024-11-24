// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Club.Models;
using System.Collections.Generic;

namespace Club.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Espacio> Espacios { get; set; }
        public DbSet<Reservacion> Reservaciones { get; set; }
        public DbSet<Pago> Pagos { get; set; }
    }
}
