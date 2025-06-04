using i4optioncore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories.Dhan
{
    public interface IDhanBL
    {
        Task DownloadDhanSymbols();
        Task<string> GenerateDhanConsent();
        Task<List<DhanModel.OrderDetails>> GetOrders(int userId);
        Task<List<DhanModel.PositionDetails>> GetPositions(int userId);
        Task PlaceOrder(DhanModel.DhanOrderRequest request);
        Task UpdateDhanCredentials(IDhanCredentialsRequest request);
        Task UpdateDhanOrder(DhanModel.OrderDetails request);
        Task UpdateDhanToken(string token);
    }
}
