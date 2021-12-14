using System;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public interface ICustomerHandler
    {
        Task SyncFromSmartVistaToPortal();
        Task<Response> SyncFromOOSToPortal();
    }
}
