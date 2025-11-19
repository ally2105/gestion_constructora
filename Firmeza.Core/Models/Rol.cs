using Microsoft.AspNetCore.Identity;

namespace Firmeza.Core.Models
{
    public class Rol : IdentityRole<int>
    {
        public Rol() : base() { }
        public Rol(string roleName) : base(roleName) { }
    }
}
