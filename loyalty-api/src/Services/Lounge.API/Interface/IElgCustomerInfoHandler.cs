using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace API.Interface
{
    public interface IElgCustomerInfoHandler
    {
        Task<Response> UpdateAsync(decimal id, ElgCustomerInfoUpdateModel model);
        Task<Response> GetByFilterAsync(ElgCustomerInfoQueryModel model); 
        Task<Response> GetByIdAsync(decimal id);    
    }
}
