using API.Data;
using API.Dtos;
using API.Entity;
using API.Exception;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

public class InvoiceService(ApplicationDbContext ctx) : IInvoiceService
{
    public async Task<Invoice> Create(InvoiceDto invoiceDto)
    {
        var order = await ctx.Orders.FindAsync(invoiceDto.OrderId);
        if (order == null) throw new NotFoundException($"Order with id: {invoiceDto.OrderId} not found");

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
        var invoice = await (
                from inv in ctx.Invoices
                join ord in ctx.Orders on inv.OrderId equals ord.Id
                where inv.Id == id
                select new Invoice
                {
                    Id = inv.Id,
                    OrderId = inv.OrderId,
                    InvoiceNumber = inv.InvoiceNumber,
                    Order = ord
                }
            )
            .FirstOrDefaultAsync(i => i.Id == id);
        
        return invoice ?? throw new NotFoundException($"Invoice with id: {id} not found");
    }

    public async Task<Invoice> FindByOrderId(int orderId)
    {
        var invoice = await (
                from inv in ctx.Invoices
                join ord in ctx.Orders on inv.OrderId equals ord.Id
                where ord.Id == orderId
                select new Invoice
                {
                    Id = inv.Id,
                    OrderId = inv.OrderId,
                    InvoiceNumber = inv.InvoiceNumber,
                    Order = ord
                }
            )
            .FirstOrDefaultAsync(i => i.OrderId == orderId);
        
        return invoice ?? throw new NotFoundException($"Invoice with order id: {orderId} not found");
    }
}