using System.Threading.Tasks;
using Utils;
using API.Models;
using System;

namespace API.Interface
{
    public interface IElgReviewQualityHandler
    {
        Task<Response> ReviewQualityAsync(ElgReviewQualityModel model, ELoungeBaseModel baseModel);
        Task<Response> GetSurveyQuestions(decimal loungeId, decimal surveySectionId);
        Task<Response> GetReportData(DateTime startDate, DateTime endDate);
        //21/05/2021:vinhtq 
        Task<Response> GetQuestionAnswers(decimal questionId);

    }
}
