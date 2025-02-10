namespace DeveloperStore.Domain.Entities;

public class CartItem
{
    private decimal _price;

    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public decimal SetPrice(decimal value) => _price = CalculateItemDiscount(value);
    public decimal Price => _price;

    public virtual Cart Cart { get; set; }
    public virtual Product Product { get; set; }

    private decimal CalculateItemDiscount(decimal itemPrice)
    {
        if (Quantity > 4 && Quantity < 10)
            itemPrice = itemPrice * 0.9m;

        if (Quantity >= 10 && Quantity <= 20)
            itemPrice = itemPrice * 0.8m;

        return itemPrice;
    }
}