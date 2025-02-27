﻿using DeveloperStore.Domain.Enums;
using DeveloperStore.Domain.Shared;
using DeveloperStore.Domain.ValueObjects;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

public sealed record UpdateUserCommand(int Id, string Email, string UserName, string Password, Name Name, Address Address, string Phone, Status Status, Role Role) : IRequest<Result<UserResponse>>;
