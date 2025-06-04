using i4optioncore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories.UpdateRedis
{
    public interface IUpdateBL
    {
        Task UpdateTouchlineRedis(List<string> symbols);
        Task UpdateFullTouchlineRedis();
        Task UpdateSymbolList();
        Task<bool> UpdateIV(List<CommonModel.IVRequest> request);
        bool UpdateMarketHoliday(bool IsMarketHoliday, int Days);
        Task UpdateSnapshotRedis();
        Task UpdateSegmentTouchlineRedis(List<DateTime> dates, string segment);
        Task UpdateSymbolTouchlineRedis(List<DateTime> dates, string symbol);
        Task<bool> UpdateIVPIVR(DateTime date);
        Task UpdateMaxPain();
        Task UpdateRedisFOREODDATA();
        Task UpdateVolumeCommentary();
        Task UpdateBreadth();
        Task UpdateSpotVolumeCommentary();
        void CopyMasterSync();
        Task UpdateFutureRollover();
        Task UpdateEOD(DateTime date);
        Task UpdateEODSegment(DateTime date);
        Task UpdatePCR();
        Task UpdateSegmentTouchline(DateTime date);
        Task UpdateTouchline(DateTime date);
        Task UpdateVolatility(DateTime date);
    }
}
