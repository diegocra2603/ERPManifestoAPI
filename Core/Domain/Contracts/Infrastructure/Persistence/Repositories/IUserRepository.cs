using Domain.Entities.Users;
using Domain.ValueObjects;

namespace Domain.Contracts.Infrastructure.Persistence.Repositories;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByCodeAsync(string code);
    Task<User?> GetByIdAsync(UserId userId);
    Task<IReadOnlyList<User>> GetAllWithRoleAsync();
}