namespace Contacto.Server.Models.DTOs
{
    public class ContactUpdateDto
    {
        public required string Id { get; set; }
        public string? Name { get; set; }
        public string? MobilePhone { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? BirthDate { get; set; }
    }

}
