using API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;
using API.Interface;
using API.Models;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class ElgReviewQualityHandler : IElgReviewQualityHandler
    {
        private readonly RepositoryHandler<ElgReviewQualityModel, ElgReviewQualityModel, ElgReviewQualityQueryModel> _elgReviewQualityhandler
               = new RepositoryHandler<ElgReviewQualityModel, ElgReviewQualityModel, ElgReviewQualityQueryModel>();

        private readonly RepositoryHandler<SrvSurveyQuestions, SrvSurveyQuestions, ElgReviewQuality> _srvQuestionsHandler
               = new RepositoryHandler<SrvSurveyQuestions, SrvSurveyQuestions, ElgReviewQuality>();

        private readonly RepositoryHandler<SrvQuestionsOptions, SrvQuestionsOptions, ElgReviewQuality> _srvQuestionAnswersHandler
               = new RepositoryHandler<SrvQuestionsOptions, SrvQuestionsOptions, ElgReviewQuality>();

        private readonly RepositoryHandler<ReviewQualityReportModel, ReviewQualityReportModel, ElgReviewQualityQueryModel> _reviewQualityReporthandler
               = new RepositoryHandler<ReviewQualityReportModel, ReviewQualityReportModel, ElgReviewQualityQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<ElgReviewQualityHandler> _logger;

        public ElgReviewQualityHandler(ILogger<ElgReviewQualityHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> ReviewQualityAsync(ElgReviewQualityModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                if (model.LstQuestionsAndAnswers == null || model.LstQuestionsAndAnswers.Count < 1)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                _logger.LogInformation("ReviewQualityAsync - REQ: model: " + JsonConvert.SerializeObject(model) + " | baseModel: " + JsonConvert.SerializeObject(baseModel));

                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    foreach (var surveyAnswerModel in model.LstQuestionsAndAnswers)
                    {
                        try
                        {
                            var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REVIEW_QUALITY.PRC_UPDATE_ANSWERS");
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("i_user_id", model.UserId, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_question_option_id", surveyAnswerModel.QuestionOptionId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("i_answer_numeric", surveyAnswerModel.AnswerNumeric, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("i_answer_text", surveyAnswerModel.AnswerText, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_answer_yn", surveyAnswerModel.AnswerYN, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                            var result = await _elgReviewQualityhandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

                            if (result.Data != null && !result.Data.Status.Equals("00"))
                            {
                                iTrans.Rollback();
                                return new ResponseObject<ResponseModel>(result.Data, "Không thành công", StatusCode.Fail);
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
                }

                // Update usser survey section
                await UpdateUserSurveySection(model.UserId, model.SurveySectionId, baseModel);

                return new ResponseObject<ResponseModel>(null, "Thành công", StatusCode.Success);
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

        public async Task<Response> UpdateUserSurveySection(string userId, decimal SurveySectionId, ELoungeBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("ElgReviewQuality - UpdateUserSurveySection - REQ: userId: " + SurveySectionId + "| basemodel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REVIEW_QUALITY.PRC_UPDATE_SURVEY_SECTION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_user_id", userId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_survey_section_id", SurveySectionId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgReviewQualityhandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ElgReviewQuality - UpdateUserSurveySection - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetQuestionAnswers(decimal questionId)
        {
            try
            {
                if (_logger != null)
                    _logger.LogInformation("ElgReviewQuality - GetQuestionAnswers - REQ: questionId: " + questionId);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REVIEW_QUALITY.PRC_GET_ANSWERS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_question_id", questionId, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _srvQuestionAnswersHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                if (_logger != null)
                    _logger.LogInformation("ElgReviewQuality - GetQuestionAnswers - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetSurveyQuestions(decimal loungeId, decimal surveySectionId)
        {
            try
            {
                _logger.LogInformation("ElgReviewQuality - GetSurveyQuestions - REQ: loungeId: " + loungeId + "| surveySectionId: " + surveySectionId);

                SrvReviewQualityQuestionsModel modelRes = new SrvReviewQualityQuestionsModel();

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REVIEW_QUALITY.PRC_GET_SURVEY_QUESTIONS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_lounge_id", loungeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_srv_section_id", surveySectionId, OracleMappingType.Decimal, ParameterDirection.Input);

                var responseQuestions = await _srvQuestionsHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<SrvSurveyQuestions>>;
                if (responseQuestions == null || responseQuestions.Data == null)
                {
                    _logger.LogInformation("SrvSurveyQuestions - GetSurveyQuestionsAsync - Check Get Questions: FAIL");
                    return new ResponseObject<SrvReviewQualityQuestionsModel>(null, "Thông tin không chính xác", StatusCode.Success);
                }
                _logger.LogInformation("SrvSurveyQuestions - GetSurveyQuestionsAsync - RES:" + JsonConvert.SerializeObject(responseQuestions));
                foreach (SrvSurveyQuestions question in responseQuestions.Data)
                {
                    var responseAnswers = await GetQuestionAnswers(question.Id) as ResponseObject<List<SrvQuestionsOptions>>;

                    if (responseAnswers == null || responseAnswers.Data == null)
                    {
                        _logger.LogInformation("SrvSurveyQuestions - GetQuestionAnswers - Check Get Answers: FAIL");
                        return new ResponseObject<SrvReviewQualityQuestionsModel>(null, "Thông tin không chính xác", StatusCode.Success);
                    }
                    question.lstQuestionAnswer = responseAnswers.Data;
                }
                modelRes.listQuestions = responseQuestions.Data;

                return new ResponseObject<SrvReviewQualityQuestionsModel>(modelRes, "Thành công", StatusCode.Success);
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


        public async Task<Response> GetReportData(DateTime startDate, DateTime endDate)
        {
            try
            {
                try
                {
                    var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_REVIEW_QUALITY.REPORT_QUALITY");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("p_StartDate", startDate, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("p_EndDate", endDate, OracleMappingType.Date, ParameterDirection.Input);
                    var result = await _reviewQualityReporthandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ReviewQualityReportModel>>;
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
