using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgCustomerClassHandler
    {
        Task<Response> CreateAsync(ElgCustomerClassCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ElgCustomerClassCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> GetByFilterAsync(ElgCustomerClassQueryModel model);
        Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel);
        Task<Response> GetAllByConditionAsync(string condition);
    }
}
