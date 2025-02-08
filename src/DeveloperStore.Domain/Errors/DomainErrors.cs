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
