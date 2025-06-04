using i4optioncore.DBModelsMaster;
using i4optioncore.Models;
using System.Collections.Generic;

namespace i4optioncore.Repositories
{
    public interface IImportBL
    {
        void Import52WeekData(List<_52weekHighLow> request);
        void ImportEarningRatioData(ImportModel.EarningRatioRequest request);
    }
}