using UserManagementService.Core.Entities;
using UserManagementService.Core.Interfaces;
using UserManagementService.Core.Specifications;
using UserManagementService.Application.DTOs;
using UserManagementService.Application.Utilities;

namespace UserManagementService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UniqueEmailSpecification _uniqueEmailSpecification;
        private readonly PasswordStrengthSpecification _passwordStrengthSpecification;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _uniqueEmailSpecification = new UniqueEmailSpecification(userRepository);
            _passwordStrengthSpecification = new PasswordStrengthSpecification();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task CreateUserAsync(CreateUserDto createUserDto)
        {
            if (!await _uniqueEmailSpecification.IsSatisfiedByAsync(createUserDto.Email))
            {
                throw new Exception("Email is already in use.");
            }

            if (!_passwordStrengthSpecification.IsSatisfiedBy(createUserDto.Password))
            {
                throw new Exception("Password does not meet strength requirements.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                PasswordHash = PasswordHasher.HashPassword(createUserDto.Password),
                Role = createUserDto.Role,
                IsEmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;
            user.Role = updateUserDto.Role;
            user.IsEmailConfirmed = updateUserDto.IsEmailConfirmed;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return false;
            }

            return true;
        }

        public async Task CreateUserAsync(User user)
        {
            if (!await _uniqueEmailSpecification.IsSatisfiedByAsync(user.Email))
            {
                throw new Exception("Email is already in use.");
            }

            if (!_passwordStrengthSpecification.IsSatisfiedBy(user.PasswordHash))
            {
                throw new Exception("Password does not meet strength requirements.");
            }

            user.Id = Guid.NewGuid();
            user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.IsEmailConfirmed = user.IsEmailConfirmed;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existingUser);
        }
    }
}
