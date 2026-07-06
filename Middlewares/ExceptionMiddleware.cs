using System.Text.Json;
using TraickMiniDicom.Responses;
using Microsoft.Extensions.Logging;

namespace TraickMiniDicom.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            //normal akış
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kritik bir hata oluştu. Hata: {Message}", ex.Message);
            //hata yakalandığında akışı durdur ve hatayı işle
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        //content type'ı json 
        context.Response.ContentType = "application/json";
        
        //durum kodunu 500 yap
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        // ApiResponse sınıfını kullanarak hata mesajını oluştur
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Beklenmedik bir hata oluştu.",
            Errors = new List<string> { exception.Message }
        };
        
        // ApiResponse nesnesini JSON formatına çevir ve yanıt olarak gönder
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        return context.Response.WriteAsync(json);

    }
}