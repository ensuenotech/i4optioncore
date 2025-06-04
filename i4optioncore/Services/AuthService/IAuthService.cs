using System.Threading.Tasks;

namespace i4optioncore.Services
{
    public interface IAuthService
    {
        int GetUserId(string jwtToken);
        bool IsTokenValid(string token);
        Task LogOut(int userId);
    }
}