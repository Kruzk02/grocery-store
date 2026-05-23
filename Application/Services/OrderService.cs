using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Application.Repository;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, IMemoryCache cache)
    : IOrderService
{
    public async Task<OrderResponse> Create(OrderDto orderDto)
    {
        Customer? customer = await customerRepository.FindById(orderDto.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id {orderDto.CustomerId} not found");
        }

        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            Customer = customer
        };

        return OrderResponse.FromEntity(await orderRepository.Add(order));
    }

    public async Task<OrderResponse> Update(int id, OrderDto orderDto)
    {
        Order? order = await orderRepository.FindById(id);
        if (order == null)
        {
            throw new NotFoundException($"Order with id {id} not found");
        }

        if (orderDto.CustomerId != order.CustomerId && orderDto.CustomerId != 0)
        {
            Customer? customer = await customerRepository.FindById(orderDto.CustomerId);
            if (customer == null)
            {
                throw new NotFoundException($"Customer with id {orderDto.CustomerId} not found");
            }

            order.CustomerId = orderDto.CustomerId;
            order.Customer = customer;
        }

        await orderRepository.Update(order);
        return OrderResponse.FromEntity(order);
    }

    public async Task<OrderResponse> FindById(int id)
    {
        var cacheKey = $"order:{id}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            Order? order = await orderRepository.FindById(id);

            return OrderResponse.FromEntity(order ?? throw new NotFoundException($"Order with id: {id} not found."));
        }) ?? throw new InvalidOperationException();
    }

    public async Task<List<OrderResponse>> FindByCustomerId(int customerId)
    {
        var cacheKey = $"customer:{customerId}:orders";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            IReadOnlyList<Order> orders = await orderRepository.FindByCustomerId(customerId);

            return orders.Select(OrderResponse.FromEntity).ToList();
        }) ?? throw new InvalidOperationException();
    }

    public async Task<bool> Delete(int id)
    {
        Order? order = await orderRepository.FindById(id);
        if (order == null)
        {
            throw new NotFoundException($"Order with id {id} not found");
        }

        cache.Remove($"order:{id}");
        await orderRepository.Delete(order);
        return true;
    }
}
