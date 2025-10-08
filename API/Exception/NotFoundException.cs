namespace API.Exception;

public class NotFoundException(string message) : System.Exception(message);