using EShop.Web.Data;
using EShop.Web.Models.Domain;
using EShop.Web.Models.DTO;
using EShop.Web.Models.Email;
using EShop.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

public class PaymentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public PaymentsController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET: Payment/Payment
    [HttpGet]
    public IActionResult Payment(Guid orderId)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: Payment/Pay
    [HttpPost]
    public async Task<IActionResult> Pay(Guid orderId, string? cardNumber, string? ccv, string? expirationDate)
    {
        var order = _context.Orders
            .Include(o => o.ProductsInOrder)
            .Include("ProductsInOrder.Product")
            .Include(o => o.Supplier)
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null) 
        { 
            return NotFound();
        }

        bool withPayment = false;
       
        if (cardNumber != null && ccv != null && expirationDate != null) 
        {
            withPayment = true;
            // Simulate the payment process (this would be replaced by real payment logic)
            var paymentSuccess = SimulatePayment(cardNumber, ccv, expirationDate);

            if (!paymentSuccess)
            {

                // Redirect to the payment failed page
                return RedirectToAction("PaymentFailed");

            }
            else
            {
                Payment payment = new Payment
                {
                    CardNumber = long.Parse(cardNumber),
                    CCVNumber = int.Parse(ccv),
                    DateCreated = DateTime.UtcNow,
                    Amount = order.Price,
                    ExpirationDate = DateTime.Parse(expirationDate),
                    Status = EShop.Web.Models.Enums.PaymentStatus.Success
                };

                _context.Add(payment);
            }
        }

        await SendEmailToSupplierAfterSuccessfulPaymentAsync(order);

        order.Status = EShop.Web.Models.Enums.OrderStatus.SentToSupplier;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        // Redirect to the success page
        return RedirectToAction("Success", new { orderId = orderId, withPayment = withPayment});
    }

    private bool SimulatePayment(string cardNumber, string ccv, string expirationDate)
    {
        // Simulate a successful payment
        if (
            String.IsNullOrEmpty(cardNumber) || String.IsNullOrWhiteSpace(cardNumber) ||
            String.IsNullOrEmpty(ccv) || String.IsNullOrWhiteSpace(expirationDate) ||
            String.IsNullOrEmpty(expirationDate) || String.IsNullOrEmpty(expirationDate)
            )
            return false;
        return true;
    }

    // GET: Payment/Success
    [HttpGet]
    public IActionResult Success(Guid orderId, bool withPayment)
    {
        // Fetch the order to show success message
        var order = _context.Orders
            .Include(o => o.Supplier)
            .Include(o => o.ProductsInOrder)
            .Include("ProductsInOrder.Product")
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        SuccessDto dto = new SuccessDto
        {
            Order = order,
            WithPayment = withPayment
        };

        return View(dto);
    }

    // GET: Payment/PaymentFailed
    [HttpGet]
    public IActionResult PaymentFailed()
    {
        return View();
    }

    private async Task SendEmailToSupplierAfterSuccessfulPaymentAsync(Order order)
    {
        EmailMessage message = new EmailMessage();
        message.Subject = "Successfull order";
        message.MailTo = order.Supplier.Email;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("New order is created. The order conatins: ");

        var productsInOrder = order.ProductsInOrder.ToList();

        float orderPriceWithoutDelivery = 0;

        for (int i = 1; i <= productsInOrder.Count(); i++)
        {
            var currentItem = productsInOrder[i - 1];
            float productTotal = currentItem.Quantity * currentItem.Product.Price;
            orderPriceWithoutDelivery += productTotal;
            sb.AppendLine(i.ToString() + ". " + currentItem.Product.ProductName + " with quantity of: " + currentItem.Quantity
                + " and price of: $" + currentItem.Product.Price + "(total: " + productTotal + ")");
        }

        float distanceInKm = 5; //default vrednost (se presmetuva rastojanieto megju adresata za dostava i dostavuvacot)

        sb.AppendLine("Price without delivery: " + orderPriceWithoutDelivery);
        sb.AppendLine("Price for delivery: " + order.Supplier.PricePerKm * distanceInKm);
        sb.AppendLine("Total price for the order: " + order.Price.ToString());

        message.Content = sb.ToString();

        await _emailService.SendEmailAsync(message);
    }
}
