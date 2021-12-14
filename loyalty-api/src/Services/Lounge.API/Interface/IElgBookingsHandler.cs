using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgBookingsHandler
    {
        Task<Response> CreateAsync(ElgBookingsCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> UpdateAsync(decimal id, ElgBookingsCreateUpdateModel model, ELoungeBaseModel baseModel);
        Task<Response> GetByFilterAsync(ElgBookingsQueryModel model);
        Task<Response> GetReserverBookingFilterAsync(ElgBookingsQueryModel model);
        Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> CheckBookingAsync(string query);
        Task<Response> CheckBookingV2Async(string query);

        //vinhtq:26/05/2020
        Task<Response> GetBookingsCheckinFilterAsync(ElgCheckInQueryModel model);

        //vinhtq::28/05/2020
        Task<Response> UpdateBookingCustomerBehavior(decimal bookingId, decimal isAddBehavior, string updateBy);

        //vinhtq::08/12/2020
        Task<Response> CheckBookingV3Async(decimal elgCustId);
    }
}
