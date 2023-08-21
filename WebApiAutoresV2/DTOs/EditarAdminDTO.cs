using System.ComponentModel.DataAnnotations;

namespace WebApiAutoresV2.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
