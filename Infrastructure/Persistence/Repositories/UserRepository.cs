using Domain.Contracts.Infrastructure.Persistence.Repositories;
using Domain.Entities.Users;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByCodeAsync(string code)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Code == code)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Email == email)
            .ToListAsync();

        return users.FirstOrDefault(u => u.AuditField.DeletedAt == null);
    }

    public async Task<User?> GetByIdAsync(UserId userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<User>> GetAllWithRoleAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync();
    }
}