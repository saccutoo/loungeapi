using System.Threading.Tasks;
using Utils;
using API.Models;
using System;

namespace API.Interface
{
    public interface IPrmPromotionHandler
    {
        Task<Response> GetByFilterAsync(PrmPromotionQueryModel queryModel);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> GetListPromoValidByTransLog(decimal transLogId);
        Task<Response> GetListPromoValidByLegacyRefNo(string legacyRefNo, string license, string phone, string employeePosCd);
        Task<Response> GetListGiftValidByLegacyRefNo(string legacyRefNo, string license, string phone);
        Task<Response> CreateAsync(PrmPromotionCreateModel model);
        Task<Response> UpdateAsync(decimal id, PrmPromotionUpdateModel model);
        Task<Response> DeleteByIdAsync(decimal id);
        Task<Response> ApproveByIdAsync(decimal id, string approvedBy);
        Task<Response> RejectByIdAsync(decimal id, string approvedBy, string approvedComment);
        Task<Response> UpdateStatusAsync(decimal id, string status);
    }
}
