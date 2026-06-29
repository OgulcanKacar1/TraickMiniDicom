namespace TraickMiniDicom.Extensions;
{
    public static class ControllerExtensions{
        public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiResponse<T> response)
        {
            return response.Success ? controller.Ok(response) : controller.BadRequest(response);
        }
    }
}