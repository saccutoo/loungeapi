using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.Interface
{
    public interface IElgNotificationHandler
    {
        Task<Response> GetAllByTypeAsync(string type);
        Task<Response> GetByIdAsync(decimal Id);
        Task<Response> GetByFaceIdAsync(string faceId);
        Task<Response> GetByFilterAsync(ElgNotificationQueryModel query);
        Task<Response> CreateAsync(ElgNotificationCreateModel model);
     }
}
