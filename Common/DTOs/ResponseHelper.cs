using AuthMicroService.Common.Constants;
using AuthMicroService.Common.DTOs;

namespace AuthMicroService.Common.Helpers;

public static class ResponseHelper
{
    public static ApiResponse<T> Success<T>(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            status = true,
            Message = message,
            Data = data,
            Code = LogConst.AUTH_SERVICE_200,
        };
    }

    public static ApiResponse<object> Success(string message = "Success")
    {
        return new ApiResponse<object>
        {
            status = true,
            Message = message,
            Code = LogConst.AUTH_SERVICE_200
        };
    }

    public static ApiResponse<object> Failure(string message)
    {
        return new ApiResponse<object>
        {
            status = false,
            Code = LogConst.AUTH_SERVICE_500,
            Message = message
        };
    }
}