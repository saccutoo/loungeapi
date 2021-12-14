using System.Threading.Tasks;
using Utils;
using API.Models;
using System;

namespace API.Interface
{
    public interface IElgCheckinHandler
    {
        Task<Response> CheckinAsync(ElgCheckinPeopleGoWithCheckinModel model, ELoungeBaseModel baseModel);
        Task<Response> GetListCheckInPeopleGoWith(decimal bookingId);

        Task<Response> CheckinNewAsync(ElgCheckinPeopleGoWithCheckinModel model, ELoungeBaseModel baseModel);
    }
}
