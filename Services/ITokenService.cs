using Joker_app_backend.Models;

namespace Joker_app_backend.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}