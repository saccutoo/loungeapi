using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucVoucherTypeHandler
    {
        Task<Response> GetListVoucherType(string query);
    }
}
