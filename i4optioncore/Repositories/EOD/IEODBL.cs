using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories.EOD
{
    public interface IEODBL
    {
        Task DeleteEODScan(int id);
        List<IEODScreenerData> EODScreener(List<IEODScreenerDataRequest> requests);
        Task<List<Eodscan>> GetEODScan(int userId);
        Task SaveEODScan(EODScanRequest request);
    }
}
