using WebStore.Models.Cart;

namespace WebStore.Infrastructure.Interfaces
{
    public interface ICartService
    {
        void DecrementFromCart(int id);

        void RemoveFromCart(int id);

        void RemoveAll();

        void AddToCart(int id);

        CartViewModel TransformCart();

    }
}