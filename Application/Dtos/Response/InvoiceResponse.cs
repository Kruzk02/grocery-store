using Domain.Entity;

namespace Application.Dtos.Response;

public record InvoiceResponse(int Id, int OrderId, string InvoiceNumber, DateTime IssueDate, DateTime DueTime)
{
    public static InvoiceResponse FromEntity(Invoice invoice)
    {
        return new InvoiceResponse(invoice.Id, invoice.OrderId, invoice.InvoiceNumber, invoice.DueDate, invoice.IssueDate);
    }
}
