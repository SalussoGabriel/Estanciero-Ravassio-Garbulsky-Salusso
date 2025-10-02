using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroReponse
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }

        public ApiResponse(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
            Errors = new List<string>();
        }

        public ApiResponse(string message, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            Errors = errors ?? new List<string>();
        }
    }
}
