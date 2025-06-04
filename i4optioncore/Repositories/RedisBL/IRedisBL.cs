using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public interface IRedisBL
    {
        Task<string> GetValue(string Key);
        Task SetValue(string Key, string Value);
        Task SetValueWithExpiry(string Key, string Value, double? expiryMinutes);
        Task<bool> DeleteKey(string Key);
        Task<bool> FlushAll();
    }
}
