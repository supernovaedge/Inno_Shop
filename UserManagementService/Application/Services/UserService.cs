using UserManagementService.Core.Entities;
using UserManagementService.Core.Interfaces;
using UserManagementService.Core.Specifications;
using UserManagementService.Application.DTOs;
using UserManagementService.Application.Utilities;
using FluentValidation;

namespace UserManagementService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IValidator<CreateUserDto> _createUserDtoValidator;
        private readonly IValidator<UpdateUserDto> _updateUserDtoValidator;

        public UserService(IUserRepository userRepository, IEmailService emailService, IValidator<CreateUserDto> createUserDtoValidator, IValidator<UpdateUserDto> updateUserDtoValidator)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _createUserDtoValidator = createUserDtoValidator;
            _updateUserDtoValidator = updateUserDtoValidator;
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
            var validationResult = await _createUserDtoValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
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

            // Send confirmation email
            var confirmationLink = $"https://yourapp.com/confirm-email?userId={user.Id}";
            var emailBody = $"Please confirm your email by clicking <a href=\"{confirmationLink}\">here</a>.";
            await _emailService.SendEmailAsync(user.Email, "Confirm your email", emailBody);
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var validationResult = await _updateUserDtoValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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
            var createUserDto = new CreateUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.PasswordHash, // Assuming PasswordHash is the plain password here
                Role = user.Role
            };

            await CreateUserAsync(createUserDto);
        }

        public async Task UpdateUserAsync(User user)
        {
            var updateUserDto = new UpdateUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                IsEmailConfirmed = user.IsEmailConfirmed
            };

            await UpdateUserAsync(user.Id, updateUserDto);
        }
    }
}
