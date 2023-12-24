﻿using BusinessLogic.Abstractions;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using BusinessLogic.Extensions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using System.Security;

namespace BusinessLogic.Services;
public class RefreshTokenService : IRefreshTokenService
{
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;

    public RefreshTokenService(IJwtTokenProvider jwtTokenProvider, IRepository<RefreshToken> refreshTokenRepository)
    {
        _jwtTokenProvider = jwtTokenProvider;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokensDto> Refresh(TokensDto tokens)
    {
        try
        {
            var principal = _jwtTokenProvider.GetPrincipalFromExpiredToken(tokens.AccessToken);
            var userId = principal.GetId() ?? throw new EntityNotFoundException("No info about user id in the token");
            var refreshTokenDataModel = await _refreshTokenRepository.FirstOrDefaultAsync(new RefreshTokenByUserIdAndTokenSpec(userId, tokens.RefreshToken));
            if (refreshTokenDataModel is null
                || refreshTokenDataModel.Token != tokens.RefreshToken
                || refreshTokenDataModel.ExpiryTime <= DateTimeOffset.UtcNow)
            {
                throw new EntityValidationFailedException("Invalid client request");
            }

            var newAccessToken = _jwtTokenProvider.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _jwtTokenProvider.GenerateRefreshToken();
            refreshTokenDataModel.Token = newRefreshToken;
            await _refreshTokenRepository.UpdateAsync(refreshTokenDataModel);

            return new TokensDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }
        catch (SecurityException ex)
        {
            throw new EntityValidationFailedException(ex.Message, ex);
        }
    }

    public async Task Revoke(Guid userId, string refreshToken)
    {
        var refreshTokenDataModel = await _refreshTokenRepository.FirstOrDefaultAsync(new RefreshTokenByUserIdAndTokenSpec(userId, refreshToken)) 
            ?? throw new EntityNotFoundException("User was not found");

        await _refreshTokenRepository.DeleteAsync(refreshTokenDataModel);
    }
}
