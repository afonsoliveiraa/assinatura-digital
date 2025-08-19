using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssinaturaDigital.Models
{
    [Table("document")] // nome exato da tabela
    public class Document
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}