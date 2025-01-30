namespace EventDrivenCheckout.Models
{
    public class CartItem
    {
        required public Product Products { get; set; }
        public int Quantity { get; set; }
    }
}
