﻿using BusinessLogic.Dtos;

namespace BusinessLogic.Abstractions;
public interface IBlogService
{
    Task<BlogDto> Create(Guid userId, CreateBlogDto createBlogDto);
    Task<BlogDto> Update(Guid blogId, UpdateBlogDto updateBlogDto);
}
