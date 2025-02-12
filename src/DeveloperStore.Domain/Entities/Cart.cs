namespace DeveloperStore.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreateDate { get; set; }
    public bool Active { get; set; }

    public virtual User User { get; set; } = default!;

    public virtual ICollection<CartItem> CartItems { get; set; } = [];
}