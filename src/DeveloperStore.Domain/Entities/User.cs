using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public Name Name { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }
    public Status Status { get; set; }
    public Role Role { get; set; }

    public virtual ICollection<Cart> Carts { get; set; }
}
