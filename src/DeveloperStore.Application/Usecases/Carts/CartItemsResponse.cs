namespace DeveloperStore.Application.Usecases.Carts;

public sealed record CartItemsResponse(int CartId, int ProductId, int Quantity, decimal ItemPrice);
