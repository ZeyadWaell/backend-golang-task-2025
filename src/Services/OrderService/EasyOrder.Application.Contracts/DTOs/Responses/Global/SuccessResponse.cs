using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.DTOs.Responses.Global
{
    public class SuccessResponse<T> : BaseApiResponse
    {
        public T Data { get; }

        public SuccessResponse(string message, T data, int statusCode = 200)
            : base(true, message, statusCode)
        {
            Data = data;
        }
    }
}
