namespace AuthMicroService.Common.Models;

public class ApiResponse<T>
{
    public bool status { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Code { get; set; }

    public T? Data { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}