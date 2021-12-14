using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IPrmProductHandler
    {
        Task<Response> GetByFilterAsync(PrmProductQueryModel queryModel);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> CreateAsync(PrmProductCreateModel model);
        Task<Response> UpdateAsync(decimal id,PrmProductUpdateModel model);
        Task<Response> DeleteByIdAsync(decimal id);
    }
}
