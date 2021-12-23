using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.Interface
{
    public interface IElgCustomerHandler
    {
        Task<Response> CreateAsync(ElgCustomerCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ElgCustomerCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> ResolveConflictAsync(decimal id, ElgCustomerResolveConflictModel model, ELoungeBaseModel baseModel);
        Task<Response> GetByFilterAsync(ElgCustomerQueryModel model);
        Task<Response> FilterByUploadIds(decimal uploadid1, decimal uploadid2);
        Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel);
        Task<Response> UpdateUploadIDStatusAsync(decimal id, string status, ELoungeBaseModel baseModel);
        Task<Response> UpdateMultiUploadIDStatusAsync(decimal old_uploadid, decimal new_uploadid, string status, ELoungeBaseModel baseModel);
        Task<Response> GetAllByConditionAsync(string condition);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> GetApprovedUploadIds();
        Task<Response> GetPendingUploadIds();
        Task<Response> KYCCustomerAsync(string query);
        Task<Response> ExportCustomerAsync(ElgCustomerExportModel model);
        Task<Response> KYCCustomerV2Async(string query, decimal elgCustId);

        //Vinhtq1
        Task<Response> UpdateDetailCustomer(ElgCustomerBaseModel model);
        Task<Response> GetElgCustomerById(decimal elgCustId);
        Task<Response> GetByCustIdAsync(string elgCustId);
        Task<Response> GetListCustomerV3Async(string textSearch, string fullName, string phoneNum, string cusname, string representUserName, string email, string status);
        Task<Response> GetDistinctByCustIdAsync(string custId);
    }
}
