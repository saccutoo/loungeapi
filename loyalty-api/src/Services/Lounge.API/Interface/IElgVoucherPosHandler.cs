using System.Threading.Tasks;
using Utils;
using API.Models;
using Lounge.API.Models;

namespace API.Interface
{
    public interface IElgVoucherPosHandler
    {
        #region GET DATA
        Task<Response> GetByFilterAsync(ElgVoucherPosQueryModel model);
        #endregion GET DATA

        #region CRUD DATA
        
        Task<Response> UpdateStatusAsync(ElgVoucherPosChangeStausModel stausModel);
        Task<Response> CreateAsync(ElgVoucherPosCreateModel createModel);
        #endregion CRUD DATA
    }
}
