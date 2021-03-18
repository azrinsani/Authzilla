using Microsoft.AspNetCore.Identity;

namespace Authzilla
{
    public class AppUser : IdentityUser { public string Server { get; set; }}
}
