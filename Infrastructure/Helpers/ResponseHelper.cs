using Application.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Helpers
{
    public class ResponseHelper
    {
        public static BaseResponse BuildResponse(string responseCode, ModelStateDictionary errs)
        {
            string firstError = "";
            var errors = new List<string>();
            if (errs != null)
            {
                var errorList = errs.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                foreach (var error in errorList)
                {
                    errors.Add(error);
                }
                firstError = errors.FirstOrDefault();
            }
            var response = new BaseResponse
            {
                Code = responseCode,
                Description = $"Format Error : {firstError}"
            };
            return response;
        }
    }
}
