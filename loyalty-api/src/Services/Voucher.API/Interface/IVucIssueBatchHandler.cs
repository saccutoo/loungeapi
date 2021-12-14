using System.Threading.Tasks;
using Utils;
using Voucher.API.Models;
namespace Voucher.API.Interface
{
    public interface IVucIssueBatchHandler
    {
        Task<Response> GetListByFilter(VucVoucherQueryModel model);
        Task<Response> CreateAsync(VucIssueBatchCreateUpdateModel model, EVoucherBaseModel baseModel);
        Task<Response> UpdateAsync(string id, VucIssueBatchCreateUpdateModel model, EVoucherBaseModel baseModel);
        Task<Response> GetIssueBathById(string id);
        Task<Response> GetListIssueBath(string query);
    }
}
