using System.Threading.Tasks;
using Ecommerce.Core.Entities.Identity;

namespace Ecommerce.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}