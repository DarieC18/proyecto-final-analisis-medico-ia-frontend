using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MedAnalyzer.Infraestructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public required string LastName { get; set; }

        [Required]
        [StringLength(10)]
        public required string NumberIdentification { get; set; } //cedula

        public bool Status { get; set; } // true = Activo, false = Inactivo

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        
    }
}
