using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace AssinaturaDigital.Models {
    [Table("attest")]
    public class Attest {
        public int Id { get; set; }

        [Required] // valida presença
        public string System { get; set; } = string.Empty;

        [Required]
        public string Entity { get; set; } = string.Empty;

        [Required]
        public string Operation { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("type_signature")]
        public int TypeSignature { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = GenerateToken();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Coleção de signatures recebidas no JSON
        public List<Signature> Signatures { get; set; } = new List<Signature>();

        private static string GenerateToken() {
            var bytes = new byte[10];
            RandomNumberGenerator.Fill(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

}
