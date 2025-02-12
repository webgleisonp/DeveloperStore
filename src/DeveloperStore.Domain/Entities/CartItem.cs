namespace DeveloperStore.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    private decimal _price;

    public decimal Price
    {
        get { return _price; }
        set { _price = CalculateItemDiscount(value); }
    }

    public virtual Cart Cart { get; set; }
    public virtual Product Product { get; set; }

    private decimal CalculateItemDiscount(decimal itemPrice)
    {
        if (Quantity >= 10 && Quantity <= 20)
            return itemPrice * 0.8m;
        if (Quantity > 4)
            return itemPrice * 0.9m;
        return itemPrice;
    }
}
