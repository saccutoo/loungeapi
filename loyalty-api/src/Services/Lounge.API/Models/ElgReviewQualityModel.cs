using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{
    public class ElgReviewQualityModel
    {
        public string UserId { get; set; }
        public decimal SurveySectionId { get; set; }
        public List<SurveyAnswerModel> LstQuestionsAndAnswers { get; set; }
    }

    public class SurveyAnswerModel
    {
        public decimal QuestionOptionId { get; set; }
        public decimal AnswerNumeric { get; set; }
        public string AnswerText { get; set; }
        public string AnswerYN { get; set; }
    }

    public class ElgReviewQualityQueryModel : PaginationRequest
    {

    }

    public class ReviewQualityReportModel
    {
        public decimal Id { get; set; }
        public string NameVn { get; set; }
        public DateTime CreateDate { get; set; }
        public string QuestionId { get; set; }

    }
}
