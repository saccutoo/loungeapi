using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgBookingCheckoutHandler
    {
        Task<Response> GetByFilterAsync(ElgBookingCheckoutQueryModel model);
        Task<Response> GetAllAsync(string query);
        Task<Response> CheckoutAsync(ElgCheckOutCifsModel cifs, ELoungeBaseModel baseModel);
    }
}
