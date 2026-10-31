
namespace DonationHub.Application.Patterns
{
    public class Result
    {
        public async Task<GenericResult<T>> Success<T>(T? data, string? Message = "Proccess successfully completed")
        {
            return await Task.FromResult(new GenericResult<T>
            {
                IsSuccess = true,
                Message = Message,
                Data = data
            });
        }

        public static async Task<GenericResult<T>> FailureAsync<T>(string message)
        {
            return await Task.FromResult(new GenericResult<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            });
        }
    }
}
