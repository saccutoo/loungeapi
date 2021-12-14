using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using API.Interface;
using API.Models;
using API.Infrastructure.Migrations;
using Newtonsoft.Json;

namespace API.BussinessLogic
{
    public class ElgBookingCheckoutHandler : IElgBookingCheckoutHandler
    {
        private readonly RepositoryHandler<ElgBookingCheckout, ElgBookingCheckoutBaseModel, ElgBookingCheckoutQueryModel> _elgBookingCheckoutHandler
               = new RepositoryHandler<ElgBookingCheckout, ElgBookingCheckoutBaseModel, ElgBookingCheckoutQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgBookingCheckoutHandler> _logger;

        public ElgBookingCheckoutHandler(ILogger<ElgBookingCheckoutHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetByFilterAsync(ElgBookingCheckoutQueryModel model)
        {
            try
            {
                _logger.LogInformation("ElgBookingCheckout - GetByFilterAsync - REQ: model: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKOUT.PRC_FILTER_CHECKIN");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingCheckoutHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("ElgBookingCheckout - GetByFilterAsync - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetAllAsync(string query)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKOUT.PRC_GET_ALL_CHECKIN");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_search_text", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingCheckoutHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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

        public async Task<Response> CheckoutAsync(ElgCheckOutCifsModel cifs,  ELoungeBaseModel baseModel)
        {
            try
            {
                
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKOUT.PRC_BOOKING_CHECKOUT");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_ids", cifs.Cifs, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingCheckoutHandler.ExecuteProcOracle(procName, dyParam);


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
