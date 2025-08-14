using Microsoft.EntityFrameworkCore;
using AssinaturaDigital.Models; // <-- necessário para encontrar as models


namespace AssinaturaDigital.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Adicione seus DbSets aqui
        public DbSet<Attest> Attests { get; set; }
        public DbSet<Signature> Signatures { get; set; }
    }
}
