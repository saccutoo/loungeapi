using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IPrmProductInstanceHandler
    {
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> GetByPromotionIdAsync(decimal promoId);
        Task<Response> CreateAsync(PrmProductInstanceCreateModel model);
        Task<Response> UpdateAsync(decimal id, PrmProductInstanceUpdateModel model);
        Task<Response> DeleteByIdAsync(decimal id);
    }
}
