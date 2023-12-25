using AutoMapper;
using BusinessLogic.Abstractions;
using BusinessLogic.Constants;
using BusinessLogic.Dtos;
using BusinessLogic.Exceptions;
using DataAccess.Abstractions;
using DataAccess.Entities;
using DataAccess.Specifications;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;
public class BlogService : IBlogService
{
    private readonly IValidator<CreateBlogDto> _createValidator;
    private readonly IValidator<UpdateBlogDto> _updateValidator;
    private readonly IRepository<Blog> _blogRepository;
    private readonly IRepository<User> _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public BlogService(IValidator<CreateBlogDto> createValidator, IValidator<UpdateBlogDto> updateValidator, IRepository<Blog> blogRepository, UserManager<User> userManager, IMapper mapper, IRepository<User> userRepository)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _blogRepository = blogRepository;
        _userManager = userManager;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<BlogDto> Create(Guid userId, CreateBlogDto createBlogDto)
    {
        var user = await _userRepository.FirstOrDefaultAsync(new UserByIdSpec(userId))
            ?? throw new EntityNotFoundException($"User with id {userId} was not found.");

        if (user.Blog is not null)
        {
            throw new EntityValidationFailedException("User already has a blog.");
        }

        var tag = PrepareTag(createBlogDto.Tag);
        var blogWithTagExists = await _blogRepository.AnyAsync(new BlogByTagSpec(tag));
        if (blogWithTagExists)
        {
            throw new EntityAlreadyExistsException($"Blog with tag {tag} already exists.");
        }

        user.Blog = new Blog
        {
            AvatarLocalFileName = user.AvatarLocalFileName,
            CreatedAt = DateTime.UtcNow,
            Description = createBlogDto.Description,
            Tag = tag,
            Name = user.Name
        };

        await _userManager.AddToRoleAsync(user, RolesConstants.Author);

        return _mapper.Map<BlogDto>(user.Blog);
    }

    private static string PrepareTag(string tag) => tag.Trim().ToLower();
}
