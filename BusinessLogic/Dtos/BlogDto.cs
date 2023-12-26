﻿namespace BusinessLogic.Dtos;
public record BlogDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarLocalFileName { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
