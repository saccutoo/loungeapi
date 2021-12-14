using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IEbankVoucherHandler
    {
        Task<Response> GetVoucherList(string customerId, string channelId, string tranType, decimal tranAmount);
        Task<Response> UpdateElgVoucherSync(EbankVoucherUpdateModel model);
        Task<Response> CheckElgVoucherSync(EbankVoucherCheckModel model);
    }
}
