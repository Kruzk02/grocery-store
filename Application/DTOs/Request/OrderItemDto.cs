namespace Application.DTOs.Request;

public record OrderItemDto(int OrderId, int ProductId, int Quantity) { }
