using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgCustomerTypeHandler
    {
        Task<Response> GetAllByConditionAsync(string condition);
    }
}
