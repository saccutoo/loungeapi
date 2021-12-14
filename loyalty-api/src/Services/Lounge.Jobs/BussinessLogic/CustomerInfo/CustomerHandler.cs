using System.Threading.Tasks;
using Utils;
using Microsoft.Extensions.Logging;
using System.Data;
using Devart.Data.Oracle;
using API.Infrastructure.Repositories;
using System;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;

namespace API.BussinessLogic
{
    public class CustomerHandler : ICustomerHandler
    {
        private readonly RepositoryHandler<ElgCustomerBaseModel, ElgCustomerBaseModel, PaginationRequest> _svCustomerHandler
               = new RepositoryHandler<ElgCustomerBaseModel, ElgCustomerBaseModel, PaginationRequest>();

        private readonly RepositoryHandler<ElgMaxCustomer, ElgMaxCustomer, PaginationRequest> _svMaxCustomerHandler
               = new RepositoryHandler<ElgMaxCustomer, ElgMaxCustomer, PaginationRequest>();

        private readonly RepositoryHandler<ElgConfigurate, ElgConfigurateBaseModel, ElgConfigurateQueryModel> _elgConfigurateHandler
               = new RepositoryHandler<ElgConfigurate, ElgConfigurateBaseModel, ElgConfigurateQueryModel>();

        private readonly RepositoryHandler<ElgCustomer, ElgCustomerBaseModel, ElgCustomerQueryModel> _elgCustomerHandler
               = new RepositoryHandler<ElgCustomer, ElgCustomerBaseModel, ElgCustomerQueryModel>();

        private readonly ILogger<CustomerHandler> _logger;
        public CustomerHandler(ILogger<CustomerHandler> logger = null)
        {
            _logger = logger;
        }

        //dong bo tu smart vista sang portal
        public async Task SyncFromSmartVistaToPortal()
        {
            //#region Lấy ID đồng bộ cuối cùng
            //var procName = "PKG_ELG_CMS_CONFIGURATE.GET_CONFIG_BY_CODE";
            //var dyParam = new OracleDynamicParameters();
            //dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
            //dyParam.Add("P_CODE", CustomerContains.SV_SYNC_ID, OracleMappingType.Varchar2, ParameterDirection.Input);

            //var result = await _elgConfigurateHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<ElgConfigurate>>;
            //if (result?.StatusCode == StatusCode.Fail)
            //{
            //    _logger.LogError("SyncFromSmartVistaToPortal Xảy ra lỗi trong quá trình lấy dữ liệu: PKG_ELG_CMS_CONFIGURATE.GET_CONFIG_BY_CODE");
            //    return new Response(StatusCode.Fail, "Xảy ra lỗi trong quá trình lấy dữ liệu");
            //}

            //if (result?.Data?.Count <= 0)
            //{
            //    _logger.LogError("SyncFromSmartVistaToPortal Không có config: SV_SYNC_ID");
            //    return new Response(StatusCode.Fail, "Không có config");
            //}
            //var minID = result.Data[0].Value;
            //_logger.LogInformation("SyncFromSmartVistaToPortal Last ID sync: " + minID);

            //#endregion Lấy ID đồng bộ cuối cùng

            #region Lấy ID đồng bộ cuối cùng
            var procName = "ITCARDAPP.PKG_ELG_CUSTOMER.GET_MAXID_CUSTOMER";
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

            var result = await _svMaxCustomerHandler.ExecuteProcOracleReturnRow("ConnectionString:OracleITCardApp", procName, dyParam,false) as ResponseObject<ElgMaxCustomer>;
            if (result?.StatusCode == StatusCode.Fail)
            {
                _logger.LogError("SyncFromSmartVistaToPortal Xảy ra lỗi trong quá trình lấy dữ liệu: PKG_ELG_CUSTOMER.GET_MAXID_CUSTOMER");
                return;// new Response(StatusCode.Fail, "Xảy ra lỗi trong quá trình lấy dữ liệu");
            }

            decimal minID = 0;
            var maxId = result.Data.MaxId;
            _logger.LogInformation("SyncFromSmartVistaToPortal Last ID sync: " + minID);

            #endregion Lấy ID đồng bộ cuối cùng
            while(minID < maxId)
            {
                var resDataSV = await getDataFromSvAsync(minID) as ResponseObject<List<ElgCustomerBaseModel>>;
                
                if (resDataSV.StatusCode == StatusCode.Success && resDataSV?.Data?.Count > 0)
                {
                    var dataSv = resDataSV.Data;
                    minID = dataSv.Max(x => x.Id);
                    await insertDataToPortal(dataSv);
                }
                else
                {
                    _logger.LogInformation("Xảy ra lỗi khi lấy dữ liệu hoặc Không có dữ liệu đồng bộ");
                    break;
                }
            }                        

        }

        private async Task<Response> insertDataToPortal(List<ElgCustomerBaseModel> dataSV)
        {
            #region insert data to customer
            using (var unitOfWorkOracle = new UnitOfWorkOracle())
            {
                var iConn = unitOfWorkOracle.GetConnection();
                var iTrans = iConn.BeginTransaction();
                for (int i = 0; i < dataSV.Count; i++)
                {
                    try
                    {
                        var item = dataSV[i];

                        var procNameAddOrUpdate = "PKG_ELG_CMS_CUSTOMER.SYNC_CUSTOMER_FROM_SV";
                        var dyParamAddOrUpdate = new OracleDynamicParameters();
                        dyParamAddOrUpdate.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParamAddOrUpdate.Add("i_pos_id", item.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_pos_name ", item.PosName, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_contractid", item.ContractId, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_bin", item.Bin, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_upload_id", 0, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_cif_num", item.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_cust_id", item.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_fullname", item.FullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_represent_name", item.RepresentUserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_represent_id", item.RepresentUserId, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_birthday", item.BirthDay, OracleMappingType.Date, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_email", item.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_phone_num", item.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_expire_date", item.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_gender", item.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_create_by", "SMARTVISTAR", OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_ref_id", item.Id, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParamAddOrUpdate.Add("i_status", item.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                        var result1 = await _elgCustomerHandler.ExecuteProcOracle(procNameAddOrUpdate, iConn, iTrans, dyParamAddOrUpdate) as ResponseObject<ResponseModel>;

                        if (result1.Data != null && !result1.Data.Status.Equals("00"))
                        {
                            _logger.LogInformation("PKG_ELG_CMS_CUSTOMER.SYNC_CUSTOMER_FROM_SV output data: " + Newtonsoft.Json.JsonConvert.SerializeObject(result1));
                            iTrans.Rollback();
                            return new ResponseObject<ResponseModel>(result1.Data, "Không thành công", StatusCode.Fail);
                        }
                    }
                    catch (Exception ex)
                    {
                        iTrans.Rollback();
                        if (_logger != null)
                        {
                            _logger.LogError(ex, "Exception Error");
                            return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                        }
                        else throw ex;
                    }
                }

                iTrans.Commit();
                return new ResponseObject<ResponseModel>(null, "Thành công", StatusCode.Success);
            }

            #endregion insert data to customer


        }


        private async Task<Response> getDataFromSvAsync(decimal minId)
        {

            #region lấy dữ liệu từ SV
            var pSize = Helpers.GetConfig("SVConfig:TopSize");
            var sizeInt = 0;
            var isParseSuccess = int.TryParse(pSize, out sizeInt);
            if (!isParseSuccess)
            {
                sizeInt = 10;
            }
            var procNameSV = "ITCARDAPP.PKG_ELG_CUSTOMER.GET_CUSTOMER_TOSYNC";
            var dyParamSV = new OracleDynamicParameters();
            dyParamSV.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
            dyParamSV.Add("P_SIZE", sizeInt, OracleMappingType.Decimal, ParameterDirection.Input);
            dyParamSV.Add("P_MINID", minId, OracleMappingType.Decimal, ParameterDirection.Input);

            var svDataResult = await _svCustomerHandler.ExecuteProcOracleReturnRow("ConnectionString:OracleITCardApp", procNameSV, dyParamSV) as ResponseObject<List<ElgCustomerBaseModel>>;
            if (svDataResult?.StatusCode == StatusCode.Fail)
            {
                _logger.LogError("SyncFromSmartVistaToPortal Xảy ra lỗi trong quá trình lấy dữ liệu: PKG_ELG_CUSTOMER.GET_CUSTOMER_TOSYNC");
                return new Response(StatusCode.Fail, "Xảy ra lỗi trong quá trình lấy dữ liệu");
            }

            if (svDataResult?.Data?.Count <= 0)
            {
                _logger.LogInformation("SyncFromSmartVistaToPortal Không có data: PKG_ELG_CUSTOMER.GET_CUSTOMER_TOSYNC");
                return new Response(StatusCode.Fail, "Không có data");
            }
            
            return svDataResult;
            
            #endregion lấy dữ liệu từ SV

        }


        //dong bo tu OOS sang portal
        public async Task<Response> SyncFromOOSToPortal()
        {

            #region đồng bộ dữ liệu
            var procName = "PKG_ELG_CMS_CUSTOMER.SYNC_CUSTOMER_FROM_OOS";
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);

            var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam);
            if (result?.StatusCode == StatusCode.Fail)
            {
                _logger.LogError("SyncFromOOSToPortal Xảy ra lỗi trong quá trình đồng bộ dữ liệu: PKG_ELG_CMS_CUSTOMER.SYNC_CUSTOMER_FROM_OOS");
                return new Response(StatusCode.Fail, "Xảy ra lỗi trong quá trình đồng bộ dữ liệu");
            }
            else
            {
                _logger.LogInformation("SyncFromOOSToPortal SUCCESS");
                return new Response(StatusCode.Success, "SUCCESS");
            }

            #endregion đồng bộ dữ liệu
        }

    }

}


