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
    }
}
