using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BaseResponse
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public static BaseResponse Success(string message = null)
        {
            return new BaseResponse()
            {
                Code = "00",
                Description = message ?? "Approved or Completed Successfully"
            };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this).ToLower();
        }

        public static BaseResponse Failure(string code = "99", string message = null)
        {
            return new BaseResponse()
            {
                Code = code ?? "99",
                Description = message ?? "Sorry, an error occurred while processing your request.Please try again later."
            };
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; set; }
        public static BaseResponse<T> Success(T data, string code = null, string message = null)
        {
            return new BaseResponse<T>()
            {
                Code = code ?? "00",
                Description = message ?? "Approved or Completed Successfully",
                Data = data
            };
        }

        public static BaseResponse<T> Failure(T data, string code = null, string message = null)
        {
            return new BaseResponse<T>()
            {
                Code = code ?? "99",
                Description = message ?? "Sorry, an error occurred while processing your request.Please try again later.",
                Data = data
            };
        }
    }

    public class PageBaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int PageCount { get; set; }
        public int DataCount { get; set; }
        public int? TotalCount { get; set; }
        public static PageBaseResponse<T> Success(T data, int? pageNumber, int? pageSize, int pageCount, int dataCount, int totalCount, string message = null)
        {
            return new PageBaseResponse<T>()
            {
                Code = "00",
                Description = message ?? "Approved or Completed successfully",
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                DataCount = dataCount,
                PageCount = pageCount,
                TotalCount = totalCount
            };
        }

        public static PageBaseResponse<T> Failure(T data, int? pageNumber, int? pageSize, int pageCount, int dataCount, int totalCount ,string message = null)
        {
            return new PageBaseResponse<T>()
            {
                Code = "99",
                Description = message ?? "Sorry, an error occurred while processing your request.Please try again later.",
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                DataCount = dataCount,
                PageCount = pageCount,
                TotalCount = totalCount
            };
        }
    }
}
