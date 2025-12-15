using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.APIs.Extensions
{
    public static class UserManagerExtensions
    {
        // ÇáÏÇáÉ Ïí ÈÊÌíÈ ÇáíæÒÑ ÈÇáÚäæÇä ÈÊÇÚå Úä ØÑíŞ ÇáÜ Claims (ÇáÊæßä)
        public static async Task<AppUser> FindUserByClaimsPrincipleWithAddressAsync(this UserManager<AppUser> input, ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);

            return await input.Users
                .Include(x => x.Address) // Ïí ÇáÊÑíßÇíÉ (Include Address)
                .SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}