using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface ISrvSurveySectionsHandler
    {
        Task<Response> GetSurveySectionsAsync(decimal loungeId);
    }
}
