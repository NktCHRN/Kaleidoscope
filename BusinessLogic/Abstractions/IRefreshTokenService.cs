using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IRefreshTokenService
{
    Task<TokensDto> Refresh(TokensDto tokens);
    Task Revoke(Guid userId, string refreshToken);
}
