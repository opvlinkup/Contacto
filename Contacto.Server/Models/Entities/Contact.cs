using System.ComponentModel.DataAnnotations;

namespace Contacto.Server.Models.Entities
{
    public class Contact
    {
        [Key] public Guid Id { get; init; } = Guid.NewGuid();
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string MobilePhone { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string JobTitle { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
