using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucVoucherAmtConditionsHandler
    {
        Task<Response> CreateAsync(VucVoucherAmtConditionsCreateModel model, EVoucherBaseModel baseModel);
        Task<Response> DeleteAsync(decimal voucherId);
        Task<Response> GetVoucherConditionsAll(decimal voucherId);
    }
}
