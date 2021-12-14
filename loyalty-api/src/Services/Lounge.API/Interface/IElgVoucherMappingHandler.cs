using System.Threading.Tasks;
using Utils;
using API.Models;
using Lounge.API.Models;

namespace API.Interface
{
    public interface IElgVoucherMappingHandler
    {
        #region GET DATA
        Task<Response> GetByFilterAsync(ElgVoucherMappingQueryModel model);
        Task<Response> GetCustByCif(string cif);
        Task<Response> GetApprovedUploadIds(string templateIds, int pageSize, int pageIndex);

        Task<Response> GetFileUploads(string pos);
        #endregion GET DATA

        #region CRUD DATA

        Task<Response> MappingVoucherCustomer(VucMappingVoucherCustModel model, EVoucherBaseModel baseModel);
        Task<Response> GetMapVoucherListByFilter(VucMapVoucherCustQueryModel model);

        Task<Response> Approved(string listIds, string userName, string pos, decimal uploadId);

        Task<Response> CreateAsync(VucMappingManualVoucherCustModel createModel, EVoucherBaseModel baseModel);
        #endregion CRUD DATA
    }
}
