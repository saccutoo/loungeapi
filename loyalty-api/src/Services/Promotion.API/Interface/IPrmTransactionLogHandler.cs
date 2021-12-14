using System.Threading.Tasks;
using Utils;
using API.Models;
using System;
using System.Collections.Generic;

namespace API.Interface
{
    public interface IPrmTransactionLogHandler
    {
        Task<Response> GetByFilterAsync(PrmTransactionLogQueryModel queryModel);
        Task<Response> GetListAccountingByTransactionId(decimal transactionId);
        Task<Response> GetByIdAsync(decimal id);
        Task<Response> CreateLogAsync(PrmTransactionLogCreateModel model);
        Task<Response> UpdateLogAsync(decimal transactionId, List<PrmProductTransactionLogCreateModel> listPrmProductTransactionLogModel);
        Task<Response> ApproveByIdAsync(decimal id, string approvedBy);
        Task<Response> RejectByIdAsync(decimal id, string approvedBy, string approvedComment);
        Task<Response> ResendByIdAsync(decimal id);
        Task<Response> DeleteByIdAsync(decimal id);
        Task<Response> RevokeByIdAsync(decimal id, string accRevoke, string status, string revokedBy, string revokeComment, string posRevoke, string userStaffRevoke);
        Task<Response> CheckAcctTideIsClosedByTransLog(decimal id,string posCd);
    }
}
