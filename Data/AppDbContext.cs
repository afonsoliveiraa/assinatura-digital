using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // necessário para IdentityDbContext
using Microsoft.EntityFrameworkCore;
using AssinaturaDigital.Models; // para encontrar ApplicationUser, Attest e Signature

namespace AssinaturaDigital.Data
{
    // Herdando de IdentityDbContext<ApplicationUser> para suportar Identity
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Suas tabelas existentes
        public DbSet<Attest> Attests { get; set; }
        public DbSet<Signature> Signatures { get; set; }
    }
}
