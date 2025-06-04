using i4optioncore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories.OptionWindow
{
    public interface IOptionWindowBL
    {
        #region Stocks
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksActive();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksFarActivity();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksOH();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksOL();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksOIGainer();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksOILooser();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksBuyers();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksWriters();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetStocksItmUnwinding();
        #endregion
        #region Index
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexActive();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexFarActivity();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexOH();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexOL();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexOIGainer();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexOILooser();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexBuyers();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexWriters();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetIndexItmUnwinding();
        #endregion
    }
}
