using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface IElgReportHandler
    {
        Task<Response> GetListCustomerAsync(ElgReportQueryModel model);
        Task<Response> ExportListCustomerAsync(ElgReportQueryModel model);
        Task<Response> GetListBookingAsync(ElgReportQueryModel model);
        Task<Response> ExportListBookingAsync(ElgReportQueryModel model);
        Task<Response> ExportSummaryBookingAsync(ElgReportQueryModel model);
        Task<Response> ExportDashboardAsync(ElgReportQueryModel queryModel);
        Task<Response> ExportElgCheckinPeopleGoWidthAsync(ElgCheckinPeopleGoWidthQueryModel queryModel);
    }
}
