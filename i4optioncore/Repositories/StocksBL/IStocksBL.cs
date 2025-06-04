using i4optioncore.DBModelsMaster;
using i4optioncore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public interface IStocksBL
    {
        Task<List<Eod>> GetEods(DateTime date, List<string> Symbols);
        List<FOIG> GetFOIG(FOIGRequest request);
        Task<List<FutureRollover>> GetFutureRollOver();
        List<StocksModel.HistoricPerformance> GetHistoricPerformances(string Stock);
        Task<List<CommonModel.HistoryRecord>> GetHistory(List<string> symbols, DateTime time, string type = "daily");
        Task<List<IIntradayPCR>> GetIntradayPCR(string symbol, DateTime expiry);
        ICollection<OptionDashboard> GetOptionDashboards();
        Task<List<CommonModel.HistoryRecord>> GetOptionHistory(string stockName, DateTime date);
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetOptionTouchline(string stockName, DateTime date);
        Task<ICollection<IIntradayPCR>> GetPcrByInterval(GetPCRRequest request);
        List<IIntradayPCR> GetPcrByDate(IPCRByDateRequest request);
    }
}
