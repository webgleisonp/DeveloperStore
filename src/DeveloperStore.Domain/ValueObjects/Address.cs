namespace DeveloperStore.Domain.ValueObjects;

public sealed record Address(string City, string Street, int Number, string PostCode, string? Latitude, string? Longitude);
