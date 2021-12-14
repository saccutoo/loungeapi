using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucMapVoucherCust
    {
        Task<Response> ValidateVoucherCustomer(decimal voucherId, string transType, decimal numOfVoucherTarget);
        Task<Response> GetMapVoucherListByFilter(VucMapVoucherCustQueryModel model);
        Task<Response> CancelVoucherMapping(decimal mapId, EVoucherBaseModel baseModel);
        Task<Response> MapVoucherCustomerEbank(VucMapVoucherCustMappingModel model, EVoucherBaseModel baseModel);
    }
}
