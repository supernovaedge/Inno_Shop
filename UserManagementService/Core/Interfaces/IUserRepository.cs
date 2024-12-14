using UserManagementService.Core.Entities;

namespace UserManagementService.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}