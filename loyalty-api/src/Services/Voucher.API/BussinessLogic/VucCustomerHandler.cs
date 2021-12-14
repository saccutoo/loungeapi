using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Voucher.API.Interface;
using Voucher.API.Models;
using Voucher.API.Infrastructure.Migrations;
using Newtonsoft.Json;

namespace Voucher.API.BussinessLogic
{
    public class VucCustomerHandler : IVucCustomerHandler
    {
        private readonly RepositoryHandler<VucCustomer, VucCustomerModel, VucCustomerQueryModel> _vucCustomerHandler
               = new RepositoryHandler<VucCustomer, VucCustomerModel, VucCustomerQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<VucCustomerHandler> _logger;
        public VucCustomerHandler(ILogger<VucCustomerHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        public async Task<Response> GetCustomerByFilter(VucCustomerQueryModel model)
        {
            try
            {              
                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CUSTOMER.GET_BY_TRANSACTION_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pPageSize", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pPageIndex", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pTransactionId", model.TransactionId, OracleMappingType.Decimal, ParameterDirection.Input);               

                var result = await _vucCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);                

                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }       
    }
}
