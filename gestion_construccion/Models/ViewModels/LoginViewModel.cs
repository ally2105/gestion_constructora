using System.ComponentModel.DataAnnotations;

namespace gestion_construccion.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Recordarme?")]
        public bool RememberMe { get; set; }
    }
}
