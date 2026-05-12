using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace ECommerce528.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;

        public CheckoutController(
            UserManager<ApplicationUser> userManager,
            IRepository<Order> orderRepository, 
            IRepository<OrderItem> orderItemRepository,
            IRepository<Cart> cartRepository,
            IRepository<Product> productRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Success(int orderId)
        {
            // Update Order Status (pending => in processing)
            // Update Payment Status (pending => completed)
            // Update transaction id

            var order = await _orderRepository.GetOneAsync(e => e.Id == orderId);
            if (order is null) return NotFound();

            var service = new SessionService();
            var session = service.Get(order.SessionId);

            order.OrderStatus = OrderStatus.InProcessing;
            order.PaymentStatus = PaymentStatus.Completed;
            order.TransactionId = session.PaymentIntentId;
            await _orderRepository.CommitAsync();

            // create order items
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationuserId == user.Id, includes: [e => e.Product]);

            foreach (var item in userCart)
            {
                await _orderItemRepository.CreateAsync(new()
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    ProductPrice = (decimal)item.ProductPrice,
                    Quantity = item.Quantity,
                });
            }
            await _orderItemRepository.CommitAsync();

            // decrease product quantity
            foreach (var item in userCart)
            {
                if (item.Product != null)
                    item.Product.Quantity -= item.Quantity;
            }
            await _productRepository.CommitAsync();

            // delete old cart
            foreach (var item in userCart)
                _cartRepository.Delete(item);
            await _cartRepository.CommitAsync();

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
