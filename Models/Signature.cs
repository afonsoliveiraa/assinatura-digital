using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace AssinaturaDigital.Models
{
    [Table("signature")] // nome exato da tabela
    public class Signature
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(14)]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        // Relacionamento com Attest
        [Required]
        public int AttestId { get; set; }
        
        [ForeignKey("AttestId")]
        public Attest? Attest { get; set; }  // ← anulável, sem [Required]

        [Required]
        public string SignatureToken { get; set; } = GenerateToken();

        [Required]
        public string Status { get; set; } = "PENDENTE";

        // Campos de data
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SignedAt { get; set; }  // pode ser null se ainda não assinou

        // Gera token único
        private static string GenerateToken()
        {
            var bytes = new byte[10];
            RandomNumberGenerator.Fill(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}