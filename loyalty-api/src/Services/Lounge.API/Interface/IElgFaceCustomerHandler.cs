using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.Interface
{
    public interface IElgFaceCustomerHandler
    {
        Task<Response> GetAllByTypeAsync(string type);
        Task<Response> GetByIdAsync(decimal Id);
        Task<Response> GetByFaceIdAsync(string faceId);
        Task<Response> GetByCustIdAsync(string custId);
        Task<Response> CreateAsync(ElgFaceCustomerCreateModel model);
     }
}
