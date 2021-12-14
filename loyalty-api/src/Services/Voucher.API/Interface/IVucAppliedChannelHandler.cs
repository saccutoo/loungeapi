using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucAppliedChannelHandler
    {
        Task<Response> GetListVoucherChannel(string query);
    }
}
