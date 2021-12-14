using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgStarffHandler
    {
        Task<Response> CreateAsync(ElgStarffCreateModel model, ELoungeBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ElgStarffUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> GetByFilterAsync(ElgStarffQueryModel model);
        Task<Response> GetByAllListAsync(string condition);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> GetByUserNameAsync(string userName);
        Task<Response> ChangePassword(decimal id, string oldPass, string newPass, bool force = false);
        Task<Response> LoginJWT(ElgStarfLoginModel loginModel);
        Task<Response> RefreshToken(string token, string refreshToken);
        Task<Response> Logout(decimal id);
        Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel);
    }
}
