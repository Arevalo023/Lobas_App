using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface IUserService
    {
        List<UserResponseDto> GetAll();

        UserResponseDto? GetById(int id);

        bool Update(int id, UserUpdateDto userDto);

        bool Delete(int id);

        bool UpdateBiometricStatus(int id, bool biometricEnabled);
    }
}