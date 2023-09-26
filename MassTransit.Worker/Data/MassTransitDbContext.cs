using MassTransit.Worker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Worker.Data
{
    public class MassTransitDbContext : DbContext
    {
        public DbSet<Nfe> NFes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Notas.db");
        }
    }
}
