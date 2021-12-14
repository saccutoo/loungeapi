using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgCustClassConditionHandler
    {
        Task<Response> CreateAsync(ElgCustClassConditionCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ElgCustClassConditionCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> GetByFilterAsync(ElgCustClassConditionQueryModel model);
        Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel);
        Task<Response> GetAllByConditionAsync(string condition);
    }
}
