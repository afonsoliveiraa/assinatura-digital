using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AssinaturaDigital.Models // Adicione este namespace
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 números.")]
        public string Cpf { get; set; } = string.Empty;
    }
}