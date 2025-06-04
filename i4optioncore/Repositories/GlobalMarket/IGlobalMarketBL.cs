using i4optioncore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwelveDataSharp.Library.ResponseModels;

namespace i4optioncore.Repositories.GlobalMarket
{
    public interface IGlobalMarketBL
    {
        Task<List<GlobalMarketModel.ADR>> GetADRs();
        Task<List<GlobalMarketModel.Bond>> GetBonds();
        Task<List<GlobalMarketModel.Commodity>> GetCommodities();
        Task<List<GlobalMarketModel.Currency>> GetCurrencies();
        Task<List<MarketData>> GetQuote();
    }
}
