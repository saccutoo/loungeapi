using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgBookingStatusHandler
    {
        Task<Response> GetAllByConditionAsync(string condition);
    }
}
