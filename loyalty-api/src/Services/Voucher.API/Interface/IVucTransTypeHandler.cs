using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucTransTypeHandler
    {
        Task<Response> GetListTransType(string query);
    }
}
