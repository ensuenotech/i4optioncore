
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public interface ICacheBL
    {
        void Delete(string Key);
        string GetValue(string Key);
        bool SetValue(string Key, string Value, double? expiryMinutes = 60 * 24);
    }
}
