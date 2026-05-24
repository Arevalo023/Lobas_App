using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserResponseDto> GetAll()
        {
            List<User> users = _userRepository.GetAll();

            return users.Select(MapToResponseDto).ToList();
        }

        public UserResponseDto? GetById(int id)
        {
            User? user = _userRepository.GetById(id);

            if (user == null)
            {
                return null;
            }

            return MapToResponseDto(user);
        }

        public bool Update(int id, UserUpdateDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                throw new Exception("El nombre es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                throw new Exception("El correo es obligatorio");
            }

            User user = new User
            {
                Name = userDto.Name.Trim(),
                Email = userDto.Email.Trim(),
                BiometricEnabled = userDto.BiometricEnabled
            };

            return _userRepository.Update(id, user);
        }

        public UserResponseDto UpdateName(int id, UserNameUpdateDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Name))
            {
                throw new Exception("El nombre es obligatorio");
            }

            User? existingUser = _userRepository.GetById(id);

            if (existingUser == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            string newName = userDto.Name.Trim();

            if (string.Equals(existingUser.Name.Trim(), newName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("El nombre es igual al actual. No hay cambios.");
            }

            DateTime now = DateTime.Now;

            if (existingUser.LastNameChangedAt.HasValue)
            {
                DateTime nextAllowedChange = existingUser.LastNameChangedAt.Value.AddDays(3);

                if (now < nextAllowedChange)
                {
                    throw new InvalidOperationException(
                        $"Solo puedes cambiar tu nombre una vez cada 3 dias. Podras cambiarlo el {nextAllowedChange:dd/MM/yyyy HH:mm}.");
                }
            }

            bool updated = _userRepository.UpdateName(id, newName, now);

            if (!updated)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            User updatedUser = _userRepository.GetById(id)!;

            return MapToResponseDto(updatedUser);
        }

        public bool Delete(int id)
        {
            return _userRepository.Delete(id);
        }

        public bool UpdateBiometricStatus(int id, bool biometricEnabled)
        {
            return _userRepository.UpdateBiometricStatus(id, biometricEnabled);
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
                LastNameChangedAt = user.LastNameChangedAt,
                IsActive = user.IsActive
            };
        }
    }
}
