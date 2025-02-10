namespace DeveloperStore.Application.Usecases.Carts;

public sealed record CartsRequest(int UserId, DateTime CreateDate, IEnumerable<CartItemsRequest> CartItems);