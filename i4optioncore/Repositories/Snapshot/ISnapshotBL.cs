using i4optioncore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories.Snapshot
{
    public interface ISnapshotBL
    {
        Task<List<CommonModel.HistoryRecord>> GetIntradaySnapshot(string displayName, DateTime time);
        Task<List<CommonModel.HistoryRecord>> GetIntradaySnapshots(List<string> symbols, DateTime time);
        Task<List<CommonModel.HistoryRecord>> GetOptionIntradaySnapshot(string displayName, DateTime time, DateTime expiry);
        Task<List<CommonModel.HistoryRecord>> GetHistorySnapshot(string displayName, DateTime time);
        Task<List<CommonModel.HistoryRecord>> GetOptionHistorySnapshot(string displayName, DateTime time, DateTime expiry);
    }
}
