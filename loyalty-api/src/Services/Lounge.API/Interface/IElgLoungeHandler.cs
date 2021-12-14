using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.Interface
{
    public interface IElgLoungeHandler
    {
        Task<Response> GetAllListAsync();
    }
}
