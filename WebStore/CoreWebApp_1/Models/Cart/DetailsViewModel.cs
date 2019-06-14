using WebStore.Models.Order;

namespace WebStore.Models.Cart
{
    public class DetailsViewModel
    {
        public CartViewModel CartViewModel { get; set; }
        public OrderViewModel OrderViewModel { get; set; }
    }
}