using System;
using System.Collections.Generic;
using Utils;

namespace API.Infrastructure.Migrations
{
    public class ElgReviewQuality : PaginationRequest
    {

    }

    public class SrvSurveySections
    {
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string RequiredYN { get; set; }
        public string SubHeading { get; set; }
        public decimal SurveyHeaderId { get; set; }
        public string Title { get; set; }
        public string SurveyHeaderName { get; set; }
        public string SurveyHeaderDescription { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }

    public class SrvSurveyQuestions
    {
        public decimal Id { get; set; }
        public string NameEn { get; set; }
        public string NameVn { get; set; }
        public string ContentEn { get; set; }
        public string ContentVn { get; set; }
        public decimal InputTypeId { get; set; }
        public decimal SurveySectionId { get; set; }
        public string AllowMultipleChoice { get; set; }
        public decimal MinChoice { get; set; }
        public decimal MaxChoice { get; set; }
        public string AllowComment { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string SurveySectionName { get; set; }
        public string SurveySectionTitle { get; set; }
        public string InputTypeName { get; set; }
        public string InputTypeDescription { get; set; }
        public string InputTypeControl { get; set; }
        public List<SrvQuestionsOptions> lstQuestionAnswer { get; set; }
        public decimal QuestionId { get; set; }
    }

    public class SrvQuestionsOptions
    {
        public decimal? Id { get; set; }
        public decimal QuestionOptionId { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionVn { get; set; }
        public string NameEn { get; set; }
        public string NameVn { get; set; }
        public string Image_URL { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }

    public class SrvReviewQualityQuestionsModel
    {
        public List<SrvSurveyQuestions> listQuestions;
    }
}
