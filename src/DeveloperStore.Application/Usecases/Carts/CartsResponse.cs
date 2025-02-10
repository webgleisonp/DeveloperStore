namespace DeveloperStore.Application.Usecases.Carts;

public sealed record CartsResponse(int Id, int UserId, DateTime CreateDate, IEnumerable<CartItemsResponse> CartItens);