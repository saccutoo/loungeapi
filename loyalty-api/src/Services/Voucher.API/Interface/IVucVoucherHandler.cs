using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucVoucherHandler
    {
        Task<Response> CreateAsync(VucVoucherCreateUpdateModel model, EVoucherBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, VucVoucherCreateUpdateModel model, EVoucherBaseModel baseModel);
        Task<Response> GetListVoucherByFilter(VucVoucherQueryModel model);
        Task<Response> GetListVoucherAll(string query);
        Task<Response> GetVoucherByID(decimal voucherId);
        Task<Response> ApproveVoucher(decimal voucherId, EVoucherBaseModel baseModel);
        Task<Response> RejectVoucher(decimal voucherId, EVoucherBaseModel baseModel);
        Task<Response> CancelVoucher(decimal voucherId, EVoucherBaseModel baseModel);
        Task<Response> GetListVoucherForMapping(decimal channelId, decimal issueBatchId, decimal voucherTypeId);
    }
}
