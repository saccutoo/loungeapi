using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface ISrvSurveyQuestionsHandler
    {
        Task<Response> GetSurveyQuestionsAsync(decimal surveySectionId);
    }
}
