namespace TraickMiniDicom.Responses
{
    public class ServiceResult<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }


        // İşlem Başarılı İse
        public static ServiceResult<T> IsSuccess(T data)
        {
            return new ServiceResult<T> {Success = true, Data = data};
        }

        // Hatalı ise
        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T> {Success = false, ErrorMessage = errorMessage};
        }
    }
}