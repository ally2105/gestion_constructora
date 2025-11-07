using Microsoft.AspNetCore.Identity;

namespace gestion_construccion.web.Models
{
    // Heredamos de IdentityRole usando int como tipo para la clave primaria
    public class Rol : IdentityRole<int>
    {
        public Rol() : base() { }
        public Rol(string roleName) : base(roleName) { }
    }
}
