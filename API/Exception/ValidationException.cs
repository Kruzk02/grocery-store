namespace API.Exception;

public class ValidationException(IDictionary<string, string?[]> errors) : System.Exception("Validation Error")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}