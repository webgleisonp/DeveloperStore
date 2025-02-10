namespace DeveloperStore.Application.Usecases.Carts;

public sealed record CartItemsRequest(int ProductId, int Quantity, decimal ItemPrice);