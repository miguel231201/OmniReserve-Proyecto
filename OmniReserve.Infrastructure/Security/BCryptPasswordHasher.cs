using BCrypt.Net;
using OmniReserve.Application.Common.Interfaces;

namespace OmniReserve.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        // BCrypt maneja el salting internamente
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}