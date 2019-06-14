using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Infrastructure.Interfaces;
using WebStore.Models.Order;

namespace WebStore.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IOrdersService _ordersService;
        public ProfileController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Orders()
        {
            var orders = _ordersService.GetUserOrders(User.Identity.Name);
            var orderModels = new List<UserOrderViewModel>(orders.Count());
            foreach (var order in orders)
            {
                orderModels.Add(new UserOrderViewModel()
                {
                    Id = order.Id,
                    Name = order.Name,
                    Address = order.Address,
                    Phone = order.Phone,
                    TotalSum = order.OrderItems.Sum(o => o.Price * o.Quantity)
                });
            }
            return View(orderModels);
        }
    }
}