using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Application.Usecases.Users;

public sealed record UserRequest(string Email, string Password, Name Name, Address Address, string Phone, Status Status, Role Role);
