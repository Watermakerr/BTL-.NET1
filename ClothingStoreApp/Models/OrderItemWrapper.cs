namespace ClothingStoreApp.Models
{
    public class OrderItemWrapper
    {
        public Cart Cart { get; set; }
        public Product Product { get; set; }

        public OrderItemWrapper(Cart cart, Product product)
        {
            Cart = cart;
            Product = product;
        }

        public decimal TotalPrice => Cart.Quantity * Product.Price;
    }
}