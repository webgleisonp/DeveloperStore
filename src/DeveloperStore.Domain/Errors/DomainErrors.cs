using DeveloperStore.Domain.Shared;

namespace DeveloperStore.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static Error UsersTableIsEmpty => new("404", "Users table is empty");

        public static Error UserNotFound => new("404", "User not found");

        public static Error UserExists => new("400", "User exists.");

        public static Error InvalidCredentials => new("400", "Invalid credentials");
    }

    public static class Cart
    {
        public static Error CartsTableIsEmpty => new("404", "Carts table is empty");

        public static Error CartNotFound => new("404", "Cart not found");

        public static Error CartExists => new("400", "There is already an active cart for this user");
    }

    public static class CartItem
    {
        public static Error CartItemsTableIsEmpty => new("404", "Cart Items table is empty");

        public static Error CartItemNotFound => new("404", "Cart item not found");

        public static Error CartItemExists => new("400", "Cart item is already included");
    }

    public static class Product
    {
        public static Error ProductsTableIsEmpty => new("404", "Products table is empty");

        public static Error ProductNotFound => new("404", "Product not found");

        public static Error ProductExists => new("400", "Product exists");
    }

    public static class Pagination
    {
        public static Error InvalidPage => new(
            "Pagination.InvalidPage",
            "Page must be greater than 0");

        public static Error InvalidPageSize => new(
            "Pagination.InvalidPageSize",
            "PageSize must be greater than 0");

        public static Error PageExceedsLimit(int page, int totalPages) => new(
            "Pagination.PageExceedsLimit",
            $"Page {page} exceeds the total number of pages ({totalPages})");
    }
}
