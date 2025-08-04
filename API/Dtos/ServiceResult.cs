namespace API.Dtos;

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];

    public static ServiceResult Ok(string message = "") =>
        new() { Success = true, Message = message };

    public static ServiceResult Failed(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors?.ToList() ?? [] };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string message = "") =>
        new() { Success = true, Data = data, Message = message };

    public new static ServiceResult<T> Failed(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors?.ToList() ?? [] };
}