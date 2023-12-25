using AutoMapper;
using BusinessLogic.Dtos;
using WebApi.Models.Requests.Account;
using WebApi.Models.Responses.Account;

namespace WebApi.Mappers;

public class AccountMapperProfile : Profile
{
    public AccountMapperProfile() 
    {
        CreateMap<RegisterAccountRequest, RegisterAccountDto>();
        CreateMap<LoginAccountRequest, LoginAccountDto>();

        CreateMap<TokensRequest, TokensDto>();
        CreateMap<TokensDto, TokensResponse>();
    }
}
