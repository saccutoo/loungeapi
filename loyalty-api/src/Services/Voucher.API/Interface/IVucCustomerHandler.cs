using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucCustomerHandler
    {
        Task<Response> GetCustomerByFilter(VucCustomerQueryModel model);       
    }
}
