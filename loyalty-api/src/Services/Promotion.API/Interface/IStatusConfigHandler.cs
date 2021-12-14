using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IStatusConfigHandler
    {
        Task<Response> GetAllActive();
    }
}
