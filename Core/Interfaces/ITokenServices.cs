using Core.Entities.Identity;

namespace Core.Interfaces
{
    public interface ITokenServices
    {
        string CreateToken(AppUser user);
    }
}
