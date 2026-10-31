
namespace DonationHub.Application.Patterns
{
    public class GenericResult<T>
    { 
        public string? Message { get; set; }

        public bool? IsSuccess { get; set; }

        public int? StatusCode { get; set; }

        public T? Data { get; set; }
    }
}
