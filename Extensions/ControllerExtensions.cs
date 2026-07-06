using Microsoft.AspNetCore.Mvc;
using TraickMiniDicom.Responses;
namespace TraickMiniDicom.Extensions
{
    public static class ControllerExtensions{
        public static IActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
        {
            var apiResponse = new ApiResponse<T>
            {
                Success = result.Success,
                Data = result.Data,
                Message = result.Success ? "İşlem Başarılı" : result.ErrorMessage
            };

            return result.Success ? controller.Ok(apiResponse) : controller.BadRequest(apiResponse);
        }
    }
}