using Domain.Entity;

namespace Application.Dtos.Response;

public record OrderResponse(int Id, int CustomerId, IReadOnlyList<int> OrderItemIds, decimal Total, DateTime CreatedAt)
{
    public static OrderResponse FromEntity(Order order)
    {
        return new OrderResponse(order.Id, order.CustomerId, order.Items.Select(x => x.Id).ToList(), order.Total,
            order.CreatedAt);
    }
}
