using Microsoft.EntityFrameworkCore;
using UserManagementService.Core.Entities;
using UserManagementService.Core.Interfaces;
using UserManagementService.Infrastructure.Data;

namespace UserManagementService.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(UserManagementDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
    }
}