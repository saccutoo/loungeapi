using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucVoucherStatusHandler
    {
        Task<Response> GetListStatus(string query);
    }
}
