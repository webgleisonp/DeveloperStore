using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Application.Abstractions;

public interface IJwtProvider
{
    string Generate(User user);
}
