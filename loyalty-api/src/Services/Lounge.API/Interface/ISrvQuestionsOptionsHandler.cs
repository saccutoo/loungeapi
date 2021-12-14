using System.Threading.Tasks;
using Utils;
using API.Models;

namespace API.Interface
{
    public interface ISrvQuestionsOptionsHandler
    {
        Task<Response> GetQuestionsOptionsAsync(decimal questionId);
    }
}
