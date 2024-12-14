using UserManagementService.Core.Interfaces;

namespace UserManagementService.Core.Specifications
{
    public class UniqueEmailSpecification
    {
        private readonly IUserRepository _userRepository;

        public UniqueEmailSpecification(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> IsSatisfiedByAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null;
        }
    }
}
