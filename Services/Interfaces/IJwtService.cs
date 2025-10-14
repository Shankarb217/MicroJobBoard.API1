using MicroJobBoard.API.Models;

namespace MicroJobBoard.API.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
