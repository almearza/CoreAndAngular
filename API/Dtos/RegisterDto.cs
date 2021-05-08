using System;
using System.ComponentModel.DataAnnotations;
namespace API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4)]
        public string Password { get; set; }
         [Required]
         public string KnownUs { get; set; }
         [Required]
        public DateTime BirthDate { get; set; }
         [Required]
        public string Gender { get; set; }
         [Required]
        public string City { get; set; }
         [Required]
        public string Country { get; set; }
    }
}