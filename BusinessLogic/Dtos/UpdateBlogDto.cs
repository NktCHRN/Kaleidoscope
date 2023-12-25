﻿namespace BusinessLogic.Dtos;
public record UpdateBlogDto
{
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
}
