using API.Data;
using API.Entity;
using API.Exception;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

public class InvoiceService(ApplicationDbContext ctx) : IInvoiceService
{
    public async Task<Invoice> Create(int orderId)
    {
        var order = await ctx.Orders.FindAsync(orderId);
        if (order == null) throw new NotFoundException($"Order with id: {orderId} not found");

        var invoice = new Invoice
        {
            OrderId = order.Id,
            Order = order,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            InvoiceNumber = $"INV-{DateTime.UtcNow.Year}:{order.Id:D4}"
        };
        
        var result = await ctx.Invoices.AddAsync(invoice);
        await ctx.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Invoice> FindById(int id)
    {
        var invoice = await ctx.Invoices.FindAsync(id);
        return invoice ?? throw new NotFoundException($"Invoice with id: {id} not found");
    }

    public async Task<Invoice> FindByOrderId(int orderId)
    {
        var invoice = await ctx.Invoices
            .Where(i => i.OrderId == orderId)
            .FirstOrDefaultAsync(i => i.Order.Id == orderId);
        
        return invoice ?? throw new NotFoundException($"Invoice with order id: {orderId} not found");
    }
}