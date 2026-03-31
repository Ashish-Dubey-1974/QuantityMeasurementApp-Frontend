using ModelLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IUserService
    {
        AuthResponseDto Register(RegisterDto dto);
        AuthResponseDto Login(LoginDto dto);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthDto dto);
    }
}
