using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserResponseDto Register(UserCreateDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                throw new Exception("El nombre es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                throw new Exception("El correo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new Exception("La contraseña es obligatoria");
            }

            User? existingUser = _userRepository.GetByEmail(userDto.Email.Trim());

            if (existingUser != null)
            {
                throw new Exception("Ya existe un usuario con ese correo");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            User user = new User
            {
                Name = userDto.Name.Trim(),
                Email = userDto.Email.Trim(),
                PasswordHash = passwordHash,
                AuthProvider = "Local",
                ProviderUserId = null,
                BiometricEnabled = false
            };

            int newId = _userRepository.Create(user);

            User createdUser = _userRepository.GetById(newId)!;

            return MapToResponseDto(createdUser);
        }

        public UserResponseDto Login(UserLoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                throw new Exception("El correo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new Exception("La contraseña es obligatoria");
            }

            User? user = _userRepository.GetByEmail(loginDto.Email.Trim());

            if (user == null)
            {
                throw new Exception("Correo o contraseña incorrectos");
            }

            if (user.AuthProvider != "Local")
            {
                throw new Exception("Este usuario usa login social");
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new Exception("El usuario no tiene contraseña configurada");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!passwordValid)
            {
                throw new Exception("Correo o contraseña incorrectos");
            }

            return MapToResponseDto(user);
        }

        public UserResponseDto SocialLogin(SocialLoginDto socialLoginDto)
        {
            if (string.IsNullOrWhiteSpace(socialLoginDto.Email))
            {
                throw new Exception("El correo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(socialLoginDto.AuthProvider))
            {
                throw new Exception("El proveedor de autenticación es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(socialLoginDto.ProviderUserId))
            {
                throw new Exception("El Id del proveedor es obligatorio");
            }

            User? user = _userRepository.GetByProvider(
                socialLoginDto.AuthProvider.Trim(),
                socialLoginDto.ProviderUserId.Trim()
            );

            if (user != null)
            {
                return MapToResponseDto(user);
            }

            User? existingEmailUser = _userRepository.GetByEmail(socialLoginDto.Email.Trim());

            if (existingEmailUser != null)
            {
                return MapToResponseDto(existingEmailUser);
            }

            User newUser = new User
            {
                Name = string.IsNullOrWhiteSpace(socialLoginDto.Name)
                    ? socialLoginDto.Email.Trim()
                    : socialLoginDto.Name.Trim(),
                Email = socialLoginDto.Email.Trim(),
                PasswordHash = null,
                AuthProvider = socialLoginDto.AuthProvider.Trim(),
                ProviderUserId = socialLoginDto.ProviderUserId.Trim(),
                BiometricEnabled = false
            };

            int newId = _userRepository.Create(newUser);

            User createdUser = _userRepository.GetById(newId)!;

            return MapToResponseDto(createdUser);
        }

        private UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AuthProvider = user.AuthProvider,
                ProviderUserId = user.ProviderUserId,
                BiometricEnabled = user.BiometricEnabled,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
    }
}