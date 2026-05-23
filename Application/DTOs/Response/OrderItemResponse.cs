using Domain.Entity;

namespace Application.DTOs.Response;

public record OrderItemResponse(int Id, int OrderId, int ProductId, int Quantity, decimal SubTotal)
{
    public static OrderItemResponse FromEntity(OrderItem orderItem)
    {
        return new OrderItemResponse(orderItem.Id, orderItem.OrderId, orderItem.ProductId, orderItem.Quantity,
            orderItem.SubTotal);
    }
}
