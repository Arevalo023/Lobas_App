using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface IAuthService
    {
        UserResponseDto Register(UserCreateDto userDto);

        UserResponseDto Login(UserLoginDto loginDto);

        UserResponseDto SocialLogin(SocialLoginDto socialLoginDto);
    }
}