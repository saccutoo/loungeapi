using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using Utils;
using API.Interface;
using API.Models;
using API.Infrastructure.Migrations;

namespace API.BussinessLogic
{
    public class ElgReportHandler : IElgReportHandler
    {
        private readonly RepositoryHandler<ElgReport, ElgReportCustomerModel, ElgReportQueryModel> _elgReportCustomerHandler
               = new RepositoryHandler<ElgReport, ElgReportCustomerModel, ElgReportQueryModel>();
        private readonly RepositoryHandler<ElgReport, ElgReportBookingModel, ElgReportQueryModel> _elgReportBookingHandler
               = new RepositoryHandler<ElgReport, ElgReportBookingModel, ElgReportQueryModel>();
        private readonly RepositoryHandler<ElgExportDashboardModel, ElgExportDashboardModel, ElgReportQueryModel> _elgReportDashboardandler
               = new RepositoryHandler<ElgExportDashboardModel, ElgExportDashboardModel, ElgReportQueryModel>();
        private readonly RepositoryHandler<ElgSummaryReport, ElgSummaryReportModel, ElgReportQueryModel> _elgReportSummaryBookingHandler
               = new RepositoryHandler<ElgSummaryReport, ElgSummaryReportModel, ElgReportQueryModel>();

        private readonly RepositoryHandler<ElgCheckinPeopleGoWidthBaseModel, ElgCheckinPeopleGoWidthBaseModel, ElgCheckinPeopleGoWidthQueryModel> _elgReportElgCheckinPeopleGoWidthdandler
       = new RepositoryHandler<ElgCheckinPeopleGoWidthBaseModel, ElgCheckinPeopleGoWidthBaseModel, ElgCheckinPeopleGoWidthQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgReportHandler> _logger;

        public ElgReportHandler(ILogger<ElgReportHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetListCustomerAsync(ElgReportQueryModel model)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_GET_LIST_CUSTOMERS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_fromdate", model.FromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", model.ToDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_custids", model.CustIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_use", model.VoucherUse, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgReportCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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

        public async Task<Response> ExportListCustomerAsync(ElgReportQueryModel model)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_EXPORT_LIST_CUSTOMERS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_fromdate", model.FromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", model.ToDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_custids", model.CustIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_use", model.VoucherUse, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgReportCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);



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

        public async Task<Response> GetListBookingAsync(ElgReportQueryModel model)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_GET_LIST_BOOKINGS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_fromdate", model.FromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", model.ToDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_custids", model.CustIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_use", model.VoucherUse, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgReportBookingHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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

        public async Task<Response> ExportListBookingAsync(ElgReportQueryModel model)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_EXPORT_LIST_BOOKINGS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_fromdate", model.FromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", model.ToDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_custids", model.CustIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_use", model.VoucherUse, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgReportBookingHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);



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

        public async Task<Response> ExportSummaryBookingAsync(ElgReportQueryModel model)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_EXPORT_SUMMARY_BOOKING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_fromdate", model.FromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", model.ToDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_custids", model.CustIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_use", model.VoucherUse, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgReportSummaryBookingHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);



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

        public async Task<Response> ExportDashboardAsync(ElgReportQueryModel queryModel)
        {
            try
            {
                var dateFromDefault = new DateTime(1, 1, 1, 0, 0, 1);
                var dateToDefault = new DateTime(9999, 12, 30, 23, 59, 59);
                var fromDate = queryModel.FromDate.HasValue ? new DateTime(queryModel.FromDate.Value.Year, queryModel.FromDate.Value.Month, queryModel.FromDate.Value.Day, 0, 0, 1) : dateFromDefault;
                var toDate = queryModel.ToDate.HasValue ? new DateTime(queryModel.ToDate.Value.Year, queryModel.ToDate.Value.Month, queryModel.ToDate.Value.Day, 23, 59, 59) : dateToDefault;

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_EXPORT_DASHBOARD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_fromdate", fromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", toDate, OracleMappingType.Date, ParameterDirection.Input);

                var result = await _elgReportDashboardandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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

        public async Task<Response> ExportElgCheckinPeopleGoWidthAsync(ElgCheckinPeopleGoWidthQueryModel queryModel)
        {
            try
            {
                var dateFromDefault = new DateTime(1, 1, 1, 0, 0, 1);
                var dateToDefault = new DateTime(9999, 12, 30, 23, 59, 59);
                var fromDate = queryModel.FromDate.HasValue ? new DateTime(queryModel.FromDate.Value.Year, queryModel.FromDate.Value.Month, queryModel.FromDate.Value.Day, 0, 0, 1) : dateFromDefault;
                var toDate = queryModel.ToDate.HasValue ? new DateTime(queryModel.ToDate.Value.Year, queryModel.ToDate.Value.Month, queryModel.ToDate.Value.Day, 23, 59, 59) : dateToDefault;

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REPORT.PRC_EXPORT_ACCOMPANYING_PERSON");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_fromdate", fromDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_todate", toDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_page_size", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgReportElgCheckinPeopleGoWidthdandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
