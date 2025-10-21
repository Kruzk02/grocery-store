namespace API.Dtos;

public record ProductDto(string Name, string Description, decimal Price, int CategoryId, int Quantity) {}