using Microsoft.AspNetCore.Identity;

namespace IdentityManagerServerApi.Data
{
    public class ApplicationUser : IdentityUser     //menna me identity user kiyna eka extend krama menna me wage , apita e identity user ekath ekkala labena properties thiynwa hariyt gttoth username wage ewa , meka blgnna puluwan identity user kiyna eka udin ctrl+click kirimen
    {
        public string? Name { get; set; }
    }
}
