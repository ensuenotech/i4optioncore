using i4optioncore.Models;
using KiteConnect;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public interface IKiteBL
    {
        decimal CalculateMargin(List<CalculateMarginRequest> request);
        List<Historical> GetHistory(string symbol, DateTime FromDate, DateTime ToDate, string Interval);
        Task UpdateRequestToken(string requestToken);
    }
}
