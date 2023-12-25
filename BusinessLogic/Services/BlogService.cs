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
    private readonly IBlobRepository _blobRepository;

    public BlogService(IValidator<CreateBlogDto> createValidator, IValidator<UpdateBlogDto> updateValidator, IRepository<Blog> blogRepository, UserManager<User> userManager, IMapper mapper, IRepository<User> userRepository, IBlobRepository blobRepository)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _blogRepository = blogRepository;
        _userManager = userManager;
        _mapper = mapper;
        _userRepository = userRepository;
        _blobRepository = blobRepository;
    }

    public async Task<BlogDto> Create(Guid userId, CreateBlogDto createBlogDto)
    {
        var validationResults = _createValidator.Validate(createBlogDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

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

    public async Task<BlogDto> Update(Guid userId, Guid blogId, UpdateBlogDto updateBlogDto)
    {
        var validationResults = _updateValidator.Validate(updateBlogDto);
        if (!validationResults.IsValid)
        {
            throw new EntityValidationFailedException(validationResults.Errors);
        }

        var blog = await _blogRepository.FirstOrDefaultAsync(new BlogByIdSpec(blogId))
            ?? throw new EntityNotFoundException($"Blog with id {blogId} was not found.");

        if (blog.UserId != userId)
        {
            throw new EntityValidationFailedException($"You cannot update this blog.");
        }

        var tag = PrepareTag(updateBlogDto.Tag);
        if (tag != blog.Tag)
        {
            var blogWithTagExists = await _blogRepository.AnyAsync(new BlogByTagSpec(tag));
            if (blogWithTagExists)
            {
                throw new EntityAlreadyExistsException($"Blog with tag {tag} already exists.");
            }
        }

        if (updateBlogDto.AvatarLocalFileName != blog.AvatarLocalFileName 
            && !string.IsNullOrEmpty(updateBlogDto.AvatarLocalFileName)
            && !await _blobRepository.ExistsAsync(updateBlogDto.AvatarLocalFileName))
        {
            throw new EntityValidationFailedException($"Image with name {updateBlogDto.AvatarLocalFileName} was not found");
        }

        blog.AvatarLocalFileName = updateBlogDto.AvatarLocalFileName;
        blog.Name = updateBlogDto.Name;
        blog.Description = updateBlogDto.Description;
        blog.Tag = tag;
        blog.User.Name = updateBlogDto.Name;
        blog.User.AvatarLocalFileName = updateBlogDto.AvatarLocalFileName;

        await _blogRepository.UpdateAsync(blog);

        return _mapper.Map<BlogDto>(blog);
    }

    public async Task<BlogDto> GetByTag(string tag)
    {
        tag = PrepareTag(tag);
        var blog = await _blogRepository.FirstOrDefaultAsync(new BlogByTagSpec(tag))
            ?? throw new EntityNotFoundException($"Blog with tag {tag} was not found.");
        return _mapper.Map<BlogDto>(blog);
    }

    private static string PrepareTag(string tag) => tag.Trim().ToLower();
}
