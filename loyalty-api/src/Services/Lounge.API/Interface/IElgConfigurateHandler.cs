using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgConfigurateHandler
    {
        Task<Response> CreateAsync(ElgConfigurateCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ElgConfigurateCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> GetByFilterAsync(ElgConfigurateQueryModel model);
    }
}
